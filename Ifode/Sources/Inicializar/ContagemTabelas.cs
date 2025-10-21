using Npgsql;

namespace SistemaReserva
{
    public class contagemTabelas
    {
        private readonly Conection conection;

        public contagemTabelas()
        {
            this.conection = new Conection();
        }

        public int[] contar()
        {
            int[] registros = new int[6];

            try
            {
                using (NpgsqlConnection conn = conection.getConn())
                {
                    registros[0] = ContarRegistros(conn, "adm");
                    registros[1] = ContarRegistros(conn, "usuario");
                    registros[2] = ContarRegistros(conn, "suite");
                    registros[3] = ContarRegistros(conn, "job");
                    registros[4] = ContarRegistros(conn, "reserva");
                    registros[5] = ContarRegistros(conn, "reserva_job");

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return registros;
        }

        private int ContarRegistros(NpgsqlConnection conn, string nomeTabela)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM {nomeTabela}";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}