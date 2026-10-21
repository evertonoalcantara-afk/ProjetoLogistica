namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Guarda quem tá logado no sistema. Como é "static", os dados
    // ficam disponíveis em qualquer tela sem precisar passar
    // por parâmetro em cada método.
    //
    // Uso:
    //   Session.Usuario = "admin";
    //   Session.Perfil  = "Admin";
    //   if (Session.IsAdmin) { ... }
    // ============================================================

    public static class Session
    {
        public static string Usuario { get; set; } = "";
        public static string Perfil { get; set; } = "Operador";

        public static bool IsAdmin
        {
            get { return Perfil == "Admin"; }
        }
    }
}