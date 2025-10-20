using Npgsql;

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
                        DropTodasAsTables(conn);
                        CriarTodasAsTables(conn);
                        InserirDados(conn);
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante configuração: {ex.Message}");
                throw;
            }
        }

        private bool TabelaExiste(NpgsqlConnection conn, string nomeTabela)
        {
            string query = $"SELECT EXISTS(SELECT 1 FROM information_schema.tables WHERE table_name = '{nomeTabela}')";

            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            {
                return (bool)cmd.ExecuteScalar();
            }
        }

        private void DropTodasAsTables(NpgsqlConnection conn)
        {
            string[] tabelas = { "reserva_job", "reserva", "suite", "job", "usuario", "adm" };

            foreach (string tabela in tabelas)
            {
                try
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tabela} CASCADE", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  • Erro ao remover {tabela}: {ex.Message}");
                }
            }
        }

        private void CriarTodasAsTables(NpgsqlConnection conn)
        {
            string sqlCriarTabelas = @"
                CREATE TABLE ADM (
                    idAdm SERIAL PRIMARY KEY,
                    nome VARCHAR(100) NOT NULL,
                    email VARCHAR(150) UNIQUE NOT NULL,
                    senha VARCHAR(100) NOT NULL
                );

                CREATE TABLE USUARIO (
                    idUsuario SERIAL PRIMARY KEY,
                    nome VARCHAR(100) NOT NULL,
                    email VARCHAR(150) UNIQUE NOT NULL,
                    senha VARCHAR(100) NOT NULL
                );

                CREATE TABLE SUITE (
                    idSuite SERIAL PRIMARY KEY,
                    nome VARCHAR(100) NOT NULL,
                    descricao TEXT,
                    preco DECIMAL(10,2) NOT NULL,
                    disponibilidade BOOLEAN DEFAULT TRUE,
                    capacidade INT NOT NULL,
                    idAdm INT NOT NULL,
                    CONSTRAINT fk_suite_adm FOREIGN KEY (idAdm) REFERENCES ADM(idAdm)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE
                );

                CREATE TABLE JOB (
                    idJob SERIAL PRIMARY KEY,
                    nome VARCHAR(100) NOT NULL,
                    descricao TEXT,
                    tarifa DECIMAL(10,2) NOT NULL,
                    disponibilidade BOOLEAN DEFAULT TRUE
                );

                CREATE TABLE RESERVA (
                    idReserva SERIAL PRIMARY KEY,
                    dataInicio TIMESTAMP NOT NULL,
                    dataFim TIMESTAMP NOT NULL,
                    status VARCHAR(50) NOT NULL,
                    total DECIMAL(10,2) NOT NULL,
                    idUsuario INT NOT NULL,
                    idSuite INT NOT NULL,
                    CONSTRAINT fk_reserva_usuario FOREIGN KEY (idUsuario) REFERENCES USUARIO(idUsuario)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE,
                    CONSTRAINT fk_reserva_suite FOREIGN KEY (idSuite) REFERENCES SUITE(idSuite)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE
                );

                CREATE TABLE RESERVA_JOB (
                    idReserva INT NOT NULL,
                    idJob INT NOT NULL,
                    PRIMARY KEY (idReserva, idJob),
                    CONSTRAINT fk_reservajob_reserva FOREIGN KEY (idReserva) REFERENCES RESERVA(idReserva)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE,
                    CONSTRAINT fk_reservajob_job FOREIGN KEY (idJob) REFERENCES JOB(idJob)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE
                );
            ";

            using (NpgsqlCommand cmd = new NpgsqlCommand(sqlCriarTabelas, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void InserirDados(NpgsqlConnection conn)
        {
            string sqlAdm = "INSERT INTO ADM(nome,email,senha) VALUES('Administrador','lab@Database.com','lab@Database2025')";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sqlAdm, conn))
            {
                cmd.ExecuteNonQuery();
            }

            string sqlSuite = @"
                INSERT INTO SUITE (nome, descricao, preco, disponibilidade, capacidade, idAdm) VALUES
                ('Suite com Piscina', 'Suíte luxuosa com piscina privada aquecida, vista para o jardim tropical, banheiro com hidromassagem e varanda ampla com espreguiçadeiras.', 450.00, TRUE, 4, 1),
                ('Suite com Piscina', 'Suíte com piscina de borda infinita, sauna seca, banheiro com ducha tropical e varanda com vista para as montanhas. Capacidade máxima 4 pessoas.', 520.00, TRUE, 4, 1),
                ('Suite com Piscina', 'Suíte elegante com piscina privada de água salgada, deck de madeira, banheiro com banho de vapor e mini bar climatizado.', 480.00, TRUE, 4, 1),
                ('Suite com Piscina', 'Suíte contemporânea com piscina integrada ao quarto, piso aquecido, banheiro em mármore com hidromassagem e área de descanso.', 550.00, TRUE, 4, 1),
                ('Suite com Piscina', 'Suíte aconchegante com piscina climatizada, varanda com jacuzzi, banheiro com rainfall shower e sistema de som ambiente.', 490.00, TRUE, 4, 1),
                ('Suite Premium', 'Suíte de luxo com decoração sofisticada, cama king size, minibar, home theater 4K, banheiro com sauna e varanda com vista panorâmica.', 350.00, TRUE, 2, 1),
                ('Suite Premium', 'Suíte premium com decoração moderna, cama king com colchão de espuma viscoelástica, jaccuzi, home cinema e bar completo.', 420.00, TRUE, 2, 1),
                ('Suite Premium', 'Suíte de alto padrão com mobiliário designer, cama queen pillow top, sauna seca, banheiro com banheira de imersão e varanda privativa.', 380.00, TRUE, 2, 1),
                ('Suite Premium', 'Suíte luxuosa com cortinas motorizadas, cama king size com seda egípcia, minibar premium, sistema de entretenimento completo e área lounge.', 440.00, TRUE, 2, 1),
                ('Suite Premium', 'Suíte elegante e aconchegante, cama pillow-top king, banheiro em mármore travertino, home theater, bar molhado e vista frontal.', 390.00, TRUE, 2, 1),
                ('Suite Básica', 'Suíte confortável e acessível, com cama de casal, ar condicionado, banheiro privativo, TV LED e wifi de alta velocidade.', 150.00, TRUE, 2, 1),
                ('Suite Básica', 'Suíte funcional e bem decorada, cama de casal, ar condicionado central, banheiro com chuveiro manual, TV smart e internet rápida.', 170.00, TRUE, 2, 1),
                ('Suite Básica', 'Suíte simples mas moderna, cama de casal confortável, ar condicionado silencioso, banheiro completo, TV e wifi banda larga.', 160.00, TRUE, 2, 1),
                ('Suite Básica', 'Suíte prática para hóspedes de negócio, cama de casal firme, ar condicionado, banheiro espaçoso, TV e conexão internet 100Mbps.', 155.00, TRUE, 2, 1),
                ('Suite Básica', 'Suíte aconchegante com cama de casal, ar condicionado silencioso, banheiro com produtos de higiene, TV smart e wifi estável.', 165.00, TRUE, 2, 1),
                ('Red Room', 'Suíte temática com iluminação vermelha ambiente, jacuzzi rond, cama redonda com dossel e decoração sensual. Apenas para maiores de 18 anos.', 500.00, TRUE, 2, 1),
                ('Red Room', 'Suíte de experiência especial com iluminação LED vermelha personalizável, banheira redonda, cama circular com cortinas de seda e acessórios temáticos.', 580.00, TRUE, 2, 1),
                ('Red Room', 'Suíte erótica com ambientação vermelha sofisticada, jacuzzi privado, cama redonda espelhada no teto, sistema de som surround e lighting dinâmico.', 620.00, TRUE, 2, 1),
                ('Red Room', 'Suíte temática sensual com iluminação vermelha ajustável, banheira de hidromassagem redonda, cama estilo oriental e decoração exótica.', 550.00, TRUE, 2, 1),
                ('Red Room', 'Suíte de luxo temática com ambiente vermelho elegante, jacuzzi aquecido, cama redonda com almofadas de seda e sistema de entretenimento adulto.', 600.00, TRUE, 2, 1)
            ";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sqlSuite, conn))
            {
                cmd.ExecuteNonQuery();
            }

            string sqlJob = @"
                INSERT INTO JOB (nome, descricao, tarifa, disponibilidade) VALUES
                ('Juliana', 'Acompanhante profissional para companhia e entretenimento na suíte.', 300.00, TRUE),
                ('Fernanda', 'Acompanhante premium com serviços especializados e experiência em atendimento VIP.', 450.00, TRUE),
                ('Amanda', 'Acompanhante para companhia casual na suíte.', 200.00, TRUE),
                ('Beatriz', 'Acompanhante com fantasias e temas personalizados conforme preferência.', 350.00, TRUE),
                ('Carolina', 'Acompanhante discreta e elegante para acompanhamento em eventos e na suíte.', 400.00, TRUE),
                ('Débora', 'Acompanhante exclusiva com reserva garantida e serviço premium.', 550.00, TRUE),
                ('Estela', 'Acompanhante de altíssimo padrão com atendimento personalizado e confidencial.', 650.00, TRUE),
                ('Fabiana', 'Acompanhante com características especiais conforme solicitação.', 380.00, TRUE)
            ";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sqlJob, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}