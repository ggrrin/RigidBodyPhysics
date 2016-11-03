using PhysicsSimulator.Engine.Game;
using PhysicsSimulator.Engine.Physics;
using System;
using System.Collections.Generic;

namespace PhysicsSimulator.Engine
{
    /// <summary>
    /// Datová třídá sloužící čistě pro serializaci všech potřebných objektů mapy.
    /// </summary>
    [Serializable]
    class MapData
    {
        /// <summary>
        /// cislo levelu dane mapy
        /// </summary>
        public int Level { get; set; }


        /// <summary>
        /// scale platna pro danou mapu
        /// </summary>
        public float UnitsOnWidth { get; set; }


        /// <summary>
        /// telesa hry
        /// </summary>
        public List<IPhysicBody> Bodies { get; set; }


        /// <summary>
        /// seznameventboxu
        /// </summary>
        public List<EventBox> EventBoxes { get; set; }


        /// <summary>
        /// inicializuje novou instanci data mapy
        /// </summary>
        /// <param name="level">cislo levelu</param>
        /// <param name="unitsOnWidth">scale platna</param>
        /// <param name="bodies">herni telesa</param>
        /// <param name="boxes">event boxy</param>
        public MapData(int level, float unitsOnWidth, List<IPhysicBody> bodies, List<EventBox> boxes)
        {
            this.Level = level;
            this.UnitsOnWidth = unitsOnWidth;
            this.Bodies = bodies;
            this.EventBoxes = boxes;
        }
    }
}
