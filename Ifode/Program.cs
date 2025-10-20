using System.Text;
using SistemaReserva.CRUD;

namespace SistemaReserva
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int opt = -1;

            while(opt != 0){
                Console.WriteLine("==== SISTEMA DE RESERVA DE MOTEL ====\n");
                Console.WriteLine("Qual opção deseja :");
                Console.WriteLine("[1] - Fazer login");
                Console.WriteLine("[2] - Fazer seu cadastro");
                Console.WriteLine("[0] - Sair do sistema");
                opt = Int32.Parse(Console.ReadLine());
                switch(opt){
                    case 1:
                        Console.Write("Email: ");
                        string emailLog = Console.ReadLine();

                        Console.Write("Senha: ");
                        var senhaLog = lerSenha();
                        Console.WriteLine();
                        Console.CursorVisible = true;

                        verificarLogin(emailLog, senhaLog.ToString());
                        break;
                    case 2:
                        Console.WriteLine("Digite seu nome: ");
                        String nomeCad = Console.ReadLine();
                        Console.WriteLine("Digite seu email:");
                        String emailCad = Console.ReadLine();
                        Console.WriteLine("Digite sua senha: ");
                        var senhaCad = lerSenha();

                        UsuarioLogin.CadastrarUsuario(nomeCad,emailCad,senhaCad);
                        break;
                    case 0:
                        Console.WriteLine("Saindo do sistema...");
                        break;
                    default:
                        Console.WriteLine("Opção desconhecida...");
                break;
                }
            }
        }


        public static void verificarLogin(String email, String senha)
        {

            if (AdmLogin.VerificarLogin(email, senha))
            {
                Console.WriteLine("Login de administrador realizado com sucesso!\n");
                MenuAdm.MostrarMenu();

            }
            else if (UsuarioLogin.VerificarLogin(email, senha))
            {
                Console.WriteLine("Login de usuário realizado com sucesso!\n");
                MenuUser.MostrarMenu();
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

                    UsuarioLogin.CadastrarUsuario(nome, email, senha);
                }
                else
                {
                    Console.WriteLine("\nEncerrando o sistema...");
                }
            }
        }
        
        static string lerSenha(){
            var sb = new StringBuilder();
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(intercept: true); // intercept = true evita mostrar a tecla

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        // apaga o último '*' na tela
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    sb.Append(key.KeyChar);
                    Console.Write('*'); // mostra asterisco em vez do caractere real
                }
            }

            return sb.ToString();
        }
    }
}
