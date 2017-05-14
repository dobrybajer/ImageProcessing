namespace ImageProcessing.Algorithms
{
    using System.Drawing;

    using Model;

    internal class RobertsCross : BaseAlgorithm
    {
        public RobertsCross() : base(AlgorithmType.RobertsCross) { }

        public override Bitmap ProcessImage(Bitmap image)
        {
            var newImage = new Bitmap(image);

            return newImage;
        }
    }
}
