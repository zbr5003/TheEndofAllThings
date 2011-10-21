using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Prequel
{
    class NarrationScene : Scene
    {
        //----------------
        // Narration variables
        //----------------

        SpriteFont displayFont;
        String[] narrationStringSet;
        bool proceed;
        Vector2 originalOrigin = new Vector2(100f, 100f);
        Vector2 currentOrigin = new Vector2(100f, 100f);
        Vector2 interval = new Vector2(0f, 22f);

        //----------------
        // Constructor
        //----------------

        public NarrationScene(ContentManager contentManager, String[] newNarrationStringSet, bool newProceed)
        {
            displayFont = contentManager.Load<SpriteFont>("displayFont");
            narrationStringSet = newNarrationStringSet;
            proceed = newProceed;
        }

        //----------------
        // Game loop functions
        //----------------

        public void Start()
        {
        }
        
        public bool Update(GameTime gameTime)
        {
            if (proceed)
            {
                KeyboardState keyState = Keyboard.GetState();
                if (keyState.IsKeyDown(Keys.Space))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            
            //Draw each member of the string set.
            for (int i = 0; i < narrationStringSet.Length; i++)
            {
                spriteBatch.DrawString(displayFont, narrationStringSet[i], currentOrigin, Color.Black);
                currentOrigin += interval;
            }

            if (proceed)
            {
                currentOrigin += interval;
                spriteBatch.DrawString(displayFont, "Press SPACE to continue.", currentOrigin, Color.Black);
            }

            currentOrigin = originalOrigin;

            spriteBatch.End();
        }
    }
}
