using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using SatTrack.Configs;
using SatTrack.Services.Interfaces;

namespace SatTrack.Services
{
    public class CelestrakClientService
    {

        private readonly HttpClient _httpClient;
        private readonly CelestrakSettings _settings;
        private readonly ILoggingService _loggingService;

        public CelestrakClientService(HttpClient httpClient, IOptions<CelestrakSettings> options, ILoggingService loggingService)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _loggingService = loggingService;
        }


        public async Task<string> GetGroupDataAsync(string group)
        {

            var url = $"{_settings.BaseURL}?GROUP={group}&FORMAT={_settings.Format}";
            var response = await _httpClient.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch
            {
                _loggingService.PersistGroupUpdateLog($"Failed to get group data. Status Code: {response.StatusCode}", group, true);
                return "";
            }


        }

    }
}
