namespace SatTrack.Services.Interfaces
{
    public interface ILoggingService
    {
        void PersistGroupUpdateLog(string message, string groupName, bool isError);

    }
}
