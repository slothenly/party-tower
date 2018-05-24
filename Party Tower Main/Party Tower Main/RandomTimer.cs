using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Party_Tower_Main
{
    /// <summary>
    /// Timer with a randomized standard upon each reset
    /// </summary>
    class RandomTimer : Timer
    {
        #region Fields

        /// <summary>
        /// Minimum the standard can be
        /// </summary>
        private int minTime;
        /// <summary>
        /// Maximum the standard can be
        /// </summary>
        private int maxTime;
        /// <summary>
        /// Random Generator used for randomizing the standard
        /// </summary>
        private Random rng;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a timer who's standard is generated between OR equal to the min and max allowed time
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="minTime"></param>
        /// <param name="MaxTime"></param>
        public RandomTimer (Random rng, int minTime, int maxTime) 
            : base (rng.Next(minTime, (maxTime + 1)))
        {
            this.minTime = minTime;
            this.maxTime = maxTime;
            this.rng = rng;
        }

        /// <summary>
        /// Creates a timer who's first standard is set, and future standards can be generated based on a min and max allowed time
        /// </summary>
        /// <param name="minTime"></param>
        /// <param name="maxTime"></param>
        /// <param name="standard"></param>
        public RandomTimer(int standard, Random rng, int minTime, int maxTime)
            : base(standard)
        {
            this.minTime = minTime;
            this.maxTime = maxTime;
            this.rng = rng;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Sets the timer back to 0 and standard to a value between or equal to the min and max
        /// </summary>
        public override void ResetTimer()
        {
            standard = rng.Next(minTime, maxTime);      
            timer = 0;                                 
        }

        /// <summary>
        /// Update's the Timer
        /// Returns true if the timer past the limiter.
        /// </summary>
        /// <param name="gameTime"></param>
        public override bool UpdateTimer(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= standard)
            {
                ResetTimer();
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}
