using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.Interaction.Core;
using System;

namespace PhysicsSimulator.Engine.Physics
{

    /// <summary>
    /// Třída simulující zeme. NEPOUZIVANE!! :spatne by se to pouzivalo
    /// </summary>
    [Serializable]
    class Land : HalfSurface, IPhysicBody
    {
        /// <summary>
        /// pozice vychoziho bodu
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// hustota
        /// </summary>
        private float density;

        /// <summary>
        /// hmotnost
        /// </summary>
        private float mass;

        /// <summary>
        /// rychlost
        /// </summary>
        private Vector2 velocity;

        /// <summary>
        /// uhlova rcyhlost
        /// </summary>
        private Vector3 angularVelocity;

        /// <summary>
        /// polomer zeme
        /// </summary>
        private const float RADIUS = 6378000;

        /// <summary>
        /// bonding box
        /// </summary>
        private Box bound;

        /// <summary>
        /// moment setrvacnosti
        /// </summary>
        private float momentOfInertia;

        #region "Properties"

        /// <summary>
        /// zda je obvladatelny hracem
        /// </summary>
        private bool player = false;


        /// <summary>
        /// zda je obvladatelny hracem
        /// </summary>
        public bool Player
        {
            get { return player; }
            set { player = value; }
        }

        private float elasticity;


        /// <summary>
        /// elasticita
        /// </summary>
        public float Elasticity
        {
            get { return elasticity; }
            set { elasticity = value; }
        }

        /// <summary>
        /// moment setrvacnosti
        /// </summary>
        public float MomentOfInertia
        {
            get { return momentOfInertia; }
        }


        /// <summary>
        /// uhlova rychlost
        /// </summary>
        public Vector3 AngularVelocity
        {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }


        /// <summary>
        /// bounding box
        /// </summary>
        public Box Bound
        {
            //TODO: SETBOUNDING BOOOOOOOXO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
            get { return bound; }
        }


        /// <summary>
        /// pozice
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }


        /// <summary>
        /// hustota
        /// </summary>
        public float Density
        {
            get
            {
                return density;
            }

            set
            {
                density = value;
            }
        }


        /// <summary>
        /// hmotnost
        /// </summary>
        public float Mass
        {
            get { return mass; }
        }


        /// <summary>
        /// rychlsot
        /// </summary>
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = Vector2.Zero;
            }
        }

        /// <summary>
        /// moment
        /// </summary>
        public Vector2 Momentum
        {
            get { return Vector2.Zero; }
        }

        #endregion

        /// <summary>
        /// inicializace
        /// </summary>
        /// <param name="color">barvicka</param>
        /// <param name="position">pozice</param>
        /// <param name="density">hustota</param>
        /// <param name="elasticity">elasticita</param>
        /// <param name="normal">normala zeme</param>
        public Land(Color color, Vector2 position, float density, float elasticity, Vector2 normal)
            : base(color, normal, position)
        {
            this.bound = new Box(new Vector2(float.NegativeInfinity, float.NegativeInfinity), new Vector2(float.PositiveInfinity, float.PositiveInfinity));
            this.elasticity = elasticity;
            this.position = position - RADIUS * normal;// ahaaa teziste OK
            this.density = density;
            this.mass = float.PositiveInfinity;
            this.momentOfInertia = mass;
            this.velocity = Vector2.Zero;
        }



        /// <summary>
        /// klonovaci funce
        /// </summary>
        /// <returns>vrati klona</returns>
        public IPhysicBody Clone()
        {
            var a = new Land(this.color, this.position, this.density, this.elasticity, this.Normal);
            a.player = this.player;
            return a;
        }
    }
}
