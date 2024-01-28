using System.Windows.Forms;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;

            // Show the NotifyIcon in the system tray.
            notifyIcon1.Visible = true;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            Hide();
        }
    }
}