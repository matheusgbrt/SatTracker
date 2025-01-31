using SatTrack.DAL;
using SatTrack.Services.Interfaces;
using System.Runtime.InteropServices;

namespace SatTrack.Services
{
    public class LoggingService(ElderveilContext context) : ILoggingService
    {
        private readonly ElderveilContext _context = context;

        public async void PersistGroupUpdateLog(string message, string groupName, bool isError)
        {
            try
            {
                var groupUpdateLog = new GroupUpdateLog();
                groupUpdateLog.GroupName = groupName;
                groupUpdateLog.Timestamp = DateTime.Now;
                groupUpdateLog.Active = true;
                groupUpdateLog.Message = message;
                _context.GroupUpdateLogs.Add(groupUpdateLog);
                await _context.SaveChangesAsync();
            }
            catch
            {

            }
        }


    }
}
