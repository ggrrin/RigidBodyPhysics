using Microsoft.Xna.Framework;
using System;

namespace PhysicsSimulator.Engine.Game
{
    /// <summary>
    /// Eventbox který interakcí s hráčovým kamenem teleporuje kámen na stanovené místo.
    /// </summary>
    [Serializable]
    class TeleportEventBox : EventBox
    {
        public Vector2 Location { get; set; }

        public TeleportEventBox(Vector2 min, Vector2 max)
            : base(min, max)
        {
            Color = Color.Blue;
            base.evType = EventBoxEnum.Teleport;
        }

        /// <summary>
        /// teleportuje teleso
        /// </summary>
        /// <param name="body"></param>
        public override void AffectBody(Physics.IPhysicBody body)
        {
            if (body.Player)
                body.Translate(Location - body.Position);
        }
    }
}
