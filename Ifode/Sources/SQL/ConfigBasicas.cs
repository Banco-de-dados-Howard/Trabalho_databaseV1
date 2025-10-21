using Npgsql;
using System.IO;

namespace SistemaReserva
{
    public class ConfigBasica
    {
        private readonly Conection conection;

        public ConfigBasica()
        {
            this.conection = new Conection();
        }

        public void ExecutarConfiguracaoInicial()
        {
            try
            {
                using (NpgsqlConnection conn = conection.getConn())
                {
                    bool admExists = TabelaExiste(conn, "adm");
                    bool reservaJobExists = TabelaExiste(conn, "reserva_job");

                    if (!admExists || !reservaJobExists)
                    {
                        ExecutarScriptSQL(conn);
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro na configuração inicial: {ex.Message}", ex);
            }
        }

        private bool TabelaExiste(NpgsqlConnection conn, string nomeTabela)
        {
            try
            {
                string query = $"SELECT EXISTS(SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = '{nomeTabela.ToLower()}')";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    return result != null && (bool)result;
                }
            }
            catch
            {
                return false;
            }
        }

        private void ExecutarScriptSQL(NpgsqlConnection conn)
        {
            string caminhoScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Banco.sql");

            if (!File.Exists(caminhoScript))
            {
                caminhoScript = Path.Combine(Directory.GetCurrentDirectory(), "Banco.sql");

                if (!File.Exists(caminhoScript))
                {
                    throw new FileNotFoundException($"Arquivo Banco.sql não encontrado. Procurado em: {caminhoScript}");
                }
            }

            string scriptSQL = File.ReadAllText(caminhoScript);

            // Remove comentários de bloco /* */
            scriptSQL = System.Text.RegularExpressions.Regex.Replace(scriptSQL, @"/\*.*?\*/", "", System.Text.RegularExpressions.RegexOptions.Singleline);

            string[] comandos = scriptSQL.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string comando in comandos)
            {
                string comandoLimpo = comando.Trim();

                // Remove comentários de linha --
                var linhas = comandoLimpo.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                comandoLimpo = string.Join("\n", linhas.Where(l => !l.TrimStart().StartsWith("--")));

                if (string.IsNullOrWhiteSpace(comandoLimpo))
                {
                    continue;
                }

                try
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(comandoLimpo, conn))
                    {
                        cmd.CommandTimeout = 120;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // Se for um erro crítico de DROP/CREATE, propaga o erro
                    if (comandoLimpo.ToUpper().Contains("CREATE TABLE") || 
                        comandoLimpo.ToUpper().Contains("DROP TABLE"))
                    {
                        throw new Exception($"Erro ao executar comando SQL: {ex.Message}\nComando: {comandoLimpo.Substring(0, Math.Min(200, comandoLimpo.Length))}...", ex);
                    }
                }
            }
        }
    }
}