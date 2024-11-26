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
using Fibratek.Data;
using Fibratek.Controllers;

namespace Fibratek.UI.Controls
{
    public partial class MainForm : Form
    {
        List<GraphControl> controls = new List<GraphControl>();
        List<CameraView> views = new List<CameraView>();
        public MainForm()
        {
            InitializeComponent();
            RestController.Start();
            for (int i = 0; i < 3; i++)
            {
                controls.Add(new GraphControl(i+1) { Width = flowLayoutPanel1.Width });
                flowLayoutPanel1.Controls.Add(controls[i]);

                views.Add(new CameraView(i+1) { Width = flowLayoutPanel2.Width, Height = controls[i].Height });
                flowLayoutPanel2.Controls.Add(views[i]);

                SessionSettings.Values.Add(new List<double>());
            }
        }

        private bool startStopControl1_StateChanged(Utils.StateMode state)
        {
            SessionSettings.State = state;

            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Заглушка ради UI

            if (SessionSettings.State == Utils.StateMode.Running)
            {
                for (int i = 0; i < 3; i++)
                {
                    OpenCvSharp.Mat mat = new Mat(1000, 1000, MatType.CV_8UC1);
                    mat.Randn(new Scalar(128), new Scalar(100));

                    int pix = Calculation.Calc.GetMaxLine(mat, LocalSettings.Instance.ThreshValue);
                    double mm = Calculation.Calc.GetTrueValue(pix);

                    views[i].UpdateImage(mat, pix, mm);
                    controls[i].AddValue(mm);

                    SessionSettings.Values[i].Add(mm);
                    while (SessionSettings.Values[i].Count > 1000) SessionSettings.Values[i].RemoveAt(0);
                }

            }
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            controls.ForEach(s=>s.Width = flowLayoutPanel1.Width);  
        }
    }
}
