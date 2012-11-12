using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace DomainParsingTest
{
    /// <summary>
    /// <c>DomainParser</c> - Parses and retrieves the domain from a subdomain
    /// <remarks>
    /// It does so by getting a tld and sld domains lists from urls list, saving it to
    /// disk for cache and checking every number of days for updates
    /// </remarks>
    /// </summary>
    class DomainParser
    {
        /// <summary>
        /// Contains a list of tlds and slds.
        /// </summary>
        private static HashSet<string> _domains = new HashSet<string>();

        /// <summary>
        /// Contains a list of URLs to retrieve tlds and slds from
        /// </summary>
        private static readonly string[] _sources = new string[]
            {
                "http://namebench.googlecode.com/svn/trunk/data/second_level_domains.txt",
                "http://dnsbl.invaluement.com/domainroots.txt",
                "http://www.logsat.com/spamfilter/pub/ccTLDs.txt", 
                "http://data.iana.org/TLD/tlds-alpha-by-domain.txt"
            };

        /// <summary>
        /// Domain List Cache Filename
        /// </summary>
        private const string _cachefilename = "slds.txt";

        /// <summary>
        /// Domain List Expiration/Refresh Time
        /// </summary>
        private static readonly TimeSpan _cachetime = TimeSpan.FromDays(7);

        /// <summary>
        /// In Memory Last Time the List Was Updated
        /// </summary>
        private static DateTime _cachelastupdate = DateTime.MinValue;

        /// <summary>
        /// Retrieves a list from either disk or URLs based on availability, 
        /// <paramref name="_cachefilename"/> and <paramref name="_cachetime"/>.
        /// </summary>
        /// <returns>string containing list of domains</returns>
        private static string GetList()
        {
            string sldlist = string.Empty;

            bool shouldupdate = true;

            //check if cache file exists
            if (File.Exists(_cachefilename))
            {
                //then check if the cache file is too old.
                FileInfo ficache = new FileInfo(_cachefilename);
                if (ficache.LastWriteTimeUtc > (DateTime.UtcNow - _cachetime))
                    shouldupdate = false;

                //in any case, if it exists, read it (so at least we have some data if the url update fails)
                sldlist = File.ReadAllText(_cachefilename);
            }

            //Do we need to udpate?
            if (shouldupdate == true)
            {
                try
                {
                    StringBuilder sblist = new StringBuilder();

                    WebClient wc = new WebClient();

                    //go over each source and download the list
                    foreach (var src in _sources)
                    {
                        sblist.Append(wc.DownloadString(src));
                        sblist.Append("\r\n");
                    }


                    //if we fail to update, no harm done, the sldlist won't be changed.
                    sldlist = sblist.ToString();

                    //but if we fail to save to disk, at least we got the updated version in memory.
                    File.WriteAllText(_cachefilename, sblist.ToString());
                }
                catch (Exception ex) 
                { 
                    //Error handling
                }
            }

            return sldlist;
        }

        /// <summary>
        /// Populates the HashSet <paramref name="_domains"/> with <paramref name="list"/>.
        /// </summary>
        /// <param name="list">crlf delimited domain list</param>
        private static void PopulateHashSet(string list)
        {
            HashSet<string> newset = new HashSet<string>();
            using (StringReader sr = new StringReader(list))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    //clean up the line, remove spaces and change everything to lowercase.
                    line = line.Trim().ToLower();

                    //if its not an empty line
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        //make sure it starts with a dot.
                        if (!line.StartsWith("."))
                            line = "." + line;

                        newset.Add(line);
                    }

                    line = sr.ReadLine();
                }
            }

            _domains = newset;
        }

        /// <summary>
        /// Make sure the cache is updated
        /// </summary>
        private static void CheckCache()
        {
            if (_cachelastupdate < (DateTime.UtcNow - _cachetime))
            {
                PopulateHashSet(GetList());
                _cachelastupdate = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Retrieves a domain from a subdomain
        /// </summary>
        /// <param name="subdomain">the subdomain to be parsed</param>
        /// <returns>domain</returns>
        public static string GetDomain(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                return null;

            //make sure we have a fresh version of the domain list
            CheckCache();

            //clean up the subdomain
            var cleandomain = subdomain.Trim().ToLower();
            
            //split it into parts by dot
            var domainparts = cleandomain.Split('.');

            //assign the top of the domain parts
            string result = domainparts[domainparts.Length - 1];

            //go over the rest of the parts and add them to the domain until we failed to find a 
            //match in the _domains HashSet, this means we've reached the domain.
            for (int i = domainparts.Length-2; i >= 0; i--)
            {
                if (!_domains.Contains("." + result))
                    break;

                result = domainparts[i] + "." + result;
            }

            return result;
        }

        
    }
}
