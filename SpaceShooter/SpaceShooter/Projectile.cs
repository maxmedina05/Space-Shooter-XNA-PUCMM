using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    class Projectile
    {
        public Texture2D Texture;
        public Vector2 Position;
        public bool Active;
        private int elapsedTime;
        public int Damage;

        private Viewport viewport;

        public int Height
        {
            get { return Texture.Height; }
        }

        public int Width
        {
            get { return Texture.Width; }
        }

        private float projectileMoveSpeed;

        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;

            Active = true;

            Damage = 10;
            projectileMoveSpeed = 20f;
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            Position.Y -= projectileMoveSpeed;

            if (elapsedTime > 5000f)
                Active = false;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);

        }
    }
}
