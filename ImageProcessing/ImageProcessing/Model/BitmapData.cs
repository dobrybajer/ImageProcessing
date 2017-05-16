namespace ImageProcessing.Model
{
    internal class BitmapData
    {
        public byte[] DataBytes { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Stride { get; set; }

        public int Offset { get; set; }

        public int BytesPerPixel { get; set; }

        public int TotalLength { get; set; }

        public void UpdateValues(
            byte[] dataBytes, 
            int width, 
            int height, 
            int stride, 
            int offset, 
            int bytesPerPixel, 
            int totalLength)
        {
            DataBytes = dataBytes;
            Width = width;
            Height = height;
            Stride = stride;
            Offset = offset;
            BytesPerPixel = bytesPerPixel;
            TotalLength = totalLength;
        }
    }
}
