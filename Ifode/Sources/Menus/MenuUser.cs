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
                Console.WriteLine("\n=== MENU USUÁRIO ===");
                Console.WriteLine("1 - Fazer Reserva");
                Console.WriteLine("2 - Consultar Reservas");
                Console.WriteLine("3 - Cancelar Reserva");
                Console.WriteLine("0 - Sair");
                Console.Write("\nEscolha uma opção: ");
                
                if (!int.TryParse(Console.ReadLine(), out opt))
                {
                    Console.WriteLine("Digite um número válido!");
                    continue;
                }

                switch (opt)
                {
                    case 1: 
                        Console.WriteLine("\n=== SUITES DISPONÍVEIS ===");
                        var suites = suiteCRUD.GetAllSuites().FindAll(s => s.Disponibilidade);
                        foreach (var s in suites)
                            Console.WriteLine($"{s.IdSuite} - {s.Nome} (R${s.Preco}, Capacidade: {s.Capacidade})");

                        Console.Write("Escolha o ID da suíte: ");
                        int idSuite = int.Parse(Console.ReadLine());
                        var suiteSelecionada = suites.Find(s => s.IdSuite == idSuite);
                        if (suiteSelecionada == null)
                        {
                            Console.WriteLine("Suíte inválida!");
                            break;
                        }

                        
                        Console.WriteLine("\n=== JOBS DISPONÍVEIS ===");
                        var jobs = jobCRUD.GetAllJobs().FindAll(j => j.Disponibilidade);
                        foreach (var j in jobs)
                            Console.WriteLine($"{j.IdJob} - {j.Nome} (R${j.Tarifa})");

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

                        
                        Console.Write("Data de início (DD-MM-YYYY): ");
                        DateTime dataInicio = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        Console.Write("Data de fim (DD-MM-YYYY): ");
                        DateTime dataFim = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        
                        decimal total = suiteSelecionada.Preco;
                        foreach (var jid in jobsSelecionados)
                        {
                            var job = jobs.Find(j => j.IdJob == jid);
                            if (job != null) total += job.Tarifa;
                        }

                        // Criar reserva
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

                        Console.WriteLine($"Reserva criada com sucesso! Total: R${total}");
                        break;

                    case 2: 
                        Console.WriteLine("\n=== SUAS RESERVAS ===");
                        var minhasReservas = reservaCRUD.GetAllReservas().FindAll(r => r.IdUsuario == idUsuario);
                        foreach (var r in minhasReservas)
                        {
                            var s = suiteCRUD.GetAllSuites().Find(su => su.IdSuite == r.IdSuite);
                            Console.WriteLine($"ID: {r.IdReserva} - Suíte: {s.Nome} - Início: {r.DataInicio} - Fim: {r.DataFim} - Total: R${r.Total} - Status: {r.Status}");
                        }
                        break;

                    case 3: // Cancelar Reserva
                        Console.Write("Digite o ID da reserva que deseja cancelar: ");
                        int idCancel = int.Parse(Console.ReadLine());
                        reservaCRUD.DeleteReserva(idCancel);
                        Console.WriteLine("Reserva cancelada com sucesso!");
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
