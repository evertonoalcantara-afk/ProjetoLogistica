using System;
using System.IO;
using System.Text;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Essa classe grava os eventos importantes do sistema em
    // arquivos de texto, dentro de uma pasta "Logs".
    //
    // Um arquivo por dia: log_2026-10-09.txt
    //
    // Serve pra rastrear quem fez login e quando, registrar erros
    // pra debug e ter histórico de atividades (auditoria).
    // ============================================================

    public static class Log
    {
        // Essa trava evita que duas linhas se misturem no arquivo
        // caso duas partes do sistema tentem escrever ao mesmo tempo.
        private static readonly object _trava = new object();

        public static void Info(string mensagem)
        {
            Escrever("INFO", mensagem);
        }

        public static void Erro(string mensagem, Exception ex = null)
        {
            string texto = mensagem;
            if (ex != null)
            {
                texto = texto + " | Detalhe: " + ex.Message;
            }
            Escrever("ERRO", texto);
        }

        private static void Escrever(string tipo, string mensagem)
        {
            try
            {
                // Cria a pasta "Logs" se ainda não existir
                string pasta = Path.Combine(AppContext.BaseDirectory, "Logs");
                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(pasta);
                }

                // Um arquivo por dia
                string arquivo = Path.Combine(pasta,
                    "log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

                string linha = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " +
                               "[" + tipo + "] " +
                               mensagem +
                               Environment.NewLine;

                lock (_trava)
                {
                    File.AppendAllText(arquivo, linha, Encoding.UTF8);
                }
            }
            catch
            {
                // Se nem o log der certo, ignora.
            }
        }
    }
}