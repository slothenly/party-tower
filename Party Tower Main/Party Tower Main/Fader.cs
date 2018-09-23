using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Party_Tower_Main
{
    class Fader
    {
        //fields
        float alpha_percent;
        float opacity_value;
        float timeframe;
        float change_per_frame;
        public bool finished;

        //methods
        /// <summary>
        //  updates every frame changing the opacity a percentage per frame
        /// </summary>
        /// <param name="timeframe">amount of frames</param>
        public Fader(float timeframe_input)
        {
            this.timeframe = timeframe_input;
            change_per_frame = (1 / timeframe);
            opacity_value = 0.0f;

            finished = false;
        }

        public float change_alpha_percent_positive(float current)
        {
            if (opacity_value < 1)
            {
                opacity_value += change_per_frame;
            }
            else if (opacity_value > 1)
            {
                opacity_value = 1;
                finished = true;
            }

            return opacity_value;
        }

        public float change_alpha_percent_negative(float current)
        {
            if (opacity_value > 0)
            {
                opacity_value -= change_per_frame;
            }
            else if (opacity_value < 0)
            {
                opacity_value = 0;
                finished = false;
            }

            return opacity_value;
        }

        public void Draw(SpriteBatch sb, Texture2D fading_screen_texture, Rectangle fading_screen_rect, float fading_screen_alpha_percent)
        {
            // TODO: Add your drawing code here
            sb.Begin();
            sb.Draw(fading_screen_texture, fading_screen_rect, Color.White * fading_screen_alpha_percent);
            sb.End();
        }
    }
}
