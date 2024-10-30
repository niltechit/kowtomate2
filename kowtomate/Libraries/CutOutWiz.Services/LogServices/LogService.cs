using CutOutWiz.Core.Models.Log;
using Microsoft.Extensions.Configuration;

namespace CutOutWiz.Services.LogServices
{
    public class LogService : ILogService
    {
        private readonly IConfiguration _configuration;
        private readonly string logDirectory;

        public LogService(IConfiguration configuration)
        {
            _configuration = configuration;
            logDirectory = _configuration["AppSettings:LogDirectory"];
        }

        public LogModel GetLogsByDate(DateTime date)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string logFile = $"{logDirectory}/{date.ToString("dd-MMM-yyy")}_AppLogs.txt";

            if (!File.Exists(logFile))
            {
                return new LogModel();
            }

            string[] logs = File.ReadAllLines(logFile);

            return new LogModel
            {
                Logs = logs.ToList()
            };
        }

        public void Log(string message)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string logFile = $"{logDirectory}/{DateTime.Today.ToString("dd-MMM-yyy")}_AppLogs.txt";

            if (!File.Exists(logFile))
            {
                File.Create(logFile).Close();
            }

            using (StreamWriter w = File.AppendText(logFile))
            {
                w.WriteLine("[{0} {1}] : {2}\r\n", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), message);
                w.Close();
            }
        }
    }
}
