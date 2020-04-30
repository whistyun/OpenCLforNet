using NUnit.Framework;
using System.Linq;

namespace OpenCLforNetTest
{
    public unsafe class SimpleMemoryTest : TestBase
    {
        [Test]
        public void CreateTest()
        {
            using (var mem = Context.CreateSimpleMemory(100))
            {
                Assert.NotNull(mem.Context);
                Assert.AreEqual(100, mem.Size);
            }
        }

        [Test]
        public void ReadWriteTest()
        {
            using (var mem = Context.CreateSimpleMemory(100))
            {
                byte[] dat = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
                byte[] ans = new byte[100];

                fixed (void* datPtr = dat)
                fixed (void* ansPtr = ans)
                {
                    mem.WriteUnsafe(CommandQueue, true, 0, dat.Length, datPtr).Wait();

                    mem.ReadUnsafe(CommandQueue, true, 0, ans.Length, ansPtr).Wait();
                    Assert.AreEqual(dat, ans);

                    mem.ReadUnsafe(CommandQueue, true, ansPtr).Wait();
                    Assert.AreEqual(dat, ans);

                    mem.WriteUnsafe(CommandQueue, true, datPtr).Wait();
                    Assert.AreEqual(dat, ans);
                }
            }
        }

        [Test]
        public void ReadPartTest()
        {
            using (var mem = Context.CreateSimpleMemory(100))
            {
                byte[] dat = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
                byte[] ans = new byte[25];

                fixed (void* datPtr = dat)
                fixed (void* ansPtr = ans)
                {
                    mem.WriteUnsafe(CommandQueue, true, 0, dat.Length, datPtr).Wait();

                    mem.ReadUnsafe(CommandQueue, true, 0, 25, ansPtr).Wait();
                    Assert.AreEqual(dat.Skip(0).Take(25), ans);

                    mem.ReadUnsafe(CommandQueue, true, 25, 25, ansPtr).Wait();
                    Assert.AreEqual(dat.Skip(25).Take(25), ans);

                    mem.ReadUnsafe(CommandQueue, true, 50, 25, ansPtr).Wait();
                    Assert.AreEqual(dat.Skip(50).Take(25), ans);

                    mem.ReadUnsafe(CommandQueue, true, 75, 25, ansPtr).Wait();
                    Assert.AreEqual(dat.Skip(75).Take(25), ans);
                }
            }
        }

        [Test]
        public void WritePartTest()
        {
            using (var mem = Context.CreateSimpleMemory(100))
            {
                byte[] dat = Enumerable.Range(0, 100).Select(n => (byte)n).ToArray();
                byte[] ans = new byte[100];

                fixed (byte* datPtr = dat)
                fixed (byte* ansPtr = ans)
                {
                    // 0-clear
                    mem.WriteUnsafe(CommandQueue, true, 0, ans.Length, ansPtr).Wait();


                    mem.WriteUnsafe(CommandQueue, true, 0, 25, datPtr).Wait();
                    mem.ReadUnsafe(CommandQueue, true, 0, 25, ansPtr).Wait();
                    Assert.AreEqual(dat.Take(25), ans.Take(25));

                    mem.WriteUnsafe(CommandQueue, true, 25, 25, datPtr + 25).Wait();
                    mem.WriteUnsafe(CommandQueue, true, 50, 25, datPtr + 50).Wait();
                    mem.WriteUnsafe(CommandQueue, true, 75, 25, datPtr + 75).Wait();
                    mem.ReadUnsafe(CommandQueue, true, ansPtr).Wait();
                    Assert.AreEqual(dat, ans);
                }
            }
        }

    }
}
