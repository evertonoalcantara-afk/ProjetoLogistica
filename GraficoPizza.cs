using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ProjetoLogistica
{
    public class GraficoPizza : Control
    {
        // ANOTAÇÃO: Esse atributo abaixo serve pra tirar o aviso WFO1000
        // do Visual Studio. Ele só diz pro designer que a gente não vai
        // editar essa propriedade por lá.
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, decimal> Dados { get; set; } = new Dictionary<string, decimal>();

        
        public GraficoPizza()
        {
            // Deixa o desenho mais suave e sem piscar
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Se não tem dados, mostra mensagem
            if (Dados == null || Dados.Count == 0 || Dados.Values.Sum() == 0)
            {
                using (Font f = new Font("Segoe UI", 11F, FontStyle.Italic))
                using (SolidBrush b = new SolidBrush(Color.Gray))
                {
                    g.DrawString("Sem dados para exibir", f, b, ClientRectangle,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
                return;
            }

            int tamanhoLegenda = 160;
            int diametro = Math.Min(Width - tamanhoLegenda - 40, Height - 40);
            if (diametro < 20) return; // tela muito pequena

            int x = 30;
            int y = (Height - diametro) / 2;
            Rectangle areaPizza = new Rectangle(x, y, diametro, diametro);

            float total = (float)Dados.Values.Sum();
            float anguloInicial = -90f; // Começa no topo (12 horas)
            int index = 0;

            // Cores da nossa paleta pra alternar nas fatias
            Color[] cores = new Color[] {
                Estilo.Indigo, Estilo.Lilas, Estilo.Menta, Estilo.IndigoEscuro, Estilo.LilasEscuro
            };

            foreach (var item in Dados)
            {
                float anguloFatia = 360f * (float)item.Value / total;

                using (SolidBrush b = new SolidBrush(cores[index % cores.Length]))
                    g.FillPie(b, areaPizza, anguloInicial, anguloFatia);

                using (Pen caneta = new Pen(Color.White, 2))
                    g.DrawPie(caneta, areaPizza, anguloInicial, anguloFatia);

                anguloInicial += anguloFatia;
                index++;
            }

            // Desenha a legenda do lado direito
            int legendaX = Width - tamanhoLegenda + 10;
            int legendaY = 40;

            index = 0;
            foreach (var item in Dados)
            {
                // Quadradinho de cor
                using (SolidBrush b = new SolidBrush(cores[index % cores.Length]))
                    g.FillRectangle(b, legendaX, legendaY, 16, 16);

                // Texto
                using (Font f = new Font("Segoe UI", 10F))
                using (SolidBrush b = new SolidBrush(Estilo.TextoEscuro))
                {
                    string texto = $"{item.Key}: {item.Value:N0}";
                    g.DrawString(texto, f, b, legendaX + 24, legendaY);
                }

                legendaY += 28;
                index++;
            }
        }
    }
}