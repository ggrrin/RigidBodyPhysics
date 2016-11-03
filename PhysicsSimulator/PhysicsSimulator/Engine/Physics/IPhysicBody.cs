using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.Interaction.Core;
namespace PhysicsSimulator.Engine.Physics
{
    /// <summary>
    /// Rozhraní určující základní vlastnosti tuhých těles.
    /// </summary>
    interface IPhysicBody : IInteractable
    {
        /// <summary>
        /// urcuje zda je ovladatelny hrace,
        /// </summary>
        bool Player { get; set; }

        /// <summary>
        /// ohranicujci obdlnik
        /// </summary>
        Box Bound { get; }

        /// <summary>
        /// pozicae
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// hustota
        /// </summary>
        float Density { get; set; }

        /// <summary>
        /// homtnost
        /// </summary>
        float Mass { get; }

        /// <summary>
        /// Rychlost
        /// </summary>
        Vector2 Velocity { get; set; }


        /// <summary>
        /// uhlova rychlsot
        /// </summary>
        Vector3 AngularVelocity { get; set; }

        /// <summary>
        /// moment
        /// </summary>
        Vector2 Momentum { get; }

        /// <summary>
        /// moment setrvacnosti
        /// </summary>
        float MomentOfInertia { get; }

        /// <summary>
        /// elasticita
        /// </summary>
        float Elasticity { get; }

        /// <summary>
        /// Duplikacni funce
        /// </summary>
        /// <returns>vraci clona</returns>
        IPhysicBody Clone();
    }
}
