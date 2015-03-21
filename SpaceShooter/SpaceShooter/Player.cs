using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace SpaceShooter
{
    class Player
    {
        public Texture2D Sprite;
        public Vector2 Position;
        public bool Active;
        public int Health;


        public int Width
        {
            get { return Sprite.Width; }
        }

        public int Height
        {
            get { return Sprite.Height; }
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Sprite = texture;
            Position = position;
            Active = true;
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
