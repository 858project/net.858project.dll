using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Project858.Drawing
{
    /// <summary>
    /// Rozne pomocne metody na upravu a spravu obrazkov
    /// </summary>
    public static class ImageUtility
    {
        #region - Properties -
        /// <summary>
        /// (Get) Vrati metrix na zmenu farebnej skaly obrazku
        /// </summary>
        private static ColorMatrix DisabledImageColorMatrix
        {
            get
            {
                if (_disabledImageColorMatrix == null)
                {
                    _disabledImageColorMatrix = GetDisabledImageColorMatrix();
                }
                return _disabledImageColorMatrix;
            }
        }
        #endregion

        #region - Variable -
        /// <summary>
        /// Color Metrix na zmenu farebnej sklay
        /// </summary>
        private static ColorMatrix _disabledImageColorMatrix = null;
        #endregion

        #region - Method -
        /// <summary>
        /// Nastavi obrazku Opacity parameter
        /// </summary>
        /// <param name="image">Obrazok na ktory chcem nastavit opacity</param>
        /// <param name="imgOpac">Hodnota Opacity</param>
        /// <returns>Upraveny obrazok</returns>
        public static Image SetOpacity(Image image, float imgOpac)
        {
            Bitmap bmpPic = new Bitmap(image.Width, image.Height);
            Graphics gfxPic = Graphics.FromImage(bmpPic);
            ColorMatrix cmxPic = new ColorMatrix();
            cmxPic.Matrix33 = imgOpac;

            ImageAttributes iaPic = new ImageAttributes();
            iaPic.SetColorMatrix(cmxPic, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            gfxPic.DrawImage(image, new Rectangle(0, 0, bmpPic.Width, bmpPic.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, iaPic);
            gfxPic.Dispose();

            return bmpPic;
        }
        /// <summary>
        /// Spracuje vstupny obrazok a vrati obrazok so zmenenou farebnou skalou
        /// </summary>
        /// <param name="normalImage">Obrazok ktory chceme upravit</param>
        /// <returns>Upraveny obrazok</returns>
        public static Image CreateDisabledImage(Image normalImage)
        {
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.ClearColorKey();
            imageAttr.SetColorMatrix(DisabledImageColorMatrix);
            Size size = normalImage.Size;
            Bitmap image = new Bitmap(size.Width, size.Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.DrawImage(normalImage, new Rectangle(0, 0, size.Width, size.Height), 0, 0, size.Width, size.Height, GraphicsUnit.Pixel, imageAttr);
            graphics.Dispose();
            return image;
        }
        /// <summary>
        /// Vytvori a vrati Matrix na upravu obrazku do disbale modu
        /// </summary>
        /// <returns>ColorMatrix na upravu obrazku</returns>
        private static ColorMatrix GetDisabledImageColorMatrix()
        {
            float[][] numArray = new float[5][];
            numArray[0] = new float[] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
            numArray[1] = new float[] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
            numArray[2] = new float[] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
            float[] numArray3 = new float[5];
            numArray3[3] = 1f;
            numArray[3] = numArray3;
            numArray[4] = new float[] { 0.38f, 0.38f, 0.38f, 0f, 1f };
            float[][] numArray2 = new float[5][];
            float[] numArray4 = new float[5];
            numArray4[0] = 1f;
            numArray2[0] = numArray4;
            float[] numArray5 = new float[5];
            numArray5[1] = 1f;
            numArray2[1] = numArray5;
            float[] numArray6 = new float[5];
            numArray6[2] = 1f;
            numArray2[2] = numArray6;
            float[] numArray7 = new float[5];
            numArray7[3] = 0.7f;
            numArray2[3] = numArray7;
            numArray2[4] = new float[5];

            //vratime matrix
            return MultiplyColorMatrix(numArray2, numArray);
        }
        /// <summary>
        /// Vynasobi dve farebne sklady
        /// </summary>
        /// <param name="matrix1">Prva Matrix na nasobenie</param>
        /// <param name="matrix2">Druha Matrix na nasobenie</param>
        /// <returns>Vysledok nasobenia metrix</returns>
        private static ColorMatrix MultiplyColorMatrix(float[][] matrix1, float[][] matrix2)
        {
            int num = 5;
            float[][] newColorMatrix = new float[num][];
            for (int i = 0; i < num; i++)
            {
                newColorMatrix[i] = new float[num];
            }
            float[] numArray2 = new float[num];
            for (int j = 0; j < num; j++)
            {
                for (int k = 0; k < num; k++)
                {
                    numArray2[k] = matrix1[k][j];
                }
                for (int m = 0; m < num; m++)
                {
                    float[] numArray3 = matrix2[m];
                    float num6 = 0f;
                    for (int n = 0; n < num; n++)
                    {
                        num6 += numArray3[n] * numArray2[n];
                    }
                    newColorMatrix[m][j] = num6;
                }
            }
            return new ColorMatrix(newColorMatrix);
        }
        #endregion
    }
}
