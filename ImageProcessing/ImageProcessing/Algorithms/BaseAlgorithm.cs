namespace ImageProcessing.Algorithms
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    using Model;
    using BitmapData = Model.BitmapData;

    internal class BaseAlgorithm
    {
        #region Private Properties

        private readonly AlgorithmType _type;

        private bool UseOnlyFirstKernel => Kernel2 == null;

        #endregion

        #region Protected Properties

        protected int NumberOfEdgePixels;

        protected const int MagnitudeLimit = 50;

        protected double[,] Kernel1 { get; set; }

        protected double[,] Kernel2 { get; set; }

        #endregion

        #region Constructors

        public BaseAlgorithm(AlgorithmType type)
        {
            _type = type;
        }

        #endregion

        #region Public Methods

        public void ProcessInput(InputData input, OutputData output)
        {
            double count = 1;
            foreach (var image in input.Images)
            {
                Console.CursorLeft = 0;
                Console.Write($"{_type}: {count++/input.Images.Count*100}%");

                NumberOfEdgePixels = 0;

                var stopwatch = Stopwatch.StartNew();
                var processedImage = ProcessImage(image.Value);
                stopwatch.Stop();

                output.Data[_type].Images.Add(image.Key, processedImage);
                output.Data[_type].ExecutionTime.Add(image.Key, stopwatch.ElapsedMilliseconds);
                output.Data[_type].EdgePixelsPercentage.Add(image.Key,
                    NumberOfEdgePixels/(double) (image.Value.Width*image.Value.Height));
            }

            Console.WriteLine(" ");
        }

        public virtual Bitmap ProcessImage(Bitmap image)
        {
            return null;
        }

        /// <summary>
        ///     This is common algorithm used for: Laplace Operator (LAPL1), Roberts Cross and Sobel Operator.
        ///     In this version firstly original image is changed to gray scale, then processing is made for image bytes rather
        ///     than exact pixels
        /// </summary>
        /// <param name="image">Original image</param>
        /// <returns>Processed image in real processed colors.</returns>
        public Bitmap ProcessImageGrayScale(Bitmap image)
        {
            if (image != null)
            {
                image = ToGrayScale(image);
                var originBitmapData = GetByteDataFromBitmap(image);

                var bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite,
                    image.PixelFormat);

                try
                {
                    var totalLength = Math.Abs(bmData.Stride)*image.Height;
                    var rgbValues = new byte[totalLength];
                    var ptr = bmData.Scan0;

                    Marshal.Copy(ptr, rgbValues, 0, totalLength);

                    for (var y = 1; y < image.Height - 1; y++)
                    {
                        for (var x = 1; x < image.Width - 1; x++)
                        {
                            NumberOfEdgePixels += MakeConvolutionWithPixel(originBitmapData, rgbValues, x, y);
                        }
                    }

                    Marshal.Copy(rgbValues, 0, ptr, totalLength);
                }

                finally
                {
                    image.UnlockBits(bmData);
                }
            }

            return image;
        }

        #endregion

        #region Protected Methods

        protected static BitmapData GetByteDataFromBitmap(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                var data = new BitmapData();
                var bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                    bitmap.PixelFormat);

                try
                {
                    var height = bitmap.Height;
                    var width = bitmap.Width;
                    var stride = bmData.Stride;
                    var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat)/8;
                    var offset = stride - width*bytesPerPixel;
                    var totalLength = Math.Abs(stride)*height;
                    var ptr = bmData.Scan0;

                    var rgbValues = new byte[totalLength];
                    Marshal.Copy(ptr, rgbValues, 0, totalLength);

                    data.UpdateValues(rgbValues, width, height, stride, offset, bytesPerPixel, totalLength);
                }
                finally
                {
                    bitmap.UnlockBits(bmData);
                }

                return data;
            }

            return null;
        }

        protected static Bitmap ToGrayScale(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                var grayScaleBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                var data = GetByteDataFromBitmap(bitmap);
                var dstBmData =
                    grayScaleBitmap.LockBits(new Rectangle(0, 0, grayScaleBitmap.Width, grayScaleBitmap.Height),
                        ImageLockMode.ReadWrite, bitmap.PixelFormat);

                try
                {
                    var i = 0;
                    for (var y = 0; y < data.Height; y++)
                    {
                        for (var x = 0; x < data.Width; x++, i += data.BytesPerPixel)
                        {
                            var grayValue = (byte) (0.21f*data.DataBytes[i] +
                                                    0.71f*data.DataBytes[i + 1] +
                                                    0.07f*data.DataBytes[i + 1]);

                            data.DataBytes[i] = grayValue;
                            data.DataBytes[i + 1] = grayValue;
                            data.DataBytes[i + 2] = grayValue;
                        }
                        i += data.Offset;
                    }

                    var dstPtr = dstBmData.Scan0;
                    Marshal.Copy(data.DataBytes, 0, dstPtr, data.TotalLength);
                }

                finally
                {
                    grayScaleBitmap.UnlockBits(dstBmData);
                }

                return grayScaleBitmap;
            }

            return null;
        }

        protected int MakeConvolutionWithPixel(BitmapData srcData, byte[] dst, int x, int y)
        {
            double finalX = 0, finalY = 0;

            for (var i = 0; i < Kernel1.GetLength(0); i++)
            {
                for (var j = 0; j < Kernel1.GetLength(1); j++)
                {
                    var posI = y + i - 1;
                    var posJ = x + j - 1;

                    var pos = (posI*srcData.Width + posJ)*srcData.BytesPerPixel;

                    finalX += Kernel1[i, j]*srcData.DataBytes[pos];
                    if (!UseOnlyFirstKernel)
                    {
                        finalY += Kernel2[i, j]*srcData.DataBytes[pos];
                    }
                }
            }

            var clampedValue = UseOnlyFirstKernel
                ? (byte) Clamp(Math.Abs(finalX), 0, 255.0)
                : (byte) Clamp(Math.Sqrt(finalX*finalX + finalY*finalY), 0, 255.0);

            var currentPixelPos = (y*srcData.Width + x)*srcData.BytesPerPixel;

            if (clampedValue <= MagnitudeLimit)
            {
                dst[currentPixelPos] = Color.Black.R;
                dst[currentPixelPos + 1] = Color.Black.G;
                dst[currentPixelPos + 2] = Color.Black.B;
            }
            else
            {
                dst[currentPixelPos] = Color.White.R;
                dst[currentPixelPos + 1] = Color.White.G;
                dst[currentPixelPos + 2] = Color.White.B;
            }

            //dst[currentPixelPos] = clampedValue;
            //dst[currentPixelPos + 1] = clampedValue;
            //dst[currentPixelPos + 2] = clampedValue;

            return clampedValue > MagnitudeLimit ? 1 : 0;
        }

        #endregion

        #region Private Methods

        private static double Clamp(double val, double min, double max)
        {
            return val < min ? min : (val > max ? max : val);
        }

        #endregion
    }
}