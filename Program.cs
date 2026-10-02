namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Esse é o ponto de partida do programa. É tipo a "porta de entrada".
    // Quando o usuário abre o .exe, o Windows chama esse Main() aqui.
    //
    // A gente só diz 2 coisas:
    //   1) Configure o WinForms (isso é automático, deixa padrão)
    //   2) Abra a tela de Login
    // ============================================================

    internal static class Program
    {
        [STAThread] // essa linha é obrigatória pra coisas de Windows Forms
        static void Main()
        {
            // Configuração automática do .NET 8 pro WinForms rodar certo
            ApplicationConfiguration.Initialize();

            // Abre a tela de login (é a primeira tela do sistema)
            Application.Run(new FrmLogin());
        }
    }
}