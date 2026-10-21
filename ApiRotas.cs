using System;
using System.Globalization; // Necessário pra forçar o ponto decimal
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjetoLogistica
{
    // ============================================================
    // ANOTAÇÃO:
    // Essa classe conversa com a internet pra descobrir a distância.
    // 
    // ERRO CORRIGIDO: O erro "400 Bad Request" acontecia porque o 
    // Windows em português formata números com VÍRGULA (ex: 23,5).
    // Mas a API do OSRM só aceita PONTO (ex: 23.5).
    // Solução: usar CultureInfo.InvariantCulture para forçar o ponto.
    // ============================================================

    public static class ApiRotas
    {
        private static readonly HttpClient _http = new HttpClient();

        static ApiRotas()
        {
            _http.DefaultRequestHeaders.Add("User-Agent", "RotaMaisApp_Faculdade");
            _http.Timeout = TimeSpan.FromSeconds(15);
        }

        public static async Task<double> CalcularDistanciaAsync(string origem, string destino)
        {
            try
            {
                var coordOrigem = await BuscarCoordenadasAsync(origem);
                var coordDestino = await BuscarCoordenadasAsync(destino);

                if (coordOrigem == null || coordDestino == null)
                    return 0;

                // AQUI ESTÁ A CORREÇÃO! Formatando com ponto em vez de vírgula:
                string lonOrigem = coordOrigem.Value.Lon.ToString(CultureInfo.InvariantCulture);
                string latOrigem = coordOrigem.Value.Lat.ToString(CultureInfo.InvariantCulture);
                string lonDestino = coordDestino.Value.Lon.ToString(CultureInfo.InvariantCulture);
                string latDestino = coordDestino.Value.Lat.ToString(CultureInfo.InvariantCulture);

                // Montando a URL com os pontos certinhos
                string urlRota = $"https://router.project-osrm.org/route/v1/driving/" +
                                 $"{lonOrigem},{latOrigem};{lonDestino},{latDestino}?overview=false";

                string jsonRota = await _http.GetStringAsync(urlRota);

                using (JsonDocument doc = JsonDocument.Parse(jsonRota))
                {
                    double metros = doc.RootElement
                                       .GetProperty("routes")[0]
                                       .GetProperty("distance").GetDouble();

                    return Math.Round(metros / 1000, 2);
                }
            }
            catch (Exception ex)
            {
                // Deixei o erro visível pra caso aconteça outra coisa
                System.Windows.Forms.MessageBox.Show(
                    "Erro na API: " + ex.Message,
                    "Debug",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return 0;
            }
        }

        private static async Task<(double Lat, double Lon)?> BuscarCoordenadasAsync(string cidade)
        {
            try
            {
                string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cidade)}&format=json&limit=1";
                string json = await _http.GetStringAsync(url);

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement raiz = doc.RootElement;

                    if (raiz.GetArrayLength() == 0) return null;

                    JsonElement primeiro = raiz[0];

                    double lat = double.Parse(primeiro.GetProperty("lat").GetString(), CultureInfo.InvariantCulture);
                    double lon = double.Parse(primeiro.GetProperty("lon").GetString(), CultureInfo.InvariantCulture);

                    return (lat, lon);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}