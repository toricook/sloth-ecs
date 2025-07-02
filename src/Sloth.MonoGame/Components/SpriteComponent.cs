using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sloth.Core.Components;
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
        public Texture2D Texture { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Color Color { get; set; } = Color.White;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        public SpriteComponent(Texture2D texture)
        {
            Texture = texture;
        }
    }
}