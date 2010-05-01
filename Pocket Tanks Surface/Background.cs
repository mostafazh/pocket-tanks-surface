using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pocket_Tanks_Surface
{
    public class Background
    {
        // Background texture
        private Texture2D Texture;
        private int Width;
        private int Height;

        public Background(Texture2D Texture, int Width, int Height)
        {
            this.Texture = Texture;
            this.Width = Width;
            this.Height = Height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw background texture in a separate pass.
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, new Rectangle(0, 0, Width,Height),Color.LightGray);
            spriteBatch.End();
        }

        public void Unload()
        {
            Texture.Dispose();
        }
    }
}
