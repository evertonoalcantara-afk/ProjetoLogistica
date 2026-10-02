using System.Drawing;
using System.Drawing.Text;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Ao invés de baixar arquivo .png de ícone pela internet,
    // a gente usa uma fonte de ícones que já vem no Windows
    // chamada "Segoe MDL2 Assets".
    //
    // A ideia é: cada "código" tipo \uE74E é uma letrinha invisível
    // que quando desenhada com ESSA fonte, vira um desenho.
    //
    // O método Desenhar() pega o código, desenha numa imagenzinha
    // pequena e devolve ela pra usar no botão.
    //
    // Dica: a prof falou que a gente pode usar o site
    // https://learn.microsoft.com/en-us/windows/apps/design/style/segoe-ui-symbol-font
    // pra achar outros códigos de ícone se precisar.
    // ============================================================

    public static class Icones
    {
        // Essa função recebe o "glyph" (código do ícone), a cor e o tamanho
        // e devolve uma Image pronta pra colocar no botão
        public static Image Desenhar(string glyph, Color cor, int tamanho = 22)
        {
            Bitmap bmp = new Bitmap(tamanho, tamanho);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // fundo transparente (senão fica quadrado branco atrás)
                g.Clear(Color.Transparent);

                // deixa o desenho mais nítido
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Aqui cria a fonte no tamanho certo e o "pincel" da cor
                using (Font font = new Font("Segoe MDL2 Assets", tamanho - 6,
                                            FontStyle.Regular, GraphicsUnit.Pixel))
                using (SolidBrush brush = new SolidBrush(cor))
                {
                    // Descobre o tamanho que o ícone vai ocupar
                    SizeF size = g.MeasureString(glyph, font);

                    // Desenha centralizado na imagenzinha
                    g.DrawString(glyph, font, brush,
                        (tamanho - size.Width) / 2f,
                        (tamanho - size.Height) / 2f);
                }
            }

            return bmp;
        }

        // ============================================================
        // 📚 A LISTA DOS CÓDIGOS
        // Cada "\uXXXX" é um ícone diferente. A gente deu nomes
        // em português pra ficar fácil de usar tipo: Icones.Salvar
        // ============================================================
        public const string Salvar = "\uE74E"; // 💾 disquete
        public const string Editar = "\uE70F"; // ✏️ lápis
        public const string Excluir = "\uE74D"; // 🗑️ lixeira
        public const string Pesquisar = "\uE721"; // 🔍 lupa
        public const string Novo = "\uE710"; // ➕ mais
        public const string Limpar = "\uE894"; // 🧹 vassoura
        public const string Atualizar = "\uE72C"; // 🔄 girar
        public const string Login = "\uE77B"; // 👤 pessoa
        public const string Entrar = "\uE7EE"; // ➡️ setinha entrando (ícone de login de verdade)
        public const string Sair = "\uE7E8"; // 🚪 porta
        public const string Filtro = "\uE71C"; // 🎛️ filtro
        public const string Caminhao = "\uE806"; // 🚛 caminhão
        public const string Pessoa = "\uE77B"; // 👤 pessoa
        public const string Mapa = "\uE707"; // 🗺️ mapa
        public const string Combust = "\uE945"; // ⛽ combustível
        public const string Caixa = "\uE7B8"; // 📦 caixa
        public const string Home = "\uE80F"; // 🏠 casa
        public const string Back = "\uE72B"; // ⬅️ voltar
        public const string Busca = "\uE721"; // 🔍 lupa (mesmo do pesquisar)
    }
}