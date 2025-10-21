using System.Text;

namespace SistemaReserva
{
    internal class Program
    {
        static void Main(string[] args)
        {
            testeBanco();

            Console.Clear();
            Console.WriteLine("==== DADOS DO SISTEMA DE RESERVA DE MOTEL ====");
            contagemTabelas cont = new contagemTabelas();
            int[] valores = cont.contar();

            Console.WriteLine();
            Console.WriteLine($"Total de registros diferentes: ");
            Console.WriteLine($"{" ADM",-15} : {valores[0],15}");
            Console.WriteLine($"{" Usuários",-15} : {valores[1],15}");
            Console.WriteLine($"{" Suites",-15} : {valores[2],15}");
            Console.WriteLine($"{" JOBs",-15} : {valores[3],15}");
            Console.WriteLine($"{" Reservas",-15} : {valores[4],15}");
            Console.WriteLine($"{" Reservas_job",-15} : {valores[5],15}");
            Console.WriteLine();

            Console.WriteLine("Feito por: \n - Danton Barbosa\n - Eduardo Guedes\n - Gessiele Lima\n - João Gabriel Bastos\n - Lucas Paton\n - Matheus ALves");
            Console.WriteLine();
            Console.WriteLine("Disciplina - Banco de dados");
            Console.WriteLine("Professor - Howard Roatti");

            int opt = -1;

            while (opt != 0)
            {

                Console.WriteLine("==== MOTEL IFODE ====\n");
                Console.WriteLine("Qual opção deseja :");
                Console.WriteLine("[1] - Fazer login");
                Console.WriteLine("[2] - Fazer seu cadastro");
                Console.WriteLine("[0] - Sair do sistema");
                Console.Write("Opção: ");

                if (!int.TryParse(Console.ReadLine(), out opt))
                {
                    Console.WriteLine("Opção inválida!\n");
                    continue;
                }

                switch (opt)
                {
                    case 1:
                        Console.Write("Email: ");
                        string emailLog = Console.ReadLine();

                        Console.Write("Senha: ");
                        var senhaLog = lerSenha();
                        Console.WriteLine();
                        Console.CursorVisible = true;

                        verificarLogin(emailLog, senhaLog);
                        break;

                    case 2:
                        Console.Write("Digite seu nome: ");
                        string nomeCad = Console.ReadLine();
                        Console.Write("Digite seu email: ");
                        string emailCad = Console.ReadLine();
                        Console.Write("Digite sua senha: ");
                        var senhaCad = lerSenha();

                        int idUsuario = UsuarioLogin.CadastrarUsuario(nomeCad, emailCad, senhaCad);
                        Console.WriteLine($"\nCadastro realizado com sucesso! Seu ID de usuário: {idUsuario}");
                        MenuUser.MostrarMenu(idUsuario);
                        break;

                    case 0:
                        Console.WriteLine("Saindo do sistema...");
                        break;

                    default:
                        Console.WriteLine("Opção desconhecida...\n");
                        break;
                }
                Console.Clear();
            }
        }

        public static void verificarLogin(string email, string senha)
        {
            int idAdm = AdmLogin.ObterIdAdm(email, senha);
            int idUsuario = UsuarioLogin.ObterIdUsuario(email, senha);

            if (idAdm != 0)
            {
                Console.WriteLine("Login de administrador realizado com sucesso!\n");
                MenuAdm.MostrarMenu(idAdm);
            }
            else if (idUsuario != 0)
            {
                Console.WriteLine("Login de usuário realizado com sucesso!\n");
                MenuUser.MostrarMenu(idUsuario);
            }
            else
            {
                Console.WriteLine("Login não encontrado no sistema.");
                Console.Write("Deseja se cadastrar como novo usuário? (s/n): ");
                string resp = Console.ReadLine().Trim().ToLower();

                if (resp == "s" || resp == "sim")
                {
                    Console.Write("\nDigite seu nome completo: ");
                    string nome = Console.ReadLine();
                    int novoId = UsuarioLogin.CadastrarUsuario(nome, email, senha);
                    Console.WriteLine($"\nCadastro realizado com sucesso! Seu ID: {novoId}");
                    MenuUser.MostrarMenu(novoId);
                }
                else
                {
                    Console.WriteLine("\nEncerrando o sistema...");
                }
            }
        }

        static string lerSenha()
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write('*');
                }
            }

            return sb.ToString();
        }

        static void testeBanco()
        {
            ConfigBasica config = new ConfigBasica();
            config.ExecutarConfiguracaoInicial();
        }
    }
}
