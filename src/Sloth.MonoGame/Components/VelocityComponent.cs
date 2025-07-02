using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Sloth.Core.Components;
using Sloth.Core.Enums;
using Sloth.Core.Extensions;

namespace Sloth.MonoGame.Components
{
    /// <summary>
    /// Component that represents the velocity of an entity in 2D space.
    /// </summary>
    public class VelocityComponent : Component
    {
        public Direction Direction;
        public float Speed;

        /// <summary>
        /// Creates a new velocity component with the specified velocity.
        /// </summary>
        /// <param name="velocity">Initial velocity vector</param>
        public VelocityComponent(Direction direction = Direction.Down, float speed = 0f)
        {
            Direction = direction;
            Speed = speed;
        }

        /// <summary>
        /// Sets the velocity to the specified direction with the specified speed.
        /// </summary>
        /// <param name="direction">Direction vector (will be normalized)</param>
        /// <param name="speed">Speed magnitude</param>
        public void Set(Direction direction, float speed)
        {
            Direction = direction;
            Speed = speed;
        }

        public void SetToZero()
        {
            Speed = 0;
            // Direction stays the same as before
        }

        public Vector2 GetVelocity()
        {
            return Direction.ToVector2() * Speed;
        }

    }
}
