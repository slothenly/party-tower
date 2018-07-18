using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Party_Tower_Main
{
    class Table : GameObject
    {
        private bool cakePlacedOnTable;

        public bool CakePlacedOnTable
        {
            get { return cakePlacedOnTable; }
            set { cakePlacedOnTable = value; }
        }

        public Table(Rectangle hitbox, Texture2D defaultSprite)
        {
            this.hitbox = hitbox;
            this.defaultSprite = defaultSprite;
            cakePlacedOnTable = false;
        }
    
        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(defaultSprite, hitbox, Color.White);
        }
    }
}
