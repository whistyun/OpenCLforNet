using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenCLforNet.Memory;

namespace OpenCLforNetTest
{
    public class ShaderTest : TestBase
    {
        const int WorkSize = 100;
        const int Adding = 53;

        string ByteSource = @"
            kernel void testKernel(global char* array, char rate){
                int idx = get_global_id(0);
                array[idx] += rate;
            }
        ";

        string CharSource = @"
            kernel void testKernel(global short* array, short rate){
                int idx = get_global_id(0);
                array[idx] += rate;
            }
        ";

        string ShortSource = @"
            kernel void testKernel(global short* array, short rate){
                int idx = get_global_id(0);
                array[idx] += rate;
            }
        ";

        string IntSource = @"
            kernel void testKernel(global int* array, int rate){
                int idx = get_global_id(0);
                array[idx] += rate;
            }
        ";

        string LongSource = @"
            kernel void testKernel(global long* array, long rate){
                int idx = get_global_id(0);
                array[idx] += rate;
            }
        ";

        string FloatSource = @"
            kernel void testKernel(global float* array, float rate){
                int idx = get_global_id(0);
                array[idx] += rate;
            }
        ";

        string DoubleSource = @"
            kernel void testKernel(global double* array, double rate){
                int idx = get_global_id(0);
                array[idx] += rate;
            }
        ";

        [Test]
        public void ByteShader()
        {

            using (var program = Context.CreateProgram(ByteSource))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, WorkSize).Select(v => (byte)v).ToArray();
                var output = new byte[WorkSize];
                var expected = input.Select(v => (byte)(v + Adding)).ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var simpleMemory = Context.CreateByteSimpleMemory(WorkSize))
                {
                    simpleMemory.Write(CommandQueue, true, 0, 50, input);
                    simpleMemory.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(simpleMemory, (byte)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    simpleMemory.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }

        [Test]
        public void CharShader()
        {
            using (var program = Context.CreateProgram(CharSource))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, WorkSize).Select(v => (char)v).ToArray();
                var output = new char[WorkSize];
                var expected = input.Select(v => (char)(v + Adding)).ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var simpleMemory = Context.CreateCharSimpleMemory(WorkSize))
                {
                    simpleMemory.Write(CommandQueue, true, 0, 50, input);
                    simpleMemory.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(simpleMemory, (char)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    simpleMemory.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }

        [Test]
        public void ShortShader()
        {
            using (var program = Context.CreateProgram(ShortSource))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, WorkSize).Select(v => (short)v).ToArray();
                var output = new short[WorkSize];
                var expected = input.Select(v => (short)(v + Adding)).ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var simpleMemory = Context.CreateShortSimpleMemory(WorkSize))
                {
                    simpleMemory.Write(CommandQueue, true, 0, 50, input);
                    simpleMemory.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(simpleMemory, (short)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    simpleMemory.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }

        [Test]
        public void IntShader()
        {

            using (var program = Context.CreateProgram(IntSource))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, WorkSize).Select(v => (int)v).ToArray();
                var output = new int[WorkSize];
                var expected = input.Select(v => (int)(v + Adding)).ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var simpleMemory = Context.CreateIntSimpleMemory(WorkSize))
                {
                    simpleMemory.Write(CommandQueue, true, 0, 50, input);
                    simpleMemory.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(simpleMemory, (int)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    simpleMemory.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }

        [Test]
        public void LongShader()
        {

            using (var program = Context.CreateProgram(LongSource))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, WorkSize).Select(v => (long)v).ToArray();
                var output = new long[WorkSize];
                var expected = input.Select(v => (long)(v + Adding)).ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var simpleMemory = Context.CreateLongSimpleMemory(WorkSize))
                {
                    simpleMemory.Write(CommandQueue, true, 0, 50, input);
                    simpleMemory.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(simpleMemory, (long)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    simpleMemory.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }

        [Test]
        public void FloatShader()
        {

            using (var program = Context.CreateProgram(FloatSource))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, WorkSize).Select(v => (float)v).ToArray();
                var output = new float[WorkSize];
                var expected = input.Select(v => (float)(v + Adding)).ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var simpleMemory = Context.CreateFloatSimpleMemory(WorkSize))
                {
                    simpleMemory.Write(CommandQueue, true, 0, 50, input);
                    simpleMemory.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(simpleMemory, (float)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    simpleMemory.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }

        [Test]
        public void DoubleShader()
        {

            using (var program = Context.CreateProgram(DoubleSource))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                var input = Enumerable.Range(0, WorkSize).Select(v => (double)v).ToArray();
                var output = new double[WorkSize];
                var expected = input.Select(v => (double)(v + Adding)).ToArray();

                kernel.SetWorkSize(WorkSize);
                using (var simpleMemory = Context.CreateDoubleSimpleMemory(WorkSize))
                {
                    simpleMemory.Write(CommandQueue, true, 0, 50, input);
                    simpleMemory.Write(CommandQueue, true, 50, WorkSize - 50, input, 50);

                    kernel.SetArgs(simpleMemory, (double)Adding);
                    kernel.NDRange(CommandQueue).Wait();

                    simpleMemory.Read(CommandQueue, true, output);

                    Assert.AreEqual(expected, output);
                }
            }

        }
    }
}
