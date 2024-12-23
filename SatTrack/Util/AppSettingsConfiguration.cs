
namespace SatTrack.Util
{
    public static class appSettingsConfiguration
    {
        private static IConfiguration _configuration;


        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public static string GetSetting(string key)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("Configuration has not been initialized. Call ConfigurationHelper.Initialize() first.");
            }

            return _configuration[key] ?? "";
        }
    }
}
