using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace OpenCLforNetSample.TemplateMatching
{
    public class DesktopCaptureUtil
    {
        public static Bitmap TakeScreensoht()
        {
            var captureRect = Screen.PrimaryScreen.Bounds;
            var captureImage = new Bitmap(
                captureRect.Width, captureRect.Height,
                PixelFormat.Format24bppRgb);

            using (var g = Graphics.FromImage(captureImage))
            {
                g.CopyFromScreen(
                    captureRect.X, captureRect.Y,
                    0, 0,
                    captureRect.Size,
                    CopyPixelOperation.SourceCopy);
            }

            return captureImage;
        }

        public static Bitmap ClipImageFromDesktop()
        {
            Rectangle captureRect;
            using (var f = new DesktopCaptureDialog())
            {
                f.ShowDialog();
                captureRect = f.ClippedRectangle;
            }

            var captureImage = new Bitmap(
                captureRect.Width, captureRect.Height,
                PixelFormat.Format24bppRgb);

            using (var g = Graphics.FromImage(captureImage))
            {
                g.CopyFromScreen(
                    captureRect.X, captureRect.Y,
                    0, 0,
                    captureRect.Size,
                    CopyPixelOperation.SourceCopy);
            }

            return captureImage;
        }
    }

    class DesktopCaptureDialog : Form
    {
        public Rectangle ClippedRectangle = new Rectangle();
        public Point Point1 = new Point();
        public Point Point2 = new Point();

        private Brush brush = new SolidBrush(Color.FromArgb(64, Color.Red));
        private Pen markPen = new Pen(Color.Red, 1f);

        private bool mouseDown;
        private Bitmap bmp;
        private PictureBox pictureBox;

        public DesktopCaptureDialog()
        {
            this.pictureBox = new PictureBox();
            this.pictureBox.BackColor = Color.Transparent;
            this.pictureBox.Dock = DockStyle.Fill;
            this.pictureBox.Margin = new Padding(0);
            this.pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBox.MouseDown += this.pictureBox_MouseDown;
            this.pictureBox.MouseMove += this.pictureBox_MouseMove;
            this.pictureBox.MouseUp += this.pictureBox_MouseUp;

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(0, 0);
            this.Controls.Add(this.pictureBox);
            this.Cursor = Cursors.Cross;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += Form2_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Screen sc = Owner == null ?
                Screen.PrimaryScreen :
                Screen.FromControl(Owner);

            Rectangle rc = sc.Bounds;
            this.Bounds = rc;

            bmp = new Bitmap(rc.Width, rc.Height);
            Bitmap backgroundBmp = new Bitmap(rc.Width, rc.Height);

            using (Graphics g = Graphics.FromImage(backgroundBmp))
            {
                g.CopyFromScreen(sc.Bounds.X, sc.Bounds.Y, 0, 0, this.pictureBox.Size, CopyPixelOperation.SourceCopy);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0x55, 0xAA, 0xAA, 0xAA)), 0, 0, sc.Bounds.Width, sc.Bounds.Height);
                this.pictureBox.BackgroundImage = backgroundBmp;
            }

            this.DrawRegion();
            this.pictureBox.Image = bmp;

            this.Activate();
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            Point2 = e.Location;

            ClippedRectangle.X = Math.Min(Point1.X, Point2.X);
            ClippedRectangle.Y = Math.Min(Point1.Y, Point2.Y);
            ClippedRectangle.Width = Math.Abs(Point1.X - Point2.X);
            ClippedRectangle.Height = Math.Abs(Point1.Y - Point2.Y);

            Point1.X = ClippedRectangle.X;
            Point1.Y = ClippedRectangle.Y;
            Point2.X = ClippedRectangle.Right;
            Point2.Y = ClippedRectangle.Bottom;

            Close();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point2 = e.Location;
                DrawRegion();
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            Point1 = e.Location;
            Point2 = e.Location;
            DrawRegion();
        }

        private void DrawRegion()
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                var x = Math.Min(Point1.X, Point2.X);
                var y = Math.Min(Point1.Y, Point2.Y);
                var wid = Math.Abs(Point1.X - Point2.X);
                var hei = Math.Abs(Point1.Y - Point2.Y);

                g.FillRectangle(brush, x, y, wid, hei);
                g.DrawRectangle(markPen, x, y, wid, hei);

                this.pictureBox.Image = bmp;
            }
        }

    }
}
