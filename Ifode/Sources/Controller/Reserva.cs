using Npgsql;
using SistemaReserva.Models;

namespace SistemaReserva.CRUD
{
    public class ReservaCRUD
    {
        private readonly Conection conexao = new Conection();

        public void AddReserva(Reserva reserva)
        {
            using var conn = conexao.getConn();
            string sql = @"INSERT INTO RESERVA (dataInicio, dataFim, status, total, idUsuario, idSuite)
                           VALUES (@dataInicio, @dataFim, @status, @total, @idUsuario, @idSuite)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("dataInicio", reserva.DataInicio);
            cmd.Parameters.AddWithValue("dataFim", reserva.DataFim);
            cmd.Parameters.AddWithValue("status", reserva.Status);
            cmd.Parameters.AddWithValue("total", reserva.Total);
            cmd.Parameters.AddWithValue("idUsuario", reserva.IdUsuario);
            cmd.Parameters.AddWithValue("idSuite", reserva.IdSuite);
            cmd.ExecuteNonQuery();
        }

        public List<Reserva> GetAllReservas()
        {
            var lista = new List<Reserva>();
            using var conn = conexao.getConn();
            using var cmd = new NpgsqlCommand("SELECT * FROM RESERVA", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Reserva
                {
                    IdReserva = reader.GetInt32(0),
                    DataInicio = reader.GetDateTime(1),
                    DataFim = reader.GetDateTime(2),
                    Status = reader.GetString(3),
                    Total = reader.GetDecimal(4),
                    IdUsuario = reader.GetInt32(5),
                    IdSuite = reader.GetInt32(6)
                });
            }
            return lista;
        }

        public void UpdateReserva(Reserva reserva)
        {
            using var conn = conexao.getConn();
            string sql = @"UPDATE RESERVA SET dataInicio=@dataInicio, dataFim=@dataFim, status=@status, 
                           total=@total, idUsuario=@idUsuario, idSuite=@idSuite
                           WHERE idReserva=@idReserva";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("dataInicio", reserva.DataInicio);
            cmd.Parameters.AddWithValue("dataFim", reserva.DataFim);
            cmd.Parameters.AddWithValue("status", reserva.Status);
            cmd.Parameters.AddWithValue("total", reserva.Total);
            cmd.Parameters.AddWithValue("idUsuario", reserva.IdUsuario);
            cmd.Parameters.AddWithValue("idSuite", reserva.IdSuite);
            cmd.Parameters.AddWithValue("idReserva", reserva.IdReserva);
            cmd.ExecuteNonQuery();
        }

        public void DeleteReserva(int id)
        {
            using var conn = conexao.getConn();
            using var cmd = new NpgsqlCommand("DELETE FROM RESERVA WHERE idReserva=@idReserva", conn);
            cmd.Parameters.AddWithValue("idReserva", id);
            cmd.ExecuteNonQuery();
        }
    }
}
