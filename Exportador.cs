using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Essa classe transforma uma DataGridView em arquivo .csv ou .html.
    // O CSV é bom pro Excel. O HTML a gente abre no navegador e
    // pode imprimir/salvar em PDF.
    // ============================================================

    public static class Exportador
    {
        public static void ExportarCSV(DataGridView grid, string titulo)
        {
            if (grid.DataSource == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados para exportar.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Arquivo CSV (*.csv)|*.csv";
                dlg.FileName = titulo + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    // Cabeçalho (nomes das colunas)
                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        sb.Append(grid.Columns[i].HeaderText);
                        if (i < grid.Columns.Count - 1) sb.Append(";");
                    }
                    sb.AppendLine();

                    // Linhas
                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.IsNewRow) continue;

                        for (int i = 0; i < grid.Columns.Count; i++)
                        {
                            string valor = row.Cells[i].Value?.ToString() ?? "";
                            // Se tiver ponto e vírgula no texto, coloca entre aspas
                            if (valor.Contains(";") || valor.Contains("\""))
                                valor = "\"" + valor.Replace("\"", "\"\"") + "\"";

                            sb.Append(valor);
                            if (i < grid.Columns.Count - 1) sb.Append(";");
                        }
                        sb.AppendLine();
                    }

                    // Salva com acentuação correta (UTF-8 com BOM pro Excel reconhecer)
                    File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));

                    Log.Info("Relatório CSV exportado: " + titulo);

                    // Abre a pasta onde o arquivo foi salvo
                    Process.Start("explorer.exe", "/select,\"" + dlg.FileName + "\"");
                }
                catch (Exception ex)
                {
                    Log.Erro("Erro ao exportar CSV", ex);
                    MessageBox.Show("Erro ao salvar arquivo: " + ex.Message, "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void ExportarHTML(DataGridView grid, string titulo)
        {
            if (grid.DataSource == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados para exportar.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Arquivo HTML (*.html)|*.html";
                dlg.FileName = titulo + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".html";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    // Cabeçalho HTML com CSS embutido usando as cores do sistema
                    sb.AppendLine("<!DOCTYPE html>");
                    sb.AppendLine("<html lang='pt-BR'><head><meta charset='UTF-8'>");
                    sb.AppendLine("<title>" + titulo + "</title>");
                    sb.AppendLine("<style>");
                    sb.AppendLine("body { font-family: 'Segoe UI', Arial, sans-serif; background: #EAF9F6; padding: 40px; color: #1F2937; }");
                    sb.AppendLine(".header { display: flex; justify-content: space-between; align-items: center; border-bottom: 3px solid #5C6BC0; padding-bottom: 10px; margin-bottom: 20px; }");
                    sb.AppendLine("h1 { color: #374785; margin: 0; }");
                    sb.AppendLine(".info { color: #5C6BC0; font-size: 14px; text-align: right; }");
                    sb.AppendLine("table { border-collapse: collapse; width: 100%; background: white; box-shadow: 0 4px 12px rgba(0,0,0,0.05); }");
                    sb.AppendLine("th { background: #5C6BC0; color: white; padding: 12px; text-align: left; }");
                    sb.AppendLine("td { padding: 10px 12px; border-bottom: 1px solid #E4DBF3; }");
                    sb.AppendLine("tr:nth-child(even) { background: #F4FBF9; }");
                    sb.AppendLine(".footer { margin-top: 30px; text-align: center; color: #888; font-size: 12px; }");
                    sb.AppendLine("@media print { body { background: white; padding: 0; } table { box-shadow: none; border: 1px solid #ccc; } }");
                    sb.AppendLine("</style></head><body>");

                    sb.AppendLine("<div class='header'>");
                    sb.AppendLine("<h1>🚚 " + titulo + "</h1>");
                    sb.AppendLine("<div class='info'>Gerado em: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "<br>Por: " + Session.Usuario + "</div>");
                    sb.AppendLine("</div>");

                    sb.AppendLine("<table><thead><tr>");
                    foreach (DataGridViewColumn col in grid.Columns)
                    {
                        if (col.Visible) sb.AppendLine("<th>" + col.HeaderText + "</th>");
                    }
                    sb.AppendLine("</tr></thead><tbody>");

                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.IsNewRow) continue;
                        sb.AppendLine("<tr>");
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (grid.Columns[cell.ColumnIndex].Visible)
                            {
                                string val = cell.Value?.ToString() ?? "";
                                sb.AppendLine("<td>" + val + "</td>");
                            }
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.AppendLine("</tbody></table>");

                    sb.AppendLine("<div class='footer'>Rota+ - Sistema de Gerenciamento Logístico</div>");
                    sb.AppendLine("</body></html>");

                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);

                    Log.Info("Relatório HTML exportado: " + titulo);

                    // Abre o arquivo HTML no navegador padrão
                    Process.Start(new ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Log.Erro("Erro ao exportar HTML", ex);
                    MessageBox.Show("Erro ao salvar arquivo: " + ex.Message, "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}