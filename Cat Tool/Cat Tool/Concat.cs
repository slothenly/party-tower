using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cat_Tool
{
    public partial class Concat : Form
    {
        private List<Button> buttons = new List<Button>();
        Font textFont;
        

        /// <summary>
        /// Initialize the form and set up all button placement/text/etc.
        /// </summary>
        public Concat()
        {
            InitializeComponent();
            this.Text = "Contatination Tool";
            this.Height = 800;
            this.Width = 1400;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(75, 20);

            const int ROWS = 5;
            const int COLUMNS = 5;

            int btnWidth = (Width - 90) / COLUMNS;
            int btnHeight = (Height - 250) / ROWS;
            int xPos;
            int yPos;
            textFont = new Font("Ariel", 18.0f);

            for (int rows = 0; rows < ROWS; rows++)
            {
                for (int columns = 0; columns < COLUMNS; columns++)
                {
                    xPos = btnWidth * rows;
                    yPos = btnHeight * columns;

                    Button btn = new Button();
                    btn.Left = xPos + 40;
                    btn.Top = yPos + 25;
                    btn.Width = btnWidth;
                    btn.Height = btnHeight;
                    btn.Text = "<empty>";
                    btn.Font = textFont;
                    btn.Click += AddNewMap;

                    buttons.Add(btn);
                    this.Controls.Add(btn);
                }
            }

            Button concat = new Button();
            concat.Font = textFont;
            concat.Height = btnHeight + 20;
            concat.Width = btnWidth + 85;
            concat.Left = btnWidth * 2;
            concat.Top = (btnHeight * 5) + 50;
            concat.Text = "Combine";
            concat.Click += Concatinate;
            buttons.Add(concat);
            this.Controls.Add(concat);
            
        }

        private void AddNewMap(object sender, EventArgs e)
        {
            Dialogue d = new Dialogue(true);
            d.ShowDialog();
        }

        private void Concatinate(object sender, EventArgs e)
        {
            Dialogue d = new Dialogue(false);
            d.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Concat main = new Concat();



        }
    }
}
