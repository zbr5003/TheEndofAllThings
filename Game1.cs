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

namespace Prequel
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //----------------
        // Game properties
        //----------------

        float tickInterval = 20f;

        //----------------
        // Scene management
        //----------------

        Queue<Scene> sceneQueue;
        Scene currentScene;
        
        //----------------
        // Game loop and class functions
        //----------------

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(tickInterval);
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Define the string arrays for the narration scenes.
            String[] narrationStringSet1 = {"Initiating XVR-57 Neuropsychic Conduit fire test.",
                                               "Void Fiber link set to 10% capacity.",
                                               "YT-4 Target Drones set to swarm attack mode.",
                                               "",
                                               "Test subject will use the XVR-57 to clear the field",
                                               "of all drones."};
            String[] narrationStringSet2 = {"WARNING: Results indicate that sustained use of the XVR-57 at",
                                               "high Void Fiber link capacities will result in rapid widespread",
                                               "organ necrosis in the wielder, leading to death.",
                                               "",
                                               "RECOMMENDATION: The XVR-57 project should be terminated",
                                               "immediately. All records pertaining to the project should be",
                                               "classified above top secret, and the prototype should be",
                                               "transferred to the secure archive for permanent storage,",
                                               "along with all associated artifacts and documentation."};

            //Initialize the scene queue, load it up, dequeue the first scene, and start.
            sceneQueue = new Queue<Scene>();
            sceneQueue.Enqueue(new NarrationScene(Content, narrationStringSet1, true));
            sceneQueue.Enqueue(new GameScene(tickInterval, Content));
            sceneQueue.Enqueue(new NarrationScene(Content, narrationStringSet2, false));
            currentScene = sceneQueue.Dequeue();
            currentScene.Start();
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

            //Update the current scene. If it returns true, then change over to the next
            //scene (unless there are no scenes left).
            bool sceneChange = currentScene.Update(gameTime);
            if (sceneChange)
            {
                if (sceneQueue.Count > 0)
                {
                    currentScene = sceneQueue.Dequeue();
                    currentScene.Start();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            currentScene.Draw(gameTime, spriteBatch, GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
