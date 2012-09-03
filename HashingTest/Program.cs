using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace HashingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Why not use Random class for cryptographic purposes.
            Console.WriteLine("Why not use Random for cryptographic purposes:");
            PseudoRandomTest();

            //Truely random number generator
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Truely Random: " + Random(10, 100000).ToString());
            }

            Console.WriteLine("PBKDF2:");

            string password = "Pass@word1";
            byte[] passbytes = Encoding.UTF8.GetBytes(password);

            byte[] salt = new byte[8];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(salt);

            //Keith Brown implementation
            var hash1 = PBKDF2.SHA1GetBytes(passbytes, salt, 10, 20);
            //.NET implementation
            Rfc2898DeriveBytes rfc2897 = new Rfc2898DeriveBytes(passbytes, salt, 10);
            var hash2 = rfc2897.GetBytes(20);

            Console.WriteLine(BitConverter.ToString(hash1).Replace("-", ""));
            Console.WriteLine(BitConverter.ToString(hash2).Replace("-", ""));


            Console.WriteLine("PBKDF1:");

            //.NET implementation
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(passbytes, salt, "SHA1", 10);
            var hash3 = pdb.GetBytes(20);
            
            //My Implementation
            var hash4 = PBKDF1.SHA1GetBytes(passbytes, salt, 10, 20);
            
            Console.WriteLine(BitConverter.ToString(hash3).Replace("-", ""));
            Console.WriteLine(BitConverter.ToString(hash4).Replace("-", ""));

            //Test ChangePassword and ValidatePassword
            User user = new User
            {
                 Username = "testuser"
            };
            Console.WriteLine("Initial User details:");
            PrintUserDetails(user);

            ChangePassword(user, "new password");

            Console.WriteLine("User details after password changed to 'new password':");
            PrintUserDetails(user);

            Console.WriteLine("Result of validating user password against 'new password':");
            Console.WriteLine(ValidatePassword(user, "new password"));

            Console.WriteLine("Changing user details to 'newer password':");
            ChangePassword(user, "newer password");
            PrintUserDetails(user);

            Console.WriteLine("Changing user details back to 'new password':");
            ChangePassword(user, "new password");
            PrintUserDetails(user);

            Console.WriteLine("Result of validating user password against 'new password':");
            Console.WriteLine(ValidatePassword(user, "new password"));

            Console.WriteLine("Result of validating user password against 'newer password':");
            Console.WriteLine(ValidatePassword(user, "newer password"));

            //sql injection
            //SqlConnection conn = new SqlConnection("connstr");
            //conn.Open();
            //var cmd = conn.CreateCommand();
            //cmd.CommandText = "select * from users where username = @username";
            //cmd.CommandType = System.Data.CommandType.Text;
            //cmd.Parameters.Add(new SqlParameter("@username", "testuser"));
            //cmd.ExecuteReader();
                
        }

        /// <summary>
        /// Test method that shows the same results returning from two distinct Random functions
        /// </summary>
        private static void PseudoRandomTest()
        {
            Random rnd1 = new Random(0);
            Random rnd2 = new Random(0);
            byte[] bytes1 = new byte[40];
            byte[] bytes2 = new byte[40];
            rnd1.NextBytes(bytes1);
            rnd2.NextBytes(bytes2);

            //On my machine and I guess on yours, produces similar results.
            Console.WriteLine("1st " + BitConverter.ToString(bytes1).Replace("-", ""));
            Console.WriteLine("2nd " + BitConverter.ToString(bytes2).Replace("-", ""));
        }

        /// <summary>
        /// Demo implementation of RNGCryptoServiceProvider producing integers
        /// </summary>
        private static int Random(int min, int max)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] intbytes = new byte[4];
            rng.GetBytes(intbytes);
            int rndval = BitConverter.ToInt32(intbytes, 0);
            int range = max - min;
            return Math.Abs((rndval % range)) + min;
        }

        #region User validation

        /// <summary>
        /// Basic user class
        /// </summary>
        class User
        {
            public string Username { get; set; }
            public byte[] Salt { get; set; }
            public int Iterations { get; set; }
            public byte[] Password { get; set; }
        }

        private static void PrintUserDetails(User user)
        {
            Console.WriteLine("User: {0}, salt: {1}, iterations: {2}, Password: {3}", user.Username, (user.Salt != null) ? BitConverter.ToString(user.Salt).Replace("-", "") : "",
                        user.Iterations,
                        (user.Password != null) ? BitConverter.ToString(user.Password).Replace("-", "") : "");
        }

        /// <summary>
        /// How to change a password with PBKDF2
        /// </summary>
        static void ChangePassword(User user, string password)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[8];
            rng.GetBytes(salt);

            int iterations = Random(10000,11000);

            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations);

            user.Salt = salt;
            user.Password = rfc2898.GetBytes(40);
            user.Iterations = iterations;
        }

        /// <summary>
        /// How to validate a password with PBKDF2
        /// </summary>
        static bool ValidatePassword(User user, string password)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, user.Salt, user.Iterations);
            var hashedpassword = rfc2898.GetBytes(40);
            if (ArrayCompare(hashedpassword, user.Password))
                return true;
            return false;
        }

        #endregion

        #region byte array compare

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        static bool ArrayCompare(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        #endregion

    }
}
