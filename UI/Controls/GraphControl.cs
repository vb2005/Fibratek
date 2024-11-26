using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fibratek.UI.Controls
{
    public partial class GraphControl : UserControl
    {
        List<double> values = new List<double>();
        public GraphControl()
        {
            InitializeComponent();
        }

        public GraphControl(int id)
        {
            InitializeComponent();
            groupBox1.Text = $"Камера №{id}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            values = new List<double>();
            chart1.Series[0].Points.Clear();
        }

        public void AddValue(double value) {
            values.Add(value);
            label1.Text = "Значение: " + value.ToString("0.00") + "\r\n";
            label2.Text = "Мин: " + values.Min().ToString("0.00");
            label3.Text = "Макс: " + values.Max().ToString("0.00");
            label4.Text = "Среднее: " + values.Average().ToString("0.00");
            label5.Text = "Замеров: " + values.Count.ToString("0");

            chart1.Series[0].Points.AddXY(chart1.Series[0].Points.Count, value);
        }
    }
}
