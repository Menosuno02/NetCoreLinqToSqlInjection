namespace NetCoreLinqToSqlInjection.Models
{
    public interface ICoche
    {
        // Las interfaces no tienen ámbito, solamente
        // los métodos y propiedades que debe tener un objeto

        string Marca { get; set; }
        string Modelo { get; set; }
        string Imagen { get; set; }
        int Velocidad { get; set; }
        int VelocidadMaxima { get; set; }
        int Acelerar();
        int Frenar();
    }
}
