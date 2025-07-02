using Microsoft.Xna.Framework;
using Sloth.Core.Components;

namespace Sloth.MonoGame.Components
{
    /// <summary>
    /// Component that represents the position of an entity in 2D space.
    /// </summary>
    public class PositionComponent : Component
    {
        /// <summary>
        /// The current position of the UPPER-LEFT CORNER of the entity
        /// </summary>
        public Vector2 Position { get; set; }
        
        /// <summary>
        /// The previous position of the entity (useful for interpolation and collision resolution).
        /// </summary>
        public Vector2 PreviousPosition { get; set; }


        /// <summary>
        /// Creates a new position component at the specified position.
        /// </summary>
        /// <param name="position">Initial position vector</param>
        public PositionComponent(Vector2 position)
        {
            Position = position;
            PreviousPosition = position;
        }

        /// <summary>
        /// Creates a new position component at (0,0).
        /// </summary>
        public PositionComponent() : this(Vector2.Zero)
        { }

        /// <summary>
        /// Creates a new position component at the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public PositionComponent(float x, float y) : this(new Vector2(x, y))
        { }

        /// <summary>
        /// Updates the previous position to the current position.
        /// Call this at the end of a frame after all position changes.
        /// </summary>
        public void StorePreviousPosition()
        {
            PreviousPosition = new Vector2(Position.X, Position.Y);
        }

        /// <summary>
        /// Gets the interpolated position between previous and current positions.
        /// Useful for smooth rendering between physics updates.
        /// </summary>
        /// <param name="alpha">Interpolation factor (0.0 to 1.0)</param>
        /// <returns>Interpolated position</returns>
        public Vector2 GetInterpolatedPosition(float alpha)
        {
            return Vector2.Lerp(PreviousPosition, Position, alpha);
        }

    }
}
