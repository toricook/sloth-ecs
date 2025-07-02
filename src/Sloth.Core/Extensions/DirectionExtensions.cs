using Sloth.Core.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Sloth.Core.Extensions
{
    public static class DirectionExtensions
    {
        public static Vector2 ToVector2(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => new Vector2(0, -1),
                Direction.Down => new Vector2(0, 1),
                Direction.Left => new Vector2(-1, 0),
                Direction.Right => new Vector2(1, 0),
                _ => Vector2.Zero
            };
        }
    }
}
