using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Prequel
{
    interface Scene
    {
        bool Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
        void Start();
    }
}
