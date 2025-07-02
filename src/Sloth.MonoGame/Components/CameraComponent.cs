using Microsoft.Xna.Framework;
using Penguin.Core.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sloth.MonoGame.Components
{
    /// <summary>
    /// Enum defining the available camera modes.
    /// </summary>
    public enum CameraMode
    {
        /// <summary>
        /// Camera automatically follows a target entity (typically the player).
        /// </summary>
        FollowTarget,

        /// <summary>
        /// Camera is freely controlled by the user (developer mode).
        /// </summary>
        FreePan
    }

    /// <summary>
    /// Component that defines a camera view with multiple operation modes.
    /// </summary>
    public class CameraComponent : Component
    {
        /// <summary>
        /// The position of the camera in world space.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// The origin of the camera (center point of the view).
        /// </summary>
        public Vector2 Origin { get; set; } = Vector2.Zero;

        /// <summary>
        /// The zoom level of the camera (1.0f = normal size).
        /// </summary>
        public float Zoom { get; set; } = 1.0f;

        /// <summary>
        /// The rotation of the camera in radians.
        /// </summary>
        public float Rotation { get; set; } = 0.0f;

        /// <summary>
        /// The target entity to follow (when in FollowTarget mode).
        /// </summary>
        public Entity Target { get; set; } = null;

        /// <summary>
        /// The viewport rectangle (screen area).
        /// </summary>
        public Rectangle Viewport { get; set; }

        /// <summary>
        /// Smoothing factor for following a target (0 = no smoothing, 1 = maximum smoothing).
        /// </summary>
        public float SmoothFactor { get; set; } = 0.1f;

        /// <summary>
        /// The current operating mode of the camera.
        /// </summary>
        public CameraMode Mode { get; set; } = CameraMode.FollowTarget;

        /// <summary>
        /// The speed at which the camera pans in FreePan mode.
        /// </summary>
        public float PanSpeed { get; set; } = 500.0f;

        /// <summary>
        /// The speed at which the camera zooms in and out.
        /// </summary>
        public float ZoomSpeed { get; set; } = 0.1f;

        /// <summary>
        /// The minimum allowed zoom level.
        /// </summary>
        public float MinZoom { get; set; } = 0.1f;

        /// <summary>
        /// The maximum allowed zoom level.
        /// </summary>
        public float MaxZoom { get; set; } = 5.0f;

        /// <summary>
        /// The position of the camera when entering FreePan mode.
        /// Used to restore position when switching back to FollowTarget mode.
        /// </summary>
        public Vector2 SavedPosition { get; set; } = Vector2.Zero;

        /// <summary>
        /// The zoom level of the camera when entering FreePan mode.
        /// Used to restore zoom when switching back to FollowTarget mode.
        /// </summary>
        public float SavedZoom { get; set; } = 1.0f;

        /// <summary>
        /// Toggles between camera modes and saves/restores state as appropriate.
        /// </summary>
        public void ToggleMode()
        {
            if (Mode == CameraMode.FollowTarget)
            {
                // Save current state before switching to FreePan
                SavedPosition = Position;
                SavedZoom = Zoom;
                Mode = CameraMode.FreePan;
            }
            else
            {
                // Restore previous state when switching back to FollowTarget
                if (Target != null)
                {
                    Mode = CameraMode.FollowTarget;
                }
                else
                {
                    Console.WriteLine("No target for follow camera!");
                }
            }
        }

        /// <summary>
        /// Gets the view transformation matrix for rendering.
        /// </summary>
        public Matrix GetViewMatrix()
        {
            // Calculate the center point of the camera view in world space
            // This converts our upper-left position to the center-based position needed for the view matrix
            Vector2 centerPosition = new Vector2(
                Position.X + (Viewport.Width / (2f * Zoom)),
                Position.Y + (Viewport.Height / (2f * Zoom))
            );

            return Matrix.CreateTranslation(new Vector3(-centerPosition, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1.0f)) *
                   Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
    }

}
