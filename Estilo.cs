using System.Drawing;
using System.Windows.Forms;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO DA AULA:
    // Essa classe é tipo o "painel de controle" das cores do sistema.
    // Se a gente quiser mudar a cor do sistema inteiro depois, é só mexer aqui.
    // Todos os botões, telas e grids vão puxar as cores daqui.
    //
    // Dica da prof: "nunca deixe cor fixa espalhada pelo código, centralize"
    // ============================================================

    public static class Estilo
    {
        // ============================================================
        // 🎨 AS CORES (menta + lilás + índigo)
        // A gente escolheu essas 3 famílias e fez 5 tons de cada uma,
        // pra poder fazer degradezinho e não ficar tudo na mesma cor.
        // ============================================================

        // 💚 MENTA (da mais clarinha → mais escurinha)
        public static readonly Color MentaBemClaro = Color.FromArgb(224, 247, 243); // quase branco, pro fundo
        public static readonly Color MentaClaro = Color.FromArgb(184, 237, 227); // pros itens de baixo
        public static readonly Color Menta = Color.FromArgb(152, 228, 214); // a cor "base"
        public static readonly Color MentaMedio = Color.FromArgb(110, 207, 184); // pra destaque
        public static readonly Color MentaEscuro = Color.FromArgb(77, 179, 154);  // pra borda

        // 💜 LILÁS (mesma lógica, 5 tons)
        public static readonly Color LilasBemClaro = Color.FromArgb(238, 232, 248);
        public static readonly Color LilasClaro = Color.FromArgb(228, 219, 243);
        public static readonly Color Lilas = Color.FromArgb(177, 156, 217);
        public static readonly Color LilasMedio = Color.FromArgb(155, 130, 203);
        public static readonly Color LilasEscuro = Color.FromArgb(123, 95, 174);

        // 🔷 ÍNDIGO (o mais escuro, é a cor "principal")
        public static readonly Color IndigoBemClaro = Color.FromArgb(232, 234, 246);
        public static readonly Color IndigoClaro = Color.FromArgb(197, 202, 233);
        public static readonly Color Indigo = Color.FromArgb(92, 107, 192);
        public static readonly Color IndigoMedio = Color.FromArgb(74, 90, 174);
        public static readonly Color IndigoEscuro = Color.FromArgb(55, 71, 133);

        // Cores neutras pro texto (não é preto puro, é um cinza azulado)
        public static readonly Color TextoEscuro = Color.FromArgb(31, 41, 55);
        public static readonly Color TextoClaro = Color.FromArgb(255, 255, 255);

        // ============================================================
        // 🛠️ OS MÉTODOS AJUDANTES
        // A ideia é: em vez de ficar configurando botão por botão em cada tela,
        // a gente chama Estilo.Botao(btn, cor) e ele já deixa bonito.
        // ============================================================

        // Aplica o estilo padrão num botão
        public static void Botao(Button b, Color cor)
        {
            b.FlatStyle = FlatStyle.Flat;         // sem borda 3D
            b.FlatAppearance.BorderSize = 0;      // sem contorno
            b.BackColor = cor;
            b.ForeColor = Color.White;            // texto branco
            b.Font = new Font("Segoe UI Semibold", 10F);
            b.Cursor = Cursors.Hand;              // vira maozinha
            b.Height = 38;

            // Quando passa o mouse, a cor fica mais clara
            b.FlatAppearance.MouseOverBackColor = Clarear(cor, 20);

            // Quando clica, fica mais escura
            b.FlatAppearance.MouseDownBackColor = Escurecer(cor, 20);
        }

        // Aplica o estilo padrão numa caixa de texto
        public static void TextBox(TextBox t)
        {
            t.Font = new Font("Segoe UI", 11F);
            t.BorderStyle = BorderStyle.FixedSingle;
            t.BackColor = Color.White;
            t.ForeColor = TextoEscuro;
        }

        // Aplica o estilo padrão numa grid (tabela)
        public static void Grid(DataGridView g)
        {
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.GridColor = LilasClaro;
            g.EnableHeadersVisualStyles = false; // pra poder customizar o cabeçalho

            // Cabeçalho índigo com texto branco
            g.ColumnHeadersDefaultCellStyle.BackColor = Indigo;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Indigo;
            g.ColumnHeadersHeight = 36;

            // As linhas normais
            g.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            g.DefaultCellStyle.BackColor = Color.White;
            g.DefaultCellStyle.ForeColor = TextoEscuro;
            g.DefaultCellStyle.SelectionBackColor = Menta;
            g.DefaultCellStyle.SelectionForeColor = TextoEscuro;
            g.RowTemplate.Height = 30;

            // Zebra (linha sim, linha não) - ajuda MUITO a ler tabela grande
            g.AlternatingRowsDefaultCellStyle.BackColor = MentaBemClaro;

            // Comportamento padrão (não deixa o usuário editar direto na grid)
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
        }

        // ============================================================
        // 🎨 Funções auxiliares que clareiam/escurecem uma cor
        // A prof explicou que cor no C# é RGB (vermelho, verde, azul),
        // aí dá pra somar ou subtrair de cada canal pra mudar o tom.
        // ============================================================

        // Soma o valor em cada canal = cor mais clara
        public static Color Clarear(Color cor, int quanto)
        {
            int r = Math.Min(255, cor.R + quanto);
            int g = Math.Min(255, cor.G + quanto);
            int b = Math.Min(255, cor.B + quanto);
            return Color.FromArgb(r, g, b);
        }

        // Subtrai o valor em cada canal = cor mais escura
        public static Color Escurecer(Color cor, int quanto)
        {
            int r = Math.Max(0, cor.R - quanto);
            int g = Math.Max(0, cor.G - quanto);
            int b = Math.Max(0, cor.B - quanto);
            return Color.FromArgb(r, g, b);
        }
    }
}