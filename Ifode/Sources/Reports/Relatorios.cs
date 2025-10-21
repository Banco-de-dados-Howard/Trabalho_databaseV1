using Npgsql;
using SistemaReserva.CRUD;

namespace SistemaReserva
{
    public class Relatorios
    {
        public static void RelatorioReservas()
        {
            var suiteCRUD = new SuiteCRUD();
            var reservaCRUD = new ReservaCRUD();
            var reservas = reservaCRUD.GetAllReservas();

            int pagina = 0;
            int itensPorPagina = 5;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n=== RELATÓRIO DE RESERVAS ===\n");

                int inicio = pagina * itensPorPagina;
                int fim = inicio + itensPorPagina;

                for (int i = inicio; i < fim && i < reservas.Count; i++)
                {
                    var r = reservas[i];
                    var s = suiteCRUD.GetAllSuites().Find(su => su.IdSuite == r.IdSuite);

                    Console.WriteLine($"Reserva #{r.IdReserva}:");
                    Console.WriteLine($"  * ID Usuário: {r.IdUsuario}");
                    Console.WriteLine($"  * Suíte: {s?.Nome ?? "Não encontrada"}");
                    Console.WriteLine($"  * Data Início: {r.DataInicio:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"  * Data Fim: {r.DataFim:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"  * Total: R$ {r.Total:F2}");
                    Console.WriteLine($"  * Status: {r.Status}");
                    Console.WriteLine();
                }

                Console.WriteLine("[1] Anterior | [2] Próxima | [0] Voltar");
                Console.Write("Opção: ");

                if (!int.TryParse(Console.ReadLine(), out int op)) continue;
                if (op == 1 && pagina > 0) pagina--;
                else if (op == 2 && fim < reservas.Count) pagina++;
                else if (op == 0) break;
            }
        }

        public static void RelatorioJobs()
        {
            JobCRUD jb = new JobCRUD();
            var jobs = jb.GetAllJobs();
            int paginaJob = 0;
            int itensPorPaginaJob = 5;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n=== JOBS CADASTRADOS ===\n");

                int inicioJob = paginaJob * itensPorPaginaJob;
                int fimJob = inicioJob + itensPorPaginaJob;

                for (int i = inicioJob; i < fimJob && i < jobs.Count; i++)
                {
                    var j = jobs[i];
                    Console.WriteLine($"{j.Nome}:");
                    Console.WriteLine($"  * ID: {j.IdJob}");
                    Console.WriteLine($"  * Descrição: {j.Descricao}");
                    Console.WriteLine($"  * Tarifa: R$ {j.Tarifa:F2}");
                    Console.WriteLine($"  * Disponível: {(j.Disponibilidade ? "Sim" : "Não")}");
                    Console.WriteLine();
                }

                Console.WriteLine("[1] Anterior | [2] Próxima | [0] Voltar");
                Console.Write("Opção: ");

                if (!int.TryParse(Console.ReadLine(), out int opJob))
                {
                    continue;
                }

                if (opJob == 1 && paginaJob > 0) paginaJob--;
                else if (opJob == 2 && fimJob < jobs.Count) paginaJob++;
                else if (opJob == 0) break;
            }
        }

        public static void RelatorioCompleto()
        {
            var conexao = new Conection();
            var resultados = new List<(string Usuario, string Suite, string Acompanhantes, decimal PrecoTotal)>();

            string sql = @"
        SELECT 
            u.nome AS nome_usuario,
            s.nome AS nome_suite,
            STRING_AGG(j.nome, ', ') AS acompanhantes,
            s.preco + SUM(j.tarifa) AS preco_total
        FROM usuario u
        JOIN reserva r ON r.idUsuario = u.idUsuario
        JOIN suite s ON s.idSuite = r.idSuite
        JOIN reserva_job rj ON rj.idReserva = r.idReserva
        JOIN job j ON j.idJob = rj.idJob
        GROUP BY u.nome, s.nome, s.preco
        ORDER BY u.nome;";

            try
            {
                using (var conn = conexao.getConn())
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string usuario = reader["nome_usuario"].ToString();
                        string suite = reader["nome_suite"].ToString();
                        string acompanhantes = reader["acompanhantes"].ToString();
                        decimal precoTotal = reader.GetDecimal(reader.GetOrdinal("preco_total"));

                        resultados.Add((usuario, suite, acompanhantes, precoTotal));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório: {ex.Message}");
                Console.ReadKey();
                return;
            }

            if (resultados.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("⚠️ Nenhum resultado encontrado.");
                Console.WriteLine("Pressione qualquer tecla para voltar...");
                Console.ReadKey();
                return;
            }

            int pagina = 0;
            int itensPorPagina = 5;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n=== RELATÓRIO COMPLETO DE RESERVAS ===\n");

                int inicio = pagina * itensPorPagina;
                int fim = inicio + itensPorPagina;

                for (int i = inicio; i < fim && i < resultados.Count; i++)
                {
                    var r = resultados[i];

                    Console.WriteLine($"Usuário: {r.Usuario}");
                    Console.WriteLine($"  * Suíte: {r.Suite}");
                    Console.WriteLine($"  * Acompanhantes: {r.Acompanhantes}");
                    Console.WriteLine($"  * Total: R$ {r.PrecoTotal:F2}");
                    Console.WriteLine();
                }

                Console.WriteLine("[1] Anterior | [2] Próxima | [0] Voltar");
                Console.Write("Opção: ");

                if (!int.TryParse(Console.ReadLine(), out int op)) continue;
                if (op == 1 && pagina > 0) pagina--;
                else if (op == 2 && fim < resultados.Count) pagina++;
                else if (op == 0) break;
            }
        }
    }
}
