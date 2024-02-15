using NetCoreLinqToSqlInjection.Models;
using System.Data;
using System.Data.SqlClient;

#region PROCEDIMIENTOS ALMACENADOS

/*
CREATE PROCEDURE SP_DELETE_DOCTOR
(@IDDOCTOR INT)
AS
	DELETE FROM DOCTOR
	WHERE DOCTOR_NO=@IDDOCTOR
GO

CREATE OR ALTER PROCEDURE SP_MODIFICAR_DOCTOR
(@IDDOCTOR INT, @HOSPITALCOD INT, @APELLIDO NVARCHAR(50),
@ESPECIALIDAD NVARCHAR(50), @SALARIO INT)
AS
	UPDATE DOCTOR
	SET HOSPITAL_COD = @HOSPITALCOD,
	APELLIDO = @APELLIDO,
	ESPECIALIDAD = @ESPECIALIDAD,
	SALARIO = @SALARIO
	WHERE DOCTOR_NO = @IDDOCTOR
GO
*/

#endregion

namespace NetCoreLinqToSqlInjection.Repositories
{
    public class RepositoryDoctoresSQLServer : IRepositoryDoctores
    {
        private DataTable tablaDoctores;
        private SqlConnection cn;
        private SqlCommand com;

        public RepositoryDoctoresSQLServer()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=sa;Password=MCSD2023;Encrypt=False";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            this.tablaDoctores = new DataTable();
            string sql = "SELECT * FROM DOCTOR";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, this.cn);
            adapter.Fill(tablaDoctores);
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
                           select datos;
            List<Doctor> doctores = new List<Doctor>();
            foreach (var row in consulta)
            {
                Doctor doc = new Doctor
                {
                    IdDoctor = row.Field<int>("DOCTOR_NO"),
                    Apellido = row.Field<string>("APELLIDO"),
                    Especialidad = row.Field<string>("ESPECIALIDAD"),
                    Salario = row.Field<int>("SALARIO"),
                    IdHospital = row.Field<int>("HOSPITAL_COD")
                };
                doctores.Add(doc);
            }
            return doctores;
        }

        public void InsertDoctor(int id, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "INSERT INTO DOCTOR VALUES(@IDHOSPITAL, @IDDOCTOR, " +
                "@APELLIDO, @ESPECIALIDAD, @SALARIO)";
            this.com.Parameters.AddWithValue("@IDHOSPITAL", idHospital);
            this.com.Parameters.AddWithValue("@IDDOCTOR", id);
            this.com.Parameters.AddWithValue("@APELLIDO", apellido);
            this.com.Parameters.AddWithValue("@ESPECIALIDAD", especialidad);
            this.com.Parameters.AddWithValue("@SALARIO", salario);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public List<Doctor> GetDoctoresEspecialidad(string especialidad)
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
                           where datos.Field<string>("ESPECIALIDAD") == especialidad
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            List<Doctor> doctores = new List<Doctor>();
            foreach (var row in consulta)
            {
                Doctor doctor = new Doctor
                {
                    IdHospital = row.Field<int>("HOSPITAL_COD"),
                    IdDoctor = row.Field<int>("DOCTOR_NO"),
                    Apellido = row.Field<string>("APELLIDO"),
                    Especialidad = row.Field<string>("ESPECIALIDAD"),
                    Salario = row.Field<int>("SALARIO")
                };
                doctores.Add(doctor);
            }
            return doctores;
        }

        public void DeleteDoctor(int idDoctor)
        {
            this.com.Parameters.AddWithValue("@IDDOCTOR", idDoctor);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_DELETE_DOCTOR";
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public Doctor FindDoctor(int iddoctor)
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
                           where datos.Field<int>("DOCTOR_NO") == iddoctor
                           select datos;
            var row = consulta.First();
            Doctor doctor = new Doctor
            {
                IdHospital = row.Field<int>("HOSPITAL_COD"),
                IdDoctor = row.Field<int>("DOCTOR_NO"),
                Apellido = row.Field<string>("APELLIDO"),
                Especialidad = row.Field<string>("ESPECIALIDAD"),
                Salario = row.Field<int>("SALARIO")
            };
            return doctor;
        }

        public void UpdateDoctor(Doctor doctor)
        {
            this.com.Parameters.AddWithValue("@IDDOCTOR", doctor.IdDoctor);
            this.com.Parameters.AddWithValue("@HOSPITALCOD", doctor.IdHospital);
            this.com.Parameters.AddWithValue("@APELLIDO", doctor.Apellido);
            this.com.Parameters.AddWithValue("@ESPECIALIDAD", doctor.Especialidad);
            this.com.Parameters.AddWithValue("@SALARIO", doctor.Salario);
            this.com.CommandText = "SP_MODIFICAR_DOCTOR";
            this.com.CommandType = CommandType.StoredProcedure;
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}
