using BuscoAPI.Entities.GeoApi;
using Newtonsoft.Json;

namespace BuscoAPI.Services
{
    public class SNDGService
    {
        private readonly String url = "https://apis.datos.gob.ar/georef/api/";
        private readonly HttpClient _httpClient;

        public SNDGService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeoData> GetProvinces()
        {
            var response = await _httpClient
                .GetAsync($"{url}provincias?campos=id,nombre");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GeoData>(responseContent);
        }

        //DEBO PONER LA PROVINCIA DE PARAMETRO
        public async Task<GeoData> GetDepartments(String provincia)
        {
            var response = await _httpClient
                .GetAsync($"{url}departamentos?campos=id,nombre&max=100&provincia={provincia}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GeoData>(responseContent);
        }
        
        public async Task<GeoData> GetCiudades(String provincia,String departamento)
        {
            var response = await _httpClient
                .GetAsync($"{url}localidades-censales?campos=id,nombre&max=100&provincia={provincia}&departamento={departamento}");

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GeoData>(responseContent);
        }
    }
}
