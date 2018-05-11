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
        int Rows;
        int Columns;
        string currentTile = null;
        bool placing = true;

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

            Rows = 9;
            Columns = 16;
            #endregion

            //  #############################################################################

            #region Main Panels Initialization
            //Left panel initialization (Tools/Selection)
            Panel leftP = new Panel();
            leftP.Height = this.Height;
            leftP.Width = this.Width / 5;
            leftP.BackColor = Color.Red;

            //Right panel initialization (Workspace)
            Panel rightP = new Panel();
            rightP.Height = Height;
            rightP.Width = (this.Width / 5) * 4;
            rightP.Left = this.Width / 5;
            rightP.BackColor = Color.White;
            rightP.AutoScroll = true;

            this.Controls.Add(leftP);
            this.Controls.Add(rightP);
            #endregion

            //  #############################################################################

            #region Left Panel Specifics

            //****************************************************
            //****************** Panel Creation ******************
            //****************************************************

            Panel typeSelector = new Panel();
            typeSelector.Height = 7 * (leftP.Height / 10);
            typeSelector.Width = leftP.Width;
            typeSelector.BackColor = Color.Gainsboro;
            typeSelector.AutoScroll = true;

            Panel btnSelector = new Panel();
            btnSelector.Top = 7 * (leftP.Height / 10);
            btnSelector.Height = 3 * (leftP.Height / 10);
            btnSelector.Width = leftP.Width;
            btnSelector.BackColor = Color.Gainsboro;


            //****************************************************
            //************** Button Panel Buttons ****************
            //****************************************************

            Button btnPlace = new Button();
            btnPlace.Width = leftP.Width / 2;
            btnPlace.Height = 2 * (btnSelector.Height / 7);
            btnPlace.BackColor = Color.Green;
            btnPlace.ForeColor = Color.White;
            btnPlace.FlatStyle = FlatStyle.Flat;
            btnPlace.FlatAppearance.BorderColor = Color.Green;
            btnPlace.FlatAppearance.MouseDownBackColor = Color.DarkGreen;
            btnPlace.FlatAppearance.BorderSize = 2;
            btnPlace.Text = "Place";
            btnPlace.Click += SetPlacing;

            Button btnRemove = new Button();
            btnRemove.Width = leftP.Width / 2;
            btnRemove.Height = 2 * (btnSelector.Height / 7);
            btnRemove.Left = btnPlace.Width;
            btnRemove.BackColor = Color.Red;
            btnRemove.ForeColor = Color.White;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderColor = Color.Red;
            btnRemove.FlatAppearance.MouseDownBackColor = Color.DarkRed;
            btnRemove.Text = "Remove";
            btnRemove.Click += SetPlacing;

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
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.FlatAppearance.BorderColor = Color.LemonChiffon;
            btnImport.FlatAppearance.MouseDownBackColor = Color.Khaki;
            btnImport.Text = "Import";
            btnImport.Click += Import;

            Button btnExport = new Button();
            btnExport.Height = 40 * (btnSelector.Height / 100);
            btnExport.Width = btnSelector.Width / 2;
            btnExport.Top = 3 * (btnSelector.Height / 7);
            btnExport.Left = btnSelector.Width / 2;
            btnExport.BackColor = Color.BurlyWood;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.FlatAppearance.BorderColor = Color.BurlyWood;
            btnExport.FlatAppearance.MouseDownBackColor = Color.Khaki;
            btnExport.Text = "Export";
            btnExport.Click += Export;


            //****************************************************
            //**************** Adding to Controls ****************
            //****************************************************

            //Adding everything to controls
            leftP.Controls.Add(typeSelector);
            leftP.Controls.Add(btnSelector);

            btnSelector.Controls.Add(btnPlace);
            btnSelector.Controls.Add(btnRemove);
            btnSelector.Controls.Add(fileName);
            btnSelector.Controls.Add(btnImport);
            btnSelector.Controls.Add(btnExport);

            //8 Characters per line
            AddNewTile("dirt", "Dirt", typeSelector);
            AddNewTile("brick", "Brick", typeSelector);
            AddNewTile("grass", "Grass", typeSelector);
            AddNewTile("moss", "Moss", typeSelector);                //            <=== Add Tilesets here
            //AddNewTile("path",   "display name", container); 
            #endregion

            //  #############################################################################

            #region Right Panel Specifics

            TabletChange(Rows, Columns, rightP);

            #endregion

            //  #############################################################################

        }
        //  #############################################################################

        #region Main Functions

        /// <summary>
        /// Adds another row to the tablet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRow(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Adds another column to the tablet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddColumn(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Takes the information of the current tablet and prints it to an external text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Export(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Takes info from an external text file and fills the current tablet with it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sets the placing bool based on which button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetPlacing(object sender, EventArgs e)
        {
            Button temp = (Button)sender;
            if (temp.Text.ToLower() == "place")
            {
                placing = true;
            }
            else if (temp.Text.ToLower() == "remove")
            {
                placing = false;
            }
        }

        /// <summary>
        /// Updates tablet based on current selected tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabletBtnClicked(object sender, EventArgs e)
        {
            ImageBox temp = (ImageBox)sender;
            if (placing == true)
            {
                temp.Image = ImageSelect(currentTile + "Tileset");
                temp.Tag = currentTile;
            }
            else
            {
                temp.Image = ImageSelect("noTexture");
                temp.Tag = null;
            }
        }

        /// <summary>
        /// Sets up tablet or changes the tablet interface when a row or column count is changed
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="parent"></param>
        private void TabletChange(int row, int col, Panel parent)
        {

            //****************************************************
            //**************** Main Tablet Setup *****************
            //****************************************************

            for (int columns = 0; columns < Columns; columns++)
            {
                for (int rows = 0; rows < Rows; rows++)
                {
                    ImageBox temp = new ImageBox();
                    temp.Width = parent.Width / Columns;
                    temp.Width = (parent.Width - temp.Width) / Columns;
                    temp.Height = temp.Width;
                    temp.Image = ImageSelect("noTexture");
                    temp.Top = rows * temp.Height;
                    temp.Left = columns * temp.Width;
                    temp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    temp.SizeMode = PictureBoxSizeMode.Zoom;
                    temp.Click += TabletBtnClicked;

                    parent.Controls.Add(temp);
                }
            }

            //****************************************************
            //***************** Tablet Extenders *****************
            //****************************************************

            int baseHeightWidth = parent.Controls[0].Height;

            Button btnAddRow = new Button();
            btnAddRow.Height = baseHeightWidth / 2;
            btnAddRow.Width = baseHeightWidth * Columns;
            btnAddRow.Text = "Add Row";
            btnAddRow.BackColor = Color.Gainsboro;
            btnAddRow.Top = baseHeightWidth * (Rows);
            btnAddRow.Left = 0;

            VertButton btnAddColumn = new VertButton();
            btnAddColumn.Width = baseHeightWidth / 2;
            btnAddColumn.Height = baseHeightWidth * Rows;
            btnAddColumn.Text = "Add Column";
            btnAddColumn.BackColor = Color.Gainsboro;
            btnAddColumn.Top = 0;
            btnAddColumn.Left = baseHeightWidth * Columns;

            parent.Controls.Add(btnAddColumn);
            parent.Controls.Add(btnAddRow);
        }

        /// <summary>
        /// Updates the current tile based on which tileset was selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCurrentTile(object sender, EventArgs e)
        {
            if (sender is ImageBox)
            {
                ImageBox temp = (ImageBox)sender;
                currentTile = temp.Tag.ToString();
                //eventually put an indicator behind the selected tile
            }
            else if (sender is Label)
            {
                Label temp = (Label)sender;
                currentTile = temp.Tag.ToString();
                //same here
            }
            else
            {
                throw new Exception("Updater is not of ImageBox or Label type");
            }
        }

        #endregion

        //  #############################################################################

        #region Helper Functions

        /// <summary>
        /// Simplified way to add a new tile to tile list and to parent panel
        /// </summary>
        /// <param name="tilePath"></param>
        private void AddNewTile(string tilePath, string tileSetName, Panel parent)
        {
            ImageBox tempImageBox = new ImageBox();
            tempImageBox.Height = 64;
            tempImageBox.Width = 64;
            tempImageBox.Top = 10 + (74 * Selectables.Count);
            tempImageBox.Left = 10;
            tempImageBox.Image = ImageSelect(tilePath);
            tempImageBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            tempImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            tempImageBox.Click += UpdateCurrentTile;
            tempImageBox.Tag = tileSetName;

            Label tempLabel = new Label();
            tempLabel.Left = 30 + tempImageBox.Width;
            tempLabel.Top = 30 + (74 * Selectables.Count);
            tempLabel.Height = tempImageBox.Height - 10;
            tempLabel.Width = parent.Width - tempImageBox.Width - 50;
            tempLabel.Text = tileSetName;
            tempLabel.ForeColor = Color.White;
            tempLabel.Font = new Font("Coder's Crux", 35, FontStyle.Regular);
            tempLabel.Click += UpdateCurrentTile;
            tempLabel.Tag = tempLabel.Text;

            parent.Controls.Add(tempImageBox);
            parent.Controls.Add(tempLabel);
            Selectables.Add(tempImageBox);

        }

        /// <summary>
        /// Selects and image from the resources folder based on the input string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Image ImageSelect(string s)
        {
            Image temp = null;
            try
            {
                temp = Image.FromFile(@"..\..\Resources\" + s + ".png");
            }
            catch (Exception e)
            {
                MessageBox.Show("Selected tile does not exist." + Environment.NewLine + e.Message);
            }
            return temp;
        }

        #endregion
    }
}
