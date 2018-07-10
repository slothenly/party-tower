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
    public partial class Dialogue : Form
    {

        TextBox whichMap;
        Button accept;
        Label tBox;

        /// <summary>
        /// Setup for adding new dialogue boxes
        /// </summary>
        /// <param name="whichPopup">t for new map, f for concat</param>
        public Dialogue(bool whichPopup)
        {
            this.Height = 400;
            this.Width = 300;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.Manual;

            accept = new Button();
            accept.Width = Width / 2;
            accept.Height = Height / 3;
            accept.Text = "Accept";
            accept.Top = Height / 2;
            accept.Left = Width / 4;
            accept.Click += Accept;

            whichMap = new TextBox();
            whichMap.Width = Width / 2;
            whichMap.Height = Height / 3;
            whichMap.Left = Width / 4;
            whichMap.Top = Height / 3;

            tBox = new Label();
            tBox.Top = whichMap.Top - 50;
            tBox.Left = (Width / 4) - 20;
            tBox.Width = (Width / 3) * 2;

            //setup for adding a map to a certain spot
            if (whichPopup == true)
            {
                tBox.Text = "What map should be added?";
                this.Text = "Map Adder";
            }

            //setup for concatination
            else
            {
                tBox.Text = "Please type map name";
                tBox.Left += 15;
                this.Text = "Concatinater";
            }

            this.Controls.Add(accept);
            this.Controls.Add(whichMap);
            this.Controls.Add(tBox);

            InitializeComponent();
        }

        private void Accept(object sender, EventArgs e)
        {
            Concat.currentData = whichMap.Text;
            Close();
        }

        private void Dialogue_Load(object sender, EventArgs e)
        {

        }
    }
}
