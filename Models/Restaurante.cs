namespace Restaurante.Models
{
    public class Restaurante
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Cnpj { get; set; }
        public string Telefone { get; set; }
        public string HorarioFuncionamento { get; set; }
        public string QuantidadeMaxima { get; set; }
        public string Logradouro { get; set; }
        public decimal Numero { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public int QuantidadeMesa { get; set; }
    }
}