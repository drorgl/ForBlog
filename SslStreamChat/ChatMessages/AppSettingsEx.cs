using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace ChatMessages
{
    /// <summary>
    /// AppSettings value, used for explicit operators.
    /// </summary>
    public class AppSettingsValue
    {
        /// <summary>
        /// Value of setting
        /// </summary>
        public string Value {get;set;}

        /// <summary>
        /// Explicit cast to string
        /// <para>instead of accessing Value</para>
        /// </summary>
        public static explicit operator string(AppSettingsValue val)
        {
            return val.Value;
        }

        /// <summary>
        /// Explicit cast to nullable int 
        /// </summary>
        public static explicit operator int?(AppSettingsValue val)
        {
            if (val == null)
                return null;

            int parsed = 0;
            if (int.TryParse(val.Value,out parsed))
                return parsed;

            return null;
        }

        /// <summary>
        /// Explicit cast to nullable bool
        /// </summary>
        public static explicit operator bool?(AppSettingsValue val)
        {
            if (val == null)
                return null;
            bool parsed = false;
            if (bool.TryParse(val.Value,out parsed))
                return parsed;
            return null;
        }

    }

    /// <summary>
    /// AppSettings Extension
    /// </summary>
    public class AppSettingsEx
    {
        private NameValueCollection nvsettings;

        public AppSettingsEx(NameValueCollection settings)
        {
            this.nvsettings = settings;
        }

        /// <summary>
        /// Settings Name
        /// </summary>
        public AppSettingsValue this[string indexer]
        {
            get
            {
                return new AppSettingsValue { Value = this.nvsettings[indexer] };
            }
        }
    }

    /// <summary>
    /// ConfigurationManager extension
    /// <para>for example: ConfigurationManagerEx.AppSettings["BufferSize"]</para>
    /// </summary>
    public static class ConfigurationManagerEx
    {
        public static AppSettingsEx AppSettings
        {
            get
            {
                return new AppSettingsEx(ConfigurationManager.AppSettings);
            }
        }
    }
}
