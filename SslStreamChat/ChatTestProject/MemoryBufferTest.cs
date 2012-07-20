using ChatMessages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ChatTestProject
{
    
    
    /// <summary>
    ///This is a test class for MemoryBufferTest and is intended
    ///to contain all MemoryBufferTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MemoryBufferTest
    {


       

        [TestMethod()]
        public void MemoryBufferOverflowTest()
        {
            Exception exc = null;
            MemoryBuffer target = new MemoryBuffer(255);
            byte[] buffer = new byte[256];
            try
            {
            target.Write(buffer, 0, buffer.Length);
            }catch (Exception ex)
            {
                exc = ex;
            }
            Assert.IsNotNull(exc,"MemoryBuffer didn't raise an exception for attempting to fill it more than length");
        }

        [TestMethod()]
        public void MemoryBufferOverflowMultiple()
        {
            Exception exc = null;
            MemoryBuffer target = new MemoryBuffer(400);
            byte[] buffer = new byte[256];
            try
            {
                target.Write(buffer, 0, buffer.Length);
                target.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                exc = ex;
            }
            Assert.IsNotNull(exc, "MemoryBuffer didn't raise an exception for attempting to fill it more than length");
        }

        [TestMethod()]
        public void ReadWriteTest()
        {
            MemoryBuffer target = new MemoryBuffer(10);
            byte[] buffer = new byte[5];
            for (byte i = 0; i < buffer.Length; i++)
                buffer[i] = i;

            for (byte i = 0; i < 20; i++)
            {
                target.Write(buffer, 0, buffer.Length);
                var tempbuf = new byte[5];
                var readcount = target.Read(tempbuf, 0, tempbuf.Length);
                CollectionAssert.AreEqual(buffer, tempbuf, "Input not equal output");
                Assert.AreEqual(tempbuf.Length, readcount, "Read count is not write count");
            }
        }

        [TestMethod]
        public void ReadWriteMultiplTest()
        {
            MemoryBuffer target = new MemoryBuffer(251);
            byte[] buffer = new byte[10];
            for (byte i = 0; i < buffer.Length; i++)
                buffer[i] = i;

            for (byte i = 0; i < 50; i++)
            {
                target.Write(buffer, 0, buffer.Length);
                var tempbuf = new byte[10];
                var readcount = target.Read(tempbuf, 0, tempbuf.Length /2);
                readcount += target.Read(tempbuf, tempbuf.Length / 2, tempbuf.Length / 2);
                CollectionAssert.AreEqual(buffer, tempbuf, "Input not equal output");
                Assert.AreEqual(tempbuf.Length, readcount, "Read count is not write count");
            }
        }

        [TestMethod]
        public void ReadWritePeekMultiplTest()
        {
            MemoryBuffer target = new MemoryBuffer(251);
            byte[] buffer = new byte[10];
            for (byte i = 0; i < buffer.Length; i++)
                buffer[i] = i;

            for (byte i = 0; i < 50; i++)
            {
                target.Write(buffer, 0, buffer.Length);
                var tempbuf = new byte[10];
                var peekbuf = new byte[10];
                var peekcount = target.Peek(peekbuf, 0, peekbuf.Length);
                CollectionAssert.AreEqual(buffer, peekbuf, "Peek not equal output");


                var readcount = target.Read(tempbuf, 0, tempbuf.Length / 2);
                readcount += target.Read(tempbuf, tempbuf.Length / 2, tempbuf.Length / 2);

                CollectionAssert.AreEqual(peekbuf, tempbuf, "Peek not equal Read");
                CollectionAssert.AreEqual(buffer, tempbuf, "Input not equal output");
                Assert.AreEqual(tempbuf.Length, readcount, "Read count is not write count");
                Assert.AreEqual(peekcount, readcount, "Read count is not Peek count");
            }
        }

        /// <summary>
        ///A test for MemoryBuffer Constructor
        ///</summary>
        [TestMethod()]
        public void MemoryBufferConstructorTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Flush
        ///</summary>
        [TestMethod()]
        public void FlushTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //target.Flush();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Peek
        ///</summary>
        [TestMethod()]
        public void PeekTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //byte[] buffer = null; // TODO: Initialize to an appropriate value
            //int offset = 0; // TODO: Initialize to an appropriate value
            //int count = 0; // TODO: Initialize to an appropriate value
            //int expected = 0; // TODO: Initialize to an appropriate value
            //int actual;
            //actual = target.Peek(buffer, offset, count);
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Push
        ///</summary>
        [TestMethod()]
        public void PushTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //byte[] buffer = null; // TODO: Initialize to an appropriate value
            //int offset = 0; // TODO: Initialize to an appropriate value
            //int count = 0; // TODO: Initialize to an appropriate value
            //target.Push(buffer, offset, count);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Read
        ///</summary>
        [TestMethod()]
        public void ReadTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //byte[] buffer = null; // TODO: Initialize to an appropriate value
            //int offset = 0; // TODO: Initialize to an appropriate value
            //int count = 0; // TODO: Initialize to an appropriate value
            //int expected = 0; // TODO: Initialize to an appropriate value
            //int actual;
            //actual = target.Read(buffer, offset, count);
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Seek
        ///</summary>
        [TestMethod()]
        public void SeekTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //long offset = 0; // TODO: Initialize to an appropriate value
            //SeekOrigin origin = new SeekOrigin(); // TODO: Initialize to an appropriate value
            //long expected = 0; // TODO: Initialize to an appropriate value
            //long actual;
            //actual = target.Seek(offset, origin);
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetLength
        ///</summary>
        [TestMethod()]
        public void SetLengthTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //long value = 0; // TODO: Initialize to an appropriate value
            //target.SetLength(value);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Write
        ///</summary>
        [TestMethod()]
        public void WriteTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //byte[] buffer = null; // TODO: Initialize to an appropriate value
            //int offset = 0; // TODO: Initialize to an appropriate value
            //int count = 0; // TODO: Initialize to an appropriate value
            //target.Write(buffer, offset, count);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CanRead
        ///</summary>
        [TestMethod()]
        public void CanReadTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = target.CanRead;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanSeek
        ///</summary>
        [TestMethod()]
        public void CanSeekTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = target.CanSeek;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CanWrite
        ///</summary>
        [TestMethod()]
        public void CanWriteTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //bool actual;
            //actual = target.CanWrite;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ChunkLength
        ///</summary>
        [TestMethod()]
        public void ChunkLengthTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //int actual;
            //actual = target.ChunkLength;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Length
        ///</summary>
        [TestMethod()]
        public void LengthTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //long actual;
            //actual = target.Length;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Position
        ///</summary>
        [TestMethod()]
        public void PositionTest()
        {
            //int length = 0; // TODO: Initialize to an appropriate value
            //MemoryBuffer target = new MemoryBuffer(length); // TODO: Initialize to an appropriate value
            //long expected = 0; // TODO: Initialize to an appropriate value
            //long actual;
            //target.Position = expected;
            //actual = target.Position;
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
