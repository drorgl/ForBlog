using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XmlDemo
{
    /// <summary>
    /// XAttribute/XElement extensions
    /// </summary>
    public static class XExtensions
    {
        /// <summary>
        /// Converts XAttribute to enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="xattribute">XAttribute object</param>
        /// <param name="defaultValue">return this value if conversion fails</param>
        /// <returns>defaultValur or converted enum value</returns>
        public static T ToEnum<T>(this XAttribute xattribute, Enum defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum");

            return (T)((xattribute == null || (string.IsNullOrEmpty(xattribute.Value))) ? defaultValue : Enum.Parse(typeof(T), xattribute.Value));
        }

        /// <summary>
        /// Converts XElement to enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="xelement">XElement object</param>
        /// <param name="defaultValue">return this value if conversion fails</param>
        /// <returns>defaultValur or converted enum value</returns>
        public static T ToEnum<T>(this XElement xelement, Enum defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum");

            return (T)((xelement == null || (string.IsNullOrEmpty(xelement.Value))) ? defaultValue : Enum.Parse(typeof(T), xelement.Value));
        }

        /// <summary>
        /// Convets XAttibute's value to byte array from base64 encoded string
        /// </summary>
        /// <param name="xattribute">XAttribute object</param>
        /// <returns>byte array if successful, null if not</returns>
        public static byte[] ToByteArray(this XAttribute xattribute)
        {
            if (string.IsNullOrEmpty(xattribute.Value))
                return null;

            return Convert.FromBase64String(xattribute.Value);
        }

        /// <summary>
        /// Convets XElement's value to byte array from base64 encoded string
        /// </summary>
        /// <param name="xelement">XElement object</param>
        /// <returns>byte array if successful, null if not</returns>
        public static byte[] ToByteArray(this XElement xelement)
        {
            if (string.IsNullOrEmpty(xelement.Value))
                return null;

            return Convert.FromBase64String(xelement.Value);
        }
    }
}
