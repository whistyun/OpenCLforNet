using NUnit.Framework;
using OpenCLforNet.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenCLforNetTest
{
    public class TypedMappingMemoryTest : TestBase
    {
        [Test]
        public void ByteCreateTest1()
        {
            using (var mem = Context.CreateByteMappingMemory(100))
            {
                Assert.NotNull(mem.Context);
                Assert.AreEqual(100 * sizeof(byte), mem.Size);
                Assert.AreEqual(100, mem.Length);
            }
        }

        [Test]
        public void ByteCreateTest2()
        {
            byte[] dat = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
            byte[] ans = new byte[100];
            using (var mem = Context.CreateSimpleMemory(dat))
            {
                Assert.AreEqual(100 * sizeof(byte), mem.Size);
                Assert.AreEqual(100, mem.Length);

                mem.Read(CommandQueue, true, ans).Wait();
                Assert.AreEqual(dat, ans);
            }
        }

        [Test]
        public void ByteReadWriteTest()
        {
            using (var mem = Context.CreateByteMappingMemory(100))
            {
                byte[] dat = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
                byte[] ans = new byte[100];

                mem.WriteDirect(CommandQueue, true, 0, dat.Length, dat, 0).Wait();
                mem.ReadDirect(CommandQueue, true, 0, ans.Length, ans, 0).Wait();
                Assert.AreEqual(dat, ans);


                mem.Write(CommandQueue, true, 0, dat.Length, dat, 0).Wait();
                Assert.AreEqual(dat, ans);

                mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait();
                Assert.AreEqual(dat, ans);

                mem.Write(CommandQueue, true, dat.Length, dat).Wait();
                Assert.AreEqual(dat, ans);

                mem.Write(CommandQueue, true, dat).Wait();
                Assert.AreEqual(dat, ans);



                mem.Read(CommandQueue, true, 0, ans.Length, ans, 0).Wait();
                Assert.AreEqual(dat, ans);

                mem.Read(CommandQueue, true, 0, ans.Length, ans).Wait();
                Assert.AreEqual(dat, ans);

                mem.Read(CommandQueue, true, ans.Length, ans).Wait();
                Assert.AreEqual(dat, ans);

                mem.Read(CommandQueue, true, ans).Wait();
                Assert.AreEqual(dat, ans);
            }
        }

        [Test]
        public void ByteReadPartTest()
        {
            using (var mem = Context.CreateByteMappingMemory(100))
            {
                byte[] dat = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
                byte[] ans = new byte[25];

                mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait();

                mem.Read(CommandQueue, true, 0, 25, ans).Wait();
                Assert.AreEqual(dat.Skip(0).Take(25), ans);

                mem.Read(CommandQueue, true, 25, 25, ans).Wait();
                Assert.AreEqual(dat.Skip(25).Take(25), ans);

                mem.Read(CommandQueue, true, 50, 25, ans).Wait();
                Assert.AreEqual(dat.Skip(50).Take(25), ans);

                mem.Read(CommandQueue, true, 75, 25, ans).Wait();
                Assert.AreEqual(dat.Skip(75).Take(25), ans);
            }
        }

        [Test]
        public void ByteWritePartTest()
        {
            using (var mem = Context.CreateByteMappingMemory(100))
            {
                byte[] dat = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
                byte[] ans = new byte[100];

                mem.Write(CommandQueue, true, 0, ans.Length, ans).Wait();

                mem.Write(CommandQueue, true, 0, 25, dat).Wait();
                mem.Read(CommandQueue, true, 0, 25, ans).Wait();
                Assert.AreEqual(dat.Take(25), ans.Take(25));

                mem.Write(CommandQueue, true, 25, 25, dat, 25).Wait();
                mem.Write(CommandQueue, true, 50, 25, dat, 50).Wait();
                mem.Write(CommandQueue, true, 75, 25, dat, 75).Wait();
                mem.Read(CommandQueue, true, ans).Wait();
                Assert.AreEqual(dat, ans);
            }
        }

        [Test]
        public void ByteMappingTest()
        {
            using (var mem = Context.CreateByteMappingMemory(100))
            {
                using (var map = mem.Mapping(CommandQueue, true, 0, 100))
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        map[i] = (byte)i;
                    }
                }

                byte[] ans = new byte[100];
                mem.Read(CommandQueue, true, ans);
                Assert.AreEqual(Enumerable.Range(0, 100).Select(n => (byte)n), ans);
            }
        }

        [Test]
        public void ByteMappingPartTest()
        {
            using (var mem = Context.CreateByteMappingMemory(100))
            {
                byte[] expected = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
                byte[] ans = new byte[100];

                using (var map = mem.Mapping(CommandQueue, true, 0, 50))
                {
                    for (int i = 0; i < 50; ++i)
                    {
                        map[i] = (byte)i;
                    }
                }

                mem.Read(CommandQueue, true, 50, ans);

                Assert.AreEqual(expected.Take(50), ans.Take(50));

                using (var map = mem.Mapping(CommandQueue, true, 50, 50))
                {
                    for (int i = 0; i < 50; ++i)
                    {
                        map[i] = (byte)(i + 50);
                    }
                }

                mem.Read(CommandQueue, true, ans);

                Assert.AreEqual(expected, ans);
            }
        }

        #region テンプレから作成

        [Test] public void CharCreateTest1() { using (var mem = Context.CreateCharMappingMemory(100)) { Assert.NotNull(mem.Context); Assert.AreEqual(100 * sizeof(char), mem.Size); Assert.AreEqual(100, mem.Length); } }
        [Test] public void CharCreateTest2() { char[] dat = Enumerable.Range(0, 100).Select(n => (char)n).ToArray(); char[] ans = new char[100]; using (var mem = Context.CreateSimpleMemory(dat)) { Assert.AreEqual(100 * sizeof(char), mem.Size); Assert.AreEqual(100, mem.Length); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void CharReadWriteTest() { using (var mem = Context.CreateCharMappingMemory(100)) { char[] dat = Enumerable.Range(0, 100).Select(n => (char)n).ToArray(); char[] ans = new char[100]; mem.WriteDirect(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); mem.ReadDirect(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void CharReadPartTest() { using (var mem = Context.CreateCharMappingMemory(100)) { char[] dat = Enumerable.Range(0, 100).Select(n => (char)n).ToArray(); char[] ans = new char[25]; mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Skip(0).Take(25), ans); mem.Read(CommandQueue, true, 25, 25, ans).Wait(); Assert.AreEqual(dat.Skip(25).Take(25), ans); mem.Read(CommandQueue, true, 50, 25, ans).Wait(); Assert.AreEqual(dat.Skip(50).Take(25), ans); mem.Read(CommandQueue, true, 75, 25, ans).Wait(); Assert.AreEqual(dat.Skip(75).Take(25), ans); } }
        [Test] public void CharWritePartTest() { using (var mem = Context.CreateCharMappingMemory(100)) { char[] dat = Enumerable.Range(0, 100).Select(n => (char)n).ToArray(); char[] ans = new char[100]; mem.Write(CommandQueue, true, 0, ans.Length, ans).Wait(); mem.Write(CommandQueue, true, 0, 25, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Take(25), ans.Take(25)); mem.Write(CommandQueue, true, 25, 25, dat, 25).Wait(); mem.Write(CommandQueue, true, 50, 25, dat, 50).Wait(); mem.Write(CommandQueue, true, 75, 25, dat, 75).Wait(); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void CharMappingTest() { using (var mem = Context.CreateCharMappingMemory(100)) { using (var map = mem.Mapping(CommandQueue, true, 0, 100)) { for (int i = 0; i < 100; ++i) { map[i] = (char)i; } } char[] ans = new char[100]; mem.Read(CommandQueue, true, ans); Assert.AreEqual(Enumerable.Range(0, 100).Select(n => (char)n), ans); } }
        [Test] public void CharMappingPartTest() { using (var mem = Context.CreateCharMappingMemory(100)) { char[] expected = Enumerable.Range(0, 100).Select(n => (char)n).ToArray(); char[] ans = new char[100]; using (var map = mem.Mapping(CommandQueue, true, 0, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (char)i; } } mem.Read(CommandQueue, true, 50, ans); Assert.AreEqual(expected.Take(50), ans.Take(50)); using (var map = mem.Mapping(CommandQueue, true, 50, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (char)(i + 50); } } mem.Read(CommandQueue, true, ans); Assert.AreEqual(expected, ans); } }
        [Test] public void ShortCreateTest1() { using (var mem = Context.CreateShortMappingMemory(100)) { Assert.NotNull(mem.Context); Assert.AreEqual(100 * sizeof(short), mem.Size); Assert.AreEqual(100, mem.Length); } }
        [Test] public void ShortCreateTest2() { short[] dat = Enumerable.Range(0, 100).Select(n => (short)n).ToArray(); short[] ans = new short[100]; using (var mem = Context.CreateSimpleMemory(dat)) { Assert.AreEqual(100 * sizeof(short), mem.Size); Assert.AreEqual(100, mem.Length); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void ShortReadWriteTest() { using (var mem = Context.CreateShortMappingMemory(100)) { short[] dat = Enumerable.Range(0, 100).Select(n => (short)n).ToArray(); short[] ans = new short[100]; mem.WriteDirect(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); mem.ReadDirect(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void ShortReadPartTest() { using (var mem = Context.CreateShortMappingMemory(100)) { short[] dat = Enumerable.Range(0, 100).Select(n => (short)n).ToArray(); short[] ans = new short[25]; mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Skip(0).Take(25), ans); mem.Read(CommandQueue, true, 25, 25, ans).Wait(); Assert.AreEqual(dat.Skip(25).Take(25), ans); mem.Read(CommandQueue, true, 50, 25, ans).Wait(); Assert.AreEqual(dat.Skip(50).Take(25), ans); mem.Read(CommandQueue, true, 75, 25, ans).Wait(); Assert.AreEqual(dat.Skip(75).Take(25), ans); } }
        [Test] public void ShortWritePartTest() { using (var mem = Context.CreateShortMappingMemory(100)) { short[] dat = Enumerable.Range(0, 100).Select(n => (short)n).ToArray(); short[] ans = new short[100]; mem.Write(CommandQueue, true, 0, ans.Length, ans).Wait(); mem.Write(CommandQueue, true, 0, 25, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Take(25), ans.Take(25)); mem.Write(CommandQueue, true, 25, 25, dat, 25).Wait(); mem.Write(CommandQueue, true, 50, 25, dat, 50).Wait(); mem.Write(CommandQueue, true, 75, 25, dat, 75).Wait(); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void ShortMappingTest() { using (var mem = Context.CreateShortMappingMemory(100)) { using (var map = mem.Mapping(CommandQueue, true, 0, 100)) { for (int i = 0; i < 100; ++i) { map[i] = (short)i; } } short[] ans = new short[100]; mem.Read(CommandQueue, true, ans); Assert.AreEqual(Enumerable.Range(0, 100).Select(n => (short)n), ans); } }
        [Test] public void ShortMappingPartTest() { using (var mem = Context.CreateShortMappingMemory(100)) { short[] expected = Enumerable.Range(0, 100).Select(n => (short)n).ToArray(); short[] ans = new short[100]; using (var map = mem.Mapping(CommandQueue, true, 0, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (short)i; } } mem.Read(CommandQueue, true, 50, ans); Assert.AreEqual(expected.Take(50), ans.Take(50)); using (var map = mem.Mapping(CommandQueue, true, 50, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (short)(i + 50); } } mem.Read(CommandQueue, true, ans); Assert.AreEqual(expected, ans); } }
        [Test] public void IntCreateTest1() { using (var mem = Context.CreateIntMappingMemory(100)) { Assert.NotNull(mem.Context); Assert.AreEqual(100 * sizeof(int), mem.Size); Assert.AreEqual(100, mem.Length); } }
        [Test] public void IntCreateTest2() { int[] dat = Enumerable.Range(0, 100).Select(n => (int)n).ToArray(); int[] ans = new int[100]; using (var mem = Context.CreateSimpleMemory(dat)) { Assert.AreEqual(100 * sizeof(int), mem.Size); Assert.AreEqual(100, mem.Length); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void IntReadWriteTest() { using (var mem = Context.CreateIntMappingMemory(100)) { int[] dat = Enumerable.Range(0, 100).Select(n => (int)n).ToArray(); int[] ans = new int[100]; mem.WriteDirect(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); mem.ReadDirect(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void IntReadPartTest() { using (var mem = Context.CreateIntMappingMemory(100)) { int[] dat = Enumerable.Range(0, 100).Select(n => (int)n).ToArray(); int[] ans = new int[25]; mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Skip(0).Take(25), ans); mem.Read(CommandQueue, true, 25, 25, ans).Wait(); Assert.AreEqual(dat.Skip(25).Take(25), ans); mem.Read(CommandQueue, true, 50, 25, ans).Wait(); Assert.AreEqual(dat.Skip(50).Take(25), ans); mem.Read(CommandQueue, true, 75, 25, ans).Wait(); Assert.AreEqual(dat.Skip(75).Take(25), ans); } }
        [Test] public void IntWritePartTest() { using (var mem = Context.CreateIntMappingMemory(100)) { int[] dat = Enumerable.Range(0, 100).Select(n => (int)n).ToArray(); int[] ans = new int[100]; mem.Write(CommandQueue, true, 0, ans.Length, ans).Wait(); mem.Write(CommandQueue, true, 0, 25, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Take(25), ans.Take(25)); mem.Write(CommandQueue, true, 25, 25, dat, 25).Wait(); mem.Write(CommandQueue, true, 50, 25, dat, 50).Wait(); mem.Write(CommandQueue, true, 75, 25, dat, 75).Wait(); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void IntMappingTest() { using (var mem = Context.CreateIntMappingMemory(100)) { using (var map = mem.Mapping(CommandQueue, true, 0, 100)) { for (int i = 0; i < 100; ++i) { map[i] = (int)i; } } int[] ans = new int[100]; mem.Read(CommandQueue, true, ans); Assert.AreEqual(Enumerable.Range(0, 100).Select(n => (int)n), ans); } }
        [Test] public void IntMappingPartTest() { using (var mem = Context.CreateIntMappingMemory(100)) { int[] expected = Enumerable.Range(0, 100).Select(n => (int)n).ToArray(); int[] ans = new int[100]; using (var map = mem.Mapping(CommandQueue, true, 0, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (int)i; } } mem.Read(CommandQueue, true, 50, ans); Assert.AreEqual(expected.Take(50), ans.Take(50)); using (var map = mem.Mapping(CommandQueue, true, 50, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (int)(i + 50); } } mem.Read(CommandQueue, true, ans); Assert.AreEqual(expected, ans); } }
        [Test] public void LongCreateTest1() { using (var mem = Context.CreateLongMappingMemory(100)) { Assert.NotNull(mem.Context); Assert.AreEqual(100 * sizeof(long), mem.Size); Assert.AreEqual(100, mem.Length); } }
        [Test] public void LongCreateTest2() { long[] dat = Enumerable.Range(0, 100).Select(n => (long)n).ToArray(); long[] ans = new long[100]; using (var mem = Context.CreateSimpleMemory(dat)) { Assert.AreEqual(100 * sizeof(long), mem.Size); Assert.AreEqual(100, mem.Length); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void LongReadWriteTest() { using (var mem = Context.CreateLongMappingMemory(100)) { long[] dat = Enumerable.Range(0, 100).Select(n => (long)n).ToArray(); long[] ans = new long[100]; mem.WriteDirect(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); mem.ReadDirect(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void LongReadPartTest() { using (var mem = Context.CreateLongMappingMemory(100)) { long[] dat = Enumerable.Range(0, 100).Select(n => (long)n).ToArray(); long[] ans = new long[25]; mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Skip(0).Take(25), ans); mem.Read(CommandQueue, true, 25, 25, ans).Wait(); Assert.AreEqual(dat.Skip(25).Take(25), ans); mem.Read(CommandQueue, true, 50, 25, ans).Wait(); Assert.AreEqual(dat.Skip(50).Take(25), ans); mem.Read(CommandQueue, true, 75, 25, ans).Wait(); Assert.AreEqual(dat.Skip(75).Take(25), ans); } }
        [Test] public void LongWritePartTest() { using (var mem = Context.CreateLongMappingMemory(100)) { long[] dat = Enumerable.Range(0, 100).Select(n => (long)n).ToArray(); long[] ans = new long[100]; mem.Write(CommandQueue, true, 0, ans.Length, ans).Wait(); mem.Write(CommandQueue, true, 0, 25, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Take(25), ans.Take(25)); mem.Write(CommandQueue, true, 25, 25, dat, 25).Wait(); mem.Write(CommandQueue, true, 50, 25, dat, 50).Wait(); mem.Write(CommandQueue, true, 75, 25, dat, 75).Wait(); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void LongMappingTest() { using (var mem = Context.CreateLongMappingMemory(100)) { using (var map = mem.Mapping(CommandQueue, true, 0, 100)) { for (int i = 0; i < 100; ++i) { map[i] = (long)i; } } long[] ans = new long[100]; mem.Read(CommandQueue, true, ans); Assert.AreEqual(Enumerable.Range(0, 100).Select(n => (long)n), ans); } }
        [Test] public void LongMappingPartTest() { using (var mem = Context.CreateLongMappingMemory(100)) { long[] expected = Enumerable.Range(0, 100).Select(n => (long)n).ToArray(); long[] ans = new long[100]; using (var map = mem.Mapping(CommandQueue, true, 0, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (long)i; } } mem.Read(CommandQueue, true, 50, ans); Assert.AreEqual(expected.Take(50), ans.Take(50)); using (var map = mem.Mapping(CommandQueue, true, 50, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (long)(i + 50); } } mem.Read(CommandQueue, true, ans); Assert.AreEqual(expected, ans); } }
        [Test] public void FloatCreateTest1() { using (var mem = Context.CreateFloatMappingMemory(100)) { Assert.NotNull(mem.Context); Assert.AreEqual(100 * sizeof(float), mem.Size); Assert.AreEqual(100, mem.Length); } }
        [Test] public void FloatCreateTest2() { float[] dat = Enumerable.Range(0, 100).Select(n => (float)n).ToArray(); float[] ans = new float[100]; using (var mem = Context.CreateSimpleMemory(dat)) { Assert.AreEqual(100 * sizeof(float), mem.Size); Assert.AreEqual(100, mem.Length); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void FloatReadWriteTest() { using (var mem = Context.CreateFloatMappingMemory(100)) { float[] dat = Enumerable.Range(0, 100).Select(n => (float)n).ToArray(); float[] ans = new float[100]; mem.WriteDirect(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); mem.ReadDirect(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void FloatReadPartTest() { using (var mem = Context.CreateFloatMappingMemory(100)) { float[] dat = Enumerable.Range(0, 100).Select(n => (float)n).ToArray(); float[] ans = new float[25]; mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Skip(0).Take(25), ans); mem.Read(CommandQueue, true, 25, 25, ans).Wait(); Assert.AreEqual(dat.Skip(25).Take(25), ans); mem.Read(CommandQueue, true, 50, 25, ans).Wait(); Assert.AreEqual(dat.Skip(50).Take(25), ans); mem.Read(CommandQueue, true, 75, 25, ans).Wait(); Assert.AreEqual(dat.Skip(75).Take(25), ans); } }
        [Test] public void FloatWritePartTest() { using (var mem = Context.CreateFloatMappingMemory(100)) { float[] dat = Enumerable.Range(0, 100).Select(n => (float)n).ToArray(); float[] ans = new float[100]; mem.Write(CommandQueue, true, 0, ans.Length, ans).Wait(); mem.Write(CommandQueue, true, 0, 25, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Take(25), ans.Take(25)); mem.Write(CommandQueue, true, 25, 25, dat, 25).Wait(); mem.Write(CommandQueue, true, 50, 25, dat, 50).Wait(); mem.Write(CommandQueue, true, 75, 25, dat, 75).Wait(); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void FloatMappingTest() { using (var mem = Context.CreateFloatMappingMemory(100)) { using (var map = mem.Mapping(CommandQueue, true, 0, 100)) { for (int i = 0; i < 100; ++i) { map[i] = (float)i; } } float[] ans = new float[100]; mem.Read(CommandQueue, true, ans); Assert.AreEqual(Enumerable.Range(0, 100).Select(n => (float)n), ans); } }
        [Test] public void FloatMappingPartTest() { using (var mem = Context.CreateFloatMappingMemory(100)) { float[] expected = Enumerable.Range(0, 100).Select(n => (float)n).ToArray(); float[] ans = new float[100]; using (var map = mem.Mapping(CommandQueue, true, 0, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (float)i; } } mem.Read(CommandQueue, true, 50, ans); Assert.AreEqual(expected.Take(50), ans.Take(50)); using (var map = mem.Mapping(CommandQueue, true, 50, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (float)(i + 50); } } mem.Read(CommandQueue, true, ans); Assert.AreEqual(expected, ans); } }
        [Test] public void DoubleCreateTest1() { using (var mem = Context.CreateDoubleMappingMemory(100)) { Assert.NotNull(mem.Context); Assert.AreEqual(100 * sizeof(double), mem.Size); Assert.AreEqual(100, mem.Length); } }
        [Test] public void DoubleCreateTest2() { double[] dat = Enumerable.Range(0, 100).Select(n => (double)n).ToArray(); double[] ans = new double[100]; using (var mem = Context.CreateSimpleMemory(dat)) { Assert.AreEqual(100 * sizeof(double), mem.Size); Assert.AreEqual(100, mem.Length); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void DoubleReadWriteTest() { using (var mem = Context.CreateDoubleMappingMemory(100)) { double[] dat = Enumerable.Range(0, 100).Select(n => (double)n).ToArray(); double[] ans = new double[100]; mem.WriteDirect(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); mem.ReadDirect(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat, 0).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat.Length, dat).Wait(); Assert.AreEqual(dat, ans); mem.Write(CommandQueue, true, dat).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans, 0).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, 0, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans.Length, ans).Wait(); Assert.AreEqual(dat, ans); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void DoubleReadPartTest() { using (var mem = Context.CreateDoubleMappingMemory(100)) { double[] dat = Enumerable.Range(0, 100).Select(n => (double)n).ToArray(); double[] ans = new double[25]; mem.Write(CommandQueue, true, 0, dat.Length, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Skip(0).Take(25), ans); mem.Read(CommandQueue, true, 25, 25, ans).Wait(); Assert.AreEqual(dat.Skip(25).Take(25), ans); mem.Read(CommandQueue, true, 50, 25, ans).Wait(); Assert.AreEqual(dat.Skip(50).Take(25), ans); mem.Read(CommandQueue, true, 75, 25, ans).Wait(); Assert.AreEqual(dat.Skip(75).Take(25), ans); } }
        [Test] public void DoubleWritePartTest() { using (var mem = Context.CreateDoubleMappingMemory(100)) { double[] dat = Enumerable.Range(0, 100).Select(n => (double)n).ToArray(); double[] ans = new double[100]; mem.Write(CommandQueue, true, 0, ans.Length, ans).Wait(); mem.Write(CommandQueue, true, 0, 25, dat).Wait(); mem.Read(CommandQueue, true, 0, 25, ans).Wait(); Assert.AreEqual(dat.Take(25), ans.Take(25)); mem.Write(CommandQueue, true, 25, 25, dat, 25).Wait(); mem.Write(CommandQueue, true, 50, 25, dat, 50).Wait(); mem.Write(CommandQueue, true, 75, 25, dat, 75).Wait(); mem.Read(CommandQueue, true, ans).Wait(); Assert.AreEqual(dat, ans); } }
        [Test] public void DoubleMappingTest() { using (var mem = Context.CreateDoubleMappingMemory(100)) { using (var map = mem.Mapping(CommandQueue, true, 0, 100)) { for (int i = 0; i < 100; ++i) { map[i] = (double)i; } } double[] ans = new double[100]; mem.Read(CommandQueue, true, ans); Assert.AreEqual(Enumerable.Range(0, 100).Select(n => (double)n), ans); } }
        [Test] public void DoubleMappingPartTest() { using (var mem = Context.CreateDoubleMappingMemory(100)) { double[] expected = Enumerable.Range(0, 100).Select(n => (double)n).ToArray(); double[] ans = new double[100]; using (var map = mem.Mapping(CommandQueue, true, 0, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (double)i; } } mem.Read(CommandQueue, true, 50, ans); Assert.AreEqual(expected.Take(50), ans.Take(50)); using (var map = mem.Mapping(CommandQueue, true, 50, 50)) { for (int i = 0; i < 50; ++i) { map[i] = (double)(i + 50); } } mem.Read(CommandQueue, true, ans); Assert.AreEqual(expected, ans); } }

        #endregion
    }
}
