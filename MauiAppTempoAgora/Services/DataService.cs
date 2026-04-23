using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new Exception("Sem conexão com a internet.");
            }
            Tempo? t = null;

            string chave = "7ba260bbffd9bb0cad06ca02f2842409";
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                $"q={cidade}&units=metric&lang=pt_br&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Cidade não encontrada.");
                }

                // ❌ Outros erros
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception("Erro ao buscar dados do clima.");
                }

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();

                    var rascunho = JObject.Parse(json);

                    DateTime sunrise = DateTimeOffset
                     .FromUnixTimeSeconds((long)rascunho["sys"]["sunrise"])
                     .LocalDateTime;

                    DateTime sunset = DateTimeOffset
                        .FromUnixTimeSeconds((long)rascunho["sys"]["sunset"])
                        .LocalDateTime;


                    t = new()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString(),
                    }; //Fecha obj do tempo
                } // Fecha if se o status do servidor for de sucesso


               
            } // fecha laço using
            return t;
        }
    }
}
