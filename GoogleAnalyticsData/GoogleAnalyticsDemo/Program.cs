using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Google.GData.Analytics;
using PublicDomain;

namespace GoogleAnalyticsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //test limiter
            //TestLimiter();

            //initialize a new limiter with 10 concurrent and 10 requests per second.
            Limiter limiter = new Limiter(TimeSpan.FromSeconds(1), 10, 10);

            string username = "username";
            string password = "password";

            //first, initialize a service, its the query provider.
            var service = new Google.GData.Analytics.AnalyticsService("AnalyticsReader");

            //you can also use service.setUserCredentials(username,password)
            service.Credentials = new Google.GData.Client.GDataCredentials(username, password);

            //first, query for all accounts
            var accountquery = new Google.GData.Analytics.AccountQuery();

            accountquery.NumberToRetrieve = 10000; //Maximum

            limiter.Enter();
            var accountresult = service.Query(accountquery);
            limiter.Exit();

            //for each account, retrieve records
            foreach (AccountEntry account in accountresult.Entries)
            {
                //get the timezone for the account, as all data is saved based on that timezone.
                Property timezone = account.Properties.Where(i => i.Name == "ga:timezone").FirstOrDefault();


                Console.WriteLine(
                  "\nProfile Title     = " + account.Title.Text +
                  "\nProfile ID        = " + account.ProfileId.Value + 
                  "\nTimeZone       = " + timezone.Value);

                TzTimeZone tzinfo = null;

                if (timezone != null)
                    tzinfo = TzTimeZone.GetTimeZone(timezone.Value);

                //retrieve analytics data
                DataQuery query = new DataQuery();
                query.Ids = account.ProfileId.Value;

                //Data

                //dimensions are the "group by"
                query.Dimensions = "ga:date,ga:hour,ga:hostname,ga:country,ga:keyword,ga:source,ga:referralPath";

                //metrics are the values
                query.Metrics = "ga:bounces,ga:newVisits,ga:pageviews,ga:timeOnSite,ga:visits";
                
                //start from yesterday
                query.GAStartDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                //until today
                query.GAEndDate = DateTime.Now.ToString("yyyy-MM-dd");
                query.StartIndex = 1;

                //Maximum allowed by Quota.
                query.NumberToRetrieve = 10000;


                limiter.Enter();
                DataFeed dataFeedVisits = service.Query(query);
                limiter.Exit();

                //process each record returned
                foreach (DataEntry dentry in dataFeedVisits.Entries)
                {
                    var line = new Dictionary<string, string>();

                    //insert into directory for each processing
                    for (int i = 0; i < dentry.Metrics.Count; i++)
                        line[dentry.Metrics[i].Name] = dentry.Metrics[i].Value;

                    for (int i = 0; i < dentry.Dimensions.Count; i++)
                        line[dentry.Dimensions[i].Name] = dentry.Dimensions[i].Value;

                    //get the UTC datetime from the ga:date + ga:hour + profile timezone combination
                    DateTime date = DateTime.ParseExact(line["ga:date"], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    int hour = Convert.ToInt32(line["ga:hour"]);
                    date = date.AddHours(hour);

                    line["dateutc"] = ConvertToUtc(date, tzinfo).ToString();


                    //dump to console all data found
                    Console.Write("UTC: {0}, ", line["dateutc"]);

                    for (int i = 0; i < dentry.Dimensions.Count; i++)
                        Console.Write("{0}: {1}, ", dentry.Dimensions[i].Name, dentry.Dimensions[i].Value);

                    for (int i = 0; i < dentry.Metrics.Count; i++)
                        Console.Write("{0}: {1}, ", dentry.Metrics[i].Name, dentry.Metrics[i].Value);

                    Console.WriteLine();

                }

            }

        }

        /// <summary>
        /// Converts a datetime from fromTimeZone to UTC
        /// </summary>
        private static DateTime ConvertToUtc(DateTime datetime, TzTimeZone fromTimeZone)
        {
            return fromTimeZone.ToUniversalTime(datetime);
        }


        #region test limiter
        private static void TestLimiter()
        {
            int concurrent = 0;
            DateTime current = DateTime.Now;

            Limiter limiter = new Limiter(TimeSpan.FromSeconds(1), 10, 10);


            Task t = new Task(() =>
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        if (current.Second != DateTime.Now.Second)
                        {
                            current = DateTime.Now;
                            Console.WriteLine(concurrent);
                            concurrent = 0;
                        }
                        Thread.Sleep(100);
                    }
                });
            t.Start();

            List<Task> tasks = new List<Task>();
            for (var x= 0; x < 200; x++)
                tasks.Add(
             new Task(() =>
                {
                    for (var i = 0; i < 200; i++)
                    {
                        limiter.Enter();
                        Interlocked.Increment(ref concurrent);
                        limiter.Exit();
                    }
                })
               );

            foreach (var c in tasks)
                c.Start();

            foreach (var c in tasks)
                c.Wait();

            t.Wait();

        }
        #endregion

    }
}
