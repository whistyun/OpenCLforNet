using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCLforNet.Function;
using OpenCLforNet.Memory;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;

namespace OpenCLforNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayPlatformsInfo();

            var device = new Device();
            using (var context = device.CreateContext())
            using (var commandQueue = context.CreateCommandQueue())
            using (var program = context.CreateProgram(source))
            using (var kernel = program.CreateKernel("testKernel"))
            {
                kernel.SetWorkSize(4);

                Console.WriteLine("\nCL_MEM_READ_WRITE");
                using (var simpleMemory = context.CreateFloatSimpleMemory(4))
                {
                    var data = new float[] { 3F, 4.5F, 0F, -4.4F };
                    kernel.SetArgs(simpleMemory, 1F);

                    var event11 = simpleMemory.Write(commandQueue, true, data);
                    var event12 = kernel.NDRange(commandQueue, event11);
                    var event13 = simpleMemory.Read(commandQueue, true, data, event12);

                    Console.WriteLine($"write time     : {event11.ExecutionTime} ns");
                    Console.WriteLine($"exec time      : {event12.ExecutionTime} ns");
                    Console.WriteLine($"read time      : {event13.ExecutionTime} ns");
                    Console.WriteLine($"result         : [{String.Join("  ", data)}]");
                }


                Console.WriteLine("\nCL_MEM_COPY_HOST_PTR");
                using (var simpleMemory = context.CreateSimpleMemory(new float[] { 3F, 4.5F, 0F, -4.4F }))
                {
                    kernel.SetArgs(simpleMemory, 2F);
                    var event21 = kernel.NDRange(commandQueue);

                    var result = new float[4];
                    var event22 = simpleMemory.Read(commandQueue, true, result, event21);

                    Console.WriteLine($"exec time      : {event21.ExecutionTime} ns");
                    Console.WriteLine($"read time      : {event22.ExecutionTime} ns");
                    Console.WriteLine($"result         : [{String.Join("  ", result)}]");
                }

                Console.WriteLine("\nCL_MEM_ALLOC_HOST_PTR");
                using (var mappingMemory = context.CreateFloatMappingMemory(4))
                {
                    var data = new float[4];
                    kernel.SetArgs(mappingMemory, 3F);

                    using (var memMap = mappingMemory.Mapping(commandQueue, true, 0, mappingMemory.Length))
                    {
                        memMap[0] = 3F;
                        memMap[1] = 4.5F;
                        memMap[2] = 0F;
                        memMap[3] = -4.4F;
                    }

                    var event33 = kernel.NDRange(commandQueue);
                    var event34 = mappingMemory.Read(commandQueue, true, data, event33);

                    Console.WriteLine($"exec time      : {event33.ExecutionTime} ns");
                    Console.WriteLine($"read time      : {event34.ExecutionTime} ns");
                    Console.WriteLine($"result         : [{String.Join("  ", data)}]");
                }

                Console.WriteLine("\nCL_MEM_USE_HOST_PTR");
                using (var mappingMemory = context.CreateMappingMemory(new float[] { 3F, 4.5F, 0F, -4.4F }))
                {
                    kernel.SetArgs(mappingMemory, 4F);

                    var event41 = kernel.NDRange(commandQueue);

                    var result = new float[4];
                    var event42 = mappingMemory.Read(commandQueue, true, result, event41);

                    Console.WriteLine($"exec time      : {event41.ExecutionTime} ns");
                    Console.WriteLine($"read time      : {event42.ExecutionTime} ns");
                    Console.WriteLine($"result         : [{String.Join("  ", result)}]");
                }

                Console.WriteLine("\nSVM");
                using (var svmBuffer = context.CreateFloatSVMBuffer(4))
                {
                    kernel.SetArgs(svmBuffer, 5F);

                    svmBuffer[0] = 3F;
                    svmBuffer[1] = 4.5F;
                    svmBuffer[2] = 0F;
                    svmBuffer[3] = -4.4F;
                    var event51 = kernel.NDRange(commandQueue);

                    Console.WriteLine($"exec time      : {event51.ExecutionTime} ns");
                    Console.Write("result         : [  ");
                    for (var i = 0; i < 4; i++)
                        Console.Write($"{svmBuffer[i]}  ");
                    Console.WriteLine("]");
                }
            }

            Console.WriteLine("program end ...");
            Console.Read();
        }

        private static void DisplayPlatformsInfo()
        {
            foreach (var platformInfo in Platform.PlatformInfos)
            {
                int pltInfKeyLenMax = platformInfo.Keys.Select(key => key.Length).Max();
                foreach (var key in platformInfo.Keys)
                {
                    var tab = new String(' ', pltInfKeyLenMax - key.Length);
                    Console.WriteLine($"platform {key}{tab} : {platformInfo.GetValueAsString(key)}");
                }

                int devInfKeyLenMax = ClDeviceInfo.All.Select(key => key.Name.Length).Max();
                foreach (var deviceInfo in platformInfo.DeviceInfos)
                {
                    foreach (var key in ClDeviceInfo.All)
                    {
                        var tab = new String(' ', devInfKeyLenMax - key.Name.Length);

                        if (deviceInfo.ContainsKey(key))
                        {
                            Console.WriteLine($"  device {key.Name}{tab}: {key.GetAsString(deviceInfo)}");
                        }
                        else
                        {
                            Console.WriteLine($"  device {key.Name}{tab}: not supported.");
                        }

                    }
                }
            }
        }

        private static string source = @"
kernel void testKernel(global float* array, float rate){
    int idx = get_global_id(0);
    array[idx] *= rate;
}
";
    }
}
