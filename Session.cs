namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Guarda as informações do usuário que tá logado no momento.
    // Assim qualquer tela do sistema consegue saber "quem tá usando".
    //
    // Serve pra:
    //   - Mostrar o nome do usuário no menu superior
    //   - Esconder botões que só Admin pode ver
    //   - Registrar no log quem fez cada ação
    // ============================================================

    public static class Session
    {
        // Login do usuário logado (ex: "admin", "joao")
        public static string Usuario { get; set; } = "";

        // Perfil: "Admin" ou "Operador"
        public static string Perfil { get; set; } = "Operador";

        // Atalho pra saber se é Admin (facilita muito no código)
        // Uso: if (Session.IsAdmin) { ... }
        public static bool IsAdmin
        {
            get { return Perfil == "Admin"; }
        }
    }
}