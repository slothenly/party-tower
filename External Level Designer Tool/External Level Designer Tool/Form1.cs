using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace External_Level_Designer_Tool
{
    public partial class Form1 : Form
    {
        List<ImageBox> Selectables = new List<ImageBox>();

        public Form1()
        {
            #region Main Form Initilization
            InitializeComponent();
            this.Height = 800;
            this.Width = 1400;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(75, 20);
            #endregion

//  #############################################################################

            #region Main Panels initialization
            //Left panel initialization (Tools/Selection)
            Panel leftP = new Panel();
            leftP.Height = this.Height;
            leftP.Width = this.Width / 5;
            leftP.BackColor = Color.Red;

            //Right panel initialization (Workspace)
            Panel rightP = new Panel();
            rightP.Height = Height;
            rightP.Width = (this.Width  / 5) * 4;
            rightP.Left = this.Width / 5;
            rightP.BackColor = Color.White;

            this.Controls.Add(leftP);
            this.Controls.Add(rightP);
            #endregion

//  #############################################################################

            #region Left Panel Specifics

            //Selector Panel and Button Panel
            Panel typeSelector = new Panel();
            typeSelector.Height = 7 * (leftP.Height / 10);
            typeSelector.Width = leftP.Width;
            typeSelector.BackColor = Color.Bisque;
            typeSelector.AutoScroll = true;

            Panel btnSelector = new Panel();
            btnSelector.Top = 7 * (leftP.Height / 10);
            btnSelector.Height = 3 * (leftP.Height / 10);
            btnSelector.Width = leftP.Width;
            btnSelector.BackColor = Color.LightSeaGreen;

            //****************************************************
            //****************************************************

            //Button Panel Buttons
            Button btnPlace = new Button();
            btnPlace.Width = leftP.Width / 2;
            btnPlace.Height = 2 * (btnSelector.Height / 7);
            btnPlace.BackColor = Color.Green;
            btnPlace.Text = "Place";

            Button btnRemove = new Button();
            btnRemove.Width = leftP.Width / 2;
            btnRemove.Height = 2 * (btnSelector.Height / 7);
            btnRemove.Left = btnPlace.Width;
            btnRemove.BackColor = Color.Red;
            btnRemove.Text = "Remove";

            TextBox fileName = new TextBox();
            fileName.Multiline = true;
            fileName.Height = btnSelector.Height / 7;   //technically 2/7 because multiLine is turned on
            fileName.Width = btnSelector.Width;
            fileName.Top = 2 * (btnSelector.Height / 7);
            fileName.Text = @"<exported file name>";

            Button btnImport = new Button();
            btnImport.Height = 40 * (btnSelector.Height / 100);
            btnImport.Width = btnSelector.Width / 2;
            btnImport.Top = 3 * (btnSelector.Height / 7);
            btnImport.BackColor = Color.LemonChiffon;
            btnImport.Text = "Import";

            Button btnExport = new Button();
            btnExport.Height = 40 * (btnSelector.Height / 100);
            btnExport.Width = btnSelector.Width / 2;
            btnExport.Top = 3 * (btnSelector.Height / 7);
            btnExport.Left = btnSelector.Width / 2;
            btnExport.BackColor = Color.BurlyWood;
            btnExport.Text = "Export";

            //****************************************************
            //****************************************************

            //Adding everything to controls
            leftP.Controls.Add(typeSelector);
            leftP.Controls.Add(btnSelector);

            btnSelector.Controls.Add(btnPlace);
            btnSelector.Controls.Add(btnRemove);
            btnSelector.Controls.Add(fileName);
            btnSelector.Controls.Add(btnImport);
            btnSelector.Controls.Add(btnExport);
            #endregion

//  #############################################################################

            #region Right Panel Specifics

            #endregion

//  #############################################################################

        }

//  #############################################################################

        #region Helper Functions

        private void AddNewTile(string tilePath)
        {
            ImageBox temp = new ImageBox();
            temp.Height = 30;
            temp.Width = 30;
            temp.Top = 50 * Selectables.Count;
            
        }

        #endregion
    }
}
