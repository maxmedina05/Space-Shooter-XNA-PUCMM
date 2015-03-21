using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Shooter;

namespace SpaceShooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum PlayerState
        {
            Idle,
            Left,
            Right
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Player player;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        
        private Texture2D mainBackground;

        float playerMoveSpeed;

        private Texture2D meteorTexture;
        private List<Meteor> meteors;

        private TimeSpan enemySpawnTime;
        private TimeSpan previousSpawnTime;

        private Random random;

        private Texture2D projectileTexture;
        private List<Projectile> projectiles;

        private TimeSpan fireTime;
        private TimeSpan previousFireTime;
        private bool IsShooting;

        private Texture2D explosionTexture;
        private List<Animation> explosions;

        private Texture2D spaceShipIdle;
        private Texture2D spaceShipLeft;
        private Texture2D spaceShipRight;

        SoundEffect laserSound;
        SoundEffect explosionSound;
        Song gameplayMusic;
        int score;
        SpriteFont font;

        private PlayerState playerState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player();
            playerMoveSpeed = 8.0f;
            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            random = new Random();

            meteors = new List<Meteor>();
            projectiles = new List<Projectile>();
            fireTime = TimeSpan.FromSeconds(0.15f);
            explosions = new List<Animation>();
            score = 0;
            
            playerState = PlayerState.Idle;
            IsShooting = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);


            spaceShipIdle = Content.Load<Texture2D>("player");
            player.Initialize(spaceShipIdle, playerPosition);
            spaceShipLeft = Content.Load<Texture2D>("playerLeft");
            spaceShipRight = Content.Load<Texture2D>("playerRight");

            mainBackground = Content.Load<Texture2D>("mainbackground");
            meteorTexture = Content.Load<Texture2D>("meteorSmall");
            projectileTexture = Content.Load<Texture2D>("laserGreen");
            explosionTexture = Content.Load<Texture2D>("explosion");

            // Load the music
            gameplayMusic = Content.Load<Song>("sound/gameMusic");
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");
            PlayMusic(gameplayMusic);
            font = Content.Load<SpriteFont>("gameFont");
        }

        private void PlayMusic(Song song)
        {
            try
            {
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            UpdateMeteors(gameTime);
            UpdateCollision();

            UpdateProjectiles(gameTime);
            UpdateExplosion(gameTime);

            UpdatePlayer(gameTime);

            base.Update(gameTime);
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(gameTime);
                if (projectiles[i].Active == false)
                    projectiles.RemoveAt(i);
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.Position.X -= playerMoveSpeed;
                playerState = PlayerState.Left;
                player.Sprite = spaceShipLeft;
            }

            else if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.Position.X += playerMoveSpeed;
                playerState = PlayerState.Right;
                player.Sprite = spaceShipRight;
            }
            else
            {
                playerState = PlayerState.Idle;
                player.Sprite = spaceShipIdle;
            }
            
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                player.Position.Y -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                player.Position.Y += playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Space))
                IsShooting = true;
            else
                IsShooting = false;
            
            player.Position.X = MathHelper.Clamp(player.Position.X,
                0, GraphicsDevice.Viewport.Width - player.Width);

            player.Position.Y = MathHelper.Clamp(player.Position.Y,
                0, GraphicsDevice.Viewport.Height - player.Height);

            if (gameTime.TotalGameTime - previousFireTime > fireTime && IsShooting)
            {
                previousFireTime = gameTime.TotalGameTime;
                AddProjectile(player.Position + new Vector2(player.Width / 2f, 0));
                laserSound.Play();
            }

            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // Start drawing
            spriteBatch.Begin();
            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            for (int i = 0; i < meteors.Count; i++)
                meteors[i].Draw(spriteBatch);

            for (int i = 0; i < explosions.Count; i++)
                explosions[i].Draw(spriteBatch);

            for (int i = 0; i < projectiles.Count; i++)
                projectiles[i].Draw(spriteBatch);
            

            spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
            player.Draw(spriteBatch);
            // Stop drawing
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void AddMeteor()
        {
            Vector2 position = new Vector2(random.Next(100, GraphicsDevice.Viewport.Width - 100),
                (float)meteorTexture.Height / 2
                );

            Meteor meteor = new Meteor();
            meteor.Initialize(meteorTexture, position);

            meteors.Add(meteor);
        }

        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            projectiles.Add(projectile);
        }
        private void UpdateMeteors(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                AddMeteor();
            }

            for (int i = meteors.Count - 1; i >= 0; i--)
            {
                meteors[i].Update(gameTime);
                if (meteors[i].Active == false)
                {
                    if (meteors[i].Health <= 0)
                    {
                        AddExplosion(meteors[i].Position);
                        explosionSound.Play();
                        //Add to the player's score
                        score += meteors[i].Value;
                    }
                    meteors.RemoveAt(i);
                }
            }
        }

        void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }


        void UpdateExplosion(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                    explosions.RemoveAt(i);
            }
        }

        void UpdateCollision()
        {
            Rectangle rectangle1;
            Rectangle rectangle2;

            rectangle1 = new Rectangle((int)player.Position.X,
                                        (int)player.Position.Y,
                                        player.Width,
                                        player.Height);
            for (int i = 0; i < meteors.Count; i++)
            {
                rectangle2 = new Rectangle(
                    (int)meteors[i].Position.X,
                    (int)meteors[i].Position.Y,
                    meteors[i].Width,
                    meteors[i].Height);

                if (rectangle1.Intersects(rectangle2))
                {
                    player.Health -= meteors[i].Damage;
                    meteors[i].Health = 0;

                    if (player.Health <= 0)
                        player.Active = false;
                }
            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < meteors.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)meteors[j].Position.X - meteors[j].Width / 2,
                    (int)meteors[j].Position.Y - meteors[j].Height / 2,
                    meteors[j].Width, meteors[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        meteors[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }
        }
    }
}
