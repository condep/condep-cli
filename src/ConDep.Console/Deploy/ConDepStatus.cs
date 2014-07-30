using System;
using ConDep.Dsl.Logging;

namespace ConDep.Console.Deploy
{
    public class ConDepStatus
    {
        private readonly DateTime _startTime;
        private DateTime _endTime;

        public ConDepStatus()
        {
            _startTime = DateTime.Now;
        }

        public DateTime StartTime
        {
            get { return _startTime; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public void PrintSummary()
        {
            if (_endTime < _startTime)
            {
                _endTime = DateTime.Now;
            }

            string message = string.Format(@"
Start Time      : {0}
End time        : {1}
Time Taken      : {2}
", StartTime.ToLongTimeString(), EndTime.ToLongTimeString(), (EndTime - StartTime).ToString(@"%h' hrs '%m' min '%s' sec'"));
            Logger.Info("\n");
            Logger.WithLogSection("Summary", () => Logger.Info(message));
        }
    }
}