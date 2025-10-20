using System;
using Npgsql;
using SistemaReserva; 

namespace SistemaReserva.CRUD
{
    public class AdmLogin
    {
        

        public static bool VerificarLogin(string email, string senha)
        {
            Conection db = new Conection();
            using var conn = db.getConn();
            string sql = "SELECT nome FROM ADM WHERE email = @e AND senha = @s";
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@s", senha);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine($"\nBem-vindo administrador, {reader["nome"]}!");
                return true;
            }

            return false;
        }
    }
}
