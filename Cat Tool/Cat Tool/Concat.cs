using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cat_Tool
{
    public partial class Concat : Form
    {
        private Button[,] buttons = new Button[5,5];
        public static string currentData { get; set; }
        private List<List<string[,]>> mainContainer;
        private List<string[,]> nullRow;
        private string[,] nullSet;

        
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

            //main loop to set up the changeable buttons
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
                    btn.Text = "<<   >>";
                    btn.Font = textFont;
                    btn.Click += AddNewMap;
                    btn.Tag = rows.ToString() + columns.ToString();

                    buttons[rows, columns] = btn;
                    this.Controls.Add(btn);
                }
            }

            //setup for the button that actually concatinates everything
            Button concat = new Button();
            concat.Font = textFont;
            concat.Height = btnHeight + 20;
            concat.Width = btnWidth + 85;
            concat.Left = btnWidth * 2;
            concat.Top = (btnHeight * 5) + 50;
            concat.Text = "Combine";
            concat.Click += Concatinate;
            this.Controls.Add(concat);

            //setting up the main holder
            mainContainer = new List<List<string[,]>>();
            for (int i = 0; i < 5; i++)
            {
                List<string[,]> secondaryContainers = new List<string[,]>();
                mainContainer.Add(secondaryContainers);
            }

            //setting up example null row and null sets
            nullSet = new string[9, 16];
            nullRow = new List<string[,]>();
            for (int i = 0; i < 5; i++)
            {
                nullRow.Add(nullSet);
            }
        }

        /// <summary>
        /// Pops up a new dialogue, asks for an input, and places it into the visual display & buttons array
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewMap(object sender, EventArgs e)
        {
            //replace the visual button
            Dialogue d = new Dialogue(true);
            d.ShowDialog();
            Button btn = (Button)sender;
            btn.Text = currentData;
            sender = btn;

            //replace the button in the 2D array with the updated version
            char[] placement = btn.Tag.ToString().ToCharArray();
            int.TryParse(placement[0].ToString(), out int char1);
            int.TryParse(placement[1].ToString(), out int char2);
            buttons[char1, char2] = btn;
        }

        /// <summary>
        /// Actually concatinates the maps based on which ones are filled in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Concatinate(object sender, EventArgs e)
        {
            Dialogue d = new Dialogue(false);
            d.ShowDialog();

            //populate each of the containing lists with empty 9x16 arrays 
            foreach (List<string[,]> secondaryList in mainContainer)
            {
                for (int i = 0; i < 5; i++)
                {
                    string[,] temp = new string[9, 16];
                    secondaryList.Add(temp);
                }
            }

            //find the buttons that have been modified and populate them with their source file's info
            for (int rows = 0; rows < 5; rows++)
            {
                for (int columns = 0; columns < 5; columns++)
                {
                    if (buttons[rows, columns].Text != "<<   >>" && buttons[rows, columns].Text != null)
                    {
                        mainContainer[rows][columns] = ReadAndReturn(buttons[rows, columns].Text);
                    }
                }
            }

            //establish which rows don't need to be checked because they're empty
            #region Testing for Empty Rows & Columns
            //testing which rows are empty          ## Booleans tell 'Is this row filled?'
            bool[] rowChecks = new bool[5];

            for (int i = 0; i < 5; i++)
            {
                if (mainContainer[i] == nullRow)
                    rowChecks[i] = false;
                else
                    rowChecks[i] = true;
            }

            //test if any columns are empty         ## Booleans tell 'is this column filled?'
            bool catcher;
            bool[] colChecks = new bool[5];
            for (int i = 0; i < 5; i++)
            {
                //reset for the each level
                catcher = true;

                //check through the same column for every row
                for (int j = 0; j < 5; j++)
                {
                    if (mainContainer[j][i] == nullSet)
                        catcher = false;
                }

                //check whether the row is filled based on catches and files into colChecks
                if (catcher == false)
                    colChecks[i] = false;
                else
                    colChecks[i] = true;
            }

            #endregion

            //concatinate the indexes that have been placed
            for (int rows = 0; rows < 5; rows++)
            {
                for (int columns = 0; columns < 5; columns++)
                {
                    if (colChecks[columns] == true && rowChecks[rows] == true)
                    {
                        
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region Helper Functions

        /// <summary>
        /// Reading in info from text tile and plopping it into a 2d array
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private string[,] ReadAndReturn(string current)
        {
            //fields & initialization logic
            string line;
            int c = 0;
            string tempString = "";
            string[] infoTempHolder;
            int Rows;
            int Columns;

            StreamReader interpreter = new StreamReader(@"..\..\..\..\Party Tower Main\Party Tower Main\Resources\levelExports\" + current + ".txt");

            //Setup for creating the level's 2d array (pulls Row and Column counts, splits the rest into a 2D array)
            line = interpreter.ReadLine();
            infoTempHolder = line.Split(',');
            Rows = int.Parse(infoTempHolder[0]);
            Columns = int.Parse(infoTempHolder[1]);
            string[,] importedTileInfo = new string[Rows, Columns];

            //Reading in the tiles from the file and placing them into the a holder string
            string tileInfoString = interpreter.ReadToEnd();
            infoTempHolder = tileInfoString.Split(',');

            //getting rid of "\r\n"
            for (int i = 0; i <= infoTempHolder.Length - 2; i++)
            {
                tempString = infoTempHolder[i];
                char[] individualTileString = infoTempHolder[i].ToCharArray();

                if (individualTileString.Length > 4)
                {
                    tempString = "";
                    char space = ' ';
                    //make first two char spaces
                    individualTileString[0] = space;

                    individualTileString[1] = space;
                    foreach (var item in individualTileString) //go through every char in temp
                    {
                        if (item != ' ') tempString += item.ToString(); //if char is not a space add it to tile ID temp string
                    }
                }
                infoTempHolder[i] = tempString;
            }

            //actually sets all of the individual tiles into the 2d array
            for (int i = 0; i <= importedTileInfo.GetLength(0) - 1; i++)
            {

                for (int j = 0; j <= importedTileInfo.GetLength(1) - 1; j++)
                {

                    importedTileInfo[i, j] = infoTempHolder[c]; //sets tileID in level array 

                    c++; //increments c because it is seperate array of different dimensions
                }

            }
            interpreter.Close();
            return importedTileInfo;
        }

        #endregion
    }
}
