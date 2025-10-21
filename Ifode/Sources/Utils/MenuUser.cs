using System.Globalization;
using SistemaReserva.CRUD;
using SistemaReserva.Models;

namespace SistemaReserva
{
    public class MenuUser
    {
        private static SuiteCRUD suiteCRUD = new SuiteCRUD();
        private static JobCRUD jobCRUD = new JobCRUD();
        private static ReservaCRUD reservaCRUD = new ReservaCRUD();
        private static ReservaJobCRUD reservaJobCRUD = new ReservaJobCRUD();
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
                        AlterarReservaMenu(idUsuario);
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

        public static void AlterarReservaMenu(int idUsuario)
        {
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
                return;
            }

            // Exibe reservas disponíveis para alteração
            foreach (var r in reservasFuturas)
            {
                var s = suiteCRUD.GetAllSuites().Find(su => su.IdSuite == r.IdSuite);
                Console.WriteLine($"ID: {r.IdReserva} - {s?.Nome ?? "Não encontrada"} - {r.DataInicio:dd/MM/yyyy} a {r.DataFim:dd/MM/yyyy} - R$ {r.Total:F2}");
            }

            Console.Write("\nDigite o ID da reserva que deseja alterar: ");
            if (!int.TryParse(Console.ReadLine(), out int idAlterar))
            {
                Console.WriteLine("ID inválido!");
                Console.ReadKey();
                return;
            }

            var reservaAlterar = reservasFuturas.Find(r => r.IdReserva == idAlterar);
            if (reservaAlterar == null)
            {
                Console.WriteLine("Reserva não encontrada ou não pode ser alterada!");
                Console.ReadKey();
                return;
            }

            // Mostra informações atuais da reserva
            Console.Clear();
            Console.WriteLine("\n=== DADOS ATUAIS DA RESERVA ===\n");
            var suiteAtual = suiteCRUD.GetAllSuites().Find(s => s.IdSuite == reservaAlterar.IdSuite);
            Console.WriteLine($"Suíte atual: {suiteAtual?.Nome ?? "Não encontrada"}");
            Console.WriteLine($"Data início: {reservaAlterar.DataInicio:dd/MM/yyyy}");
            Console.WriteLine($"Data fim: {reservaAlterar.DataFim:dd/MM/yyyy}");

            var jobsAtuais = reservaJobCRUD.GetAllReservaJobs()
                .FindAll(rj => rj.IdReserva == reservaAlterar.IdReserva);

            if (jobsAtuais.Count > 0)
            {
                Console.WriteLine("Jobs contratados:");
                foreach (var rj in jobsAtuais)
                {
                    var job = jobCRUD.GetAllJobs().Find(j => j.IdJob == rj.IdJob);
                    if (job != null)
                        Console.WriteLine($"  - {job.Nome} (R$ {job.Tarifa:F2})");
                }
            }
            else
            {
                Console.WriteLine("Jobs contratados: Nenhum");
            }

            Console.WriteLine($"\nTotal atual: R$ {reservaAlterar.Total:F2}");
            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // ========== ALTERAÇÃO DE DATAS ==========
            Console.WriteLine("=== ALTERAR DATAS ===");
            Console.Write($"Nova data de início (deixe vazio para manter {reservaAlterar.DataInicio:dd-MM-yyyy}): ");
            string novaDataInicioStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novaDataInicioStr))
            {
                if (DateTime.TryParseExact(novaDataInicioStr, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime novaDataInicio))
                {
                    reservaAlterar.DataInicio = novaDataInicio;
                }
                else
                {
                    Console.WriteLine("Data inválida! Mantendo data anterior.");
                }
            }

            Console.Write($"Nova data de fim (deixe vazio para manter {reservaAlterar.DataFim:dd-MM-yyyy}): ");
            string novaDataFimStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novaDataFimStr))
            {
                if (DateTime.TryParseExact(novaDataFimStr, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime novaDataFim))
                {
                    reservaAlterar.DataFim = novaDataFim;
                }
                else
                {
                    Console.WriteLine("Data inválida! Mantendo data anterior.");
                }
            }

            // ========== ALTERAÇÃO DE SUÍTE ==========
            Console.WriteLine("\n=== ALTERAR SUÍTE ===");
            Console.Write("Deseja alterar a suíte? (S/N): ");
            string alterarSuite = Console.ReadLine()?.Trim().ToUpper();

            if (alterarSuite == "S" || alterarSuite == "SIM")
            {
                Console.Clear();
                Console.WriteLine("\n=== SUÍTES DISPONÍVEIS ===\n");
                var suitesDisponiveis = suiteCRUD.GetAllSuites().FindAll(s => s.Disponibilidade);

                foreach (var s in suitesDisponiveis)
                {
                    string indicador = s.IdSuite == reservaAlterar.IdSuite ? " (ATUAL)" : "";
                    Console.WriteLine($"{s.IdSuite} - {s.Nome}{indicador}:");
                    Console.WriteLine($"  * Descrição: {s.Descricao}");
                    Console.WriteLine($"  * Preço: R$ {s.Preco:F2}");
                    Console.WriteLine($"  * Capacidade: {s.Capacidade} pessoa(s)");
                    Console.WriteLine();
                }

                Console.Write("Digite o ID da nova suíte (ou ENTER para manter): ");
                string novaSuiteInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(novaSuiteInput) && int.TryParse(novaSuiteInput, out int novaSuiteId))
                {
                    var novaSuite = suitesDisponiveis.Find(s => s.IdSuite == novaSuiteId);
                    if (novaSuite != null)
                    {
                        reservaAlterar.IdSuite = novaSuiteId;
                        Console.WriteLine($"Suíte alterada para: {novaSuite.Nome}");
                    }
                    else
                    {
                        Console.WriteLine("Suíte não encontrada! Mantendo suíte anterior.");
                    }
                }
            }

            // ========== ALTERAÇÃO DE JOBS ==========
            Console.WriteLine("\n=== ALTERAR JOBS ===");
            Console.Write("Deseja alterar os jobs contratados? (S/N): ");
            string alterarJobs = Console.ReadLine()?.Trim().ToUpper();

            if (alterarJobs == "S" || alterarJobs == "SIM")
            {
                Console.Clear();
                Console.WriteLine("\n=== JOBS DISPONÍVEIS ===\n");
                var jobsDisponiveis = jobCRUD.GetAllJobs().FindAll(j => j.Disponibilidade);

                foreach (var j in jobsDisponiveis)
                {
                    bool contratado = jobsAtuais.Any(rj => rj.IdJob == j.IdJob);
                    string indicador = contratado ? " (CONTRATADO)" : "";

                    Console.WriteLine($"{j.IdJob} - {j.Nome}{indicador}:");
                    Console.WriteLine($"  * Descrição: {j.Descricao}");
                    Console.WriteLine($"  * Tarifa: R$ {j.Tarifa:F2}");
                    Console.WriteLine();
                }

                Console.WriteLine("\n[1] - Remover todos os jobs e selecionar novos");
                Console.WriteLine("[2] - Adicionar novos jobs aos existentes");
                Console.WriteLine("[3] - Remover jobs específicos");
                Console.WriteLine("[0] - Manter jobs atuais");
                Console.Write("Escolha uma opção: ");

                if (int.TryParse(Console.ReadLine(), out int opcaoJob))
                {
                    switch (opcaoJob)
                    {
                        case 1: // Remover todos e selecionar novos
                            foreach (var rj in jobsAtuais)
                            {
                                reservaJobCRUD.DeleteReservaJob(reservaAlterar.IdReserva, rj.IdJob);
                            }

                            Console.Write("\nDigite os IDs dos novos jobs separados por vírgula (ou ENTER para nenhum): ");
                            string novosJobsInput = Console.ReadLine();

                            if (!string.IsNullOrWhiteSpace(novosJobsInput))
                            {
                                foreach (var idStr in novosJobsInput.Split(','))
                                {
                                    if (int.TryParse(idStr.Trim(), out int jobId))
                                    {
                                        var jobExiste = jobsDisponiveis.Find(j => j.IdJob == jobId);
                                        if (jobExiste != null)
                                        {
                                            reservaJobCRUD.AddReservaJob(new ReservaJob
                                            {
                                                IdReserva = reservaAlterar.IdReserva,
                                                IdJob = jobId
                                            });
                                            Console.WriteLine($"Job '{jobExiste.Nome}' adicionado!");
                                        }
                                    }
                                }
                            }
                            break;

                        case 2: // Adicionar novos jobs
                            Console.Write("\nDigite os IDs dos jobs para adicionar separados por vírgula: ");
                            string adicionarJobsInput = Console.ReadLine();

                            if (!string.IsNullOrWhiteSpace(adicionarJobsInput))
                            {
                                foreach (var idStr in adicionarJobsInput.Split(','))
                                {
                                    if (int.TryParse(idStr.Trim(), out int jobId))
                                    {
                                        bool jaContratado = jobsAtuais.Any(rj => rj.IdJob == jobId);
                                        if (!jaContratado)
                                        {
                                            var jobExiste = jobsDisponiveis.Find(j => j.IdJob == jobId);
                                            if (jobExiste != null)
                                            {
                                                reservaJobCRUD.AddReservaJob(new ReservaJob
                                                {
                                                    IdReserva = reservaAlterar.IdReserva,
                                                    IdJob = jobId
                                                });
                                                Console.WriteLine($"Job '{jobExiste.Nome}' adicionado!");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Job {jobId} já está contratado!");
                                        }
                                    }
                                }
                            }
                            break;

                        case 3: // Remover jobs específicos
                            Console.Write("\nDigite os IDs dos jobs para remover separados por vírgula: ");
                            string removerJobsInput = Console.ReadLine();

                            if (!string.IsNullOrWhiteSpace(removerJobsInput))
                            {
                                foreach (var idStr in removerJobsInput.Split(','))
                                {
                                    if (int.TryParse(idStr.Trim(), out int jobId))
                                    {
                                        bool removido = jobsAtuais.Any(rj => rj.IdJob == jobId);
                                        if (removido)
                                        {
                                            reservaJobCRUD.DeleteReservaJob(reservaAlterar.IdReserva, jobId);
                                            var jobRemovido = jobsDisponiveis.Find(j => j.IdJob == jobId);
                                            Console.WriteLine($"Job '{jobRemovido?.Nome ?? jobId.ToString()}' removido!");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Job {jobId} não estava contratado!");
                                        }
                                    }
                                }
                            }
                            break;

                        case 0: // Manter jobs atuais
                            Console.WriteLine("Jobs mantidos sem alteração.");
                            break;

                        default:
                            Console.WriteLine("Opção inválida! Jobs mantidos sem alteração.");
                            break;
                    }
                }
            }

            // ========== RECALCULAR TOTAL ==========
            var suiteFinal = suiteCRUD.GetAllSuites().Find(s => s.IdSuite == reservaAlterar.IdSuite);
            decimal totalFinal = suiteFinal?.Preco ?? 0;

            var jobsFinais = reservaJobCRUD.GetAllReservaJobs()
                .FindAll(rj => rj.IdReserva == reservaAlterar.IdReserva);

            foreach (var rj in jobsFinais)
            {
                var job = jobCRUD.GetAllJobs().Find(j => j.IdJob == rj.IdJob);
                if (job != null)
                    totalFinal += job.Tarifa;
            }

            reservaAlterar.Total = totalFinal;

            // ========== SALVAR ALTERAÇÕES ==========
            try
            {
                reservaCRUD.UpdateReserva(reservaAlterar);

                Console.Clear();
                Console.WriteLine("\n=== RESERVA ALTERADA COM SUCESSO! ===\n");
                Console.WriteLine($"Suíte: {suiteFinal?.Nome ?? "Não encontrada"}");
                Console.WriteLine($"Data início: {reservaAlterar.DataInicio:dd/MM/yyyy}");
                Console.WriteLine($"Data fim: {reservaAlterar.DataFim:dd/MM/yyyy}");

                if (jobsFinais.Count > 0)
                {
                    Console.WriteLine("Jobs contratados:");
                    foreach (var rj in jobsFinais)
                    {
                        var job = jobCRUD.GetAllJobs().Find(j => j.IdJob == rj.IdJob);
                        if (job != null)
                            Console.WriteLine($"  - {job.Nome} (R$ {job.Tarifa:F2})");
                    }
                }
                else
                {
                    Console.WriteLine("Jobs contratados: Nenhum");
                }

                Console.WriteLine($"\nNovo total: R$ {totalFinal:F2}");
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErro ao salvar alterações: {ex.Message}");
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

    }
}