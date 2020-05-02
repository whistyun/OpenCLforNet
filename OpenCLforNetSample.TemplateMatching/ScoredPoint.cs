using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCLforNetSample.TemplateMatching
{
    public struct ScoredPoint
    {
        public int I;
        public int J;
        public float Score;

        public override string ToString()
        {
            return $"<{I},{J}>={Score}";
        }
    }
}
