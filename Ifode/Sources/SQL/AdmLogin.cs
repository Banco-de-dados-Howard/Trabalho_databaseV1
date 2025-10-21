using Npgsql;

namespace SistemaReserva
{
    public class AdmLogin
    {
        private static readonly Conection conexao = new Conection();

        public static bool VerificarLogin(string email, string senha)
        {
            return ObterIdAdm(email, senha) != 0;
        }

        public static int ObterIdAdm(string email, string senha)
        {
            try
            {
                using var conn = conexao.getConn();
                string sql = "SELECT idAdm FROM ADM WHERE email=@email AND senha=@senha";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("senha", senha);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao verificar login de administrador: " + e.Message);
                return 0;
            }
        }
    }
}
