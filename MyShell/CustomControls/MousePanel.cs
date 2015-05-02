using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

public partial class MousePanel : Panel
{
    public bool isLabelOver = false;
    private Label l;
    private PictureBox p;
    private string PathName = null;
    private bool isMinimal = false;

    public MousePanel(RegistryValues rv, int FormWidth, int left, int top, bool minimal = false)
    {
        isMinimal = minimal;

        InitializeComponent();

        p = new PictureBox();
        l = new Label();

        Size = new Size((isMinimal ? FormWidth / 2 : FormWidth), 80);
        Location = new Point(left, top);
        Cursor = Cursors.Hand;
        if (!isMinimal)
            PathName = rv.Value;

        Name = rv.Name;

        if(isMinimal)
            rv.Name = rv.Name.Substring(7);

        p.Size = (isMinimal ? new Size(Width - 2, Height - 2 - 30) : new Size(50, 49));
        p.Location = (isMinimal ? new Point(1, 1) : new Point((Width / 2) - (p.Width / 2), 1));
        p.Image = Icon.ExtractAssociatedIcon(rv.Value).ToBitmap();
        p.SizeMode = PictureBoxSizeMode.CenterImage;
        p.MouseClick += (sender, e) => { if(!isMinimal && p.FindForm().Opacity < 1) return; OnClick(e); };

        Controls.Add(p);

        l.AutoSize = false;
        l.AutoEllipsis = true;
        l.Size = new Size(Width - 2, 28);
        l.Location = new Point(1, (p.Location.Y + p.Height) - 1);
        l.Text = rv.Name;
        l.TextAlign = ContentAlignment.TopCenter;
        l.Font = new Font("Segoe UI", 8, FontStyle.Regular);
        l.MouseClick += (sender, e) => { if (!isMinimal && p.FindForm().Opacity < 1) return; OnClick(e); };
        l.Hide();

        Controls.Add(l);
    }

    public MousePanel(IContainer container)
    {
        container.Add(this);

        InitializeComponent();
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);

        if(!isMinimal) System.Diagnostics.Process.Start(PathName);
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        e.Control.MouseLeave += ReallyLeave;

        base.OnControlAdded(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        ReallyLeave(this, e);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);

        BackColor = Color.FromArgb(255, 100, 100, 100);
        l.Show();
    }

    private void ReallyLeave(object sender, EventArgs e)
    {
        if (ClientRectangle.Contains(PointToClient(MousePosition)))
            return;

        base.OnMouseLeave(e);

        BackColor = Color.FromArgb(0, 100, 100, 100);
        l.Hide();
    }
}