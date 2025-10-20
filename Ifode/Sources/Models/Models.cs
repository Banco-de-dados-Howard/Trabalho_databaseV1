namespace SistemaReserva.Models
{
    public class Suite
    {
        public int IdSuite { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public bool Disponibilidade { get; set; }
        public int Capacidade { get; set; }
        public int IdAdm { get; set; }
    }

    public class Job
    {
        public int IdJob { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Tarifa { get; set; }
        public bool Disponibilidade { get; set; }
    }

    public class Reserva
    {
        public int IdReserva { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public int IdUsuario { get; set; }
        public int IdSuite { get; set; }
    }

    public class ReservaJob
    {
        public int IdReserva { get; set; }
        public int IdJob { get; set; }
    }
}
