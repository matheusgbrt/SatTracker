using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using SatTrack.Configs;

namespace SatTrack.Services
{
    public class CelestrakClientService
    {

        private readonly HttpClient _httpClient;
        private readonly CelestrakSettings _settings;

        public CelestrakClientService(HttpClient httpClient,IOptions<CelestrakSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }


        public async Task<string> GetGroupDataAsync(string group)
        {
            var url = $"{_settings.BaseURL}?GROUP={group}&FORMAT={_settings.Format}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

    }
}
