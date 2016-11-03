using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Physics;
using System;

namespace PhysicsSimulator.Engine.Game
{
    /// <summary>
    /// Eventbox který interakcí s hráčovým kamenem oznamuje GameMap ze kamen se dostal do cilove pozice. Na základě toho potom GameMap ukončí daný level.
    /// </summary>
    [Serializable]
    class FinishEventBox : EventBox
    {

        //public delegate void FinishEventHadler(object sender, FinishEventArgs e);
        //public event FinishEventHadler Finish;

        private bool enabled = true;

        public FinishEventBox(Vector2 min, Vector2 max)
            : base(min, max)
        {
            Color = Color.Yellow;
            base.evType = EventBoxEnum.Finish;
        }

        /// <summary>
        /// nastavi ze hrac dojel ka mel
        /// </summary>
        /// <param name="body"></param>
        public override void AffectBody(IPhysicBody body)
        {
            if (enabled && body.Player)
            {
                body.Player = enabled = false;
            }
            //if (body.Player)
            //    if (Finish != null)
            //        Finish(this, new FinishEventArgs(body));
        }
    }

}
