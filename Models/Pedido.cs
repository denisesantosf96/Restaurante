using System;

namespace Restaurante.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public int IdMesa { get; set; }

        public int IdCliente { get; set; }

        public DateTime Data { get; set; } 

        public string Status { get; set; }

        public int QuantidadeDeItens { get; set; } 

        public string Nome { get; set; }

        public string Localizacao { get; set; }

        public int NumeroDaMesa { get; set; }

        public int IdRestaurante { get; set; }

        public string FormaPagamento { get; set; }
        public decimal Valor { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}