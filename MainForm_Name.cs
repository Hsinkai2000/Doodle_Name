using Doodle_Name.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doodle_Name
{
    public partial class MainForm : Form
    {
        // Declarations
        Bitmap bm;
        Graphics g;
        Pen pen = new Pen(Color.Black, 5);
        SolidBrush brush = new SolidBrush(Color.Black);
        Point startP = new Point(0, 0);
        Point endP = new Point(0, 0);
        Font font = new Font("Arial", 10);
        bool flagDraw = false;
        bool flagErase = false;
        bool flagText = false;
        string strText;
        string fontFamily = "Arial";
        int fontSize = 10;
        int brushSize = 10;


        // Constructor
        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // Method to copy GUID to clipboard
        private void gUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            Clipboard.SetText(attribute.Value.ToString());
        }

        // Form load event
        private void MainForm_Name_Load(object sender, EventArgs e)
        {
            // Initialize bitmap for drawing
            bm = new Bitmap(picBoxMain.Width, picBoxMain.Height);
            picBoxMain.Image = bm;
        }

        // Mouse Events
        // Mouse down event handler for drawing
        private void picBoxMain_MouseDown(object sender, MouseEventArgs e)
        {
            startP = e.Location;
            if (flagText == false)
            {
                if (e.Button == MouseButtons.Left)
                    flagDraw = true;
            }
            else
            {
                flagTextTrueOnMouseDown();
            }
        }

        // Mouse move event handler for drawing
        private void picBoxMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (flagDraw == true)
            {
                flagDrawTrueOnMouseMove(e);
            }
            startP = endP;
        }

        // Mouse up event handler
        private void picBoxMain_MouseUp(object sender, MouseEventArgs e)
        {
            flagDraw = false;
        }


        // Image Button Handler
        // Event handlers for color selection
        private void picBoxRed_Click(object sender, EventArgs e)
        {
            pen.Color = picBoxRed.BackColor;
            setPenColor();
        }

        private void picBoxBlack_Click(object sender, EventArgs e)
        {
            pen.Color = picBoxBlack.BackColor;
            setPenColor();
        }

        private void picBoxGreen_Click(object sender, EventArgs e)
        {
            pen.Color = picBoxGreen.BackColor;
            setPenColor();
        }

        private void picBoxBlue_Click(object sender, EventArgs e)
        {
            pen.Color = picBoxBlue.BackColor;
            setPenColor();
        }

        private void picBoxYellow_Click(object sender, EventArgs e)
        {
            pen.Color = picBoxYellow.BackColor;
            setPenColor();
        }

        private void picBoxWhite_Click(object sender, EventArgs e)
        {
            pen.Color = picBoxWhite.BackColor;
            setPenColor();
        }

        private void picBoxPink_Click(object sender, EventArgs e)
        {
            pen.Color = picBoxPink.BackColor;
            setPenColor();
        }

        private void picBoxMore_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Color selectedColor = colorDialog.Color;
                    pen.Color = selectedColor;
                    setPenColor();
                }
            }
        }

        // Clear drawing
        private void picBoxClear_Click(object sender, EventArgs e)
        {
            changeCurrentIcon(Resources.Clear);
            g = Graphics.FromImage(bm);
            Rectangle rect = picBoxMain.ClientRectangle;
            g.FillRectangle(new SolidBrush(Color.GhostWhite), rect);
            g.Dispose();
            picBoxMain.Invalidate();
        }

        // Eraser mode
        private void picBoxErase_Click(object sender, EventArgs e)
        {
            changeCurrentIcon(Resources.eraser);
            brush = new SolidBrush(picBoxMain.BackColor);
            flagText = false;
            flagErase = true;
        }

        // Text mode
        private void picBoxText_Click(object sender, EventArgs e)
        {
            changeCurrentIcon(Resources.text);
            flagDraw = false;
            flagText = true;
        }

        // Save drawing
        private void picBoxSave_Click(object sender, EventArgs e)
        {

            changeCurrentIcon(Resources.save);
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save Dialog";
                saveFileDialog.Filter = "Image Files(*.BMP)|*.BMP|All files (*.*)|*.*";
                checkDialogOK(saveFileDialog);
            }
        }

        // Brush mode
        private void picBoxBrush_Click(object sender, EventArgs e)
        {
            picBoxBrushColor.BackColor = pen.Color;
            picBoxBrushColor.BackgroundImage = null;
            flagText = false;
            flagErase = false;
        }

        // Toggle Brush size listBox visibility
        private void picBoxBrushSize_Click(object sender, EventArgs e)
        {
            listBoxSizes.Visible = !listBoxSizes.Visible;
        }

        // Load image
        private void picBoxLoad_Click(object sender, EventArgs e)
        {
            changeCurrentIcon(Resources.upload);
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    tryDrawBitMap(dlg);
                }
            }
        }

        // Menu Strip buttons handler
        // Change font to Arial
        private void arialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeFont("Arial");
        }

        // Change font to Calibri
        private void calibriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeFont("Calibri");
        }

        // Change font to Cambria
        private void cambriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeFont("Cambria");
        }

        // Change font size to 10pts
        private void ptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeFontSize(10);
        }

        // Change font size to 30pts
        private void ptToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            changeFontSize(30);
        }

        // Change font size to 50pts
        private void ptToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            changeFontSize(50);
        }

        // Exits Application
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Supporting Methods
        // Method to set pen color
        private void setPenColor()
        {
            picBoxBrushColor.BackColor = pen.Color;
            picBoxBrushColor.Image = null;
            brush.Color = picBoxBrushColor.BackColor;
        }

        // Method to handle drawing text on mouse down
        private void flagTextTrueOnMouseDown()
        {
            strText = txtBoxText.Text;
            g = Graphics.FromImage(bm);
            brush = new SolidBrush(pen.Color);
            g.DrawString(strText, font, brush, startP.X, startP.Y);
            g.Dispose();
            picBoxMain.Invalidate();
        }

        // Method to handle drawing when flagDraw is true on mouse move event
        private void flagDrawTrueOnMouseMove(MouseEventArgs e)
        {
            endP = e.Location;
            g = Graphics.FromImage(bm);
            if (flagErase == false)
                g.DrawLine(pen, startP, endP);
            else
                g.FillEllipse(brush, endP.X, endP.Y, brushSize, brushSize);
            g.Dispose();
            picBoxMain.Invalidate();
        }

        // Method to handle saving of bitmap
        private void useBMP(SaveFileDialog saveFileDialog)
        {
            using (Bitmap bmp = new Bitmap(picBoxMain.Width, picBoxMain.Height))
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                picBoxMain.DrawToBitmap(bmp, rect);
                bmp.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                MessageBox.Show("File Saved Successfully");
            }
        }

        // Method to check if dialog OK
        private void checkDialogOK(SaveFileDialog saveFileDialog)
        {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                useBMP(saveFileDialog);
            }
        }

        // Change font
        private void changeFont(string newFontFamily)
        {
            font.Dispose();
            fontFamily = newFontFamily;
            font = new Font(fontFamily, fontSize);
        }

        // Change font size
        private void changeFontSize(int newFontSize)
        {
            font.Dispose();
            fontSize = newFontSize;
            font = new Font(fontFamily, fontSize);
        }

        // Checks for new brush size 
        private void listBoxSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string size = listBoxSizes.SelectedItem.ToString();

            switch (size)
            {
                case "10pts":
                    brushSize = 10;
                    break;
                case "30pts":
                    brushSize = 30;
                    break;
                case "50pts":
                    brushSize = 50;
                    break;
            }
            pen.Width = brushSize;
            listBoxSizes.Visible = false;
        }

        // Method to handle loading of bitmap
        private void tryDrawBitMap(OpenFileDialog dlg)
        {
            try
            {
                bm = new Bitmap(dlg.FileName);
                DrawBitmap(bm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }

        // Method to draw bitmap
        private void DrawBitmap(Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.DrawImage(bitmap, Point.Empty);
            }
            picBoxMain.Image = bm;
        }

        // Change current icon of picBoxBrushColor
        private void changeCurrentIcon(Image resource)
        {
            picBoxBrushColor.BackgroundImage = resource;
            picBoxBrushColor.BackColor = Color.White;
        }
    }
}