using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ProjetoLogistica
{
    public class GraficoBarras : Control
    {
        // ANOTAÇÃO: Mesma coisa do GraficoPizza. Tira o aviso WFO1000.
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, decimal> Dados { get; set; } = new Dictionary<string, decimal>();

        
        public GraficoBarras()
        {
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

            if (Dados == null || Dados.Count == 0 || Dados.Values.Max() == 0)
            {
                using (Font f = new Font("Segoe UI", 11F, FontStyle.Italic))
                using (SolidBrush b = new SolidBrush(Color.Gray))
                {
                    g.DrawString("Sem dados para exibir", f, b, ClientRectangle,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
                return;
            }

            // Configurações de espaço
            int margemBaixo = 40;
            int margemCima = 30;
            int margemEsq = 40;
            int margemDir = 20;

            int areaLargura = Width - margemEsq - margemDir;
            int areaAltura = Height - margemCima - margemBaixo;

            decimal valorMaximo = Dados.Values.Max();
            int qtdBarras = Dados.Count;

            // Calcula a largura de cada barra e o espaço entre elas
            int espacoTotal = areaLargura / qtdBarras;
            int larguraBarra = (int)(espacoTotal * 0.6); // Barra ocupa 60% do espaço
            int espacoEntre = espacoTotal - larguraBarra;

            int index = 0;
            foreach (var item in Dados)
            {
                // Calcula altura proporcional da barra
                int alturaBarra = (int)(areaAltura * (double)(item.Value / valorMaximo));
                int x = margemEsq + (index * espacoTotal) + (espacoEntre / 2);
                int y = margemCima + areaAltura - alturaBarra;

                Rectangle rectBarra = new Rectangle(x, y, larguraBarra, alturaBarra);

                // Desenha a barra com um degradê bonito
                if (alturaBarra > 0)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        rectBarra, Estilo.Indigo, Estilo.Lilas, 90f))
                    {
                        g.FillRectangle(brush, rectBarra);
                    }
                }

                // Desenha o valor em cima da barra
                using (Font f = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (SolidBrush b = new SolidBrush(Estilo.TextoEscuro))
                {
                    string textoValor = item.Value.ToString("N0");
                    SizeF tam = g.MeasureString(textoValor, f);
                    g.DrawString(textoValor, f, b, x + (larguraBarra - tam.Width) / 2, y - 20);
                }

                // Desenha o rótulo embaixo
                using (Font f = new Font("Segoe UI", 8.5F))
                using (SolidBrush b = new SolidBrush(Estilo.TextoEscuro))
                {
                    string textoLabel = item.Key;
                    SizeF tam = g.MeasureString(textoLabel, f);
                    g.DrawString(textoLabel, f, b, x + (larguraBarra - tam.Width) / 2, Height - margemBaixo + 10);
                }

                index++;
            }

            // Desenha a linha base (eixo X)
            using (Pen caneta = new Pen(Color.LightGray, 2))
            {
                g.DrawLine(caneta, margemEsq, Height - margemBaixo, Width - margemDir, Height - margemBaixo);
            }
        }
    }
}