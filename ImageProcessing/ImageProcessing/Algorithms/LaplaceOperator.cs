namespace ImageProcessing.Algorithms
{
    using System.Drawing;

    using Model;

    internal class LaplaceOperator : BaseAlgorithm
    {
        #region Constructors 

        public LaplaceOperator() : base(AlgorithmType.LaplaceOperator)
        {
            // LAPL1
            Kernel1 = new double[,]
            {
                { 0, -1, 0 },
                { -1, 4, -1 },
                { 0, -1, 0 }
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
