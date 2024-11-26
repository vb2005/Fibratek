using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fibratek.UI.Controls.Utils
{  
    public partial class StartStopControl : UserControl
    {
        public delegate bool StateChangedHandler(StateMode state);
        public event StateChangedHandler StateChanged;
        private StateMode state;
        private LayoutState layout;


        public StateMode StateValue
        {
            get { return state; }
            set
            {
                state = value;
                ChangeState(value);
            }
        }
        public LayoutState LayoutValue
        {
            get { return layout; }
            set {
                layout = value;
                _layoutChange();
            }
        }


        void _layoutChange()
        {
            this.tableLayoutPanel1.ColumnStyles.Clear();
            this.tableLayoutPanel1.RowStyles.Clear();
            this.tableLayoutPanel1.Controls.Clear();

            if (LayoutValue == UI.Controls.Utils.LayoutState.Horizontal)
            {
                this.tableLayoutPanel1.ColumnCount = 3;
                this.tableLayoutPanel1.RowCount = 1;

                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

                this.tableLayoutPanel1.Controls.Add(this.button3, 2, 0);
                this.tableLayoutPanel1.Controls.Add(this.button2, 1, 0);
                this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            }
            else {
                this.tableLayoutPanel1.RowCount = 3;
                this.tableLayoutPanel1.ColumnCount = 1;

                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

                this.tableLayoutPanel1.Controls.Add(this.button3, 0, 2);
                this.tableLayoutPanel1.Controls.Add(this.button2, 0, 1);
                this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            }

        }




      

        public StartStopControl()
        {
            InitializeComponent();
        }

        public void ChangeState(StateMode state, bool noReaction = false)
        {

            if (!noReaction)
                if (StateChanged != null)
                {
                    bool succseed = StateChanged.Invoke(state);
                    if (!succseed) return;
                }
            this.state = state;
            _internalChangeState();
        }

        private void _internalChangeState()
        {

            switch (state)
            {
                case StateMode.Disabled:
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;

                    button1.BackColor = Color.Gray;
                    button2.BackColor = Color.Gray;
                    button3.BackColor = Color.Gray;
                    break;
                case StateMode.Running:
                    button1.Enabled = false;
                    button2.Enabled = true;
                    button3.Enabled = true;

                    button1.BackColor = Color.Gray;
                    button2.BackColor = Color.Orange;
                    button3.BackColor = Color.Tomato;

                    break;
                case StateMode.Paused:
                    button1.Enabled = true;
                    button2.Enabled = false;
                    button3.Enabled = true;

                    button1.BackColor = Color.LimeGreen;
                    button2.BackColor = Color.Gray;
                    button3.BackColor = Color.Tomato;
                    break;
                case StateMode.Stopped:
                    button1.Enabled = true;
                    button2.Enabled = false;
                    button3.Enabled = false;

                    button1.BackColor = Color.LimeGreen;
                    button2.BackColor = Color.Gray;
                    button3.BackColor = Color.Gray;
                    break;
                default:
                    break;
            }
        }

        private void Run(object sender, EventArgs e)
        {
            if (StateValue == StateMode.Paused)
                StateValue = StateMode.Running;
            else
                StateValue = StateMode.Running;
        }

        private void Pause(object sender, EventArgs e)
        {
            StateValue = StateMode.Paused;
        }

        private void Stop(object sender, EventArgs e)
        {
            StateValue = StateMode.Stopped;
        }
    }

    public enum StateMode { 
       Disabled, Running, Paused, Stopped, Resumed, Exit
    }

    public enum LayoutState
    {
        Vertical, Horizontal
    }
}
