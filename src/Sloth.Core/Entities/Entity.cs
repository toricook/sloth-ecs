using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Sloth.Core
{
    /// <summary>
    /// Represents a game entity, which is a collection of components.
    /// </summary>
    public class Entity
    {
        private static int _nextId = 0;
        private readonly Dictionary<Type, Component> _components = new Dictionary<Type, Component>();
        
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Whether this entity is active in the world.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// The world this entity belongs to.
        /// </summary>
        public World World { get; internal set; }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        public Entity()
        {
            Id = _nextId++;
        }

        /// <summary>
        /// Adds a component to this entity.
        /// </summary>
        /// <param name="component">The component to add.</param>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>This entity for method chaining.</returns>
        public Entity AddComponent<T>(T component) where T : Component
        {
            Type type = typeof(T);
            
            // Check if a component of this type already exists
            if (_components.ContainsKey(type))
            {
                throw new InvalidOperationException($"Entity already has a component of type {type.Name}");
            }
            
            // Add the component
            _components[type] = component;
            component.Entity = this;
            
            // Notify the world if this entity is already in a world
            if (World != null)
            {
                World.OnEntityComponentAdded(this, component);
            }
            
            return this;
        }

        /// <summary>
        /// Gets a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>The component, or null if not found.</returns>
        public T GetComponent<T>() where T : Component
        {
            Type type = typeof(T);
            return _components.TryGetValue(type, out Component component) ? (T)component : null;
        }

        /// <summary>
        /// Checks if this entity has a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of component to check for.</typeparam>
        /// <returns>True if the entity has the component, false otherwise.</returns>
        public bool HasComponent<T>() where T : Component
        {
            return _components.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Removes a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        /// <returns>True if the component was removed, false if it wasn't found.</returns>
        public bool RemoveComponent<T>() where T : Component
        {
            Type type = typeof(T);
            
            if (!_components.TryGetValue(type, out Component component))
            {
                return false;
            }
            
            _components.Remove(type);
            
            // Notify the world if this entity is in a world
            if (World != null)
            {
                World.OnEntityComponentRemoved(this, component);
            }
            
            return true;
        }

        /// <summary>
        /// Gets all components attached to this entity.
        /// </summary>
        /// <returns>An enumerable of all components.</returns>
        public IEnumerable<Component> GetAllComponents()
        {
            return _components.Values;
        }
    }
}
