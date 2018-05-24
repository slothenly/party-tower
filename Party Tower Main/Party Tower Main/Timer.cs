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
    /// Just a useful class made incase we have any in-game events that occur due to a timer.
    /// </summary>
    class Timer
    {
        #region Fields
        /// <summary>
        /// Keeps track of the current timer
        /// </summary>
        protected double timer;   
        /// <summary>
        /// Standard that the timer must count to
        /// </summary>
        protected double standard;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a regular timer that increments UP to it's standard
        /// </summary>
        /// <param name="standard"></param>
        public Timer(int standard)
        {
            timer = 0;                  // Timer starts at 0.0
            this.standard = standard;   // Uses sent in standard
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Sets the timer back to 0
        /// </summary>
        public virtual void ResetTimer()
        {
            timer = 0;                                  
        }

        /// <summary>
        /// Update's the Timer
        /// Returns true if the timer past the limiter.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual bool UpdateTimer(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if(timer > standard)
            {
                ResetTimer();
                return true;
            }

            return false;
        }

        #endregion
    }
}