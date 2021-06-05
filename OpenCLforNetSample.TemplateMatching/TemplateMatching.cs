using OpenCLforNet.Function;
using OpenCLforNet.Memory;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenCLforNetSample.TemplateMatching
{
    public class TemplateMatching : IDisposable
    {
        private static readonly string ShaderSource;

        static TemplateMatching()
        {
            var rscNm = "OpenCLforNetSample.TemplateMatching.TemplateMatching.cl";

            var assemlby = Assembly.GetExecutingAssembly();
            using (var strm = assemlby.GetManifestResourceStream(rscNm))
            using (var reader = new StreamReader(strm))
            {
                ShaderSource = reader.ReadToEnd();
            }
        }


        protected Device Device;
        protected Context Context;
        protected CommandQueue CommandQueue;
        protected CLProgram Program;
        protected Kernel PgKernel;

        public Bitmap Source { set; get; }
        public Bitmap Template { set; get; }
        public float Threashold { set; get; }


        public TemplateMatching()
        {
            Device = new Device();
            Context = Device.CreateContext();
            CommandQueue = Context.CreateCommandQueue();
            Program = Context.CreateProgram(ShaderSource);
            PgKernel = Program.CreateKernel("templateMatching");
        }


        public ScoredPoint[] Find()
        {
            ulong rangeLimit = Device.Info.Get(ClDeviceInfo.DeviceMaxWorkGroupSize);
            int rangeLimitI = (int)Math.Sqrt((int)rangeLimit);
            int rangeLimitJ = (int)rangeLimitI;

            int heiAdding = rangeLimitI - (Source.Height - Template.Height + 1) % rangeLimitI;
            int widAdding = rangeLimitJ - (Source.Width - Template.Width + 1) % rangeLimitJ;

            if (heiAdding == rangeLimitI) heiAdding = 0;
            if (widAdding == rangeLimitJ) widAdding = 0;

            var sourceSource = ImageSource.Create(Source, widAdding, heiAdding);
            var templateSource = new ImageSource(Template);

            int rangeI = sourceSource.Height - Template.Height + 1;
            int rangeJ = sourceSource.Width - Template.Width + 1;

            var ans = new ScoredPoint[rangeI * rangeJ];

            using (var imgMem = Context.CreateSimpleMemory(sourceSource.Data))
            using (var tmpMem = Context.CreateSimpleMemory(templateSource.Data))
            using (var pntAryMem = new SimpleMemory<ScoredPoint>(Context, rangeI * rangeJ))
            {
                PgKernel.SetWorkSize(rangeI, rangeJ);
                PgKernel.SetLocalSize(rangeLimitI, rangeLimitJ);
                PgKernel.SetArgs(
                    rangeJ,
                    imgMem, sourceSource.Width,
                    tmpMem, templateSource.Width, templateSource.Height,
                    pntAryMem);
                PgKernel.NDRange(CommandQueue).Wait();

                pntAryMem.Read(CommandQueue, true, ans);
            }

            return ans.Where(pnt => pnt.Score >= Threashold)
                      .OrderByDescending(pnt => pnt.Score)
                      .Take(100)
                      .ToArray();
        }


        public void Dispose()
        {
            if (!(PgKernel is null))
            {
                PgKernel.Dispose();
                PgKernel = null;
            }
            if (!(Program is null))
            {
                Program.Dispose();
                Program = null;
            }
            if (!(CommandQueue is null))
            {
                CommandQueue.Dispose();
                CommandQueue = null;
            }
            if (!(Context is null))
            {
                Context.Dispose();
                Context = null;
            }
        }
    }
}
