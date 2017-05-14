namespace ImageProcessing.Model
{
    using System.Collections.Generic;
    using System.Drawing;

    internal class InputData
    {
        public Dictionary<string, Bitmap> Images { get; set; }

        public InputData()
        {
            Images = new Dictionary<string, Bitmap>();
        }
    }
}
