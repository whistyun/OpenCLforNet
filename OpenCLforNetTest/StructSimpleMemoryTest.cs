using NUnit.Framework;
using OpenCLforNet.Memory;
using System.Linq;
using System.Runtime.InteropServices;

namespace OpenCLforNetTest
{
    public struct MySimpleStruct
    {
        public int X;
        public int Y;
        public float Point;

        public override string ToString() => $"MyStruct(X:{X}, Y:{Y}, P:{Point})";
    }

    public class StructSimpleMemoryTest : TestBase
    {
        string Source = @"
            typedef struct cl_my_obj {
                int x;
                int y;
                float p;
            } my_obj;

            kernel void testKernel(global my_obj* array, int rate){
                int idx = get_global_id(0);

                my_obj oldVal = array[idx];
                my_obj newVal = { oldVal.x+rate, oldVal.y+rate, oldVal.p+rate };

                array[idx] = newVal;
            }
        ";


        [Test]
        public void CreateTest()
        {
            using (var mem = new TypedSimpleMemory<MySimpleStruct>(Context, 20))
            {
                Assert.NotNull(mem.Context);
                Assert.AreEqual(20 * Marshal.SizeOf(typeof(MySimpleStruct)), mem.Size);
                Assert.AreEqual(20, mem.Length);
            }
        }

        [Test]
        public void ReadWriteTest()
        {
            using (var mem = new TypedSimpleMemory<MySimpleStruct>(Context, 100))
            {
                MySimpleStruct[] dat = Enumerable.Range(0, 100)
                    .Select(n => new MySimpleStruct() { X = n, Y = n + 10, Point = n * 2.2f })
                    .ToArray();
                MySimpleStruct[] ans = new MySimpleStruct[100];

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
        public void ReadPartTest()
        {
            using (var mem = new TypedSimpleMemory<MySimpleStruct>(Context, 100))
            {
                MySimpleStruct[] dat = Enumerable.Range(0, 100)
                    .Select(n => new MySimpleStruct() { X = n, Y = n + 10, Point = n * 2.2f })
                    .ToArray();
                MySimpleStruct[] ans = new MySimpleStruct[25];

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
        public void WritePartTest()
        {
            using (var mem = new TypedSimpleMemory<MySimpleStruct>(Context, 100))
            {
                MySimpleStruct[] dat = Enumerable.Range(0, 100)
                    .Select(n => new MySimpleStruct() { X = n, Y = n + 10, Point = n * 2.2f })
                    .ToArray();
                MySimpleStruct[] ans = new MySimpleStruct[100];

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
        public void ShaderTest()
        {
            const int WorkSize = 100;
            const int Adding = 53;

            using (var program = Context.CreateProgram(Source))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, 100)
                    .Select(n => new MySimpleStruct()
                    {
                        X = n,
                        Y = n + 10,
                        Point = n * 2.2f
                    })
                    .ToArray();

                var output = new MySimpleStruct[WorkSize];
                var expected = input
                    .Select(v => new MySimpleStruct()
                    {
                        X = v.X + Adding,
                        Y = v.Y + Adding,
                        Point = v.Point + Adding,
                    })
                    .ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var mem = new TypedSimpleMemory<MySimpleStruct>(Context, 100))
                {
                    mem.Write(CommandQueue, true, 0, 50, input);
                    mem.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(mem, (int)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    mem.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }
    }
}
