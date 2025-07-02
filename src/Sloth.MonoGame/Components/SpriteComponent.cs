using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penguin.Core.ECS;
using Penguin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sloth.MonoGame.Components
{
    /// <summary>
    /// Component that defines how an entity is rendered with a sprite.
    /// </summary>
    public class SpriteComponent : Component
    {
        public Sprite Sprite;

        /// <summary>
        /// The base layer depth (0.0f to 1.0f, higher values are drawn on top).
        /// This value can be overridden by Y-order sorting.
        /// </summary>
        public float BaseLayerDepth { get; set; }

        /// <summary>
        /// Position offset for the sprite relative to the entity's position.
        /// </summary>
        public Vector2 Offset { get; set; } = Vector2.Zero;

        public SpriteComponent(Sprite sprite, float baseLayerDepth)
        {
            Sprite = sprite;
            BaseLayerDepth = baseLayerDepth;
        }
    }
}