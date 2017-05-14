namespace ImageProcessing.Algorithms
{
    using System.Drawing;

    using Model;

    internal class SobelOperator : BaseAlgorithm
    {
        public SobelOperator() : base(AlgorithmType.SobelOperator) { }

        public override Bitmap ProcessImage(Bitmap image)
        {
            var newImage = new Bitmap(image);
            // TODO by Kamil
            return newImage;
        }
    }
}
