using Npgsql;

namespace SistemaReserva.CRUD
{
    public class UsuarioLogin
    {
        

        public static bool VerificarLogin(string email, string senha)
        {
            Conection db = new Conection();
            using var conn = db.getConn();
            string sql = "SELECT nome FROM USUARIO WHERE email = @e AND senha = @s";
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", senha);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"\nBem-vindo, {reader["nome"]}!");
                return true;
            }

            return false;
        }

        public static void CadastrarUsuario(string nome, string email, string senha)
        {
            Conection db = new Conection();
            using var conn = db.getConn();
            string sql = "INSERT INTO USUARIO (nome, email, senha) VALUES (@n, @e, @s)";
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@n", nome);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", senha);

            try
            {
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0){
                    Console.WriteLine("\nUsuário cadastrado com sucesso!");
                    MenuUser.MostrarMenu();
                }
                else
                    Console.WriteLine("\nErro ao cadastrar usuário!");
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23505")
                    Console.WriteLine("\nEste email já está cadastrado!");
                else
                    Console.WriteLine("\nErro no banco: " + ex.Message);
            }
        }
    }
}
