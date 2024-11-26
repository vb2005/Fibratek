using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace Fibratek.UI.Controls
{
    public partial class CameraView : UserControl
    {
        public CameraView()
        {
            InitializeComponent();
        }

        public CameraView(int id)
        {
            InitializeComponent();
            groupBox1.Text = $"Камера №{id}";
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
        }

        public void UpdateImage(Mat img, int pixValue, double mmValue)
        {
            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
            toolStripStatusLabel1.Text = pixValue + " пикс.";
            toolStripStatusLabel2.Text = mmValue.ToString("0.00") + " мм.";
        }
    }
}
