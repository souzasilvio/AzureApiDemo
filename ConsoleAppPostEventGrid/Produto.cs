using System;

namespace ConsoleAppPostEventGrid
{
    public class Produto
    {
        
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataModificacao { get; set; }
        public Decimal Preco { get; set; }


    }
}