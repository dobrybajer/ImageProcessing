﻿namespace ImageProcessing.Algorithms
{
    using System.Drawing;

    using Model;

    /// <summary>
    /// More details: https://blog.saush.com/2011/04/20/edge-detection-with-the-sobel-operator-in-ruby/
    /// </summary>
    internal class SobelOperator : BaseAlgorithm
    {
        private const int MagnitudeLimit = 128 * 128;

        public SobelOperator() : base(AlgorithmType.SobelOperator) { }

        /// <summary>
        /// In this algorithm rather than firstly changing original image to gray scale, I consider all RGB values separately
        /// </summary>
        /// <param name="image">Original mage</param>
        /// <returns>Processed image in white and black colors.</returns>
        public override Bitmap ProcessImage(Bitmap image)
        {
            var processedImage = new Bitmap(image);
            var outputImage = new Bitmap(image);

            var width = processedImage.Width;
            var height = processedImage.Height;

            var gx = new[,]
            {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };
            var gy = new[,]
            {
                {1, 2, 1},
                {0, 0, 0},
                {-1, -2, -1}
            };

            var redPixels = new int[width, height];
            var greenPixels = new int[width, height];
            var bluePixels = new int[width, height];

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    redPixels[i, j] = processedImage.GetPixel(i, j).R;
                    greenPixels[i, j] = processedImage.GetPixel(i, j).G;
                    bluePixels[i, j] = processedImage.GetPixel(i, j).B;
                }
            }

            for (var i = 1; i < processedImage.Width - 1; i++) // image width
            {
                for (var j = 1; j < processedImage.Height - 1; j++) // image height
                {
                    var newRx = 0;
                    var newRy = 0;
                    var newGx = 0;
                    var newGy = 0;
                    var newBx = 0;
                    var newBy = 0;

                    // Convolution
                    for (var yk = -1; yk < 2; yk++) // horizontal (x) kernel
                    {
                        for (var xk = -1; xk < 2; xk++) // vertical (y) kernel
                        {
                            var rc = redPixels[i + xk, j + yk];
                            newRx += gx[yk + 1, xk + 1]*rc;
                            newRy += gy[yk + 1, xk + 1]*rc;

                            var gc = greenPixels[i + xk, j + yk];
                            newGx += gx[yk + 1, xk + 1]*gc;
                            newGy += gy[yk + 1, xk + 1]*gc;

                            var bc = bluePixels[i + xk, j + yk];
                            newBx += gx[yk + 1, xk + 1]*bc;
                            newBy += gy[yk + 1, xk + 1]*bc;
                        }
                    }

                    // If magnitude is higher than defined limit - this is a edge pixel
                    outputImage.SetPixel(i, j,
                        newRx*newRx + newRy*newRy > MagnitudeLimit ||
                        newGx*newGx + newGy*newGy > MagnitudeLimit ||
                        newBx*newBx + newBy*newBy > MagnitudeLimit
                            ? Color.Black : Color.White);
                }
            }

            return outputImage;
        }
    }
}
