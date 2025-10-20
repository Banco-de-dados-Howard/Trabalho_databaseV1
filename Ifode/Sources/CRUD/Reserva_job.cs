using Npgsql;
using SistemaReserva.Models;

namespace SistemaReserva.CRUD
{
    public class ReservaJobCRUD
    {
        private readonly Conection conexao = new Conection();

        public void AddReservaJob(ReservaJob rj)
        {
            using var conn = conexao.getConn();
            string sql = @"INSERT INTO RESERVA_JOB (idReserva, idJob) VALUES (@idReserva, @idJob)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("idReserva", rj.IdReserva);
            cmd.Parameters.AddWithValue("idJob", rj.IdJob);
            cmd.ExecuteNonQuery();
        }

        public List<ReservaJob> GetAllReservaJobs()
        {
            var lista = new List<ReservaJob>();
            using var conn = conexao.getConn();
            using var cmd = new NpgsqlCommand("SELECT * FROM RESERVA_JOB", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new ReservaJob
                {
                    IdReserva = reader.GetInt32(0),
                    IdJob = reader.GetInt32(1)
                });
            }
            return lista;
        }

        public void DeleteReservaJob(int idReserva, int idJob)
        {
            using var conn = conexao.getConn();
            string sql = @"DELETE FROM RESERVA_JOB WHERE idReserva=@idReserva AND idJob=@idJob";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("idReserva", idReserva);
            cmd.Parameters.AddWithValue("idJob", idJob);
            cmd.ExecuteNonQuery();
        }
    }
}
