namespace ImageProcessing.Algorithms
{
    using System.Drawing;
    
    using Model;

    internal class Canny : BaseAlgorithm
    {
        public Canny() : base(AlgorithmType.Canny) { }

        public override Bitmap ProcessImage(Bitmap image)
        {
            var newImage = new Bitmap(image);
            
            return newImage;
        }
    }
}
