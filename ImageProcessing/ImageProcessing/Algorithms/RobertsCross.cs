namespace ImageProcessing.Algorithms
{
    using System.Drawing;

    using Model;

    internal class RobertsCross : BaseAlgorithm
    {
        #region Constructors 

        public RobertsCross() : base(AlgorithmType.RobertsCross)
        {
            Kernel1 = new double[,]
            {
                {0, 0, 0},
                {0, 1, 0},
                {0, 0, -1}
            };

            Kernel2 = new double[,]
            {
                {0, 0, 0},
                {0, 1, 0},
                {-1, 0, 0}
            };
        }

        #endregion

        #region Public Methods

        public override Bitmap ProcessImage(Bitmap image)
        {
            return ProcessImageGrayScale(image);
        }

        #endregion
    }
}
