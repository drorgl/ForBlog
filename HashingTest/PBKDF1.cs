/*
 PBKDF1 Implementation of http://www.ietf.org/rfc/rfc2898.txt
 Dror Gluska 2012
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace HashingTest
{
    /// <summary>
    /// PBKDF1 key derivation 
    /// <para>http://www.ietf.org/rfc/rfc2898.txt</para>
    /// </summary>
    public class PBKDF1
    {
        /// <summary>
        /// Retrieves MD5 hashed PBKDF1 
        /// </summary>
        public static byte[] MD5GetBytes(byte[] password,
            byte[] salt, int iterations, int howManyBytes)
        {
            return ComputePBKDF1(new MD5CryptoServiceProvider(), password, salt, iterations, howManyBytes);
        }

        /// <summary>
        /// Retrieves SHA1 hashed PBKDF1
        /// </summary>
        public static byte[] SHA1GetBytes(byte[] password,
            byte[] salt, int iterations, int howManyBytes)
        {
            return ComputePBKDF1(new SHA1Managed(), password, salt, iterations, howManyBytes);
        }

        /// <summary>
        /// Computes password + salt hashed by hashAlgo for a number of iterations and retrieve howManyBytes from the resulting hash.
        /// </summary>
        private static byte[] ComputePBKDF1(HashAlgorithm hashAlgo, byte[] password, byte[] salt, int iterations, int howManyBytes)
        {
            if (howManyBytes > (hashAlgo.HashSize/8))
                throw new ArgumentOutOfRangeException("howManyBytes should be smaller than " + (hashAlgo.HashSize/8).ToString());
             
            byte[] passandsalt = new byte[password.Length + salt.Length];
            Array.Copy(password, 0, passandsalt, 0, password.Length);
            Array.Copy(salt, 0, passandsalt, password.Length, salt.Length);

            var hash = hashAlgo.ComputeHash(passandsalt);

            for (int i = 1; i < iterations; i++)
                hash = hashAlgo.ComputeHash(hash);

            byte[] hashsubset = new byte[howManyBytes];
            Array.Copy(hash, hashsubset, howManyBytes);
            return hashsubset;
        }
    }
}
