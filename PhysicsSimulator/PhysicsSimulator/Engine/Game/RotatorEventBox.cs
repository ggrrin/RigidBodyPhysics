using Microsoft.Xna.Framework;
using System;

namespace PhysicsSimulator.Engine.Game
{

    /// <summary>
    /// Eventbox který interakcí s hráčovým kamenem zvyšuje otačívou rychlost daného kamene.
    /// </summary>
    [Serializable]
    class RotatorEventBox : EventBox
    {
        const float ROT_FACTOR = 0.05f;

        public RotatorEventBox(Vector2 min, Vector2 max)
            : base(min, max)
        {
            Color = Color.Violet;
            base.evType = EventBoxEnum.Rotator;
        }
        /// <summary>
        /// zacne otacet telesem
        /// </summary>
        /// <param name="body"></param>
        public override void AffectBody(Physics.IPhysicBody body)
        {
            float an = body.AngularVelocity.Z;
            body.AngularVelocity = new Vector3(0, 0, an + ROT_FACTOR);
        }
    }
}
