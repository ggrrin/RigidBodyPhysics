using Microsoft.Xna.Framework;
using System;

namespace PhysicsSimulator.Engine.Game
{
    /// <summary>
    /// Eventbox který interakcí s hráčovým kamenem zvyšuje rychlost kamenu.
    /// </summary>
    [Serializable]
    class SpeederEventBox : EventBox
    {
        const float SPEED_FACTOR = 1.02f;

        public SpeederEventBox(Vector2 min, Vector2 max)
            : base(min, max)
        {
            Color = Color.Red;
            base.evType = EventBoxEnum.Speeder;
        }

        /// <summary>
        /// zacne zrychlovat telso
        /// </summary>
        /// <param name="body"></param>
        public override void AffectBody(Physics.IPhysicBody body)
        {
            body.Velocity *= SPEED_FACTOR;

        }
    }
}
