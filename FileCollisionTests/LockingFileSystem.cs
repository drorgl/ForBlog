using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace FileCollisionTests
{
    /// <summary>
    /// File System Access
    /// </summary>
    class LockingFileSystem
    {
        /// <summary>
        /// Saves a stream, retry retryCount number of attempts and wait between the attempts if failed on sharing violation
        /// </summary>
        public static void SaveStream(string filename, Stream stream, int retryCount, TimeSpan wait)
        {
            var path = Path.GetDirectoryName(filename);

            if (!string.IsNullOrEmpty(path) && (!Directory.Exists(path)))
                Directory.CreateDirectory(path);

            var tries = 0;
            while (true)
            {
                try
                {
                    SaveStream(filename, stream);
                    return;
                }
                catch (IOException e)
                {
                    if (!LockingFileSystem.IsFileLocked(e))
                        throw;
                    if (++tries > retryCount)
                        throw;

                    Thread.Sleep(wait);
                }
            }
        }

        /// <summary>
        /// Saves a stream to file 
        /// </summary>
        public static void SaveStream(string filename, Stream stream)
        {
            using (var file = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {

                Stream output = file;

                stream.CopyTo(output);

                output.Flush();

                output.Close();
            }
        }

        /// <summary>
        /// Loads a stream from file, retry retryCount number of attempts and wait between the attempts if failed on sharing violation
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="retryCount"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public static Stream LoadStream(string filename, int retryCount, TimeSpan wait)
        {
            var tries = 0;
            while (true)
            {
                try
                {
                    return LoadStream(filename);
                }
                catch (IOException e)
                {
                    if (!IsFileLocked(e))
                        throw;
                    if (++tries > retryCount)
                        throw;

                    Thread.Sleep(wait);
                }
            }
        }

        /// <summary>
        /// Loads a stream
        /// </summary>
        public static Stream LoadStream(string filename)
        {
            if (!File.Exists(filename))
                return null;

            MemoryStream output = new MemoryStream();

            using (var file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Stream input = file;

                input.CopyTo(output);

                input.Close();
                return output;
            }
        }

        /// <summary>
        /// Checks exception is for file in use
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool IsFileLocked(IOException exception)
        {
            //int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            //return errorCode == 32 || errorCode == 33;
            if (exception.Message.IndexOf("because it is being used by another process") != -1)
                return true;

            return false;
        }

    }
}
