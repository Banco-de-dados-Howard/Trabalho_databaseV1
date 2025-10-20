using Npgsql;
using SistemaReserva.Models;

namespace SistemaReserva.CRUD
{
    public class JobCRUD
    {
        private readonly Conection conexao = new Conection();

        public void AddJob(Job job)
        {
            using var conn = conexao.getConn();
            string sql = @"INSERT INTO JOB (nome, descricao, tarifa, disponibilidade)
                           VALUES (@nome, @descricao, @tarifa, @disponibilidade)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("nome", job.Nome);
            cmd.Parameters.AddWithValue("descricao", job.Descricao ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("tarifa", job.Tarifa);
            cmd.Parameters.AddWithValue("disponibilidade", job.Disponibilidade);
            cmd.ExecuteNonQuery();
        }

        public List<Job> GetAllJobs()
        {
            var lista = new List<Job>();
            using var conn = conexao.getConn();
            using var cmd = new NpgsqlCommand("SELECT * FROM JOB", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Job
                {
                    IdJob = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    Descricao = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Tarifa = reader.GetDecimal(3),
                    Disponibilidade = reader.GetBoolean(4)
                });
            }
            return lista;
        }

        public void UpdateJob(Job job)
        {
            using var conn = conexao.getConn();
            string sql = @"UPDATE JOB SET nome=@nome, descricao=@descricao, tarifa=@tarifa, disponibilidade=@disponibilidade
                           WHERE idJob=@idJob";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("nome", job.Nome);
            cmd.Parameters.AddWithValue("descricao", job.Descricao ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("tarifa", job.Tarifa);
            cmd.Parameters.AddWithValue("disponibilidade", job.Disponibilidade);
            cmd.Parameters.AddWithValue("idJob", job.IdJob);
            cmd.ExecuteNonQuery();
        }

        public void DeleteJob(int id)
        {
            using var conn = conexao.getConn();
            using var cmd = new NpgsqlCommand("DELETE FROM JOB WHERE idJob=@idJob", conn);
            cmd.Parameters.AddWithValue("idJob", id);
            cmd.ExecuteNonQuery();
        }
    }
}
