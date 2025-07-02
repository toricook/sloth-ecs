using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Sloth.Core
{
    /// <summary>
    /// Manages events within the ECS framework.
    /// </summary>
    public class EventManager
    {
        // Dictionary to store event subscribers
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();
        
        // Instance for singleton pattern
        private static EventManager _instance;
        
        /// <summary>
        /// Gets the singleton instance of the EventManager.
        /// </summary>
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventManager();
                }
                return _instance;
            }
        }

        private EventManager() { }

        /// <summary>
        /// Subscribes to an event of the specified type with the provided handler.
        /// </summary>
        /// <typeparam name="T">The type of event to subscribe to.</typeparam>
        /// <param name="handler">The event handler.</param>
        public static void Subscribe<T>(Action<T> handler)
        {
            Type eventType = typeof(T);
            
            if (!Instance._subscribers.TryGetValue(eventType, out List<Delegate> handlers))
            {
                handlers = new List<Delegate>();
                Instance._subscribers[eventType] = handlers;
            }
            
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <summary>
        /// Unsubscribes from an event of the specified type with the provided handler.
        /// </summary>
        /// <typeparam name="T">The type of event to unsubscribe from.</typeparam>
        /// <param name="handler">The event handler to unsubscribe.</param>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            Type eventType = typeof(T);
            
            if (Instance._subscribers.TryGetValue(eventType, out List<Delegate> handlers))
            {
                handlers.Remove(handler);
                
                if (handlers.Count == 0)
                {
                    Instance._subscribers.Remove(eventType);
                }
            }
        }

        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <typeparam name="T">The type of event to publish.</typeparam>
        /// <param name="eventData">The event data.</param>
        public static void Publish<T>(T eventData)
        {
            Type eventType = typeof(T);
            
            if (Instance._subscribers.TryGetValue(eventType, out List<Delegate> handlers))
            {
                // Create a copy of the handlers list to allow handlers to unsubscribe during event processing
                foreach (Delegate handler in handlers.ToList())
                {
                    try
                    {
                        ((Action<T>)handler)(eventData);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception but continue processing other handlers
                        Console.WriteLine($"Error in event handler: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Clears all event subscriptions.
        /// </summary>
        public static void ClearAll()
        {
            Instance._subscribers.Clear();
        }
    }
}

