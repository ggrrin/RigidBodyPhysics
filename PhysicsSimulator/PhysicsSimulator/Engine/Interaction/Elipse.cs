using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.Engine.Interaction.Core;
using PhysicsSimulator.Engine.MathAddition;
using System;

namespace PhysicsSimulator.Engine.Interaction
{

    /// <summary>
    /// Třída realizujcí elipsy a jejich kolize s ostatními IInteractable objekty.
    /// </summary>
    [Serializable]
    class Elipse : IInteractable
    {
        #region "fields"

        /// <summary>
        /// samlink elipsy aproximace pro vykresleni
        /// </summary>
        private const int SAMPLING = 20;

        /// <summary>
        /// rotace elipsy
        /// </summary>
        protected float rotation;

        /// <summary>
        /// hlavni poloosa
        /// </summary>
        protected float a;

        /// <summary>
        /// vedlejsi polosa
        /// </summary>
        protected float b;

        /// <summary>
        /// POZOR!! pozice elipsi v jejich zvlastnich souradnicich, prave souradnice vi VLASTNOST Position
        /// </summary>
        protected Vector2 position;

        /// <summary>
        /// aproximace jedne z elipsi polygonem (pro interakci dvou elips)
        /// </summary>
        private ConvexPolygon polygonAproximation;
        private Vector2[] aproximatePoints;
        private bool needUpdate;

        /// <summary>
        /// pozice elipsy v normalnich souradnicic
        /// </summary>
        public Vector2 Position
        {
            get { return this.ToBaseCoor(this.position); }
            protected set { position = ToElipseCoor(value); }
        }

        /// <summary>
        /// Vertex buffer pro vykresleni
        /// </summary>
        private VertexPositionColor[] vertices;

        /// <summary>
        /// Barva vykreslovanychcar
        /// </summary>
        private Color color;


        private VectorDrawer vd;

        #endregion

        #region Properties

        public Vector2 APoint
        {
            get { return ToBaseCoor(position - a * Vector2.UnitX); }
        }
        public Vector2 BPoint
        {
            get { return ToBaseCoor(position + b * Vector2.UnitY); }
        }
        public Vector2 CPoint
        {
            get { return ToBaseCoor(position + a * Vector2.UnitX); }
        }
        public Vector2 DPoint
        {
            get { return ToBaseCoor(position - b * Vector2.UnitY); }
        }
        public Color @Color
        {
            get { return color; }
            set
            {
                color = value;
                UpdateVertices(SAMPLING);
            }
        }


        #endregion

        #region "Constructors"

        public Elipse() { throw new Exception("Use different constructor, this one is forbiden."); }

        /// <summary>
        /// Creates elipse!
        /// </summary>
        /// <param name="color">Color to draw it up.</param>
        /// <param name="position">Position of center of elipse.</param>
        /// <param name="a">Main halfaxis</param>
        /// <param name="b">Beside halfaxis</param>
        /// <param name="rotation">Rotation of elipse (center-counter clockwise)</param>
        public Elipse(Color color, Vector2 position, float a, float b, float rotation)
        {

            vd = new VectorDrawer();
            this.color = color;
            this.rotation = rotation % (2 * (float)Math.PI);// musí být PRVNÍ protože ToElipse používá this.rotation
            this.position = ToElipseCoor(position);
            this.a = a;
            this.b = b;

            this.needUpdate = true;

            UpdateVertices(SAMPLING);


        }

        #endregion

        public override string ToString()
        {
            return APoint.ToString() + BPoint.ToString() + CPoint.ToString() + DPoint.ToString();
        }

        #region "Update"

        /// <summary>
        /// Translate Elipse by specific translation.
        /// </summary>
        /// <param name="translation">Translation vector.</param>
        public virtual void Translate(Vector2 translation)
        {
            if (translation.Length() != 0)
            {
                this.needUpdate = true;
                this.position = ToBaseCoor(position);
                this.position += translation;
                this.position = ToElipseCoor(position);

                this.UpdateVertices(SAMPLING);
            }
        }

        /// <summary>
        /// Rotate Elipse by specific angle due to specific orign.
        /// </summary>
        /// <param name="orign">Orign of rotationg.</param>
        /// <param name="angle">Angle of rotationg.</param>
        public virtual void Rotate(Vector2 orign, float angle)
        {
            if (angle % (2 * (float)Math.PI) != 0)
            {
                this.needUpdate = true;
                this.position = ToBaseCoor(position);
                this.position -= orign;
                this.position = Helper.Transform(Helper.CreateRotationMatrix(angle), position);
                this.position += orign;

                this.rotation += angle;
                this.rotation %= (2 * (float)Math.PI);
                this.position = ToElipseCoor(position);

                this.UpdateVertices(SAMPLING);
            }

            vd.Clear();
            //vd.Add(this.Position, 0.3f, 0.1f, Color.Tomato);
        }

        #region "UpdateGraphics"

        /// <summary>
        /// Update vertices for drawing & update points for polygon aproximation.
        /// </summary>
        /// <param name="sampling">Sampling of elipse.</param>
        private void UpdateVertices(int sampling)
        {
            VertexPositionColor[] vertField = new VertexPositionColor[2 * sampling + 3];
            Vector2[] points = new Vector2[2 * sampling + 2];

            if (this.a < b)
            {
                points[0] = ToBaseCoor(position - Vector2.UnitX * a);
                vertField[0] = new VertexPositionColor(new Vector3(points[0], 0), this.color);//vrchol A
                vertField[vertField.Length - 1] = vertField[0];// zopakovanej prni vrhol pro dokonceni

                points[(vertField.Length - 1) / 2] = ToBaseCoor(position + Vector2.UnitX * a);
                vertField[(vertField.Length - 1) / 2] = new VertexPositionColor(new Vector3(points[(vertField.Length - 1) / 2], 0), this.color);//vrchol B


                float gap = 2 * a / (sampling + 1);//velikost mezer

                for (int i = 1; i < (vertField.Length - 1) / 2; i++)
                {
                    float x = (position.X - a)/*bod A*/ + gap * i; // o i posunutí a máme bod

                    points[i] = ToBaseCoor(new Vector2(x, GetY1Y2(x).X));
                    vertField[i] = new VertexPositionColor(new Vector3(points[i], 0), color);

                    int index = vertField.Length - 1 - i;
                    points[index] = ToBaseCoor(new Vector2(x, GetY1Y2(x).Y));
                    vertField[index] = new VertexPositionColor(new Vector3(points[index], 0), color);
                }
            }
            else
            {
                points[0] = ToBaseCoor(position - Vector2.UnitY * b);
                vertField[0] = new VertexPositionColor(new Vector3(points[0], 0), this.color);//vrchol A
                vertField[vertField.Length - 1] = vertField[0];// zopakovanej prni vrhol pro dokonceni

                points[(vertField.Length - 1) / 2] = ToBaseCoor(position + Vector2.UnitY * b);
                vertField[(vertField.Length - 1) / 2] = new VertexPositionColor(new Vector3(points[(vertField.Length - 1) / 2], 0), this.color);//vrchol B


                float gap = 2 * b / (sampling + 1);//velikost mezer

                for (int i = 1; i < (vertField.Length - 1) / 2; i++)
                {
                    float y = (position.Y - b)/*bod C*/ + gap * i; // o i posunutí a máme bod

                    points[i] = ToBaseCoor(new Vector2(GetX1X2(y).X, y));
                    vertField[i] = new VertexPositionColor(new Vector3(points[i], 0), color);

                    int index = vertField.Length - 1 - i;
                    points[index] = ToBaseCoor(new Vector2(GetX1X2(y).Y, y));
                    vertField[index] = new VertexPositionColor(new Vector3(points[index], 0), color);
                }
            }

            this.aproximatePoints = points;
            this.vertices = vertField;
        }

        /// <summary>
        /// Draw elipse
        /// </summary>
        public virtual void Draw()
        {
            BasicEffect effect = Configuration.effect;
            GraphicsDevice device = Configuration.device;

            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            effect.LightingEnabled = false;
            effect.DirectionalLight0.Enabled = false;

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
            }

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;


            vd.Draw();
        }

        #endregion

        #endregion

        #region "Intersection"

        #region "just bool"

        public virtual bool Intersects(IInteractable obj, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            return obj.Intersects(this, out intersec1, out intersec2, out cs);
        }

        /// <summary>
        /// Return wheather line intersects with this Elipse.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        public bool Intersects(Line line)
        {
            Vector2 v1, v2;
            CrossState cs;
            return Intersects(line, out  v1, out  v2, out cs);
        }

        /// <summary>
        /// Return wheather Elipse intersects with this halfSurface.
        /// </summary>
        /// <param name="halfSurface">halfSurface to test intersection.</param>
        public bool Intersects(HalfSurface halfSurface)
        {
            Vector2 g; CrossState cs;
            return this.Intersects(halfSurface, out g, out g, out cs);
        }

        /// <summary>
        /// Return wheather Elipse intersects with this segment.
        /// </summary>
        /// <param name="segment">segment to test intersection.</param>
        public bool Intersects(Segment segment)
        {
            Vector2 g; CrossState cs;
            return this.Intersects(segment, out g, out g, out cs);
        }

        /// <summary>
        /// Return wheather elipse intersects with this Elipse.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        public bool Intersects(Elipse elipse)
        {
            Vector2 g; CrossState cs;
            return this.Intersects(elipse, out g, out g, out cs);
        }

        /// <summary>
        /// Return wheather polygon intersects with this Elipse.
        /// </summary>
        /// <param name="elipse">Polygon to test intersection.</param>
        public bool Intersects(ConvexPolygon polygon)
        {
            Vector2 g; CrossState cs;
            return this.Intersects(polygon, out g, out g, out cs);
        }

        #endregion

        /// <summary>
        /// Return wheather point intersects with Elipse.
        /// </summary>
        /// <param name="point">Point to test intersection.</param>
        /// <returns>Returns true, when point lies on the line.</returns>
        public bool Intersects(Vector2 point)
        {
            point = ToElipseCoor(point);
            if ((float)Math.Abs(point.X - this.position.X) > a)
                return false;
            else
                return Helper.IsInInterval(GetY1Y2(point.X), point.Y);
        }

        /// <summary>
        /// Return wheather line intersects with this Elipse.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when line intersects with elipse.</returns>
        public bool Intersects(Line line, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            bool result;

            line.Rotate(Vector2.Zero, -rotation);  //neoto4iloto

            if (line.B == 0)
            {
                float xLine = -line.C / line.A;
                Vector2 y12res = GetY1Y2(xLine);

                intersec1 = ToBaseCoor(new Vector2(xLine, y12res.X));
                intersec2 = ToBaseCoor(new Vector2(xLine, y12res.Y));

                line.Rotate(Vector2.Zero, rotation);

                if (float.IsNaN(y12res.X))
                    result = false;
                else
                    result = true;
            }
            else
            {
                float A = line.A;
                float B = line.B;
                float C = line.C;
                float x0 = position.X;
                float y0 = position.Y;

                float aSq, bSq, cSq, Q;

                Q = Helper.Pow(a * b, -2);

                aSq = Helper.Pow(a * A) + Helper.Pow(b * B);
                bSq = 2 * Helper.Pow(a) * A * B * y0 + 2 * Helper.Pow(a) * A * C - 2 * Helper.Pow(b * B) * x0;
                cSq = -Helper.Pow(a * B * b) + Helper.Pow(a * C) + Helper.Pow(a * B * y0) + 2 * B * C * y0 * Helper.Pow(a) + Helper.Pow(b * B * x0);

                aSq *= Q;
                bSq *= Q;
                cSq *= Q;

                float DSqr = (float)Math.Sqrt(Helper.Pow(bSq) - 4 * aSq * cSq);

                float x1 = (-bSq + DSqr) / (2 * aSq);
                float x2 = (-bSq - DSqr) / (2 * aSq);


                intersec1 = ToBaseCoor(new Vector2(x1, line.f(x1)));
                intersec2 = ToBaseCoor(new Vector2(x2, line.f(x2)));


                line.Rotate(Vector2.Zero, rotation);

                if (float.IsNaN(x1) || float.IsNaN(x2))
                    result = false;
                else
                    result = true;

            }

            if (!result)
            {
                cs = CrossState.DontIntersecting;
                return false;
            }
            else
            {
                if (intersec1 == intersec2)
                {
                    cs = CrossState.Intersecting1Point;
                    return true;
                }
                else
                {
                    cs = CrossState.Intersecting2Point;
                    return true;
                }

            }
        }

        /// <summary>
        /// Return wheather halfSurface intersects with this Elipse.
        /// </summary>
        /// <param name="halfSurface">halfSurface to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, Elipse intersects with halfSurface.</returns>
        public virtual bool Intersects(HalfSurface halfSurface, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            return halfSurface.Intersects(this, out inters1, out inters2, out cs);
        }

        /// <summary>
        /// Return wheather segment intersects with this Elipse.
        /// </summary>
        /// <param name="segment">segment to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when Elipse intersects with segment.</returns>
        public bool Intersects(Segment segment, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            if (!this.Intersects((Line)segment, out inters1, out inters2, out cs))
                return false;
            else
            {
                /*bool po = segment.Intersects(inters1);
                bool qo = segment.Intersects(inters2);*/

                bool p = Helper.IsInSquareInterval(segment.Point1, segment.Point2, inters1);
                bool q = Helper.IsInSquareInterval(segment.Point1, segment.Point2, inters2);

                if (p && q && cs == CrossState.Intersecting2Point)
                {

                    cs = CrossState.Intersecting2Point;
                    return true;
                }
                else if (p)
                {
                    cs = CrossState.Intersecting1Point;
                    return true;
                }
                else if (q)
                {
                    inters1 = inters2;
                    cs = CrossState.Intersecting1Point;
                    return true;
                }
                else
                {
                    cs = CrossState.DontIntersecting;
                    return false;
                }
            }
        }

        /// <summary>
        /// Return wheather polygon intersects with this Elipse.
        /// </summary>
        /// <param name="polygon">Polygon to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when Elipse intersects with polygon.</returns>
        public bool Intersects(ConvexPolygon polygon, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            Vector2 nothigng;

            for (int i = 0; i < polygon.PolygonLinks.Count; i++)
                if (this.Intersects(polygon.PolygonLinks[i].Seqm, out inters1, out inters2, out cs))
                {
                    if (cs == CrossState.Intersecting2Point)
                        return true;
                    else
                    {
                        this.Intersects(polygon.PolygonLinks[(i + 1) % polygon.PolygonLinks.Count].Seqm, out inters2, out nothigng, out cs);

                        if (cs == CrossState.Intersecting1Point && inters2 == inters1)
                        {
                            cs = CrossState.Intersecting1Point;
                            return true;
                        }
                        else
                        {
                            if (float.IsNaN(inters2.X))
                            {
                                //throw new Exception();
                                inters2 = inters1;
                                cs = CrossState.Intersecting1Point;
                                return true;
                            }
                            cs = CrossState.Intersecting2Point;
                            return true;
                        }
                    }
                }

            inters1 = Vector2.Zero;
            inters2 = Vector2.Zero;
            cs = CrossState.DontIntersecting;

            return false;
        }

        /// <summary>
        /// Return wheather elipse intersects with this Elipse.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when Elipse intersects with elipse.</returns>
        public bool Intersects(Elipse elipse, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            if (polygonAproximation == null || needUpdate)
            {
                needUpdate = false;
                this.polygonAproximation = new ConvexPolygon(false, color, aproximatePoints);
            }

            return elipse.Intersects(this.polygonAproximation, out inters1, out inters2, out cs);
        }

        #endregion

        #region "Functions specific for elipse"

        /// <summary>
        /// Get y1, y2 coordinates of intersection with line determinate by y=x IN ELIPSE COOR.
        /// </summary>
        /// <param name="x">x for y=x</param>
        /// <returns>Retrns y1,y2 in vector.</returns>
        public Vector2 GetY1Y2(float x)
        {
            float subs = b * (float)Math.Sqrt(1 - ((x - position.X) / a) * ((x - position.X) / a));

            return new Vector2(subs + position.Y, position.Y - subs);
        }

        /// <summary>
        /// Get x1, x2 coordinates of intersection with line determinate by y=y IN ELIPSE COOR.
        /// </summary>
        /// <param name="x">y for y=y</param>
        /// <returns>Retrns x1,x2 in vector.</returns>
        public Vector2 GetX1X2(float y)
        {
            float subs = a * (float)Math.Sqrt(1 - ((y - position.Y) / b) * ((y - position.Y) / b));

            return new Vector2(subs + position.X, position.X - subs);
        }

        /// <summary>
        /// Transform point into elipse coor.
        /// </summary>
        /// <param name="point">Point to trasforamte.</param>
        /// <returns>Returns transformated point.</returns>
        private Vector2 ToElipseCoor(Vector2 point)
        {
            return Helper.Transform(Helper.CreateRotationMatrix(-rotation), point);

        }

        /// <summary>
        /// Transform point into base coor.
        /// </summary>
        /// <param name="point">Point to trasforamte.</param>
        /// <returns>Returns transformated point.</returns>
        private Vector2 ToBaseCoor(Vector2 point)
        {
            return Helper.Transform(Helper.CreateRotationMatrix(rotation), point);
        }

        #endregion

    }
}
