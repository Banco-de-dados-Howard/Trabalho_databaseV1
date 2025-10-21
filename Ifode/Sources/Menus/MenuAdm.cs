using SistemaReserva.CRUD;
using SistemaReserva.Models;
using SistemaReserva;

namespace SistemaReserva
{
    public class MenuAdm
    {
        public static void MostrarMenu(int idAdm)
        {
            var suiteCRUD = new SuiteCRUD();
            var jobCRUD = new JobCRUD();

            int opt;
            do
            {
                Console.Clear();
                Console.WriteLine("\n=== MENU ADMINISTRADOR ===");
                Console.WriteLine("[1] - Cadastrar Suíte");
                Console.WriteLine("[2] - Listar Suítes");
                Console.WriteLine("[3] - Cadastrar Job");
                Console.WriteLine("[4] - Listar Jobs");
                Console.WriteLine("[5] - Relatório de Reservas");
                Console.WriteLine("[6] - Relatório Completo ");
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
                        int opt1 = 0;
                        while (opt1 == 0)
                        {
                            Console.Clear();
                            var novaSuite = new Suite();
                            Console.WriteLine("\n=== CADASTRAR SUÍTE ===\n");
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
                            Console.WriteLine("\nSuíte cadastrada com sucesso!");

                            Console.WriteLine("Deseja cadastrar mais uma suite ? \n[1] sim [0] não");

                            opt1 = Int32.Parse(Console.ReadLine());
                        }

                        break;

                    case 2:
                        var suites = suiteCRUD.GetAllSuites();
                        int paginaSuite = 0;
                        int itensPorPaginaSuite = 5;

                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("\n=== SUITES CADASTRADAS ===\n");

                            int inicioSuite = paginaSuite * itensPorPaginaSuite;
                            int fimSuite = inicioSuite + itensPorPaginaSuite;

                            for (int i = inicioSuite; i < fimSuite && i < suites.Count; i++)
                            {
                                var s = suites[i];
                                Console.WriteLine($"{s.Nome}:");
                                Console.WriteLine($"  * ID: {s.IdSuite}");
                                Console.WriteLine($"  * Descrição: {s.Descricao}");
                                Console.WriteLine($"  * Preço: R$ {s.Preco:F2}");
                                Console.WriteLine($"  * Capacidade: {s.Capacidade} pessoa(s)");
                                Console.WriteLine($"  * Disponível: {(s.Disponibilidade ? "Sim" : "Não")}");
                                Console.WriteLine();
                            }

                            Console.WriteLine("[1] Anterior | [2] Próxima | [0] Voltar");
                            Console.Write("Opção: ");

                            if (!int.TryParse(Console.ReadLine(), out int opSuite))
                            {
                                continue;
                            }

                            if (opSuite == 1 && paginaSuite > 0) paginaSuite--;
                            else if (opSuite == 2 && fimSuite < suites.Count) paginaSuite++;
                            else if (opSuite == 0) break;
                        }
                        break;

                    case 3:

                        int opt3 = 0;
                        while (opt3 == 0)
                        {
                            Console.Clear();
                            var novoJob = new Job();
                            Console.WriteLine("\n=== CADASTRAR JOB ===\n");
                            Console.Write("Nome do job: ");
                            novoJob.Nome = Console.ReadLine();
                            Console.Write("Descrição: ");
                            novoJob.Descricao = Console.ReadLine();
                            Console.Write("Tarifa: ");
                            novoJob.Tarifa = decimal.Parse(Console.ReadLine());
                            novoJob.Disponibilidade = true;

                            jobCRUD.AddJob(novoJob);

                            Console.WriteLine("\nJob cadastrado com sucesso!");
                            Console.WriteLine("Deseja cadastrar mais um companhante ? \n[1] sim [0] não");
                            opt3 = Int32.Parse(Console.ReadLine());
                        }
                        break;

                    case 4:
                        Relatorios.RelatorioJobs();
                        break;

                    case 5:
                        Relatorios.RelatorioReservas();
                        break;

                    case 6:
                        Relatorios.RelatorioCompleto();
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
