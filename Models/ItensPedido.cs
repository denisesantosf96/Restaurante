namespace Restaurante.Models
{
    public class ItensPedido
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int IdGarcom { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorItem { get; set; }
        public string Nome { get; set; }
        public string Produto { get; set; }
        public string NomeProduto { get; set; }
    }
}