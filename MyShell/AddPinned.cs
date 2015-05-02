using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyShell
{
    public partial class AddPinned : Form
    {
        public string Result;
        System.Timers.Timer Animation;

        public AddPinned(Form Parent)
        {
            InitializeComponent();

            Location = new Point(Parent.Location.X, Parent.Location.Y + Parent.Height - Height);

            Owner = Parent;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtProgramName.Text) && string.IsNullOrWhiteSpace(txtProgramName.Text))
                return;

            Result = txtProgramName.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCANCEL_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Animation = new System.Timers.Timer(10);
            Animation.Elapsed += (se, ev) =>
            {
                if (Location.X <= Owner.Location.X - Width)
                {
                    Invoke(new MethodInvoker(() => { Location = new Point(Owner.Location.X - Width, Location.Y); }));
                    Animation.Stop();
                    return;
                }

                Invoke(new MethodInvoker(() => { Location = new Point(Location.X - 10, Location.Y); }));
            };
            Animation.Start();
        }
    }
}
