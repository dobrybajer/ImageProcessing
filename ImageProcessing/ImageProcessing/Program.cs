namespace ImageProcessing
{
    using System;

    using Algorithms;
    using Data;
    using Model;

    internal class Program
    {
        private static void Main()
        {
            var input = FileManager.ReadInput();
            var output = new OutputData(input.Images);

            var canny = new Canny();
            var laplaceOperator = new LaplaceOperator();
            var robertsCross = new RobertsCross();
            var sobelOperator = new SobelOperator();

            canny.ProcessInput(input, output);
            laplaceOperator.ProcessInput(input, output);
            robertsCross.ProcessInput(input, output);
            sobelOperator.ProcessInput(input, output);

            Console.WriteLine();

            FileManager.WriteOutput(output);
  
            Console.ReadKey();
        }
    }
}
