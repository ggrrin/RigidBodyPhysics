using Microsoft.Xna.Framework;
using System;

namespace PhysicsSimulator.Engine.Game
{
    /// <summary>
    /// Eventbox který interakcí s hráčovým kamenem snižuje rychlost daného kamenu.
    /// </summary>
    [Serializable]
    class SlowerEventBox : EventBox
    {
        const float SPEED_FACTOR = 0.98f;

        public SlowerEventBox(Vector2 min, Vector2 max)
            : base(min, max)
        {
            Color = Color.Gray;
            base.evType = EventBoxEnum.Slower;
        }

        /// <summary>
        /// zacne zpomalovat teleso
        /// </summary>
        /// <param name="body"></param>
        public override void AffectBody(Physics.IPhysicBody body)
        {
            body.Velocity *= SPEED_FACTOR;
        }
    }
}
