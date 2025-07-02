using Sloth.Core.Components;
using Sloth.Core.Entities;
using Sloth.Core.Enums;
using System.Collections.Generic;

namespace Sloth.Core.Systems
{
    /// <summary>
    /// Base class for all systems in the ECS framework.
    /// </summary>
    public abstract class EcsSystem
    {
        /// <summary>
        /// The world this system operates on.
        /// </summary>
        protected World World { get; private set; }
        
        /// <summary>
        /// Whether this system is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The priority of this system. Systems with lower priority values are updated first.
        /// </summary>
        public SystemPriority Priority { get; set; } = SystemPriority.First;

        /// <summary>
        /// Initializes the system with the specified world.
        /// </summary>
        /// <param name="world">The world this system will operate on.</param>
        public virtual void Initialize(World world)
        {
            World = world;
        }

        /// <summary>
        /// Updates the system.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public abstract void Update(float gameTime);

        /// <summary>
        /// Gets all entities that have all of the specified component types.
        /// </summary>
        /// <typeparam name="T">The first required component type.</typeparam>
        /// <returns>An enumerable of entities with the required components.</returns>
        protected IEnumerable<Entity> GetEntities<T>() where T : Component
        {
            return World.GetEntitiesWithComponent<T>();
        }

        /// <summary>
        /// Gets all entities that have all of the specified component types.
        /// </summary>
        /// <typeparam name="T1">The first required component type.</typeparam>
        /// <typeparam name="T2">The second required component type.</typeparam>
        /// <returns>An enumerable of entities with the required components.</returns>
        protected IEnumerable<Entity> GetEntities<T1, T2>() 
            where T1 : Component 
            where T2 : Component
        {
            return World.GetEntitiesWithComponents<T1, T2>();
        }

        /// <summary>
        /// Gets all entities that have all of the specified component types.
        /// </summary>
        /// <typeparam name="T1">The first required component type.</typeparam>
        /// <typeparam name="T2">The second required component type.</typeparam>
        /// <typeparam name="T3">The third required component type.</typeparam>
        /// <returns>An enumerable of entities with the required components.</returns>
        protected IEnumerable<Entity> GetEntities<T1, T2, T3>() 
            where T1 : Component 
            where T2 : Component 
            where T3 : Component
        {
            return World.GetEntitiesWithComponents<T1, T2, T3>();
        }
    }
}
