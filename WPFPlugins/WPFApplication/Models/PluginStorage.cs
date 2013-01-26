using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace WPFApplication.Models
{
    internal class PluginStorage
    {
        private const string PluginsFilename = "plugins.db";

        /// <summary>
        /// Retrieves an Attribute from Executing Assembly
        /// <remarks>
        ///     Taken from: http://stackoverflow.com/questions/3127288/how-can-i-retrieve-the-assemblycompany-setting-in-assemblyinfo-cs
        /// </remarks>
        /// </summary>
        private static string GetAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof (T));

            if (attribute == null)
                return null;

            return value.Invoke(attribute);
        }

        /// <summary>
        /// Removes Invalid Characters from a filename string
        /// </summary>
        private static string RemoveInvalidCharactersFromFilename(string filename)
        {
            var invalidchars = Path.GetInvalidFileNameChars();
            StringBuilder sb = new StringBuilder();
            foreach (var fchar in filename)
            {
                if (!invalidchars.Contains(fchar))
                    sb.Append(fchar);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Coalesce values for assembly attributes
        /// </summary>
        private static string CoalesceAttributes<T>(string firstChoice, Func<T,string> secondChoice) where T: Attribute
        {
            if (!string.IsNullOrWhiteSpace( firstChoice))
                return firstChoice;

            var secondChoiceValue = GetAssemblyAttribute<T>(secondChoice);
            if (!string.IsNullOrWhiteSpace(secondChoiceValue))
                return secondChoiceValue;

            return null;
        }

        /// <summary>
        /// Gets a User filename from application parameters
        /// </summary>
        private static string GetUserFilename(string Filename, string Company = null,string Application = null, string Version = null, string SubComponent = null)
        {
            var userprofile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            userprofile += "\\AppData\\Local";

            var companyvalue = CoalesceAttributes<AssemblyCompanyAttribute>(Company,i=>i.Company);
            if (companyvalue != null)
                userprofile += "\\" + RemoveInvalidCharactersFromFilename(companyvalue);

            var applicationvalue = CoalesceAttributes<AssemblyTitleAttribute>(Application,i=>i.Title);
            if (applicationvalue != null)
                userprofile += "\\" + RemoveInvalidCharactersFromFilename(applicationvalue);

            var versionvalue = CoalesceAttributes<AssemblyFileVersionAttribute>(Version, i => i.Version);
            if (versionvalue != null)
                userprofile += "\\" + RemoveInvalidCharactersFromFilename(versionvalue);

            if (!string.IsNullOrWhiteSpace(SubComponent))
                userprofile += "\\" + RemoveInvalidCharactersFromFilename(SubComponent);

            return userprofile + "\\" + Filename;
        }

        /// <summary>
        /// Loads models from file if a previously saved model exists
        /// </summary>
        public static List<PluginModel> GetModels()
        {
            var filename = GetUserFilename(PluginsFilename);

            if (!File.Exists(filename))
                return new List<PluginModel>();

            try
            {
                using (var filestream = new FileStream(filename, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (List<PluginModel>)bf.Deserialize(filestream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<PluginModel>();
            }
        }

        /// <summary>
        /// Saves models to file
        /// </summary>
        public static void SaveModels(List<PluginModel> plugins)
        {
            var filename = GetUserFilename(PluginsFilename);

            var directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var filestream = new FileStream(filename, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(filestream, plugins);
            }
        }
    }
}
