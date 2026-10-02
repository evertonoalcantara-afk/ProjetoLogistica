using MySqlConnector;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Essa classe tem UM método: Abrir().
    // Ele devolve uma conexão já aberta com o MySQL.
    //
    // Como usar:
    //   using (var conn = Conexao.Abrir())
    //   {
    //       // faz as paradas aqui dentro
    //   }
    //
    // O "using" garante que a conexão FECHA sozinha no final.
    // Se esquecer o using, a conexão fica aberta e trava o banco.
    // ============================================================

    public static class Conexao
    {
        public static MySqlConnection Abrir()
        {
            // A string de conexão vem do Config (que lê o appsettings.json)
            MySqlConnection conn = new MySqlConnection(Config.StringConexao);
            conn.Open();
            return conn;
        }
    }
}