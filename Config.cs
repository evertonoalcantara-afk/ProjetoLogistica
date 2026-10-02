using System;
using System.IO;
using System.Text.Json;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Essa classe lê o appsettings.json pra pegar dados do banco.
    // A ideia é NÃO deixar a senha e o servidor chumbados no código.
    // Assim, se mudar de PC (tipo do meu pro da facul), só edita o
    // json e pronto. Não precisa nem abrir o Visual Studio.
    //
    // A professora falou que isso se chama "separação de config".
    // ============================================================

    public static class Config
    {
        // Valores padrão (usados se o appsettings.json não existir)
        public static string Servidor = "localhost";
        public static string Banco = "projeto_logistica";
        public static string Usuario = "root";
        public static string Senha = "12345678";

        // Essa propriedade monta a string de conexão no formato
        // que o MySqlConnector entende
        public static string StringConexao
        {
            get
            {
                return "Server=" + Servidor + ";" +
                       "Database=" + Banco + ";" +
                       "Uid=" + Usuario + ";" +
                       "Pwd=" + Senha + ";";
            }
        }

        // O "static Config()" roda automaticamente UMA VEZ,
        // na primeira vez que alguém usar essa classe.
        // É tipo um "construtor" da classe estática.
        static Config()
        {
            try
            {
                string caminho = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

                // Se não achou o arquivo, nem tenta ler
                if (!File.Exists(caminho)) return;

                string conteudo = File.ReadAllText(caminho);

                // JsonDocument é tipo um "leitor de json" do próprio C#
                using (JsonDocument doc = JsonDocument.Parse(conteudo))
                {
                    JsonElement raiz = doc.RootElement;

                    if (raiz.TryGetProperty("BancoDeDados", out JsonElement bd))
                    {
                        if (bd.TryGetProperty("Servidor", out JsonElement s))
                            Servidor = s.GetString();

                        if (bd.TryGetProperty("Banco", out JsonElement b))
                            Banco = b.GetString();

                        if (bd.TryGetProperty("Usuario", out JsonElement u))
                            Usuario = u.GetString();

                        if (bd.TryGetProperty("Senha", out JsonElement p))
                            Senha = p.GetString();
                    }
                }
            }
            catch
            {
                // Se der qualquer ruim na leitura, paciência.
                // Usa os valores padrão e segue o jogo.
            }
        }
    }
}