using Npgsql;

namespace SistemaReserva
{
    public class UsuarioLogin
    {
        private static readonly Conection conexao = new Conection();

        public static bool VerificarLogin(string email, string senha)
        {
            return ObterIdUsuario(email, senha) != 0;
        }

        public static int ObterIdUsuario(string email, string senha)
        {
            try
            {
                using var conn = conexao.getConn();
                string sql = "SELECT idUsuario FROM USUARIO WHERE email=@email AND senha=@senha";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("senha", senha);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao verificar login de usuário: " + e.Message);
                return 0;
            }
        }

        public static int CadastrarUsuario(string nome, string email, string senha)
        {
            try
            {
                using var conn = conexao.getConn();
                string sql = @"INSERT INTO USUARIO (nome, email, senha) 
                               VALUES (@nome, @email, @senha) RETURNING idUsuario";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("senha", senha);

                var id = cmd.ExecuteScalar();
                return Convert.ToInt32(id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao cadastrar usuário: " + e.Message);
                return 0;
            }
        }
    }
}
