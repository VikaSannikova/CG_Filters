using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SannikovaVika_Filters_1
{
    public partial class Form1 : Form
    {
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files(*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
               image = new Bitmap(dialog.FileName);
            }
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void гауссовФильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void чернобелыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void увеличениеЯркостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new IncreaseВrightness();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void повышениеРезкостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new EmbossedFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MotionBlurFilter(3);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void эффектСтеклаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GlassEffectFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void вертикальныеВолныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new VerticalWavesFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void горизонтальныеВолныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new HorizontalWavesFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GrayWorldFilter filter = new GrayWorldFilter();
            filter.FindAvg(image);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool[,] mask = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Dilation(image, mask);
            pictureBox1.Refresh();
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool[,] mask = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Erosion(image, mask);
            pictureBox1.Refresh();
        }

        private void отрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool[,] mask = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Opening(image, mask);
            pictureBox1.Refresh();
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool[,] mask = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, true, false } };
            pictureBox1.Image = MathMorfology.Closing(image, mask);
            pictureBox1.Refresh();
        }

        private void линейноеРастяжениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearStretchingFilter filter = new LinearStretchingFilter();
            filter.FindMaxMin(image);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйФильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter(3);
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
