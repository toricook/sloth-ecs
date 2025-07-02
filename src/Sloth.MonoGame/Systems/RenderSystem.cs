using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sloth.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sloth.MonoGame.Systems
{
    /// <summary>
    /// Base class for any system in the ECS framework that needs to implement Drawing and get called in the Draw section of the game loop
    /// </summary>
    public abstract class RenderSystem : EcsSystem
    {
        /// <summary>
        /// Performs render logic
        /// </summary>
        /// <param name="viewMatrix">The transform matrix to render relative to the camera. Use identity for screen-relative renderings</param>
        /// 
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, Matrix? viewMatrix = null);
    }
}
