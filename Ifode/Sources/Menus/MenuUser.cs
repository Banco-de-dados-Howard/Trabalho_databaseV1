namespace SistemaReserva
{
    public class MenuUser
    {
        public static void MostrarMenu()
        {
            Console.WriteLine("\n=== MENU USUÁRIO ===");
            Console.WriteLine("1 - Fazer Reserva");
            Console.WriteLine("2 - Consultar Reservas");
            Console.WriteLine("3 - Cancelar Reserva");
            Console.WriteLine("0 - Sair");
            Console.Write("\nEscolha uma opção: ");
            int opt = int.Parse(Console.ReadLine());

            switch (opt)
            {
                case 1:
                    Console.WriteLine("Função de fazer reserva...");
                    break;
                case 2:
                    Console.WriteLine("Função de consultar reservas...");
                    break;
                case 3:
                    Console.WriteLine("Função de cancelar reserva...");
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