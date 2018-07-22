using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Party_Tower_Main
{
    class RebindingButton : Button
    {
        //Fields
        private string key;
        private Player player;
        private Keys[] keys;
        private string visibleText;
        private int timerNumber;
        private bool tryingToRebind = false;

        //Properties
        public string Key
        {
            get { return key; }
        }
        public override string VisibleText
        {
            get { return visibleText; }
        }
        public override bool TryingToRebind
        {
            get { return tryingToRebind; }
            set { tryingToRebind = value; }
        }

        //Constructor
        public RebindingButton(Texture2D normalTexture, Texture2D highlightedTexture, string key, Player player) 
            : base(normalTexture, highlightedTexture)
        {
            this.key = key;
            this.player = player;
            visibleText = player.BindableKb[key].ToString();

            timerNumber = 0;
        }

        /// <summary>
        /// takes the inputed key, and once the user has pressed a new key, sets the key to that. The bool is used for the lockedSelection bool in Game1
        /// </summary>
        public override bool SetNewKey()
        {
            timerNumber++; //increment timer

            keys = Keyboard.GetState().GetPressedKeys(); //get the keys being pressed

            if (keys.Length > 0 && timerNumber > 5 && keys[0] != Keys.Enter) //allow changing if it's been 5 frames since selecting a key to rebind
            {                                                                // and the key isn't enter (because changing that can very easily break menu navigation)
                timerNumber = 0;
                player.BindableKb[key] = keys[0]; //set the new key
                visibleText = player.BindableKb[key].ToString(); //update the visual string
                tryingToRebind = false; //since we rebound a key, no need to keep trying to rebind
                return false;
            }
            return true;

        }
    }
}
