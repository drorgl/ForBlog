using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileCollisionTests
{
    /// <summary>
    /// Collision Avoiding File Access
    /// <para>Complimenting the OSs</para>
    /// </summary>
    class CollisionAvoidanceFileAccess
    {
        /// <summary>
        /// DictionaryLock for the filenames
        /// </summary>
        private static DictionaryLock<string> _filenamelock = new DictionaryLock<string>();

        /// <summary>
        /// WriterLocked Save Stream
        /// </summary>
        public static void SaveStream(string filename, Stream stream)
        {
            _filenamelock.EnterWriter(filename);
            LockingFileSystem.SaveStream(filename, stream);
            _filenamelock.ExitWriter(filename);
        }

        /// <summary>
        /// Reader Locked Load Stream
        /// </summary>
        public static Stream LoadStream(string filename)
        {
            Stream retval;
            _filenamelock.EnterReader(filename);
            retval = LockingFileSystem.LoadStream(filename);
            _filenamelock.ExitReader(filename);
            return retval;
        }
    }
}
