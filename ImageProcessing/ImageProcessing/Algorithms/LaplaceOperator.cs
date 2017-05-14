namespace ImageProcessing.Algorithms
{
    using System.Drawing;

    using Model;

    internal class LaplaceOperator : BaseAlgorithm
    {
        public LaplaceOperator() : base(AlgorithmType.LaplaceOperator) { }

        public override Bitmap ProcessImage(Bitmap image)
        {
            var newImage = new Bitmap(image);
            // TODO by Łukasz
            return newImage;
        }
    }
}
