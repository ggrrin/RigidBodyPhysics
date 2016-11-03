using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction.Core;
using PhysicsSimulator.Engine.Physics;
using System;

namespace PhysicsSimulator.Engine.Game
{

    /// <summary>
    /// Společný předek všech EventBoxů, dědí možnost určení kolize a implementuje funkce pro ukládní map či používání eventboxů v editoru.
    /// </summary>
    [Serializable]
    abstract class EventBox : Box
    {
        protected EventBoxEnum evType;

        public EventBoxEnum EvType
        { get { return evType; } }

        public EventBox(Vector2 min, Vector2 max)
            : base(min, max)
        {
            DrawAble = true;
        }

        /// <summary>
        /// Uvlivni teleso ktere interaguje s event boxem
        /// </summary>
        /// <param name="body"></param>
        public abstract void AffectBody(IPhysicBody body);


        /// <summary>
        /// Naklonuje dany eventbox
        /// </summary>
        /// <returns></returns>
        public EventBox Clone()
        {
            switch (evType)
            {
                case EventBoxEnum.Speeder:
                    return new SpeederEventBox(Min, Max);
                case EventBoxEnum.Slower:
                    return new SlowerEventBox(Min, Max);
                case EventBoxEnum.Rotator:
                    return new RotatorEventBox(Min, Max);
                case EventBoxEnum.Teleport:
                    var res = new TeleportEventBox(Min, Max);
                    res.Location = (this as TeleportEventBox).Location;
                    return res;
                case EventBoxEnum.Finish:
                    return new FinishEventBox(Min, Max);
                default:
                    throw new Exception();
            }
        }
    }

    /// <summary>
    /// Mozny type eventboxu (kvuli editoru)
    /// </summary>
    enum EventBoxEnum
    {
        /// <summary>
        /// /zrclovasc obektu
        /// </summary>
        Speeder,

        /// <summary>
        /// spomalovat
        /// </summary>
        Slower,

        /// <summary>
        /// rotatator
        /// </summary>
        Rotator,


        /// <summary>
        /// teleporuje objekt
        /// </summary>
        Teleport,

        /// <summary>
        /// finish lokalita
        /// </summary>
        Finish,
    }


}
