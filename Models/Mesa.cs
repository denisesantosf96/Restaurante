namespace Restaurante.Models
{
    public class Mesa
    {
        public int Id { get ; set; }
        public int IdRestaurante { get; set; }
        public string Localizacao { get; set; }
        public int NumeroDaMesa { get; set; }
    }
}