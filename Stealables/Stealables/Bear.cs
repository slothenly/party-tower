using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LumberjackFantasy
{

    /// <summary>
    /// Enum that provides the Looking State of the Bear
    /// </summary>
    enum BearState
    {
        stationary,
        looking,
        following
    }

    /// <summary>
    /// Enum that provides the Direction of the Bear
    /// </summary>
    enum BearDirection
    {
        upleft = 5,
        upright = 6,
        downleft = 4,
        downright = 7,
        up = 0,
        down = 1,
        left = 2,
        right = 3,
    }
    class Bear : LivingObject
    {
        // "Normal" Fields -------------------------------------------------------------------

        private int scoreValue;                 // The amount of points that a bear is worth.

        // Attack and Attacked Based Fields --------------------------------------------------

        private int fieldOfAttackStandard;      // Determines how far the Field of Attack to trigger a bear swipping should be from the object   
        private Rectangle fieldOfAttack;        // Rectangle that determines if the bear should trigger an attack animation
        private bool attackStuff;       // Determines if the player can attack something. 
        private int attackAnimationF;   // Keeps track of the players current attacking animation frame;
        private Rectangle attackBox;    // Holds the Rectangle of the Bears Attack Target

        // Movement Based Fields --------------------------------------------------------------

        private BearState bearState;            // Enum that checks the state of the bear [Looking / Following / Dead]
        private BearDirection bearDirection;    // Enum that checks the direction of the bear 

        private int visionStandard;             // the number that determines how far out the BearVision should be from the actual objectTexture.
        private Rectangle bearVision;           // The Bears field of vision that helps the bears determine what kind of state they are in.

        private int whenToMoveMax;              // Determines how long a bear should max wait before making a random movement in a direction.
        private int whenToMoveMin;              // Determines how long a bear should min wait before making a random movement in a direction.
        private double whenToMoveCounter;       // Counter that updates with GameTime that tells the bear when it should move.
        private double whenToMoveLimiter;       // Randomly Generated time between max and min that tells how long a bear waits before moving.

        private int timeOfMovementMax;          // Determines how long a bear should max move in a random movement in a direction.
        private int timeOfMovementMin;          // Determines how long a bear should min move in a random movement in a direction.
        private double timeOfMovementCounter;   // Counter that updates with GameTime that tells the bear how long it should move.
        private double timeOfMovementLimiter;   // Randomly Generated time between max and min that tells how long a bear can move for.

        private bool wasAttacked;

        // Properties ------------------------------------------------------------------

        /// <summary>
        /// Attack HitBox
        /// </summary>
        public Rectangle AttackBox { get { return attackBox; } set { attackBox = value; } }
        /// <summary>
        /// Field of Attack Standard
        /// </summary>
        public int FoAStandard
        {
            get { return fieldOfAttackStandard; }
        }

        public int VisionStandard
        {
            get { return visionStandard; }
            set { visionStandard = value; }
        }


        public BearState BearState
        {
            get { return bearState; }
            set { bearState = value; }
        }

        public Rectangle BearVision
        {
            get { return bearVision; }
            set { bearVision = value; }
        }

        public Rectangle FieldOfAttack
        {
            get { return fieldOfAttack; }
            set { fieldOfAttack = value; }
        }

        public BearDirection BearDirection
        {
            get { return bearDirection; }
            set { bearDirection = value; }
        }

        public double WhenToMoveCounter
        {
            get { return whenToMoveCounter; }
            set { whenToMoveCounter = value; }
        }

        public double WhenToMoveLimiter
        {
            get { return whenToMoveLimiter; }

        }

        public double TimeOfMovementCounter
        {
            get { return timeOfMovementCounter; }
            set { timeOfMovementCounter = value; }
        }

        public double TimeOfMovementLimiter
        {
            get { return timeOfMovementLimiter; }
        }

        public int ScoreValue
        {
            get { return scoreValue; }
            set { scoreValue = value; }
        }

        //Checks if bear was attacked during this current players attack animation
        public bool WasAttacked
        {
            get { return wasAttacked; }
            set { wasAttacked = value; }
        }
        // Checks to see if Bear is currently Attacking
        public bool IsAttacking
        {
            get { return attackStuff; }
            set { attackStuff = value; }
        }

        // Increments Bears Attack animation
        public int AttackAnimationF
        {
            get { return attackAnimationF; }
            set { attackAnimationF = value; }
        }





        // Constructor ------------------------------------------------------------------------------ 
        // ! Constructor will pass in a random generator but is not supposed to save it ! 

        public Bear(int x, int y, int width, int height, Texture2D objectTexture, int maxH, int maxS,
            int visionStandard, int fieldOfAttackStandard, int scoreValue, Random rng)
            : base(x, y, width, height, objectTexture, maxH, maxS)
        {
            this.scoreValue = scoreValue * -1;                  // Sets the Score of the Bear to Negative since its in a looking state

            this.visionStandard = visionStandard;
            bearVision = new Rectangle(x - visionStandard, y - visionStandard, width + (visionStandard * 2), height + (visionStandard * 2));

            this.fieldOfAttackStandard = fieldOfAttackStandard;
            fieldOfAttack = new Rectangle(x - fieldOfAttackStandard, y - fieldOfAttackStandard,
                width + (fieldOfAttackStandard * 2), height + (fieldOfAttackStandard * 2));

            whenToMoveCounter = 0;
            whenToMoveMax = 5 + 1;                                     // Bears Wait a max of 15 Seconds before making a movement;
            whenToMoveMin = 1;                                          // Bears Wait a min of 5 Seconds before making a movement;
            whenToMoveLimiter = rng.Next(whenToMoveMin, whenToMoveMax); // Random int between 5-15 Seconds of when bear first can Move


            timeOfMovementCounter = 0;
            timeOfMovementMax = 3;                                                     // Bear can move a max 5 seconds in a direction;
            timeOfMovementMin = 1;                                                     // Bear can move a min 1 seconds in a direction;
            timeOfMovementLimiter = rng.Next(timeOfMovementMin, timeOfMovementMax);    // Can Move in a random direction for 1-5 seconds
            attackStuff = false;
            attackAnimationF = 0;
            wasAttacked = false;


        }

        public Bear(Bear b)
            : base(b.PosX, b.PosY, b.width, b.height, b.objectTexture, b.MaxHealth, b.MaxSpeed)
        {
            whenToMoveCounter = b.whenToMoveCounter;
            whenToMoveMax = b.whenToMoveMax;
            whenToMoveMin = b.whenToMoveMin;
            whenToMoveLimiter = b.whenToMoveLimiter;

            timeOfMovementCounter = b.timeOfMovementCounter;
            timeOfMovementLimiter = b.timeOfMovementLimiter;
            timeOfMovementMax = b.timeOfMovementMax;
            timeOfMovementMin = b.timeOfMovementMin;

            scoreValue = b.scoreValue;

            visionStandard = b.visionStandard;
            bearVision = new Rectangle(b.PosX - visionStandard, b.PosY - visionStandard, b.width + (visionStandard * 2), b.height + (visionStandard * 2));

            fieldOfAttackStandard = b.fieldOfAttackStandard;
            fieldOfAttack = new Rectangle(b.PosX - fieldOfAttackStandard, b.PosY - fieldOfAttackStandard,
                width + (fieldOfAttackStandard * 2), height + (fieldOfAttackStandard * 2));

            bearState = b.bearState;            // Enum that checks the state of the bear [Looking / Following / Dead]
            bearDirection = b.bearDirection;    // Enum that checks the direction of the bear 
            wasAttacked = b.wasAttacked;
        }


        /// <summary>
        /// Resets the Timers for Bear
        /// </summary>
        public void ResetBearTimers(Random rng)
        {
            whenToMoveLimiter = rng.Next(whenToMoveMin, whenToMoveMax);             // Comes up with a new time as to when to move
            whenToMoveCounter = 0;                                                  // Resets the movementTimer back to 0
            timeOfMovementLimiter = rng.Next(timeOfMovementMin, timeOfMovementMax); // Comes up with a random total time of movement.
            timeOfMovementCounter = 0;                                              // Resets the timeOfMovement counter to 0
        }

        public override void Draw(SpriteBatch sb, Vector2 camera)
        {
            //Base Draw Method for Base Build
            if (onScreen)
            {
                BearDirection animationDirection;
                if ((int)bearDirection > 3)
                {
                    animationDirection = (BearDirection)((int)bearDirection % 3);
                }
                else
                {
                    animationDirection = BearDirection;
                }
                sb.Draw(objectTexture,
                    new Vector2(objectCollisionBox.X - Convert.ToInt32(camera.X), objectCollisionBox.Y - Convert.ToInt32(camera.Y)),
                    new Rectangle(animationMoveFrame * objectCollisionBox.Width, (int)animationDirection * objectCollisionBox.Height, objectCollisionBox.Width, objectCollisionBox.Height), Color.White);

            }
        }
    }
}
