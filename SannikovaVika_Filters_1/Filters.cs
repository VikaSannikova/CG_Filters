using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace SannikovaVika_Filters_1
{
    abstract class Filters
    {
        protected virtual void BeforeProcessImage(Bitmap sourceImage){ }
  
        protected abstract Color calculateNewPixelColor(Bitmap sourseImage, int x, int y);

        public Bitmap processImage(Bitmap sourseImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);
            for (int i = 0; i < sourseImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null; ////////////////////////////////////
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourseImage, i, j));
                }
            }
            return resultImage;
        }

        public int Clamp (int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value; 
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourseColor = sourseImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourseColor.R,
                                               255 - sourseColor.B,
                                               255 - sourseColor.G);
            return resultColor;
        }

    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for(int l = -radiusY;l <= radiusY; l++)
            {
                for(int k = -radiusX; k <= radiusX; k ++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighbourColor = sourseImage.GetPixel(idX, idY);
                    resultR += neighbourColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighbourColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighbourColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp((int)resultR, 0 ,255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }

    }

   static class MathMorfology
    {
       static  public Bitmap Dilation(Bitmap sourseImage, bool[,] mask/*, BackgroundWorker worker*/)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);
            int maskH = mask.GetLength(0);
            int maskW = mask.GetLength(1);
            Color sourseColor;
            for (int j = maskH / 2; j < sourseImage.Height - maskH / 2; j++)
            {
                //worker.ReportProgress((int)((float)i / sourseImage.Width * 100));
                for (int i = maskW / 2; i < sourseImage.Width - maskW / 2; i++)
                {
                    int maxR=0, maxG=0, maxB=0;
                    sourseColor = sourseImage.GetPixel(i - maskH / 2, j - maskW / 2);
                    maxR = sourseColor.R;
                    maxG = sourseColor.G;
                    maxB = sourseColor.B;

                    for (int l = -maskH / 2; l <= maskH / 2; l++)
                        for (int k = -maskW / 2; k <= maskW/2; k++)
                        {
                            if (mask[k + maskW / 2, l + maskH / 2])
                            {
                                if (sourseImage.GetPixel(i + k, j + l).R > maxR)
                                    maxR = sourseImage.GetPixel(i + k, j + l).R;
                                if (sourseImage.GetPixel(i + k, j + l).G > maxG)
                                    maxG = sourseImage.GetPixel(i + k, j + l).G;
                                if (sourseImage.GetPixel(i + k, j + l).B > maxB)
                                    maxB = sourseImage.GetPixel(i + k, j + l).B;
                            }
                        }
                    resultImage.SetPixel(i - maskW/2 , j - maskH/2, Color.FromArgb(maxR, maxG, maxB));
                }
            }
            return resultImage;
        }
       static  public Bitmap Erosion(Bitmap sourseImage, bool[,] mask)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);
            int maskH = mask.GetLength(0);
            int maskW = mask.GetLength(1);
            Color sourseColor;
            for (int j = maskH / 2; j < sourseImage.Height - maskH / 2; j++)
            {
                //worker.ReportProgress((int)((float)i / sourseImage.Width * 100));
                for (int i = maskW / 2; i < sourseImage.Width - maskW / 2; i++)
                {
                    int minR = 255, minG = 255, minB = 255;
                    sourseColor = sourseImage.GetPixel(i - maskH / 2, j - maskW / 2);
                    minR = sourseColor.R;
                    minG = sourseColor.G;
                    minB = sourseColor.B;

                    for (int l = -maskH / 2; l <= maskH / 2; l++)
                        for (int k = -maskW / 2; k <= maskW / 2; k++)
                        {
                            if (mask[k + maskW / 2, l + maskH / 2])
                            {
                                if (sourseImage.GetPixel(i + k, j + l).R < minR)
                                    minR = sourseImage.GetPixel(i + k, j + l).R;
                                if (sourseImage.GetPixel(i + k, j + l).G < minG)
                                    minG = sourseImage.GetPixel(i + k, j + l).G;
                                if (sourseImage.GetPixel(i + k, j + l).B < minB)
                                    minB = sourseImage.GetPixel(i + k, j + l).B;
                            }
                        }
                    resultImage.SetPixel(i - maskW / 2, j - maskH / 2, Color.FromArgb(minR, minG, minB));
                }
            }
            return resultImage;
        } 

       static  public Bitmap Opening(Bitmap sourseImage, bool[,] mask)
        {
            Bitmap ResultImage = Erosion(sourseImage, mask);
            ResultImage = Dilation(ResultImage, mask);
            return ResultImage;
        }

        static public Bitmap Closing(Bitmap sourseImage, bool[,] mask)
        {
            Bitmap ResultImage = Dilation(sourseImage, mask);
            ResultImage = Erosion(ResultImage, mask);
            return ResultImage;
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }

    class GaussianFilter : MatrixFilter
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            //определяем размер ядра
            int size = 2 * radius + 1;
            //создаем ядро фильтра
            kernel = new float[size, size];
            //коэффициент нормировки ядра
            float norm = 0;
            //рассчитываем ядро нормировки фильтра
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }
            //нормируем ядро
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }

    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x,y);
            int Intensity =(int)( 0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            Color resultColor = Color.FromArgb(Intensity,
                                               Intensity,
                                               Intensity);
            return resultColor;
        }
    }

    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            int k = 100;
            int Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            Color resultColor = Color.FromArgb(Clamp(Intensity + 2*k,0,255),
                                               Clamp((int)(Intensity + 0.5*k),0,255),
                                               Clamp((Intensity - 1*k),0,255));
            return resultColor;
        }
    }

    class IncreaseВrightness : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourseColor = sourseImage.GetPixel(x, y);
            int k = 30;
            Color resultColor = Color.FromArgb(Clamp(sourseColor.R+k,0,255),
                                               Clamp(sourseColor.B+k,0,255),
                                               Clamp(sourseColor.G+k,0,255));
            return resultColor;
        }
    }

    class SobelFilter : MatrixFilter
    {
        public SobelFilter()
        {
            kernel = new float[3, 3] { { -1,-2,-1}, {0,0,0 }, {1,2,1 } };
        }
    }

    class SharpnessFilter : MatrixFilter
    {
        public SharpnessFilter()
        {
            kernel = new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
    }

    class EmbossedFilter : MatrixFilter
    {

        public void createEmbrossedKernel()
        {
            //создаем ядро фильтра
            kernel = new float[3, 3] { { 0, 1, 0 }, { 1, 0, -1 }, { 0, -1, 0 } };
            //коэффициент нормировки ядра
            float norm = 0;
            //рассчитываем ядро нормировки фильтра
            for (int i = -1; i <= -1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    norm += kernel[i + 1, j + 1];
                }
            }

            //нормируем ядро
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    kernel[i, j] /= norm;
        }
        public EmbossedFilter()
        {
            createEmbrossedKernel();
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int coef = 50;

            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idx = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idy = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighborColor = sourseImage.GetPixel(idx, idy);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp((int)(resultR + coef), 0, 255),
                Clamp((int)(resultG + coef), 0, 255),
                Clamp((int)(resultB + coef), 0, 255)
            );
        }

    }

    class MotionBlurFilter : MatrixFilter
    {
        public void createMotinBlurKernel(int rad)
        {
            int size = rad * 2 + 1;
            kernel = new float[size, size];
            float norm = 0;
            for(int i = -rad; i<=rad;i++)
                for(int j= -rad; j<=rad; j++)
                {
                    if (i == j) kernel[i + rad, j + rad] = 1f / size;
                    else
                        kernel[i + rad, j + rad] = 0;
                    norm += kernel[i + rad, j + rad];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
        public MotionBlurFilter(int rad)
            {
            createMotinBlurKernel(rad);
            }

    }

    class GlassEffectFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Random randomX = new Random(x*60);
            Random randomY = new Random(y*60);
            int newX = (int)(x + ((double)(randomX.Next(10)%2) - 0.5) * 10);
            int newY = (int)(y + ((double)(randomY.Next(10)%2) - 0.5) * 10);
            double resultR, resultG, resultB;
            if (newY < 0 || newY >= sourseImage.Height || newX < 0 || newX >= sourseImage.Width)
            {
                Color sourceColor = sourseImage.GetPixel(x, y);
                resultR = sourceColor.R;
                resultG = sourceColor.G;
                resultB = sourceColor.B;

            }
            else
            {
                Color sourseColor = sourseImage.GetPixel(newX, newY);
                resultR = sourseColor.R;
                resultG = sourseColor.G;
                resultB = sourseColor.B;
            }
            Color resultColor = Color.FromArgb(Clamp((int)resultR,0,255), 
                                               Clamp((int)resultG,0,255), 
                                               Clamp((int)resultB,0,255));
            return resultColor;
        }

    }

    class VerticalWavesFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            double resultR, resultG, resultB;
            int newX = (int)(x + 20 * Math.Sin(2 * Math.PI * x / 30)); 
            if (newX<0||newX>=sourseImage.Width)
            {
                Color sourceColor = sourseImage.GetPixel(x, y);
                resultR = sourceColor.R;
                resultG = sourceColor.G;
                resultB = sourceColor.B;
            }
            else
            {
                Color sourseColor = sourseImage.GetPixel(newX, y);
                resultR = sourseColor.R;
                resultG = sourseColor.G;
                resultB = sourseColor.B;
            }
            Color resultColor = Color.FromArgb(Clamp((int)resultR, 0, 255),
                                              Clamp((int)resultG, 0, 255),
                                              Clamp((int)resultB, 0, 255));
            return resultColor;
        }
    }

    class HorizontalWavesFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            double resultR, resultG, resultB;
            int newY = (int)(y + 20 * Math.Sin(2 * Math.PI * y / 30));
            if (newY < 0 || newY >= sourseImage.Height)
            {
                Color sourceColor = sourseImage.GetPixel(x, y);
                resultR = sourceColor.R;
                resultG = sourceColor.G;
                resultB = sourceColor.B;
            }
            else
            {
                Color sourseColor = sourseImage.GetPixel(x, newY);
                resultR = sourseColor.R;
                resultG = sourseColor.G;
                resultB = sourseColor.B;
            }
            Color resultColor = Color.FromArgb(Clamp((int)resultR, 0, 255),
                                              Clamp((int)resultG, 0, 255),
                                              Clamp((int)resultB, 0, 255));
            return resultColor;
        }
    }

    class GrayWorldFilter : Filters
    {
        private double avg, avgR, avgG, avgB;
        public void FindAvg(Bitmap sourseImage)
        {
            int kol = sourseImage.Width * sourseImage.Height;
            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            for (int i = 0; i < sourseImage.Width; i++)
                for(int j = 0;j < sourseImage.Height;  j++)
                {
                    sumR += sourseImage.GetPixel(i, j).R;
                    sumG += sourseImage.GetPixel(i, j).G;
                    sumB += sourseImage.GetPixel(i, j).B;
                }
            avgR = sumR / kol;
            avgG = sumG / kol;
            avgB = sumB / kol;
            avg = (avgR + avgG + avgB) / 3;
        }
        //protected override void BeforeProcessImage(Bitmap sourseImage)
        //{
        //    FindAvg(sourseImage);
        //}
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color resultColor = Color.FromArgb(Clamp((int)(sourseImage.GetPixel(x, y).R * avg / avgR), 0, 255),
                                               Clamp((int)(sourseImage.GetPixel(x, y).G * avg / avgG), 0, 255),
                                               Clamp((int)(sourseImage.GetPixel(x, y).B * avg / avgB), 0, 255));
            return resultColor;
        }
    }

}
