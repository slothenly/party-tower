using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Alpha_Fade_Test
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
        public Fader(float timeframe_input, float original)
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
    }
}
