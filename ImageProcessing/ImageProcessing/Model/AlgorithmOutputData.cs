namespace ImageProcessing.Model
{
    using System.Collections.Generic;
    using System.Drawing;

    internal class AlgorithmOutputData
    {
        public Dictionary<string, Bitmap> Images { get; set; }
        public Dictionary<string, long> ExecutionTime { get; set; }

        public AlgorithmOutputData()
        {
            Images = new Dictionary<string, Bitmap>();
            ExecutionTime = new Dictionary<string, long>();
        }
    }
}
