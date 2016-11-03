using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.Interaction.Core;
using PhysicsSimulator.Engine.MathAddition;
using System;

namespace PhysicsSimulator.Engine.Physics
{
    /// <summary>
    /// Třída realizujcí tuhé těleso tvaru elipsy.
    /// </summary>
    [Serializable]
    class ElipseBody : Elipse, IPhysicBody
    {
        private float density;
        private float mass;
        private Vector2 velocity;
        private Box bound;

        private Vector3 angularVelocity;

        private float momentOfInertia;

        private VectorDrawer vd;

        #region "Properties"

        private bool player = false;

        public bool Player
        {
            get { return player; }
            set { player = value; }
        }

        private float elasticity;

        public float Elasticity
        {
            get { return elasticity; }
            set { elasticity = value; }
        }


        public float MomentOfInertia
        {
            get { return momentOfInertia; }
        }

        public Vector3 AngularVelocity
        {
            get { return angularVelocity; }
            set
            {
                float res;
                res = Math.Min(Configuration.maxRotation, value.Z);
                res = Math.Max(-Configuration.maxRotation, res);

                angularVelocity = new Vector3(0, 0, res);
            }
        }

        public Box Bound
        {
            get { return bound; }
            set { bound = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Momentum
        {
            get { return mass * velocity; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }


        public float Density
        {
            get { return density; }
            set
            {
                density = value;
                momentOfInertia = CalculateMomentumInertia();
            }
        }

        #endregion

        /// <summary>
        /// inicializuje novou teleso tvaru elipsy
        /// </summary>
        /// <param name="color">brava</param>
        /// <param name="position">pozice</param>
        /// <param name="density">hustota</param>
        /// <param name="elasticity">elasticita</param>
        /// <param name="velocity">rychlost</param>
        /// <param name="angularVelocity">uhlova rychlsot</param>
        /// <param name="a">hlavni poloosa</param>
        /// <param name="b">vedlejsi poloosa</param>
        /// <param name="rotation">rotace</param>
        public ElipseBody(Color color, Vector2 position, float density, float elasticity, Vector2 velocity, float angularVelocity, float a, float b, float rotation)
            : base(color, position, a, b, rotation)
        {
            vd = new VectorDrawer();
            this.elasticity = 1;// elasticity; //TODO: todoooodoodo elasticity
            this.angularVelocity = new Vector3(0, 0, angularVelocity);

            this.velocity = velocity;
            this.density = density;
            this.mass = density * (float)Math.PI * a * b;
            this.momentOfInertia = CalculateMomentumInertia();
            float q = Math.Max(base.a, base.b);
            bound = new Box(new Vector2(Position.X - q, Position.Y - q), new Vector2(Position.X + q, Position.Y + q));
        }

        /// <summary>
        /// vypocita moment setrvacnosti telesa
        /// </summary>
        /// <returns>moment setrvacnosti</returns>
        private float CalculateMomentumInertia()
        {
            return 0.25f * density * (float)Math.PI * a * b * (Helper.Pow(a) + Helper.Pow(b)); // source : http://math.stackexchange.com/questions/152277/moment-of-inertia-of-an-ellipse-in-2d
        }

        /// <summary>
        /// posune teleso o dany vektor
        /// </summary>
        /// <param name="translation">vektor posunuti</param>
        public override void Translate(Vector2 translation)
        {

            base.Translate(translation);

            float q = Math.Max(base.a, base.b);
            bound = new Box(new Vector2(Position.X - q, Position.Y - q), new Vector2(Position.X + q, Position.Y + q));

            vd.Clear();
            //vd.Add(this.position, 0.3f, 0.5f, Color.Red);
        }

        /// <summary>
        /// Vola presnensi vypocet kolize pokud koliduji bounding boxy
        /// </summary>
        /// <param name="obj">s cim pocitat kolize</param>
        /// <param name="intersec1">vystupni bod kolize</param>
        /// <param name="intersec2">vystupni bod kolize</param>
        /// <param name="cs">status kolize</param>
        /// <returns>vrati zda koliduji</returns>
        public override bool Intersects(IInteractable obj, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            if (this.bound.Intersects(((IPhysicBody)obj).Bound))
                return base.Intersects(obj, out intersec1, out intersec2, out cs);
            else
            {
                intersec1 = Vector2.Zero;
                intersec2 = Vector2.Zero;
                cs = CrossState.DontIntersecting;

                return false;
            }
        }


        /// <summary>
        /// naklonuje telso
        /// </summary>
        /// <returns>klon</returns>
        public IPhysicBody Clone()
        {
            var aa = new ElipseBody(Color, Position, density, elasticity, velocity, angularVelocity.Z, a, b, base.rotation);
            aa.player = this.player;
            return aa;
        }
    }
}
