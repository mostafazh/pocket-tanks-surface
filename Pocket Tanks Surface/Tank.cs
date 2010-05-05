using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Surface.Core;


namespace Pocket_Tanks_Surface
{
    public delegate void moveEndedEvent();
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Tank : DrawableGameComponent
    {
        public event moveEndedEvent moveEnded; 
        public int Width = 50;
        public int Height = 50;
        public Texture2D body;
        public Texture2D gun;
        public Player owner;
        private bool flag = true;
        public Vector2 position;
        private Vector2 _newPosition;
        private Vector2 newPosition { get { Vector2 n = new Vector2(); n.Y = 609;n.X = _newPosition.X ;return n; } set { _newPosition = value; } }
        public float rotation;
        private Game game;
        public Rectangle BoundryBox { get { return new Rectangle((int)position.X, (int)position.Y, Width, Height); } }

        private Vector2 cannonOrigin = new Vector2(25, 10);

        public Tank(Game game,Player owner):this(game,owner,0,new Vector2())
        {
            
        }

        public Tank(Game game, Player owner,float rotation, Vector2 postion):base(game)
        {
            // TODO: Construct any child components here
            this.owner = owner;
            this.rotation = rotation;
            this.position = postion;
            this.newPosition = postion;
            this.game = game;
        }

        protected override void LoadContent()
        {
            gun = game.Content.Load<Texture2D>("Tank_Gun");
            body = game.Content.Load<Texture2D>("Tank_Body");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            gun.Dispose();
            body.Dispose();
            base.UnloadContent();
        }

        public bool CheckCollision(Rectangle rect)
        {
            return rect.Intersects(BoundryBox);
        }

        public void Draw()
        {
            // Draw background texture in a separate pass.
            SpriteBatch spriteBatch = new SpriteBatch(game.GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(body, position, owner.color);
            spriteBatch.Draw(gun, new Vector2((int)position.X + 25, (int)position.Y - 10), null, owner.color, rotation * 0.0174532925f, cannonOrigin, 1, SpriteEffects.None, 1);
            spriteBatch.End();
        }

        public void Move(Vector2 newPosition)
        {
            newPosition.Y = 609;
            this.newPosition = newPosition;
        }

        public override void Update(GameTime gameTime)
        {
            if (!EqualePositions(newPosition, position))
            {
                flag = true;
                Goto(newPosition);
            }
            else if (flag)
            {
                    flag = false;
                    if(moveEnded != null)
                        moveEnded();
            }
            base.Update(gameTime);
        }

        private bool EqualePositions(Vector2 newPosition, Vector2 position)
        {
            if (AlmostEqaule((int)newPosition.X,(int)position.X) && AlmostEqaule((int)newPosition.Y,(int)position.Y))
            {
                newPosition = position;
                return true;
            }
            return false;
        }

        private bool AlmostEqaule(int p1, int p2)
        {
            return p1 == p2 || p1+1 == p2 || p1+2 == p2;
        }

        private void Goto(Vector2 newPosition)
        {
            //GO UP
            if (newPosition.Y < position.Y)
                position.Y -= 1;

            //GO DOWN
            if (newPosition.Y > position.Y)
                position.Y += 1;

            //GO RIGHT
            if (newPosition.X > position.X)
                position.X += 1;

            //GO LEFT
            if (newPosition.X < position.X)
                position.X -= 1;
        }
    }
}