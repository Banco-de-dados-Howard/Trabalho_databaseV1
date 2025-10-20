using SistemaReserva.CRUD;
using SistemaReserva.Models;

namespace SistemaReserva
{
    public class MenuAdm
    {
        public static void MostrarMenu(int idAdm)
        {
            var suiteCRUD = new SuiteCRUD();
            var jobCRUD = new JobCRUD();
            var reservaCRUD = new ReservaCRUD();

            int opt;
            do
            {
                Console.WriteLine("\n=== MENU ADMINISTRADOR ===");
                Console.WriteLine("[1] - Cadastrar Suíte");
                Console.WriteLine("[2] - Listar Suítes");
                Console.WriteLine("[3] - Cadastrar Job");
                Console.WriteLine("[4] - Listar Jobs");
                Console.WriteLine("[5] - Gerar relatório das reservas");
                Console.WriteLine("0 - Sair");
                Console.Write("\nEscolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opt))
                {
                    Console.WriteLine("Digite um número válido!");
                    continue;
                }

                switch (opt)
                {
                    case 1: // Cadastrar Suíte
                        var novaSuite = new Suite();
                        Console.Write("Nome da suíte: ");
                        novaSuite.Nome = Console.ReadLine();
                        Console.Write("Descrição: ");
                        novaSuite.Descricao = Console.ReadLine();
                        Console.Write("Preço: ");
                        novaSuite.Preco = decimal.Parse(Console.ReadLine());
                        Console.Write("Capacidade: ");
                        novaSuite.Capacidade = int.Parse(Console.ReadLine());
                        novaSuite.Disponibilidade = true;
                        novaSuite.IdAdm = idAdm;

                        suiteCRUD.AddSuite(novaSuite);
                        Console.WriteLine("Suíte cadastrada com sucesso!");
                        break;

                    case 2: // Listar Suítes
                        Console.WriteLine("\n=== SUITES CADASTRADAS ===");
                        var suites = suiteCRUD.GetAllSuites();
                        foreach (var s in suites)
                            Console.WriteLine($"ID: {s.IdSuite} - {s.Nome} - R${s.Preco} - Capacidade: {s.Capacidade} - Disponível: {s.Disponibilidade}");
                        break;

                    case 3: // Cadastrar Job
                        var novoJob = new Job();
                        Console.Write("Nome do job: ");
                        novoJob.Nome = Console.ReadLine();
                        Console.Write("Descrição: ");
                        novoJob.Descricao = Console.ReadLine();
                        Console.Write("Tarifa: ");
                        novoJob.Tarifa = decimal.Parse(Console.ReadLine());
                        novoJob.Disponibilidade = true;

                        jobCRUD.AddJob(novoJob);
                        Console.WriteLine("Job cadastrado com sucesso!");
                        break;

                    case 4: // Listar Jobs
                        Console.WriteLine("\n=== JOBS CADASTRADOS ===");
                        var jobs = jobCRUD.GetAllJobs();
                        foreach (var j in jobs)
                            Console.WriteLine($"ID: {j.IdJob} - {j.Nome} - R${j.Tarifa} - Disponível: {j.Disponibilidade}");
                        break;

                    case 5: // Relatório de reservas
                        Console.WriteLine("\n=== RELATÓRIO DE RESERVAS ===");
                        var reservas = reservaCRUD.GetAllReservas();
                        foreach (var r in reservas)
                        {
                            var s = suiteCRUD.GetAllSuites().Find(su => su.IdSuite == r.IdSuite);
                            Console.WriteLine($"ID: {r.IdReserva} - Usuário: {r.IdUsuario} - Suíte: {s.Nome} - Início: {r.DataInicio} - Fim: {r.DataFim} - Total: R${r.Total} - Status: {r.Status}");
                        }
                        break;

                    case 0:
                        Console.WriteLine("Saindo...");
                        break;

                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

            } while (opt != 0);
        }
    }
}
