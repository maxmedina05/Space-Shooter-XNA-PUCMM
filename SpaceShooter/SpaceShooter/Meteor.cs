using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    class Meteor
    {
        public Texture2D Sprite;
        public Vector2 Position;
        public bool Active;
        public int Health;
        public int Damage;
        public int Value;
        private int elapsedTime;

        private float enemyMoveSpeed;

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
            Health = 10;
            Damage = 10;
            enemyMoveSpeed = 3f;

            Value = 100;
            elapsedTime = 0;
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += (int) gameTime.ElapsedGameTime.TotalMilliseconds;

            Position.Y += enemyMoveSpeed;
            if (Health <= 0 || elapsedTime > 5000f)
            {
                Active = false;
                elapsedTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    
    }
}
