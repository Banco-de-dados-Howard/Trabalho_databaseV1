using Npgsql;
using SistemaReserva.Models;

namespace SistemaReserva.CRUD
{
    public class SuiteCRUD
    {
        private readonly Conection conexao = new Conection();

        // CREATE
        public void AddSuite(Suite suite)
        {
            using var conn = conexao.getConn();
            string sql = @"INSERT INTO SUITE (nome, descricao, preco, disponibilidade, capacidade, idAdm)
                           VALUES (@nome, @descricao, @preco, @disponibilidade, @capacidade, @idAdm)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("nome", suite.Nome);
            cmd.Parameters.AddWithValue("descricao", suite.Descricao ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("preco", suite.Preco);
            cmd.Parameters.AddWithValue("disponibilidade", suite.Disponibilidade);
            cmd.Parameters.AddWithValue("capacidade", suite.Capacidade);
            cmd.Parameters.AddWithValue("idAdm", suite.IdAdm);
            cmd.ExecuteNonQuery();
        }

        // READ
        public List<Suite> GetAllSuites()
        {
            var lista = new List<Suite>();
            using var conn = conexao.getConn();
            using var cmd = new NpgsqlCommand("SELECT * FROM SUITE", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Suite
                {
                    IdSuite = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Preco = reader.GetDecimal(3),
                    Disponibilidade = reader.GetBoolean(4),
                    Capacidade = reader.GetInt32(5),
                    IdAdm = reader.GetInt32(6)
                });
            }
            return lista;
        }

        // UPDATE
        public void UpdateSuite(Suite suite)
        {
            using var conn = conexao.getConn();
            string sql = @"UPDATE SUITE SET nome=@nome, descricao=@descricao, preco=@preco, 
                           disponibilidade=@disponibilidade, capacidade=@capacidade, idAdm=@idAdm
                           WHERE idSuite=@idSuite";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("nome", suite.Nome);
            cmd.Parameters.AddWithValue("descricao", suite.Descricao ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("preco", suite.Preco);
            cmd.Parameters.AddWithValue("disponibilidade", suite.Disponibilidade);
            cmd.Parameters.AddWithValue("capacidade", suite.Capacidade);
            cmd.Parameters.AddWithValue("idAdm", suite.IdAdm);
            cmd.Parameters.AddWithValue("idSuite", suite.IdSuite);
            cmd.ExecuteNonQuery();
        }


        public Suite GetSuiteById(int idSuite)
        {
            using var conn = conexao.getConn();
            string sql = "SELECT * FROM SUITE WHERE idSuite = @idSuite";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("idSuite", idSuite);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Suite
                {
                    IdSuite = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Preco = reader.GetDecimal(3),
                    Disponibilidade = reader.GetBoolean(4),
                    Capacidade = reader.GetInt32(5),
                    IdAdm = reader.GetInt32(6)
                };
            }

            return null; // Retorna null se não encontrar
        }

        // DELETE
        public void DeleteSuite(int id)
        {
            using var conn = conexao.getConn();
            using var cmd = new NpgsqlCommand("DELETE FROM SUITE WHERE idSuite=@idSuite", conn);
            cmd.Parameters.AddWithValue("idSuite", id);
            cmd.ExecuteNonQuery();
        }
    }
}
