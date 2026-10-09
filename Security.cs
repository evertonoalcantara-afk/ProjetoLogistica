using System.Security.Cryptography;
using System.Text;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Essa classe cuida da segurança das senhas.
    //
    // O que é HASH?
    // É tipo uma "impressão digital" da senha. Você pega um texto
    // e transforma em um código grande, sem volta.
    //
    // Exemplo:
    //    "123" vira "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918"
    //
    // Não dá pra "descriptografar" de volta. Pra verificar se a senha
    // tá certa, a gente gera o hash da senha digitada e compara com
    // o hash do banco. Se forem iguais, a senha tá certa.
    //
    // Uso SHA256 porque é nativo do C# e não precisa instalar pacote.
    // O ideal seria BCrypt, mas fica pra próxima fase.
    // ============================================================

    public static class Security
    {
        // Um "sal" é um tempero que a gente junta na senha antes do hash.
        // Isso faz com que senhas iguais gerem hashes diferentes.
        // (Se não usar sal, "123" sempre gera o mesmo hash, o que é ruim.)
        private const string SAL = "sgl_logistica_2026";

        /// <summary>
        /// Gera o hash SHA256 de uma senha (com sal).
        /// </summary>
        public static string GerarHash(string senha)
        {
            if (string.IsNullOrEmpty(senha)) return "";

            using (SHA256 sha = SHA256.Create())
            {
                // junta a senha + o sal e transforma em bytes
                byte[] bytes = Encoding.UTF8.GetBytes(senha + SAL);

                // aqui é onde a mágica acontece
                byte[] hash = sha.ComputeHash(bytes);

                // Converte os bytes pra string hexadecimal legível.
                // StringBuilder é mais rápido que concatenar string com "+".
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Compara uma senha digitada com o hash salvo no banco.
        /// </summary>
        public static bool SenhaConfere(string senhaDigitada, string hashDoBanco)
        {
            string hashDaDigitada = GerarHash(senhaDigitada);
            return hashDaDigitada == hashDoBanco;
        }
    }
}