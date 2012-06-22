using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;

namespace AutoRegisterTimesheet
{
    /// <summary>
    /// Timesheet class
    /// <para>Handles storing, loading and calculating statistics for time events</para>
    /// </summary>
    public class Timesheet
    {
        public Timesheet(string logfilename)
        {
            this.LogFilename = logfilename;
        }

        public string LogFilename;

        /// <summary>
        /// Each logged event is stored in the LogLine class
        /// </summary>
        public class LogLine
        {
            /// <summary>
            /// Timestamp of event
            /// </summary>
            public DateTime Timestamp;
            /// <summary>
            /// Username performed the action
            /// </summary>
            public string Username;
            /// <summary>
            /// SessionSwitchReason + close and start events of program
            /// </summary>
            public string Event;
        }

        /// <summary>
        /// Julian calendar used to calculate date to weeks and back
        /// </summary>
        private Calendar calendar = new JulianCalendar();

        /// <summary>
        /// The first day of the week - for calculating week start dates
        /// </summary>
        public DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

        /// <summary>
        /// Number of hours a day
        /// </summary>
        public int MinimumHoursPerDay = 9;

        /// <summary>
        /// Alert the client when daily hours are met
        /// </summary>
        public bool AlertDailyHoursMet = false;

        /// <summary>
        /// Line number, an irrelevant identity number
        /// </summary>
        public volatile int RecordNumber = 1;

        /// <summary>
        /// LogLine collection of all events
        /// </summary>
        public ConcurrentBag<LogLine> LogLines = new ConcurrentBag<LogLine>();

        /// <summary>
        /// Checks if loglines changed, so statistics will run only once per change.
        /// </summary>
        private volatile bool LogLinesChanged = false;

        /// <summary>
        /// Daily statistic container
        /// </summary>
        public class DateStatistic
        {
            public DateTime Date;
            public TimeSpan TotalHours;
            public TimeSpan TotalLocked;
            public TimeSpan TotalRemote;
        }

        /// <summary>
        /// Statistics calculation results, for either displaying or further aggregates.
        /// </summary>
        public List<DateStatistic> StatisticsResults = new List<DateStatistic>();

        private int IsCalculatingResults = 0;

        /// <summary>
        /// Appends an event to log file
        /// </summary>
        /// <param name="id">identity number</param>
        /// <param name="timestamp">timestamp of event</param>
        /// <param name="username">username performed the action</param>
        /// <param name="switchname">event name</param>
        public void AppendToLog(int id, DateTime timestamp, string username, string switchname)
        {
            //Add to collection
            LogLines.Add(new LogLine
            {
                Event = switchname,
                Timestamp = timestamp,
                Username = username
            });

            LogLinesChanged = true;

            //if file doesn't exist, create a new file with a header
            if (!File.Exists(LogFilename))
            {
                using (StreamWriter sw = File.AppendText(LogFilename))
                {
                    sw.WriteLine("Line,Datetime,User,State");
                    sw.WriteLine(string.Format("{0},{1},{2},{3}", id, timestamp.ToString("o"), username, switchname));
                }
            }
            else //if a file does exist, append log line
                using (StreamWriter sw = File.AppendText(LogFilename))
            {
                sw.WriteLine(string.Format("{0},{1},{2},{3}", id, timestamp.ToString("o"), username, switchname));
            }
        }

        /// <summary>
        /// Load all events from log file
        /// </summary>
        public void LoadFromLog()
        {
            try
            {
                if (File.Exists(LogFilename))
                    using (StreamReader sr = File.OpenText(LogFilename))
                    {
                        string line = null;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("Line"))
                                continue;

                            var splitline = line.Split(',');
                            LogLines.Add(new LogLine
                            {
                                Event = splitline[3],
                                Timestamp = DateTime.Parse(splitline[1]),
                                Username = splitline[2]
                            });
                            this.RecordNumber = Convert.ToInt32(splitline[0]);
                        }
                        this.RecordNumber++;
                    }

                LogLinesChanged = true;

                //initial calculate statistics
                CalculateStatisticsResults();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Finds the first date of the provided datetime's week
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public DateTime FirstDateOfWeek(DateTime datetime)
        {
            while (datetime.DayOfWeek != this.FirstDayOfWeek)
                datetime = datetime.AddDays(-1);

            return datetime.Date;
        }

        /// <summary>
        /// Gets the week number of the provided datetime's date
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public int GetWeekNumber(DateTime datetime)
        {
            return calendar.GetWeekOfYear(datetime, CalendarWeekRule.FirstDay, this.FirstDayOfWeek);
        }

        /// <summary>
        /// Gets the first date of the provided week number
        /// </summary>
        /// <param name="weeknumber"></param>
        /// <returns></returns>
        public DateTime GetFirstDateOfWeek(int year, int weeknumber)
        {
            var datetime = new DateTime(year, 1, 1);
            return datetime.AddDays((weeknumber - 1) * 7);
        }

        

        /// <summary>
        /// Calculates the statistics and stores the results in the StatisticsResults field
        /// </summary>
        public void CalculateStatisticsResults()
        {
            //make sure its executing only once
            //I'm not using locks because I just want the method to exit and not wait until the previous one is finished.
            //any locking mechanism will just use more cpu than interlocked, so its a waste
            if (Interlocked.CompareExchange(ref IsCalculatingResults,1,0) != 0)
                return;

            //try - for the finally part, so if it fails, at least it will retry on the next execution
            try
            {

                List<LogLine> subset = null;

                //if none of the lines changed, we still want today updated (its using the datetime now for calculating today)
                if (LogLinesChanged == true)
                {
                    subset = this.LogLines.ToList();
                }
                else
                {
                    var today = DateTime.Now.Date;
                    subset = this.LogLines.Where(i => i.Timestamp.Date == today).ToList();
                }

                //calculate for each line its matching closing line (lock - unlock, remote connect - disconnect)
                var totaldetails = (from ll in subset.AsParallel()
                                    let llul = subset.Where(i => i.Timestamp > ll.Timestamp && i.Timestamp.Date == ll.Timestamp.Date && i.Event == "SessionUnlock" && ll.Event == "SessionLock")
                                    let llrm = subset.Where(i => i.Timestamp > ll.Timestamp && i.Timestamp.Date == ll.Timestamp.Date && i.Event == "RemoteDisconnect" && ll.Event == "RemoteConnect")
                                    select new
                                    {
                                        Timestamp = ll.Timestamp,
                                        Username = ll.Username,
                                        Event = ll.Event,
                                        EndUnlock = llul.Select(i => i.Timestamp).FirstOrDefault(),
                                        EndRemote = llrm.Select(i => i.Timestamp).FirstOrDefault()
                                    }).ToList();

                //calculate for each date the sum of hours 
                var totalhours = (from ll in totaldetails.AsParallel()
                                  group ll by ll.Timestamp.Date into llg
                                  select new DateStatistic
                                  {
                                      Date = llg.Key,
                                      //the purpose of this is to make sure today is updated to the last minute.
                                      TotalHours = (llg.Key == DateTime.Now.Date) ? DateTime.Now - llg.Min(i => i.Timestamp) : llg.Max(i => i.Timestamp) - llg.Min(i => i.Timestamp),
                                      TotalLocked = TimeSpan.FromHours(llg.Where(i => i.EndUnlock != DateTime.MinValue).Sum(i => (i.EndUnlock - i.Timestamp).TotalHours)),
                                      TotalRemote = TimeSpan.FromHours(llg.Where(i => i.EndRemote != DateTime.MinValue).Sum(i => (i.EndRemote - i.Timestamp).TotalHours)),
                                  }).ToList();

                //check again if all lines changed, if not, then update just today.
                if (LogLinesChanged == false)
                {
                    //just update today
                    var today = DateTime.Now.Date;
                    var todaystats = StatisticsResults.FirstOrDefault(i => i.Date == today);
                    var todaynewstats = totalhours.FirstOrDefault(i => i.Date == today);
                    todaystats.TotalHours = todaynewstats.TotalHours;
                    todaystats.TotalLocked = todaynewstats.TotalLocked;
                    todaystats.TotalRemote = todaynewstats.TotalRemote;
                }
                else
                {
                    //everything changed, replace collection
                    StatisticsResults = totalhours;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                LogLinesChanged = false;

                IsCalculatingResults = 0;
            }

            
        }


    }
}
