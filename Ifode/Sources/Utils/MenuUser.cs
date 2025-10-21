using System.Globalization;
using SistemaReserva.CRUD;
using SistemaReserva.Models;

namespace SistemaReserva
{
    public class MenuUser
    {
        public static void MostrarMenu(int idUsuario)
        {
            var suiteCRUD = new SuiteCRUD();
            var jobCRUD = new JobCRUD();
            var reservaCRUD = new ReservaCRUD();
            var reservaJobCRUD = new ReservaJobCRUD();

            int opt;
            do
            {
                Console.Clear();
                Console.WriteLine("\n=== MENU USUÁRIO ===");
                Console.WriteLine("[1] - Fazer Reserva");
                Console.WriteLine("[2] - Consultar Reservas");
                Console.WriteLine("[3] - Cancelar Reserva");
                Console.WriteLine("[4] - Alterar Reserva");
                Console.WriteLine("[0] - Sair");
                Console.Write("\nEscolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opt))
                {
                    Console.WriteLine("Digite um número válido!");
                    Console.ReadKey();
                    continue;
                }

                switch (opt)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("\n=== SUITES DISPONÍVEIS ===\n");
                        var suites = suiteCRUD.GetAllSuites().FindAll(s => s.Disponibilidade);
                        foreach (var s in suites)
                        {
                            Console.WriteLine($"{s.Nome}:");
                            Console.WriteLine($"  * ID: {s.IdSuite}");
                            Console.WriteLine($"  * Preço: R$ {s.Preco:F2}");
                            Console.WriteLine($"  * Capacidade: {s.Capacidade} pessoa(s)");
                            Console.WriteLine();
                        }

                        Console.Write("Escolha o ID da suíte: ");
                        int idSuite = int.Parse(Console.ReadLine());
                        var suiteSelecionada = suites.Find(s => s.IdSuite == idSuite);
                        if (suiteSelecionada == null)
                        {
                            Console.WriteLine("Suíte inválida!");
                            Console.ReadKey();
                            break;
                        }

                        Console.Clear();
                        Console.WriteLine("\n=== JOBS DISPONÍVEIS ===\n");
                        var jobs = jobCRUD.GetAllJobs().FindAll(j => j.Disponibilidade);
                        foreach (var j in jobs)
                        {
                            Console.WriteLine($"{j.Nome}:");
                            Console.WriteLine($"  * ID: {j.IdJob}");
                            Console.WriteLine($"  * Descrição: {j.Descricao}");
                            Console.WriteLine($"  * Tarifa: R$ {j.Tarifa:F2}");
                            Console.WriteLine();
                        }

                        Console.Write("Digite os IDs dos jobs separados por vírgula (ou ENTER para nenhum): ");
                        string inputJobs = Console.ReadLine();
                        List<int> jobsSelecionados = new List<int>();
                        if (!string.IsNullOrEmpty(inputJobs))
                        {
                            foreach (var str in inputJobs.Split(','))
                            {
                                if (int.TryParse(str.Trim(), out int jid))
                                    jobsSelecionados.Add(jid);
                            }
                        }

                        Console.Write("\nData de início (DD-MM-YYYY): ");
                        DateTime dataInicio = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        Console.Write("Data de fim (DD-MM-YYYY): ");
                        DateTime dataFim = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        decimal total = suiteSelecionada.Preco;
                        foreach (var jid in jobsSelecionados)
                        {
                            var job = jobs.Find(j => j.IdJob == jid);
                            if (job != null) total += job.Tarifa;
                        }

                        var reserva = new Reserva
                        {
                            DataInicio = dataInicio,
                            DataFim = dataFim,
                            Status = "Ativa",
                            Total = total,
                            IdUsuario = idUsuario,
                            IdSuite = idSuite
                        };
                        reservaCRUD.AddReserva(reserva);

                        var todasReservas = reservaCRUD.GetAllReservas();
                        int idReserva = todasReservas[^1].IdReserva;

                        foreach (var jid in jobsSelecionados)
                        {
                            reservaJobCRUD.AddReservaJob(new ReservaJob { IdReserva = idReserva, IdJob = jid });
                        }

                        Console.WriteLine($"\nReserva criada com sucesso! Total: R$ {total:F2}");
                        Console.WriteLine("Pressione qualquer tecla para continuar...");
                        Console.ReadKey();
                        break;

                    case 2:
                        var minhasReservas = reservaCRUD.GetAllReservas().FindAll(r => r.IdUsuario == idUsuario);
                        int paginaReserva = 0;
                        int itensPorPagina = 5;

                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("\n=== SUAS RESERVAS ===\n");

                            int inicio = paginaReserva * itensPorPagina;
                            int fim = inicio + itensPorPagina;

                            for (int i = inicio; i < fim && i < minhasReservas.Count; i++)
                            {
                                var r = minhasReservas[i];
                                var s = suiteCRUD.GetAllSuites().Find(su => su.IdSuite == r.IdSuite);

                                Console.WriteLine($"Reserva #{r.IdReserva}:");
                                Console.WriteLine($"  * Suíte: {s?.Nome ?? "Não encontrada"}");
                                Console.WriteLine($"  * Data Início: {r.DataInicio:dd/MM/yyyy HH:mm}");
                                Console.WriteLine($"  * Data Fim: {r.DataFim:dd/MM/yyyy HH:mm}");
                                Console.WriteLine($"  * Status: {r.Status}");
                                Console.WriteLine($"  * Total: R$ {r.Total:F2}");

                                // Buscar jobs da reserva
                                var reservasJobs = reservaJobCRUD.GetAllReservaJobs().FindAll(rj => rj.IdReserva == r.IdReserva);
                                if (reservasJobs.Count > 0)
                                {
                                    Console.WriteLine("  * Jobs contratados:");
                                    foreach (var rj in reservasJobs)
                                    {
                                        var job = jobCRUD.GetAllJobs().Find(j => j.IdJob == rj.IdJob);
                                        if (job != null)
                                        {
                                            Console.WriteLine($"    - {job.Nome} (R$ {job.Tarifa:F2})");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("  * Jobs contratados: Nenhum");
                                }

                                Console.WriteLine();
                            }

                            if (minhasReservas.Count == 0)
                            {
                                Console.WriteLine("Você ainda não possui reservas.");
                                Console.WriteLine("\nPressione qualquer tecla para voltar...");
                                Console.ReadKey();
                                break;
                            }

                            Console.WriteLine("[1] Anterior | [2] Próxima | [0] Voltar");
                            Console.Write("Opção: ");

                            if (!int.TryParse(Console.ReadLine(), out int opReserva))
                            {
                                continue;
                            }

                            if (opReserva == 1 && paginaReserva > 0) paginaReserva--;
                            else if (opReserva == 2 && fim < minhasReservas.Count) paginaReserva++;
                            else if (opReserva == 0) break;
                        }
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("\n=== CANCELAR RESERVA ===\n");
                        var reservasAtivas = reservaCRUD.GetAllReservas().FindAll(r => r.IdUsuario == idUsuario && r.Status == "Ativa");

                        if (reservasAtivas.Count == 0)
                        {
                            Console.WriteLine("Você não possui reservas ativas para cancelar.");
                            Console.WriteLine("Pressione qualquer tecla para continuar...");
                            Console.ReadKey();
                            break;
                        }

                        foreach (var r in reservasAtivas)
                        {
                            var s = suiteCRUD.GetAllSuites().Find(su => su.IdSuite == r.IdSuite);
                            Console.WriteLine($"ID: {r.IdReserva} - {s?.Nome ?? "Não encontrada"} - {r.DataInicio:dd/MM/yyyy} - R$ {r.Total:F2}");
                        }

                        Console.Write("\nDigite o ID da reserva que deseja cancelar: ");
                        int idCancel = int.Parse(Console.ReadLine());

                        var reservaCancel = reservasAtivas.Find(r => r.IdReserva == idCancel);
                        if (reservaCancel == null)
                        {
                            Console.WriteLine("Reserva não encontrada ou não pertence a você!");
                        }
                        else
                        {
                            reservaCRUD.DeleteReserva(idCancel);
                            Console.WriteLine("Reserva cancelada com sucesso!");
                        }

                        Console.WriteLine("Pressione qualquer tecla para continuar...");
                        Console.ReadKey();
                        break;
                    case 4: // Alterar Reserva
                        Console.Clear();
                        Console.WriteLine("\n=== ALTERAR RESERVA ===\n");

                        // Filtra reservas que ainda irão acontecer
                        var reservasFuturas = reservaCRUD.GetAllReservas()
                            .FindAll(r => r.IdUsuario == idUsuario && r.DataInicio > DateTime.Now && r.Status == "Ativa");

                        if (reservasFuturas.Count == 0)
                        {
                            Console.WriteLine("Você não possui reservas futuras para alterar.");
                            Console.WriteLine("Pressione qualquer tecla para continuar...");
                            Console.ReadKey();
                            break;
                        }

                        foreach (var r in reservasFuturas)
                        {
                            var s = suiteCRUD.GetAllSuites().Find(su => su.IdSuite == r.IdSuite);
                            Console.WriteLine($"ID: {r.IdReserva} - {s?.Nome ?? "Não encontrada"} - {r.DataInicio:dd/MM/yyyy} a {r.DataFim:dd/MM/yyyy} - R$ {r.Total:F2}");
                        }

                        Console.Write("\nDigite o ID da reserva que deseja alterar: ");
                        int idAlterar = int.Parse(Console.ReadLine());

                        var reservaAlterar = reservasFuturas.Find(r => r.IdReserva == idAlterar);
                        if (reservaAlterar == null)
                        {
                            Console.WriteLine("Reserva não encontrada ou não pode ser alterada!");
                            Console.ReadKey();
                            break;
                        }

                        Console.Write($"Nova data de início ({reservaAlterar.DataInicio:dd-MM-yyyy}): ");
                        string novaDataInicioStr = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(novaDataInicioStr))
                            reservaAlterar.DataInicio = DateTime.ParseExact(novaDataInicioStr, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        Console.Write($"Nova data de fim ({reservaAlterar.DataFim:dd-MM-yyyy}): ");
                        string novaDataFimStr = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(novaDataFimStr))
                            reservaAlterar.DataFim = DateTime.ParseExact(novaDataFimStr, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        // Recalcular total (suite + jobs)
                        var suiteSelecionadaAlt = suiteCRUD.GetAllSuites().Find(s => s.IdSuite == reservaAlterar.IdSuite);
                        decimal totalAlterado = suiteSelecionadaAlt.Preco;

                        var reservasJobsAlterar = reservaJobCRUD.GetAllReservaJobs()
                            .FindAll(rj => rj.IdReserva == reservaAlterar.IdReserva);
                        foreach (var rj in reservasJobsAlterar)
                        {
                            var job = jobCRUD.GetAllJobs().Find(j => j.IdJob == rj.IdJob);
                            if (job != null) totalAlterado += job.Tarifa;
                        }

                        reservaAlterar.Total = totalAlterado;

                        reservaCRUD.UpdateReserva(reservaAlterar);

                        Console.WriteLine($"\nReserva alterada com sucesso! Novo total: R$ {totalAlterado:F2}");
                        Console.WriteLine("Pressione qualquer tecla para continuar...");
                        Console.ReadKey();
                        break;

                    case 0:
                        Console.WriteLine("Saindo...");
                        break;

                    default:
                        Console.WriteLine("Opção inválida!");
                        Console.ReadKey();
                        break;
                }
            } while (opt != 0);
        }
    }
}