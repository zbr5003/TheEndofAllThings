using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

//Note that for this version of the game, sprite scale and enemy position
//properties will be hard coded. I might make them variable in the full game.

namespace Prequel
{
    class GameScene : Scene
    {
        //----------------
        // Game configuration variables
        //----------------

        private float spriteScale = 0.3f;
        private float tickInterval;

        //----------------
        // Utility variables
        //----------------
        
        private Vector2 spriteSize;
        private Vector2 shotSize;
        private Random randomGenerator;

        //----------------
        // Textures
        //----------------

        private Texture2D enemyHealthy;
        private Texture2D enemyWounded;
        private Texture2D enemyShot;

        private Texture2D playerHealthy;
        private Texture2D playerWounded;
        private Texture2D playerNearDeath;
        private Texture2D playerShot;
        private SpriteFont displayFont;

        //----------------
        // Sound effects
        //----------------

        private SoundEffect shotEffect;
        private SoundEffect hitEffect;

        //----------------
        // Enemy positions and status
        //----------------

        private EnemyProperties enemyProperties = new EnemyProperties(3, 6, 100f, 550f, 20f, 60f, 60f, 1000f, .5f);
        private Vector2[,] enemyBasePositions;

        //Status variables for enemy movement
        private float enemyCurrentY;
        private int enemyYMovementDirection;

        //Possible enemy states
        private enum EnemyStates { Healthy, Wounded, Dead };
        private EnemyStates[,] enemyStates;

        private int enemiesLeft;

        //These rectangles will be used for hit detection.
        private Rectangle[] enemyXRectangles;
        private Rectangle[] enemyYRectangles;

        //For the time being, I'm using the Space Invaders model of only one shot at a time.
        private Vector2 enemyShotPosition;
        //This defines how far to the right a shot can go.
        private float enemyShotXLimit = 0f;
        
        //----------------
        // Player position and status
        //----------------

        private PlayerProperties playerProperties = new PlayerProperties(.25f, .5f, 50f);
        private float playerCurrentY;

        private enum PlayerStates { Healthy, Wounded, NearDeath, Dead };
        private PlayerStates playerState;

        //For the time being, I'm using the Space Invaders model of only one shot at a time.
        private Vector2 playerShotPosition;
        //This defines how far to the right a shot can go. It's hardcoded for now, like a
        //number of other things.
        private float playerShotXLimit = 800f;

        //----------------
        // Constructor and model initialization
        //----------------

        //Strictly speaking, this function *does* have a certain amount of code that
        //shouldn't really be executed repeatedly. I'll factor that out into a separate
        //one-time function eventually.
        private void Initialize()
        {
            randomGenerator = new Random();

            //Load up enemy positions and statuses. Also load up enemy hit rectangles.
            enemiesLeft = enemyProperties.XCount * enemyProperties.YCount;
            enemyCurrentY = 0f;
            enemyYMovementDirection = 1;

            enemyBasePositions = new Vector2[enemyProperties.XCount, enemyProperties.YCount];
            enemyStates = new EnemyStates[enemyProperties.XCount, enemyProperties.YCount];
            enemyXRectangles = new Rectangle[enemyProperties.XCount];
            enemyYRectangles = new Rectangle[enemyProperties.YCount];
            for (int i = 0; i < enemyProperties.XCount; i++)
            {
                for (int j = 0; j < enemyProperties.YCount; j++)
                {
                    enemyBasePositions[i, j] = new Vector2(enemyProperties.InitialX + enemyProperties.XInterval * i, enemyProperties.InitialY + enemyProperties.YInterval * j);
                    enemyStates[i, j] = EnemyStates.Healthy;
                }
                enemyXRectangles[i] = new Rectangle((int)(enemyProperties.InitialX + enemyProperties.XInterval * i), (int)enemyProperties.InitialY, (int)(spriteSize.X), (int)(enemyProperties.YInterval * (enemyProperties.YCount - 1) + spriteSize.Y + enemyProperties.YMovementRange));
            }
            for (int j = 0; j < enemyProperties.YCount; j++)
            {
                enemyYRectangles[j] = new Rectangle((int)enemyProperties.InitialX, (int)(enemyProperties.InitialY + enemyProperties.YInterval * j), (int)(enemyProperties.XInterval * (enemyProperties.XCount - 1) + spriteSize.X), (int)spriteSize.Y);
            }

            //Load up player position and status.
            playerCurrentY = 240f - spriteSize.Y / 2;
            playerState = PlayerStates.Healthy;
        }

        public GameScene(float newTickInterval, ContentManager contentManager)
        {
            tickInterval = newTickInterval;

            //Load up the textures.
            enemyHealthy = contentManager.Load<Texture2D>("enemyHealthy");
            enemyWounded = contentManager.Load<Texture2D>("enemyWounded");
            enemyShot = contentManager.Load<Texture2D>("enemyShot");

            playerHealthy = contentManager.Load<Texture2D>("playerHealthy");
            playerWounded = contentManager.Load<Texture2D>("playerWounded");
            playerNearDeath = contentManager.Load<Texture2D>("playerNearDeath");
            playerShot = contentManager.Load<Texture2D>("playerShot");

            displayFont = contentManager.Load<SpriteFont>("displayFont");

            //Load up the sound effects.
            shotEffect = contentManager.Load<SoundEffect>("shot");
            hitEffect = contentManager.Load<SoundEffect>("hit");

            //Get the texture size. (Enemy and Player textures are the same size, so you can do this just once.)
            spriteSize = new Vector2(enemyHealthy.Width * spriteScale, enemyHealthy.Height * spriteScale);
            shotSize = new Vector2(playerShot.Width * spriteScale, playerShot.Height * spriteScale);

            //Initialize game model.
            Initialize();
        }

        //----------------
        // Gameplay functions
        //----------------

        public void Start()
        {
            PlayerShoots();
            EnemyShoots();
        }

        //This will require some logic to make sure the forward-most player in a
        //row attacks. It'll also need to pick a random row.
        public void EnemyShoots()
        {
            //NOTE: This is an EXPERIMENTAL DEVELOPMENT way to pick a row.
            //It may not work.
            while(enemiesLeft > 0)  //Only start doing a random search if there are
            {                       //actually enemies left.
                //First, pick a random row.
                int firingY = randomGenerator.Next(0, enemyProperties.YCount);
                //Next, find the first non-dead member of that row.
                for (int firingX = 0; firingX < enemyProperties.XCount; firingX++)
                {
                    if (enemyStates[firingX, firingY] != EnemyStates.Dead)
                    {
                        enemyShotPosition = enemyBasePositions[firingX, firingY] + new Vector2(0f, enemyCurrentY + spriteSize.Y / 2);
                        shotEffect.Play();
                        return;
                    }
                }
            }
        }

        private void PlayerShoots()
        {
            playerShotPosition = new Vector2(playerProperties.XPosition + spriteSize.X, playerCurrentY + spriteSize.Y / 2);
            shotEffect.Play();
        }

        //This will return what enemy is currently overlapping with the player's shot (or
        //(-1, -1) if there isn't one).
        private Tuple<int, int> ShotEnemy()
        {
            //First define a rectangle for the shot.
            Rectangle shotRect = new Rectangle((int)playerShotPosition.X, (int)playerShotPosition.Y, (int)shotSize.X, (int)shotSize.Y);

            //Next, check if it intersects with any of the X rectangles. Only do it if it's
            //beyond a certain point though.
            if (playerShotPosition.X + shotSize.X > enemyProperties.InitialX)
            {
                for (int i = 0; i < enemyProperties.XCount; i++)
                {
                    if (shotRect.Intersects(enemyXRectangles[i]))
                    {
                        for (int j = 0; j < enemyProperties.YCount; j++)
                        {
                            Rectangle tempRect = new Rectangle(enemyYRectangles[j].X, enemyYRectangles[j].Y, enemyYRectangles[j].Width, enemyYRectangles[j].Height);
                            tempRect.Offset(new Point(0, (int)enemyCurrentY));
                            if (shotRect.Intersects(tempRect))
                            {
                                return new Tuple<int, int>(i, j);
                            }
                        }

                        //If the loop exited, it means the bullet doesn't intersect
                        //with any of the Y rectangles.
                        return new Tuple<int, int>(-1, -1);
                    }
                }
            }

            //If the loop exited, it means the bullet doesn't intersect with any
            //of the X rectangles.
            return new Tuple<int, int>(-1, -1);
        }

        //This will return whether the player has been shot.
        private bool ShotPlayer()
        {
            //First define a rectangle for the shot.
            Rectangle shotRect = new Rectangle((int)enemyShotPosition.X, (int)enemyShotPosition.Y, (int)shotSize.X, (int)shotSize.Y);

            //Next, define a rectangle for the player.
            Rectangle playerRect = new Rectangle((int)playerProperties.XPosition, (int)playerCurrentY, (int)spriteSize.X, (int)spriteSize.Y);

            return shotRect.Intersects(playerRect);
        }

        //----------------
        // Game loop functions
        //----------------

        public bool Update(GameTime gameTime)
        {
            //First check: is the player dead? If so, display a 'Press Space to try again'
            //message, and wait for the spacebar.
            if (playerState == PlayerStates.Dead)
            {
                KeyboardState keyState = Keyboard.GetState();
                if (keyState.IsKeyDown(Keys.Space))
                {
                    //Restart the game.
                    Initialize();
                    Start();
                }
            }
            else
            {

                //----------------
                // Sprite movement logic
                //----------------

                //Player movement logic
                //Player limits are hardcoded for now. These will be set in game constants later.
                KeyboardState keyState = Keyboard.GetState();
                if (keyState.IsKeyDown(Keys.Up))
                {
                    playerCurrentY = (playerCurrentY - playerProperties.MovementSpeed * tickInterval > 20f) ? playerCurrentY - playerProperties.MovementSpeed * tickInterval : 20f;
                }
                else if (keyState.IsKeyDown(Keys.Down))
                {
                    playerCurrentY = (playerCurrentY + playerProperties.MovementSpeed * tickInterval < 460f - spriteSize.Y) ? playerCurrentY + playerProperties.MovementSpeed * tickInterval : 460f - spriteSize.Y;
                }

                //Enemy movement logic
                enemyCurrentY += enemyProperties.YMovementRange / enemyProperties.YMovementInterval * tickInterval * enemyYMovementDirection;
                if (enemyCurrentY >= enemyProperties.YMovementRange)
                {
                    enemyCurrentY = enemyProperties.YMovementRange;
                    enemyYMovementDirection = -1;
                }
                else if (enemyCurrentY <= 0)
                {
                    enemyCurrentY = 0;
                    enemyYMovementDirection = 1;
                }

                //----------------
                // Shot movement logic
                //----------------

                //Player shot movement logic
                //First, if the shot exceeds screen boundaries, shoot a new shot.
                if (playerShotPosition.X >= playerShotXLimit)
                {
                    PlayerShoots();
                }
                //Otherwise, propagate the shot.
                else
                {
                    playerShotPosition += new Vector2(playerProperties.ShotSpeed * tickInterval, 0);
                }

                //Enemy shot movement logic
                //First, if the shot exceeds screen boundaries, shoot a new shot.
                if (enemyShotPosition.X <= enemyShotXLimit)
                {
                    EnemyShoots();
                }
                //Otherwise, propagate the shot.
                else
                {
                    enemyShotPosition += new Vector2(-1 * enemyProperties.ShotSpeed * tickInterval, 0);
                }

                //----------------
                // Hit detection logic
                //----------------

                //Player hit detection logic
                if (ShotPlayer())
                {
                    hitEffect.Play();
                    if (playerState == PlayerStates.Healthy)
                    {
                        playerState = PlayerStates.Wounded;
                        EnemyShoots();
                    }
                    else if (playerState == PlayerStates.Wounded)
                    {
                        playerState = PlayerStates.NearDeath;
                        EnemyShoots();
                    }
                    else if(playerState == PlayerStates.NearDeath)
                    {
                        playerState = PlayerStates.Dead;
                    }
                }

                //Enemy hit detection logic
                Tuple<int, int> hitEnemy = ShotEnemy();
                if (hitEnemy.Item1 != -1)
                {
                    if (enemyStates[hitEnemy.Item1, hitEnemy.Item2] == EnemyStates.Healthy)
                    {
                        enemyStates[hitEnemy.Item1, hitEnemy.Item2] = EnemyStates.Wounded;
                        PlayerShoots();
                    }
                    else if (enemyStates[hitEnemy.Item1, hitEnemy.Item2] == EnemyStates.Wounded)
                    {
                        enemyStates[hitEnemy.Item1, hitEnemy.Item2] = EnemyStates.Dead;
                        hitEffect.Play();
                        enemiesLeft--;
                        PlayerShoots();
                    }
                }
            }
            return enemiesLeft == 0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.White);

            //Draw enemies
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            for (int i = 0; i < enemyProperties.XCount; i++)
            {
                for (int j = 0; j < enemyProperties.YCount; j++)
                {
                    if (enemyStates[i, j] == EnemyStates.Healthy)
                    {
                        spriteBatch.Draw(enemyHealthy, enemyBasePositions[i, j] + new Vector2(0, enemyCurrentY), null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
                    }
                    else if (enemyStates[i, j] == EnemyStates.Wounded)
                    {
                        spriteBatch.Draw(enemyWounded, enemyBasePositions[i, j] + new Vector2(0, enemyCurrentY), null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
                    }
                }
            }

            //Draw shots
            if (playerState != PlayerStates.Dead)
            {
                spriteBatch.Draw(playerShot, playerShotPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(enemyShot, enemyShotPosition, null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
            }

            //Draw player
            if (playerState == PlayerStates.Healthy)
            {
                spriteBatch.Draw(playerHealthy, new Vector2(playerProperties.XPosition, playerCurrentY), null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
            }
            else if (playerState == PlayerStates.Wounded)
            {
                spriteBatch.Draw(playerWounded, new Vector2(playerProperties.XPosition, playerCurrentY), null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
            }
            else if (playerState == PlayerStates.NearDeath)
            {
                spriteBatch.Draw(playerNearDeath, new Vector2(playerProperties.XPosition, playerCurrentY), null, Color.White, 0f, Vector2.Zero, spriteScale, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.DrawString(displayFont, "Press Space to restart fire test", new Vector2(200f, 230f), Color.Black);
            }

            spriteBatch.End();
        }
    }
}
