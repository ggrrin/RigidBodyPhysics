using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.Interaction.Core;
using PhysicsSimulator.Engine.MathAddition;
using System;
using System.Collections.Generic;



namespace PhysicsSimulator.Engine.Physics
{
    /// <summary>
    /// Třída realizujcí tuhé těleso tvaru konvexního mnohoúhelníku
    /// </summary>
    [Serializable]
    class ConvexBody : ConvexPolygon, IPhysicBody
    {

        private float density;
        private Vector2 velocity;
        private float mass;
        private Vector3 angularVelocity;
        private float momentOfInertia;

        private Box bound;
        private float radius;



        #region "Properties"

        private float elasticity;

        private bool player = false;
        public bool Player
        {
            get { return player; }
            set { player = value; }
        }

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
        }


        public float Density
        {
            get { return density; }
            set
            {
                density = value;

                var l = new List<Vector2>();
                foreach (var i in PolygonLinks)
                    l.Add(i.Point1);
                momentOfInertia = MomentumOfInertia(position, l.ToArray());
            }
        }

        public float Mass
        {
            get { return mass; }
        }

        public Vector2 Momentum
        {
            get { return mass * velocity; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        #endregion


        /// <summary>
        /// inicializuje novy konvexni teleso
        /// </summary>
        /// <param name="color">barva</param>
        /// <param name="position">pozice</param>
        /// <param name="density">hustota</param>
        /// <param name="elasticity">elasticita</param>
        /// <param name="velocity">rcyhlost</param>
        /// <param name="angularVelocity">uhlova rychlst</param>
        /// <param name="rotation">pocattecni rotace</param>
        /// <param name="points">seznam vrcholu podle nebo proti smeru hodinovych rucicek</param>
        public ConvexBody(Color color, Vector2 position, float density, float elasticity, Vector2 velocity, float angularVelocity, float rotation, params Vector2[] points)
            : base(true, color, points)
        {

            this.elasticity = elasticity;
            this.elasticity = 1f; //TODO: todood
            this.angularVelocity = new Vector3(0, 0, angularVelocity);
            base.position = position;
            this.velocity = velocity;
            this.density = density;

            Vector2 centerOfGravity = CalculateCenterOfGravity(points);

            //this.momentOfInertia = MomentInertiaAproximation(centerOfGravity, points);
            this.momentOfInertia = this.MomentumOfInertia(centerOfGravity, points) * 0.01f; //TODO: todoodooo


            base.Translate(position - centerOfGravity);//posunu souradnice tak, aby byli absolutni, ale fce translate posouva i position a to chcem nechat
            base.position = position;

            CalculateBoundingBox(points);
            base.Rotate(base.position, rotation);
        }

        /// <summary>
        /// vypocte bounding box telesa
        /// </summary>
        /// <param name="points"></param>
        private void CalculateBoundingBox(Vector2[] points)
        {
            radius = 0;

            foreach (PolygonLink pl in base.PolygonLinks)
                if ((pl.Point1 - position).Length() > radius)
                    radius = (pl.Point1 - position).Length();

            bound = new Box(new Vector2(position.X - radius, position.Y - radius), new Vector2(position.X + radius, position.Y + radius));
        }

        /// <summary>
        /// posune teleso o dany vektor
        /// </summary>
        /// <param name="translation">translacni vektor</param>
        public override void Translate(Vector2 translation)
        {
            base.Translate(translation);

            bound = new Box(new Vector2(position.X - radius, position.Y - radius), new Vector2(position.X + radius, position.Y + radius));
        }

        /// <summary>
        /// Vypocte teziste telesa
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private Vector2 CalculateCenterOfGravity(Vector2[] points)
        {
            Vector2 focusPoint = Vector2.Zero;
            float allMass = 0;
            float currentTriangleMass;

            for (int i = 2; i < points.Length; i++)
            {
                currentTriangleMass = MassOfTriangle(points[0], points[i - 1], points[i]);
                allMass += currentTriangleMass;
                focusPoint += currentTriangleMass * FocusPointOfTriangle(points[0], points[i - 1], points[i]);
            }

            if (float.IsInfinity(allMass))
            {
                var bakdens = density;
                density = 1;
                focusPoint = CalculateCenterOfGravity(points);
                allMass = float.MaxValue / 100;
                density = bakdens;
            }
            else
            {
                focusPoint /= allMass;
                this.mass = allMass;
            }

            return focusPoint;
        }

        /// <summary>
        /// NEPOUZIVATNE: vypocete priblizny moment setrvacnosti telesa
        /// </summary>
        /// <param name="centerOfGravity">podle teziste</param>
        /// <param name="points">a bodu monoho uhlenika</param>
        /// <returns></returns>
        private float MomentInertiaAproximation(Vector2 centerOfGravity, Vector2[] points)
        {
            float inertiaMomentum = 0;

            for (int i = 2; i < points.Length; i++)
            {
                float currentTriangleMass = MassOfTriangle(points[0], points[i - 1], points[i]);
                float distance = (centerOfGravity - FocusPointOfTriangle(points[0], points[i - 1], points[i])).Length();

                inertiaMomentum += currentTriangleMass * Helper.Pow(distance);
            }

            if (float.IsInfinity(inertiaMomentum))
                throw new Exception();

            return inertiaMomentum;
        }

        /// <summary>
        /// Vypocte moment setrvacnosti
        /// </summary>
        /// <param name="centerOfGravity">teziste</param>
        /// <param name="points">body monohouhlenika</param>
        /// <returns></returns>
        private float MomentumOfInertia(Vector2 centerOfGravity, Vector2[] points)
        {
            float polarMomentumOfInertia = 0;

            for (int i = 2; i < points.Length; i++) //for each triangle
            {
                float x1 = (points[i - 1] - points[0]).Length();
                float x2 = (points[i] - points[i - 1]).Length();
                float x3 = (points[0] - points[i]).Length();

                float max = Max(x1, x2, x3);

                float momentumOfTriangle = 0;

                if (x1 == max)
                {
                    momentumOfTriangle = InertiaOfTrianlge(points[0], points[i - 1], points[i]);
                }
                else if (x2 == max)
                {
                    momentumOfTriangle = InertiaOfTrianlge(points[i - 1], points[i], points[0]);
                }
                else if (x3 == max)
                {
                    momentumOfTriangle = InertiaOfTrianlge(points[i], points[0], points[i - 1]);
                }
                else
                    throw new Exception();

                polarMomentumOfInertia += momentumOfTriangle + MassOfTriangle(points[i], points[i - 1], points[0]) * Vector2.Dot(FocusPointOfTriangle(points[i], points[i - 1], points[0]), centerOfGravity);

            }

            return polarMomentumOfInertia;
        }


        /// <summary>
        /// Vypocte moment setrvacnosti trjuhelniku
        /// </summary>
        /// <param name="v1">prvni bod</param>
        /// <param name="v2">druhy bod</param>
        /// <param name="v3">treti bod</param>
        /// <returns></returns>
        private float InertiaOfTrianlge(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            Vector2 xAxis = Vector2.Normalize(v2 - v1);
            Vector2 yAxis = new Vector2(xAxis.Y, -xAxis.X);
            MatrixND linearTransform;
            Vector2 t1, t2, t3;

            do
            {
                yAxis *= -1;
                xAxis *= -1;
                linearTransform = new MatrixND(xAxis, yAxis);

                t1 = Helper.Transform(linearTransform, v1);
                t2 = Helper.Transform(linearTransform, v2);
                t3 = Helper.Transform(linearTransform, v3);
            }
            while (t1.Y > t3.Y);

            MatrixND inv = MatrixND.Transpose(linearTransform);

            Vector2 qwerty = new Vector2(t3.X, t1.Y);

            /*
            ///////////////////////////////////////////////
            //grid
            for (int i = 0; i < 11; i++)
            {
                vd.Add(Vector2.UnitY * i, Vector2.UnitX * 10, Color.Black);
                vd.Add(Vector2.UnitX * i, Vector2.UnitY * 10, Color.Black);
            }

            vd.Add(t1, t2 - t1, Color.Red);
            vd.Add(t2, t3 - t2, Color.Green);
            vd.Add(t3, t1 - t3, Color.Blue);

            vd.Add(v1, v2 - v1, Color.Red * 0.5f);
            vd.Add(v2, v3 - v2, Color.Green * 0.5f);
            vd.Add(v3, v1 - v3, Color.Blue * 0.5f);

   

            

            vd.Add(qwerty, 0.25f, 0.2f,  Color.Green);

            vd.Add(Helper.Transform(inv, qwerty), 0.25f, 0.2f, Color.Bisque);
            ////////////////////////////////////////////////*/

            Vector2 focusWhole = FocusPointOfTriangle(t1, t2, t3);

            float a1 = (t1 - qwerty).Length();
            float b1 = (t1 - t3).Length();

            float a2 = (t2 - qwerty).Length();
            float b2 = (t2 - t3).Length();

            float it1 = 1 / 36 * a1 * Helper.Pow(b1, 3) + MassOfTriangle(t1, qwerty, t3) * Vector2.Dot(focusWhole, FocusPointOfTriangle(t1, qwerty, t3));
            float it2 = 1 / 36 * a2 * Helper.Pow(b2, 3) + MassOfTriangle(t2, qwerty, t3) * Vector2.Dot(focusWhole, FocusPointOfTriangle(t2, qwerty, t3));

            return it1 + it2;
        }


        /// <summary>
        /// Vypocte maximmum z n hodnot
        /// </summary>
        /// <param name="val">hodnoty</param>
        /// <returns>maximum</returns>
        private float Max(params float[] val)
        {
            float max = 0;

            foreach (float f in val)
                if (f > max)
                    max = f;

            return max;
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
        /// Vypocita hmotnost trujuhelniku 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns>vysledna hmotnost</returns>
        private float MassOfTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            Vector2 u = v2 - v1;
            Vector2 v = v3 - v1;
            float height = (float)Math.Sqrt(Vector2.Dot(u, u) - MathAddition.Helper.Pow(Vector2.Dot(u, v)) / Vector2.Dot(v, v));
            return this.density * 0.5f * v.Length() * height;
        }

        /// <summary>
        /// Vypocita teziste trouhelniku
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns>teziste</returns>
        private Vector2 FocusPointOfTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return (v1 + v2 + v3) / 3;
        }



        /// <summary>
        /// naklonuje telso
        /// </summary>
        /// <returns>klon</returns>
        public IPhysicBody Clone()
        {
            var l = new List<Vector2>();
            foreach (var i in PolygonLinks)
                l.Add(i.Point1);

            var a = new ConvexBody(Color, position, density, elasticity, velocity, angularVelocity.Z, 0, l.ToArray());
            a.Player = this.player;
            return a;
        }


    }
}
