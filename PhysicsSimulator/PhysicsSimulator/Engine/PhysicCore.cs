using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.Engine.Game;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.MathAddition;
using PhysicsSimulator.Engine.Physics;
using System;
using System.Collections.Generic;

namespace PhysicsSimulator.Engine
{
    /// <summary>
    /// Třída realizující odchytávní událostí eventBoxů a simulaci interakcí Tuhých těles. 
    /// </summary>
    class PhysicCore
    {
        //canvas scale & position

        /// <summary>
        /// vychozi scale platna
        /// </summary>
        protected float unitOnWidth = 20;

        /// <summary>
        /// vychozi pozice kamery
        /// </summary>
        protected Vector2 cameraPosition = Vector2.Zero;

        //float initialMomentum;


        /// <summary>
        /// seznam teles
        /// </summary>
        protected List<IPhysicBody> bodies;


        /// <summary>
        /// seznam event boxu
        /// </summary>
        protected List<EventBox> eventBoxes;

        /// <summary>
        /// drawer pro kresleni vektoru bodu
        /// </summary>
        private VectorDrawer vd;

        /// <summary>
        /// spomalovaci faktor teles
        /// </summary>
        protected float generalSlowFactor = 0.0f;

        /// <summary>
        /// spomalovaci faktor rotace teles
        /// </summary>
        protected float generalRotationSlowFactor = 0.005f;


        /// <summary>
        /// inicializace  
        /// </summary>
        public PhysicCore()
        {
            vd = new VectorDrawer();

            bodies = new List<IPhysicBody>();
            eventBoxes = new List<EventBox>();

            //InitializeSMT();
            //InitializerConverx();
            //InitializeElipse();
            //InitializeStrangePool();
            //InitializeCounterConverx();            
            //InitializeBasicSphereTest();
            //InitializePool();
            //InitializeLand();

            //this.initialMomentum = AllEnergy(bodies);
            //Console.WriteLine("RED: {0} , YELLOW: {2} , GREEN: {1}", bodies[0].Velocity.Length(), bodies[1].Velocity.Length(), bodies[2].Velocity.Length());        
        }


        /// <summary>
        /// herni cas
        /// </summary>
        GameTime time;


        /// <summary>
        /// vypocet pohybu teles a ostatni aktualizace dat
        /// </summary>
        /// <param name="time"> herni cas</param>
        public virtual void Update(GameTime time)
        {

            vd.Clear();

            UpdateEventBoxAffection();

            this.time = time;
            const float FACTOR = 1f;

            Vector2 intersec1, intersec2;
            CrossState cs;

            foreach (IPhysicBody a in bodies)
            {
                //zpomalovani
                if (a.Velocity != Vector2.Zero)
                {
                    float scalarVelocity = a.Velocity.Length();

                    scalarVelocity -= generalSlowFactor;
                    if (scalarVelocity < 0) scalarVelocity = 0;
                    a.Velocity = Vector2.Normalize(a.Velocity) * scalarVelocity;
                    //zpomalovani END
                }

                //zpoalovani rotace
                float alfa = a.AngularVelocity.Z;
                if (alfa != 0)
                {
                    float sign = alfa / -Math.Abs(alfa);



                    if (Math.Abs(alfa) < generalRotationSlowFactor)
                    {
                        a.AngularVelocity = Vector3.Zero;
                    }
                    else
                    {
                        a.AngularVelocity = new Vector3(0, 0, alfa + sign * generalRotationSlowFactor);
                    }

                }
                //zpoalovani rotace END

                ///////////////////////////////////////////////////////////////////////////////////


                Vector2 translation = a.Velocity * FACTOR * ((float)time.ElapsedGameTime.Milliseconds / 1000);
                float rotation = a.AngularVelocity.Z * FACTOR * ((float)time.ElapsedGameTime.Milliseconds / 1000);


                a.Translate(translation);
                a.Rotate(a.Position, rotation);

                foreach (IPhysicBody b in bodies)
                    if (a != b)
                        if (a.Intersects(b, out intersec1, out intersec2, out cs))
                        {
                            if (float.IsNaN(intersec1.X) || float.IsNaN(intersec1.Y) || float.IsNaN(intersec2.X) || float.IsNaN(intersec2.Y))
                                throw new Exception();

                            a.Translate(-translation);
                            a.Rotate(a.Position, -rotation);


                            if (intersec1 == intersec2)
                            {
                                Vector2 focusLink = a.Position - b.Position;
                                focusLink.Normalize();
                                intersec2 += new Vector2(focusLink.Y, -focusLink.X) * 0.01f;
                            }

                            Vector2 normal = Normal(a, b, intersec1, intersec2);


                            AdvanceCollision2(a, b, (intersec1 + intersec2) / 2, normal);

                        }
            }
        }

        /// <summary>
        /// Aktualizace pozice platna
        /// </summary>
        protected virtual void UpdateCanvasPositionLocation()
        {
            Configuration.effect.View = Matrix.CreateLookAt(new Vector3(cameraPosition, 1), new Vector3(cameraPosition, 0), Vector3.Up);
            Configuration.effect.Projection = Matrix.CreateOrthographic(unitOnWidth, unitOnWidth * (float)Configuration.height / (float)Configuration.width, -1000, 1000);
        }

        /// <summary>
        /// odchyt udalosti evenboxu a zpusobi ovlivneni teles
        /// </summary>
        private void UpdateEventBoxAffection()
        {
            foreach (var i in bodies)
            {
                foreach (var j in eventBoxes)
                {
                    if (j.Intersects(i.Bound))
                        j.AffectBody(i);
                }
            }
        }

        /// <summary>
        /// vyppocte pozici mysi na mape
        /// </summary>
        /// <returns>Vektor pozice mysi na mape</returns>
        protected Vector2 MousePosition()
        {
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2((float)mouse.X, -(float)mouse.Y);
            mousePos -= new Vector2((float)Configuration.width / 2, -(float)Configuration.height / 2);//mouse coordinates of center of screen with same basis as game
            mousePos *= unitOnWidth / (float)Configuration.width;
            mousePos += cameraPosition;
            return mousePos;
        }

        /// <summary>
        /// Vypocet kolizi
        /// </summary>
        /// <param name="a">prvni teleso</param>
        /// <param name="b">druhe teleso</param>
        /// <param name="intersection">pod ve kterem se telesa srazilla</param>
        /// <param name="normalOfB">normala srazky</param>
        private void AdvanceCollision2(IPhysicBody a, IPhysicBody b, Vector2 intersection, Vector2 normalOfB)
        {

            vd.Add(intersection, normalOfB, Color.SteelBlue);
            vd.Add(intersection, 0.3f, 0.3f, Color.Orange);


            float elasticity = a.Elasticity * b.Elasticity;
            Vector2 collisionPoint = intersection;

            Vector2 AToP = collisionPoint - a.Position;
            Vector2 BToP = collisionPoint - b.Position;

            //velocity of collision point
            Vector3 angularVelocityPonA = Vector3.Cross(a.AngularVelocity, new Vector3(AToP, 0));
            Vector3 angularVelocityPonB = Vector3.Cross(b.AngularVelocity, new Vector3(BToP, 0));
            Vector2 velocityPonA = a.Velocity + new Vector2(angularVelocityPonA.X, angularVelocityPonA.Y);
            Vector2 velocityPonB = b.Velocity + new Vector2(angularVelocityPonB.X, angularVelocityPonB.Y);

            //relative velocities
            Vector2 perColRelativeVelocity = velocityPonA - velocityPonB;

            float uuu = Cross(AToP, normalOfB).Z;
            float vvv = Cross(BToP, normalOfB).Z;

            float impulse = -(1 + elasticity) * Vector2.Dot(perColRelativeVelocity, normalOfB) /
                (1 / a.Mass + 1 / b.Mass + Helper.Pow(uuu) / a.MomentOfInertia + Helper.Pow(vvv) / b.MomentOfInertia);

            //vd.Add(a.Position, a.Velocity, Color.Black);

            //translation
            a.Velocity += impulse / a.Mass * normalOfB;
            b.Velocity -= impulse / b.Mass * normalOfB;

            //vd.Add(a.Position, a.Velocity, Color.SpringGreen);

            //rotation
            a.AngularVelocity += Cross(AToP, impulse * normalOfB) / a.MomentOfInertia;
            b.AngularVelocity -= Cross(BToP, impulse * normalOfB) / b.MomentOfInertia;

            //Vector2 translation = a.Velocity   * ((float)time.ElapsedGameTime.Milliseconds / 1000);
            //float rotation = a.AngularVelocity.Z * ((float)time.ElapsedGameTime.Milliseconds / 1000);

            //a.Translate(translation*0.01f);
            //a.Rotate(a.Position, rotation * 0.001f);


        }

        /// <summary>
        /// vypocte pribliznou normalu dopadu
        /// </summary>
        /// <param name="a">prvni teleso</param>
        /// <param name="b">druhy teleso</param>
        /// <param name="intersec1">prunik teles</param>
        /// <param name="intersec2">prunik teles</param>
        /// <returns></returns>
        private Vector2 Normal(IPhysicBody a, IPhysicBody b, Vector2 intersec1, Vector2 intersec2)
        {
            Vector2 directive = intersec1 - intersec2;
            Vector2 normal = new Vector2(-directive.Y, directive.X);

            Vector2 axis = b.Position - a.Position;
            Vector2 normXX = Helper.Transform(new MatrixND(axis, new Vector2(-axis.Y, axis.X)), normal);
            if (normXX.X < 0)
                normal *= -1;

            if (float.IsNaN(normal.X) || float.IsNaN(normal.Y))
                throw new Exception();

            return Vector2.Normalize(normal);
        }


        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializeSMT()
        {
            bodies.Add(new ConvexBody(Color.Green, new Vector2(5, 0), 1, 1, new Vector2(1, -2), 0f, MathHelper.PiOver4, new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1)));
            bodies.Add(new ConvexBody(Color.Yellow, new Vector2(7f, 0), 2, 1, -3 * new Vector2(1, -0.25f), 0, 0f, new Vector2(-1, 2),/* new Vector2(0, 2.5f),*/ new Vector2(-0.7f, 2), new Vector2(-0.7f, -1), new Vector2(-1, -1)));
        }


        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializeElipse()
        {
            bodies.Add(new ElipseBody(Color.Red, new Vector2(-5, 3), 1200f, 1, 1.8f * new Vector2(1, 0), 0, 2, 5, 0));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(3, -3f), 1000f, 1, new Vector2(-1, 0), 0, 1, 3, 0));
            //bodies.Add(new ElipseBody(Color.Yellow, new Vector2(13f, 0f), 2000, -8 * new Vector2(1, 0), 0, 2, 2, 0));
        }

        /// <summary>
        /// Vypocte cros produk vektoru
        /// </summary>
        /// <param name="v1">prvni vektor</param>
        /// <param name="v2">druhy vektor</param>
        /// <returns>vysledny cross product</returns>
        private Vector3 Cross(Vector2 v1, Vector2 v2)
        {
            return Vector3.Cross(new Vector3(v1, 0), new Vector3(v2, 0));
        }


        /// <summary>
        /// NEPOUZIVATNE : funguje jen castecne v urcitych pripadech
        /// </summary>

        private void AdvanceCollision1(IPhysicBody a, IPhysicBody b, Vector2 collisionPoint)
        {
            //na velikosti tohodle v nasledujicim nezalezi a +/- pro momentum /pozor na oppositevectors tam jo
            Vector2 AToCollisionLink = collisionPoint - a.Position;
            Vector2 BToCollisionLink = collisionPoint - b.Position;

            //momentums affect collision point
            Vector2 momentumAB = AToCollisionLink * Vector2.Dot(a.Momentum, AToCollisionLink) / Vector2.Dot(AToCollisionLink, AToCollisionLink);
            Vector2 momentumBA = BToCollisionLink * Vector2.Dot(b.Momentum, BToCollisionLink) / Vector2.Dot(BToCollisionLink, BToCollisionLink);

            //AtoB
            Vector2 momentumABTranslation = -BToCollisionLink * Vector2.Dot(momentumAB, -BToCollisionLink) / Vector2.Dot(-BToCollisionLink, -BToCollisionLink);
            Vector2 momentumABRotation = momentumAB - momentumABTranslation;

            //BtoA
            Vector2 momentumBATranslation = -AToCollisionLink * Vector2.Dot(momentumBA, -AToCollisionLink) / Vector2.Dot(-AToCollisionLink, -AToCollisionLink);
            Vector2 momentumBARotation = momentumBA - momentumBATranslation;


            //vd.Clear();
            //vd.Add(a.Position, AToCollisionLink, Color.Pink);

            //vd.Add(collisionPoint, momentumAB, Color.Red); // rozklad na :
            //vd.Add(collisionPoint, momentumABTranslation, Color.Red); // translaci
            //vd.Add(collisionPoint, momentumABRotation, Color.Blue); // rotaci

            //vd.Add(b.Position, b.Momentum, Color.Pink); // puvodni moment
            //vd.Add(b.Position, b.Momentum - momentumBA, Color.Green); // zbytek puvodniho memntu
            //vd.Add(b.Position, momentumABTranslation + (b.Momentum - momentumBA), Color.Black); // moment predany Acku

            //vd.Add(b.Position, BToCollisionLink, Color.Black);

            //vd.Add(collisionPoint, momentumBA, Color.Black); // rozklad na :
            //vd.Add(collisionPoint, momentumBATranslation, Color.White); // translace
            //vd.Add(collisionPoint, momentumBARotation, Color.Green); //rotace

            //vd.Add(a.Position, a.Momentum, Color.Pink); // puvodni moment
            //vd.Add(a.Position, a.Momentum - momentumAB, Color.Brown); // zbytek puvodniho momentu
            //vd.Add(a.Position, momentumBATranslation + (a.Momentum - momentumAB), Color.Purple); // moment predany Bcku



            bool aToB = !OppositeVectors(AToCollisionLink, momentumABTranslation);
            bool bToA = !OppositeVectors(BToCollisionLink, momentumBATranslation);

            if (aToB && bToA)
            {
                // Translation
                a.Velocity = (momentumBATranslation + a.Momentum - momentumAB) / a.Mass;
                b.Velocity = (momentumABTranslation + b.Momentum - momentumBA) / b.Mass;

                //Rotation
                a.AngularVelocity += (float)Math.Sqrt((2 * momentumBARotation.Length()) / (a.MomentOfInertia * a.Mass)) * Vector3.UnitZ;
                b.AngularVelocity += (float)Math.Sqrt((2 * momentumABRotation.Length()) / (b.MomentOfInertia * b.Mass)) * Vector3.UnitZ;

            }
            else if (!aToB && bToA)
            {
                a.Velocity += momentumBATranslation / a.Mass;
                b.Velocity -= momentumBA / b.Mass;
            }
            else if (aToB && !bToA)
            {
                a.Velocity -= momentumAB / a.Mass;
                b.Velocity += momentumABTranslation / b.Mass;
            }




        }

        #region "MESSSS"

        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializeLand()
        {
            bodies.Add(new Land(Color.DarkOliveGreen, new Vector2(16, 0), 2000f, 1, -Vector2.UnitX + Vector2.UnitY));
            bodies.Add(new Land(Color.DarkOliveGreen, new Vector2(16, 0), 2000f, 1, -Vector2.UnitX - Vector2.UnitY));
            bodies.Add(new Land(Color.DarkOliveGreen, new Vector2(-18, 0), 2000f, 1, Vector2.UnitX + Vector2.UnitY));
            bodies.Add(new Land(Color.DarkOliveGreen, new Vector2(-18, 0), 2000f, 1, Vector2.UnitX - Vector2.UnitY));
        }


        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializerConverx()
        {
            bodies.Add(new ConvexBody(Color.Red, new Vector2(-6, 0), 1, 1, new Vector2(1, -2), 1f, 0f, new Vector2(-1, 2), new Vector2(1, 2), new Vector2(1, -1), new Vector2(-1, -1)));
            bodies.Add(new ConvexBody(Color.Green, new Vector2(5, 0), 1, 1, new Vector2(1, -2), -0.2f, 0.3f, new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1)));
            bodies.Add(new ConvexBody(Color.Teal, new Vector2(0, 0), 40, 1, new Vector2(1, 0), 0.5f, 0,
                new Vector2(-4, 0),
                new Vector2(-3, 1),
                new Vector2(-2, 1.5F),
                new Vector2(2, 1.5f),
                new Vector2(3, 1),
                new Vector2(4, 0),
                new Vector2(3, -1),
                new Vector2(2, -1.5f),
                new Vector2(0, -2),
                new Vector2(-2, -1.5f),
                new Vector2(-3, -1)

                ));
            bodies.Add(new ConvexBody(Color.Yellow, new Vector2(7f, 0), 1, 1, -3 * new Vector2(1, -0.25f), -0.5f, 0f, new Vector2(-1, 2),/* new Vector2(0, 2.5f),*/ new Vector2(-0.7f, 2), new Vector2(-0.7f, -1), new Vector2(-1, -1)));
        }


        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializeCounterConverx()
        {
            bodies.Add(new ConvexBody(Color.Red, new Vector2(-5, 0), 1, 1, new Vector2(1, -2), 0, 0f, new Vector2(-1, 2), new Vector2(1, 2), new Vector2(1, -1), new Vector2(-1, -1)));
            bodies.Add(new ConvexBody(Color.Green, new Vector2(5, 0), 2, 1, new Vector2(1, -2), 0f, 0.3f, new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1)));
            bodies.Add(new ConvexBody(Color.Yellow, new Vector2(15f, 0), 0.1f, 1, -20 * new Vector2(1, 0), 0, 0f, new Vector2(-1, 2), new Vector2(1, 2), new Vector2(1, -1), new Vector2(-1, -1)));
        }

        /// <summary>
        /// NEPOUZIVATNE : funguje jen pro kulove opekty .. pracuje s hybnosti
        /// </summary>
        private void BasicCollision2(IPhysicBody a, IPhysicBody b)
        {
            Vector2 momentum1 = a.Momentum + b.Momentum;

            Vector2 focusLink = b.Position - a.Position; //na velikosti tohodle v nasledujicim nezalezi a +/- pro momentum /pozor na oppositevectors
            if (focusLink == Vector2.Zero)
                throw new Exception("");


            Vector2 momentumAB = focusLink * Vector2.Dot(a.Momentum, focusLink) / Vector2.Dot(focusLink, focusLink);
            Vector2 momentumBA = focusLink * Vector2.Dot(b.Momentum, focusLink) / Vector2.Dot(focusLink, focusLink);

            bool aToB = !OppositeVectors(focusLink, momentumAB);
            bool bToA = !OppositeVectors(-focusLink, momentumBA);

            if (aToB && bToA)
            {
                a.Velocity = (momentumBA + a.Momentum - momentumAB) / a.Mass;
                b.Velocity = (momentumAB + b.Momentum - momentumBA) / b.Mass;
            }
            else if (!aToB && bToA)
            {
                a.Velocity += momentumBA / a.Mass;
                b.Velocity -= momentumBA / b.Mass;
            }
            else if (aToB && !bToA)
            {
                a.Velocity -= momentumAB / a.Mass;
                b.Velocity += momentumAB / b.Mass;
            }
            else
            {
                //throw new Exception();
            }



            Vector2 momentum2 = a.Momentum + b.Momentum;


            //Console.WriteLine(momentum1 - momentum2);            

            /*if (a is Land && !(b is Land))
                b.Velocity += -(momentum1 - momentum2) / b.Mass;
            else if (b is Land && !(a is Land))
                a.Velocity += -(momentum1 - momentum2)/ a.Mass;*/

        }

        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializeBasicSphereTest()
        {
            bodies.Add(new ElipseBody(Color.Red, new Vector2(-5, 0), 1000f, 1, 1.3f * new Vector2(0, -5), 0, 2, 2, 0));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(3, 0f), 20000f, 1, new Vector2(-1, 2), 0, 1, 1, 0));
            bodies.Add(new ElipseBody(Color.Yellow, new Vector2(13f, 0f), 2000, 1, -5 * new Vector2(1, 0), 0, 2, 2, 0));
        }

        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializeStrangePool()
        {
            bodies.Add(new ElipseBody(Color.Yellow, new Vector2(-13f, 1f), 2, 1, 20 * new Vector2(1, 0), 0, 2, 2.3f, 0));
            bodies.Add(new ElipseBody(Color.Yellow, new Vector2(0f, -5f), 1.5f, 1, 4 * new Vector2(0, 1), 0, 1.5f, 2, 0));

            bodies.Add(new ElipseBody(Color.Green, new Vector2(-0.1f, -0.1f), 1, 1, Vector2.Zero, 0, 0.7f, 1.5f, -0.3f));

            bodies.Add(new ElipseBody(Color.Green, new Vector2(2, 4.2f), 1, 1, Vector2.Zero, 1f, 1.0f, 1.5f, -0.8f));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(2, 1.5f), 1, 1, Vector2.Zero, 0, 0.7f, 1.6f, 0.7f));

            bodies.Add(new ElipseBody(Color.Green, new Vector2(4, -2.2f), 1, 1, Vector2.Zero, 0, 0.5f, 1f, 0));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(3.8f, 0f), 1, 1, Vector2.Zero, 0, 0.3f, 1.1f, 0));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(4, 3.1f), 1, 1, Vector2.Zero, 0, 1, 1, 0));
        }

        /// <summary>
        /// NEPOUZIVATNE : inicializace testovacich dat
        /// </summary>
        private void InitializePool()
        {
            bodies.Add(new ElipseBody(Color.Yellow, new Vector2(-13f, 0f), 20000, 1, 20 * new Vector2(1, 0), 0, 2, 2, 0));
            bodies.Add(new ElipseBody(Color.Yellow, new Vector2(2f, -5f), 20000, 1, 4 * new Vector2(0, 1), 0, 2, 2, 0));

            bodies.Add(new ElipseBody(Color.Green, new Vector2(0, 0), 20000f, 1, Vector2.Zero, 0, 1, 1, 0));

            bodies.Add(new ElipseBody(Color.Green, new Vector2(2, -1.1f), 20000f, 1, Vector2.Zero, 0, 1, 1, 0));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(2, 1.1f), 20000f, 1, Vector2.Zero, 0, 1, 1, 0));

            bodies.Add(new ElipseBody(Color.Green, new Vector2(4, -2.1f), 25000f, 1, Vector2.Zero, 0, 1, 1, 0));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(3.8f, 0f), 20000f, 1, Vector2.Zero, 0, 1, 1, 0));
            bodies.Add(new ElipseBody(Color.Green, new Vector2(4, 2.1f), 20000f, 1, Vector2.Zero, 0, 1, 1, 0));
        }


        /// <summary>
        /// Urci zda jsou vektory proti sobe nebo ne
        /// </summary>
        /// <param name="v1">prvni vektor</param>
        /// <param name="v2">druhy vektor</param>
        /// <returns> true pokud jsou proti sobe jinak false</returns>
        private bool OppositeVectors(Vector2 v1, Vector2 v2)
        {
            float alpha1 = -v2.X / v1.X;
            float alpha2 = -v2.Y / v1.Y;

            return alpha1 >= 0 && alpha2 >= 0;
        }

        /// <summary>
        /// NEPOUZIVATNE : funguje pouze pro kulove objekty nepocitas hmotnostmi teles
        /// </summary>
        private void BasicCollision1(IPhysicBody b, IPhysicBody a)
        {
            Vector2 focusLink = b.Position - a.Position;

            Vector2 velocityAB = focusLink * Vector2.Dot(a.Velocity, focusLink) * 1 / Vector2.Dot(focusLink, focusLink);
            Vector2 velocityBA = focusLink * Vector2.Dot(b.Velocity, focusLink) * 1 / Vector2.Dot(focusLink, focusLink);


            bool aToB = !OppositeVectors(focusLink, velocityAB);
            bool bToA = !OppositeVectors(-focusLink, velocityBA);

            if (aToB && bToA)
            {
                a.Velocity = (velocityBA + a.Velocity - velocityAB);
                b.Velocity = (velocityAB + b.Velocity - velocityBA);
            }
            else if (!aToB && bToA)
            {
                a.Velocity += velocityBA;
                b.Velocity -= velocityBA;
            }
            else if (aToB && !bToA)
            {
                a.Velocity -= velocityAB;
                b.Velocity += velocityAB;
            }
            else
            {
                new Exception();
            }



        }

        /// <summary>
        /// NEPOUZIVATNE : pro testovaci ucely spocte celkovou energii soustavy
        /// </summary>
        private float AllEnergy(IEnumerable<IPhysicBody> items)
        {
            float sum = 0;
            foreach (IPhysicBody i in items)
            {
                sum += 0.5f * i.Mass * Helper.Pow(i.Velocity.Length());
                sum += 0.5f * Helper.Pow(i.AngularVelocity.Z) * i.MomentOfInertia;
            }

            return sum;
        }

        #endregion

        /// <summary>
        /// vykresli vsechy herni objekty
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        public virtual void Draw(SpriteBatch sprite, GameTime time)
        {
            foreach (IPhysicBody b in bodies)
                b.Draw();

            foreach (var i in eventBoxes)
                i.Draw();

        }

    }
}
