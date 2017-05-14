namespace ImageProcessing.Algorithms
{
    using System;
    using System.Diagnostics;
    using System.Drawing;

    using Model;

    internal class BaseAlgorithm
    {
        private readonly AlgorithmType _type;

        public BaseAlgorithm(AlgorithmType type)
        {
            _type = type;
        }

        public void ProcessInput(InputData input, OutputData output)
        {
            double count = 1;
            foreach (var image in input.Images)
            {
                Console.CursorLeft = 0;
                Console.Write($"{_type}: {count++/input.Images.Count*100}%");

                var stopwatch = Stopwatch.StartNew();
                var processedImage = ProcessImage(image.Value);
                stopwatch.Stop();

                output.Data[_type].Images.Add(image.Key, processedImage);
                output.Data[_type].ExecutionTime.Add(image.Key, stopwatch.ElapsedMilliseconds);
            }

            Console.WriteLine(" ");
        }

        public virtual Bitmap ProcessImage(Bitmap image)
        {
            return null;
        }
    }
}
