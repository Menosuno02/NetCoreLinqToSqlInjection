using NetCoreLinqToSqlInjection.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

#region PROCEDIMIENTOS ALMACENADOS

/*
CREATE OR REPLACE PROCEDURE SP_DELETE_DOCTOR
(P_IDDOCTOR DOCTOR.DOCTOR_NO%TYPE)
AS
BEGIN
  DELETE FROM DOCTOR
  WHERE DOCTOR_NO=P_IDDOCTOR;
  COMMIT;
END;

CREATE OR REPLACE PROCEDURE SP_MODIFICAR_DOCTOR
(P_IDDOCTOR DOCTOR.DOCTOR_NO%TYPE,
P_HOSPITALCOD DOCTOR.HOSPITAL_COD%TYPE,
P_APELLIDO DOCTOR.APELLIDO%TYPE,
P_ESPECIALIDAD DOCTOR.ESPECIALIDAD%TYPE,
P_SALARIO DOCTOR.SALARIO%TYPE)
AS
BEGIN
  UPDATE DOCTOR
  SET HOSPITAL_COD = P_HOSPITALCOD,
  APELLIDO = P_APELLIDO,
  ESPECIALIDAD = P_ESPECIALIDAD,
  SALARIO = P_SALARIO
  WHERE DOCTOR_NO = P_IDDOCTOR;
END;
*/

#endregion

namespace NetCoreLinqToSqlInjection.Repositories
{
    public class RepositoryDoctoresOracle : IRepositoryDoctores
    {

        private DataTable tablaDoctores;
        private OracleConnection cn;
        private OracleCommand com;

        public RepositoryDoctoresOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True; User Id=SYSTEM; Password=oracle";
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand();
            this.com.Connection = this.cn;
            string sql = "SELECT * FROM DOCTOR";
            this.tablaDoctores = new DataTable();
            OracleDataAdapter adapter = new OracleDataAdapter(sql, this.cn);
            adapter.Fill(tablaDoctores);
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
                           select datos;
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

        public void InsertDoctor(int id, string apellido,
            string especialidad, int salario, int idHospital)
        {
            string sql = "INSERT INTO DOCTOR VALUES" +
                "(:HOSPITAL_COD, :DOCTOR_NO, :APELLIDO, :ESPECIALIDAD, :SALARIO)";
            OracleParameter paramHospitalCod = new OracleParameter(":HOSPITAL_COD", idHospital);
            this.com.Parameters.Add(paramHospitalCod);
            OracleParameter paramDoctorNo = new OracleParameter(":DOCTOR_NO", id);
            this.com.Parameters.Add(paramDoctorNo);
            OracleParameter paramApellido = new OracleParameter(":APELLIDO", apellido);
            this.com.Parameters.Add(paramApellido);
            OracleParameter paramEspecialidad = new OracleParameter(":ESPECIALIDAD", especialidad);
            this.com.Parameters.Add(paramEspecialidad);
            OracleParameter paramSalario = new OracleParameter(":SALARIO", salario);
            this.com.Parameters.Add(paramSalario);
            this.com.CommandText = sql;
            this.com.CommandType = CommandType.Text;
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
        public List<Doctor> GetDoctoresEspecialidad(string especialidad)
        {
            var consulta = from datos in this.tablaDoctores.AsEnumerable()
                           where datos.Field<string>("ESPECIALIDAD").ToUpper()
                            == especialidad.ToUpper()
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

        public void DeleteDoctor(int iddoctor)
        {
            OracleParameter paramIdDoctor = new OracleParameter(":p_iddoctor", iddoctor);
            this.com.Parameters.Add(paramIdDoctor);
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
            OracleParameter paramIdDoctor = new OracleParameter(":p_iddoctor", doctor.IdDoctor);
            this.com.Parameters.Add(paramIdDoctor);
            OracleParameter paramHospitalCod = new OracleParameter(":p_hospitalcod", doctor.IdHospital);
            this.com.Parameters.Add(paramHospitalCod);
            OracleParameter paramApellido = new OracleParameter(":p_apellido", doctor.Apellido);
            this.com.Parameters.Add(paramApellido);
            OracleParameter paramEspecialidad = new OracleParameter(":p_especialidad", doctor.Especialidad);
            this.com.Parameters.Add(paramEspecialidad);
            OracleParameter paramSalario = new OracleParameter(":p_salario", doctor.Salario);
            this.com.Parameters.Add(paramSalario);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_MODIFICAR_DOCTOR";
            this.cn.Open();
            int result = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}
