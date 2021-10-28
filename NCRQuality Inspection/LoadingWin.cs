using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCRQuality_Inspection
{
    public partial class LoadingWin: Form
    {

        Action _Worker { get; set; }

        public LoadingWin(Action Worker, string TaskOnProcess, Color color)
        {
            InitializeComponent();
            if (Worker == null) throw new ArgumentNullException();

            _Worker = Worker;
            lblTaskOnProcess.Text = "PLEASE WAIT...\n \n" + TaskOnProcess;
            this.BackColor = color;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Task.Factory.StartNew(_Worker).ContinueWith(t => { this.Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
