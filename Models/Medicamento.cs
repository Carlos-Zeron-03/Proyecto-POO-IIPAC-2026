namespace ProyectoAPI.Models
{
    public class Medicamento
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public int Cantidad { get; set; }
        public required string Tipo { get; set; }
    }
}