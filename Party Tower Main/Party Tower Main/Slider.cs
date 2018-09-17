using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class Slider : Button
    {
        //fields
        private float length;
        private float returnedValue;
        private Button sliderButton;
        private Texture2D displayedIcon;

        //Properties
        //Highlighted only important for the actual button of the slider
        public new bool IsHighlighted
        {
            get { return sliderButton.IsHighlighted; }
        }
        public override float Length
        {
            get { return length; }
            set { length = value; }
        }
        public override float ReturnedValue
        {
            get { return returnedValue; }
            set { returnedValue = value; }
        }
        public override Button SliderButton
        {
            get { return sliderButton; }
        }
        public override Texture2D DisplayedIcon
        {
            get { return displayedIcon; }
        }

        //Constructor
        public Slider(Texture2D normalTexture, Texture2D highlightedTexture, float length, Texture2D displayedIcon)
            : base(normalTexture, normalTexture)
        {
            sliderButton = new Button(highlightedTexture, highlightedTexture);
            this.length = length;
            this.displayedIcon = displayedIcon;

        }

        /// <summary>
        /// Rounds the position of the sliderButton to the nearest valid position, also returns the returnedValue value 
        /// </summary>
        /// <returns></returns>
        public int RoundToNearestValidPosition()
        {
            int smallestDifference = int.MaxValue;
            int currentPoint = 0;
            int currentDifference = 0;

            //beyond the upper bound
            if (sliderButton.X > Area.X + Area.Width)
            {
                sliderButton.X = Area.X + Area.Width - (sliderButton.Area.Width / 2);
                return (int)length;
            }
            //below the lower bound
            else if (sliderButton.X < Area.X)
            {
                sliderButton.X = Area.X - (sliderButton.Area.Width / 2);
                return 0;
            }
            //legitimate rounding
            else
            {
                int currentIndex = 0; //used for setting the returnedValue to the current value
                //loop through each possible point, set the actual to the one with the least difference
                for (int i = 0; i < length; i++)
                {
                    currentPoint = Area.X + (int)((Area.Width / length) * i);
                    currentDifference = Math.Abs((currentPoint + sliderButton.X) - (sliderButton.X * 2));
                    if (currentDifference < smallestDifference) //find the smallest difference
                    {
                        //update appropriate values
                        currentIndex = i;
                        smallestDifference = currentDifference;
                    }
                }
                sliderButton.X = Area.X + (int)((Area.Width / length) * currentIndex) - (sliderButton.Area.Width / 2);
                return currentIndex;
            }
        }

        /// <summary>
        /// Position the sliderButton relative to the actual slider
        /// </summary>
        public override void SetSliderButtonArea()
        {
            Rectangle solution = new Rectangle(0, 0, 0, 0);

            //adjust these two values for dimensions
            solution.Width = 20;
            solution.Height = 50;


            solution.X = Area.X + (int)(returnedValue * (Area.Width / length)) - (solution.Width / 2);
            solution.Y = Area.Y - (solution.Height / 3);

            sliderButton.Area = solution;
        }

        /// <summary>
        /// Alter the slider based on keyboard/gamepad input
        /// </summary>
        /// <param name="pressingRight"></param>
        public override void CheckAndAlterSlider(bool pressingRight)
        {
            if (pressingRight && returnedValue < length)
            {
                returnedValue++;
                sliderButton.X = Area.X + (int)((Area.Width / length) * returnedValue) - (sliderButton.Area.Width / 2);
            }
            else if (!pressingRight && returnedValue > 0)
            {
                returnedValue--;
                sliderButton.X = Area.X + (int)((Area.Width / length) * returnedValue) - (sliderButton.Area.Width / 2);
            }
        }


        /// <summary>
        /// alter slider based on mouse input
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="previousMs"></param>
        public override void CheckAndAlterSlider(MouseState ms, MouseState previousMs)
        {
            if (ms.LeftButton == ButtonState.Pressed && returnedValue <= length && returnedValue >= 0)
            {
                //mouse going right
                if (ms.X > previousMs.X)
                {

                    //First position the sliderButton according to the mouse position
                    sliderButton.X = ms.X;


                    //then alter the buttonLocationOnSlider value
                    //Then adjust if the mouse is beyond the limits
                    returnedValue = RoundToNearestValidPosition();


                }
                //mouse going left
                if (ms.X < previousMs.X)
                {

                    //First position the sliderButton according to the mouse position
                    sliderButton.X = ms.X;


                    //then alter the buttonLocationOnSlider value
                    //Then adjust if the mouse is beyond the limits
                    returnedValue = RoundToNearestValidPosition();
                }
            }      
        }
    }
}
