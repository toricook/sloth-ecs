using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penguin.Components.Rendering;
using Penguin.Interfaces;
using Penguin.Managers;
using Penguin.Systems.Rendering;

namespace Sloth.Core
{
    /// <summary>
    /// The world container for the ECS framework.
    /// </summary>
    public class World
    {
        // Entities in the world
        private readonly List<Entity> _entities = new List<Entity>();
        
        // Systems in the world
        private readonly List<System> _systems = new List<System>();

        // Render systems in the world
        private readonly List<RenderSystem> _renderSystems = new List<RenderSystem>();
        
        // Component-to-entity lookup for faster queries
        private readonly Dictionary<Type, List<Entity>> _componentEntityMap = new Dictionary<Type, List<Entity>>();
        
        // Shared data storage for inter-system communication
        private readonly Dictionary<string, object> _sharedData = new Dictionary<string, object>();

        public IGameContentProvider ContentProvider;

        /// <summary>
        /// Creates a new world.
        /// </summary>
        public World(IGameContentProvider contentProvider)
        {
            ContentProvider = contentProvider;
        }

        /// <summary>
        /// Adds an entity to the world.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void AddEntity(Entity entity)
        {
            if (entity.World != null && entity.World != this)
            {
                throw new InvalidOperationException("Entity already belongs to another world");
            }
            
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
                entity.World = this;
                
                // Update component-entity map for quick lookup
                foreach (Component component in entity.GetAllComponents())
                {
                    RegisterComponentEntity(component.GetType(), entity);
                    OnEntityComponentAdded(entity, component);
                }
            }
        }

        /// <summary>
        /// Removes an entity from the world.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if the entity was removed, false otherwise.</returns>
        public bool RemoveEntity(Entity entity)
        {
            if (entity.World != this)
            {
                return false;
            }
            
            if (_entities.Remove(entity))
            {
                // Remove from component-entity map
                foreach (Component component in entity.GetAllComponents())
                {
                    UnregisterComponentEntity(component.GetType(), entity);
                }
                
                entity.World = null;
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Adds a system to the world.
        /// </summary>
        /// <param name="system">The system to add.</param>
        public void AddSystem(System system)
        {
            if (!_systems.Contains(system))
            {
                _systems.Add(system);
                
                // Sort systems by priority
                _systems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }

            if (system is RenderSystem renderSystem && !_renderSystems.Contains(renderSystem))
            {
                _renderSystems.Add(renderSystem);
                // Sort systems by priority
                _renderSystems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
        }

        /// <summary>
        /// Removes a system from the world.
        /// </summary>
        /// <param name="system">The system to remove.</param>
        /// <returns>True if the system was removed, false otherwise.</returns>
        public bool RemoveSystem(System system)
        {
            return _systems.Remove(system);
        }

        public void Initialize()
        {
            foreach (var sys in _systems)
            {
                sys.Initialize(this);
            }
        }

        /// <summary>
        /// Updates all systems in the world.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (System system in _systems)
            {
                if (system.Enabled)
                {
                    system.Update(gameTime);
                }
            }

        }

        /// <summary>
        /// Draws all renderable systems in the world.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Get camera component for view matrix if it exists
            CameraComponent camera = null;
            if (GameHelpers.TryGetSingleCameraEntity(this, out var c))
            {
                camera = c.GetComponent<CameraComponent>();
            }
            Matrix viewMatrix = camera?.GetViewMatrix() ?? Matrix.Identity;

            foreach (var renderSystem in _renderSystems)
            {
                if (renderSystem.Enabled)
                {
                    renderSystem.Draw(gameTime, spriteBatch, viewMatrix);
                }
            }
        }

        /// <summary>
        /// Gets all entities with a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The component type to check for.</typeparam>
        /// <returns>An enumerable of entities with the specified component.</returns>
        public IEnumerable<Entity> GetEntitiesWithComponent<T>() where T : Component
        {
            Type componentType = typeof(T);
            
            if (_componentEntityMap.TryGetValue(componentType, out List<Entity> entities))
            {
                // Return only active entities
                return entities.Where(e => e.IsActive);
            }
            
            return Enumerable.Empty<Entity>();
        }

        /// <summary>
        /// Gets all entities with components of the specified types.
        /// </summary>
        /// <typeparam name="T1">The first component type.</typeparam>
        /// <typeparam name="T2">The second component type.</typeparam>
        /// <returns>An enumerable of entities with the specified components.</returns>
        public IEnumerable<Entity> GetEntitiesWithComponents<T1, T2>() 
            where T1 : Component 
            where T2 : Component
        {
            // Get entities with the first component type, then filter for those with the second type
            return GetEntitiesWithComponent<T1>().Where(e => e.HasComponent<T2>());
        }

        /// <summary>
        /// Gets all entities with components of the specified types.
        /// </summary>
        /// <typeparam name="T1">The first component type.</typeparam>
        /// <typeparam name="T2">The second component type.</typeparam>
        /// <typeparam name="T3">The third component type.</typeparam>
        /// <returns>An enumerable of entities with the specified components.</returns>
        public IEnumerable<Entity> GetEntitiesWithComponents<T1, T2, T3>() 
            where T1 : Component 
            where T2 : Component 
            where T3 : Component
        {
            // Get entities with the first two component types, then filter for those with the third type
            return GetEntitiesWithComponents<T1, T2>().Where(e => e.HasComponent<T3>());
        }

        /// <summary>
        /// Sets shared data for inter-system communication.
        /// </summary>
        /// <param name="key">The key for the data.</param>
        /// <param name="data">The data to store.</param>
        public void SetData(string key, object data)
        {
            _sharedData[key] = data;
        }

        /// <summary>
        /// Gets shared data for inter-system communication.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <param name="key">The key for the data.</param>
        /// <returns>The data, or default(T) if not found.</returns>
        public T GetData<T>(string key)
        {
            if (_sharedData.TryGetValue(key, out object data) && data is T typedData)
            {
                return typedData;
            }
            
            return default;
        }

        /// <summary>
        /// Called when a component is added to an entity in this world.
        /// Updates the component-entity map for faster queries.
        /// </summary>
        /// <param name="entity">The entity the component was added to.</param>
        /// <param name="component">The component that was added.</param>
        internal void OnEntityComponentAdded(Entity entity, Component component)
        {
            RegisterComponentEntity(component.GetType(), entity);
        }

        /// <summary>
        /// Called when a component is removed from an entity in this world.
        /// Updates the component-entity map for faster queries.
        /// </summary>
        /// <param name="entity">The entity the component was removed from.</param>
        /// <param name="component">The component that was removed.</param>
        internal void OnEntityComponentRemoved(Entity entity, Component component)
        {
            UnregisterComponentEntity(component.GetType(), entity);
        }

        /// <summary>
        /// Registers an entity as having a component of the specified type.
        /// </summary>
        /// <param name="componentType">The component type.</param>
        /// <param name="entity">The entity to register.</param>
        private void RegisterComponentEntity(Type componentType, Entity entity)
        {
            // Register the exact component type
            if (!_componentEntityMap.TryGetValue(componentType, out List<Entity> entities))
            {
                entities = new List<Entity>();
                _componentEntityMap[componentType] = entities;
            }
            
            if (!entities.Contains(entity))
            {
                entities.Add(entity);
            }
            
            // Also register for all base types and interfaces
            foreach (Type baseType in componentType.GetInterfaces().Concat(GetBaseTypes(componentType)))
            {
                if (baseType == typeof(object) || baseType == typeof(Component))
                {
                    continue;
                }
                
                if (!_componentEntityMap.TryGetValue(baseType, out List<Entity> baseEntities))
                {
                    baseEntities = new List<Entity>();
                    _componentEntityMap[baseType] = baseEntities;
                }
                
                if (!baseEntities.Contains(entity))
                {
                    baseEntities.Add(entity);
                }
            }
        }

        /// <summary>
        /// Unregisters an entity as having a component of the specified type.
        /// </summary>
        /// <param name="componentType">The component type.</param>
        /// <param name="entity">The entity to unregister.</param>
        private void UnregisterComponentEntity(Type componentType, Entity entity)
        {
            // Unregister for the exact component type
            if (_componentEntityMap.TryGetValue(componentType, out List<Entity> entities))
            {
                entities.Remove(entity);
                
                if (entities.Count == 0)
                {
                    _componentEntityMap.Remove(componentType);
                }
            }
            
            // Also unregister for all base types and interfaces
            foreach (Type baseType in componentType.GetInterfaces().Concat(GetBaseTypes(componentType)))
            {
                if (baseType == typeof(object) || baseType == typeof(Component))
                {
                    continue;
                }
                
                if (_componentEntityMap.TryGetValue(baseType, out List<Entity> baseEntities))
                {
                    baseEntities.Remove(entity);
                    
                    if (baseEntities.Count == 0)
                    {
                        _componentEntityMap.Remove(baseType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all base types of the specified type (excluding object).
        /// </summary>
        /// <param name="type">The type to get base types for.</param>
        /// <returns>An enumerable of base types.</returns>
        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            Type baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        /// <summary>
        /// Gets a system of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of system to get.</typeparam>
        /// <returns>The system, or null if not found.</returns>
        public T GetSystem<T>() where T : System
        {
            return _systems.OfType<T>().FirstOrDefault();
        }

        public void RegisterAllSystems()
        {
            var systemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(Core.ECS.System).IsAssignableFrom(t));

            foreach (var systemType in systemTypes)
            {
                try
                {
                    var system = (Core.ECS.System)Activator.CreateInstance(systemType);
                    AddSystem(system);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create system {systemType.Name}: {ex.Message}");
                }
            }
        }
    }
}
