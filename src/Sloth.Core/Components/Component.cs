using Sloth.Core.Entities;

namespace Sloth.Core.Components
{
    /// <summary>
    /// Base class for all components in the ECS framework.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// The entity this component is attached to.
        /// </summary>
        public Entity Entity { get; internal set; }
    }
}
