

namespace SistemaReserva
{
    public class MenuAdm
    {
        public static void MostrarMenu()
        {
            Console.WriteLine("\n=== MENU ADMINISTRADOR ===");
            Console.WriteLine("[1] - Cadastrar Suíte");
            Console.WriteLine("[2] - Listar Suítes");
            Console.WriteLine("[3] - Cadastrar Job");
            Console.WriteLine("[4] - Listar Jobs");
            Console.WriteLine("[5] - gerar relatório das reservas");
            Console.WriteLine("0 - Sair");
            Console.Write("\nEscolha uma opção: ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1:
                    Console.WriteLine("Função de cadastrar suíte...");
                    break;
                case 2:
                    Console.WriteLine("Função de listar suítes...");
                    break;
                case 3:
                    Console.WriteLine("Função de cadastrar job...");
                    break;
                case 4:
                    Console.WriteLine("Função de listar jobs...");
                    break;
                case 5:
                    Console.WriteLine("Função de gerar relatório do mês...");
                    break;
                case 0:
                    Console.WriteLine("Saindo...");
                    break;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }
    }
}