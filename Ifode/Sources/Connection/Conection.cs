using Npgsql;

namespace SistemaReserva
{
    public class Conection
    {
        private readonly string connString;

        public Conection()
        {
            this.connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";
        }

        public NpgsqlConnection getConn()
        {
            try
            {
                var conn = new NpgsqlConnection(connString);
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na conexão com o banco: " + e.Message);
                throw;
            }
        }
    }
}