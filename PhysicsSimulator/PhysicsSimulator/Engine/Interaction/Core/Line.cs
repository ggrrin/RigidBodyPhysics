using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.Engine.MathAddition;
using System;

namespace PhysicsSimulator.Engine.Interaction.Core
{
    /// <summary>
    /// Základní třída realizujcí interakce přímek  s ostatními IInteractable objekty, následně je užívána skoro ve všech ostatních třídách strající se o interakci.
    /// </summary>
    [Serializable]
    class Line : IInteractable
    {
        #region "Fields"

        //graphics
        protected VertexPositionColor[] vertices;
        protected Color color;

        //mathematic realisation ax + by + c = 0
        protected float a, b, c;

        protected Vector2 point1, point2;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gives first point, which represent line.
        /// </summary>
        public Vector2 Point1
        {
            get { return this.point1; }

            private set
            {
                this.point1 = value;
            }
        }

        public virtual Color @Color
        {
            get { return color; }
            set
            {
                color = value;
                UpdateVertices();
            }
        }


        /// <summary>
        /// Gives second point, which represent line.
        /// </summary>
        public Vector2 Point2
        {
            get { return this.point2; }

            private set
            {
                this.point2 = value;
            }
        }

        /// <summary>
        /// Gives a from ax + by + c = 0
        /// </summary>
        public float A
        {
            get { return a; }
        }

        /// <summary>
        /// Gives b from ax + by + c = 0
        /// </summary>
        public float B
        {
            get { return b; }
        }

        /// <summary>
        /// Gives c from ax + by + c = 0
        /// </summary>
        public float C
        {
            get { return c; }
        }

        /// <summary>
        /// normala primky
        /// </summary>
        public Vector2 Normal
        {
            get { return new Vector2(a, b); }
            protected set { a = value.X; b = value.Y; }
        }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Forbidden constructor!
        /// </summary>
        public Line()
        {
            throw new Exception("This constructor is forbidden.");
        }

        /// <summary>
        /// Create new instance of Line.
        /// </summary>
        /// <param name="v1">First point, determining line.</param>
        /// <param name="v2">Second point, determining line.</param>
        /// <param name="color">Color of line.</param>
        public Line(Vector2 v1, Vector2 v2, Color color)
        {
            if (v1 == v2) throw new Exception("v1 and v2 are the same vectors.");

            this.color = color;

            this.point1 = v1;
            this.point2 = v2;

            //create line equation
            Vector2 direction = v2 - v1;
            this.a = direction.Y;
            this.b = -direction.X;
            this.Normal = Vector2.Normalize(Normal);
            this.c = -a * v1.X - b * v1.Y;

            this.UpdateVertices();
        }

        #endregion

        #region "Update"

        /// <summary>
        /// Translate line by specific translation.
        /// </summary>
        /// <param name="translation">Translation vector.</param>
        public virtual void Translate(Vector2 translation)
        {
            this.point1 += translation;
            this.point2 += translation;

            //create line equation
            this.c = -a * this.point1.X - b * this.point1.Y;

            this.UpdateVertices();
        }

        /// <summary>
        /// Rotate line by specific angle due to specific orign.
        /// </summary>
        /// <param name="orign">Orign of rotationg.</param>
        /// <param name="angle">Angle of rotationg.</param>
        public virtual void Rotate(Vector2 orign, float angle)
        {
            //translate axis
            this.point1 -= orign;
            this.point2 -= orign;

            MatrixND rotationMatrix = Helper.CreateRotationMatrix(angle);

            //rotate 
            this.point1 = Helper.Transform(rotationMatrix, point1);
            this.point2 = Helper.Transform(rotationMatrix, point2);
            this.Normal = Helper.Transform(rotationMatrix, Normal);

            //translate axis back
            this.point1 += orign;
            this.point2 += orign;

            //create line equation
            this.c = -a * this.point1.X - b * this.point1.Y;

            this.UpdateVertices();
        }


        #region "Update Graphics"

        /// <summary>
        /// Update vertices for drawing.
        /// </summary>
        protected virtual void UpdateVertices()
        {
            this.vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(new Vector3(GetPointOnTheLine(Configuration.leftBottomOfScreen), 0), color);
            vertices[1] = new VertexPositionColor(new Vector3(GetPointOnTheLine(Configuration.rightTopOfScreen), 0), color);
        }

        /// <summary>
        /// Draw line.
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

        }

        #endregion

        #endregion

        #region "Functions specific for line"

        /// <summary>
        /// Get point on the line, which is intersection with line determined by X or Y coordinate of v, depends on state of the line.
        /// </summary>
        /// <param name="v">Point, which determines lines.</param>
        /// <returns>Returns vector of the intersection on the line.</returns>
        public Vector2 GetPointOnTheLine(Vector2 v)
        {
            Vector2 result = new Vector2(v.X, f(v.X));

            if (float.IsNaN(result.Y) || float.IsInfinity(result.Y))
                result = new Vector2(fInvers(v.Y), v.Y);

            return result;
        }

        /// <summary>
        /// Get y value for specific x determined by line.
        /// </summary>
        /// <param name="x">Determines f(x)</param>
        /// <returns>Returns y coordinate depends on x.</returns>
        public float f(float x)
        {
            return (a * x + c) / -b;
        }

        /// <summary>
        /// Get x value for specific y determined by line. (inversion of (f(x))
        /// </summary>
        /// <param name="x">Determines f-1(y)</param>
        /// <returns>Returns x coordinate depends on y.</returns>
        public float fInvers(float y)
        {
            return (b * y + c) / -a;
        }


        /// <summary>
        /// Count distance between line and point.
        /// </summary>
        /// <param name="point">Point to measure.</param>
        /// <returns>Returns distance of point and line.</returns>
        public float Distance(Vector2 point)
        {
            return Math.Abs(this.OrientedDistance(point));
        }

        /// <summary>
        /// Count distance between line and point.
        /// </summary>
        /// <param name="point">Point to measure.</param>
        /// <returns>Returns distance of point and line.</returns>
        public float OrientedDistance(Vector2 point)
        {
            return (this.a * point.X + this.b * point.Y + this.c) / (float)Math.Sqrt(Helper.Pow(a) + Helper.Pow(b));
        }

        #endregion

        #region "Intersection"

        #region "just bool"

        /// <summary>
        /// Return wheather line intersects with this line.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        public virtual bool Intersects(Line line)
        {
            Vector2 g;
            CrossState cs;
            return this.Intersects(line, out g, out g, out cs);
        }

        /// <summary>
        /// Return wheather line intersects with this halfSurface.
        /// </summary>
        /// <param name="halfSurface">halfSurface to test intersection.</param>
        public virtual bool Intersects(HalfSurface halfSurface)
        {
            Vector2 g;
            CrossState cs;
            return this.Intersects(halfSurface, out g, out g, out cs);
        }


        /// <summary>
        /// Return wheather line intersects with this segment.
        /// </summary>
        /// <param name="segment">segment to test intersection.</param>
        public virtual bool Intersects(Segment segment)
        {
            Vector2 g;
            CrossState cs;
            return this.Intersects(segment, out g, out g, out cs);
        }

        /// <summary>
        /// Return wheather elipse intersects with this line.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        public virtual bool Intersects(Elipse elipse)
        {
            return elipse.Intersects(this);
        }

        /// <summary>
        /// Return wheather polygon intersects with this line.
        /// </summary>
        /// <param name="elipse">Polygon to test intersection.</param>
        public virtual bool Intersects(ConvexPolygon polygon)
        {
            return polygon.Intersects(this);
        }

        #endregion

        public override string ToString()
        {
            return point1.ToString() + point2.ToString();
        }

        public virtual bool Intersects(IInteractable obj, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            return obj.Intersects(this, out intersec1, out intersec2, out cs);
        }

        /// <summary>
        /// Return wheather point intersects with line.
        /// </summary>
        /// <param name="point">Point to test intersection.</param>
        /// <returns>Returns true, when point lies on the line.</returns>
        public virtual bool Intersects(Vector2 point)
        {
            if (Helper.AproximatellyEqual(GetPointOnTheLine(point), point))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Return wheather halfSurface intersects with this line.
        /// </summary>
        /// <param name="halfSurface">halfSurface to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when lines are crossed or are the same.</returns>
        public virtual bool Intersects(HalfSurface halfSurface, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            return halfSurface.Intersects(this, out intersec1, out intersec2, out cs);
        }


        /// <summary>
        /// Return wheather segment intersects with this line.
        /// </summary>
        /// <param name="segment">segment to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when lines are crossed or are the same.</returns>
        public virtual bool Intersects(Segment segment, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            intersec2 = Vector2.Zero;
            return segment.Intersects((Line)this, out intersec1, out intersec2, out cs);
        }

        /// <summary>
        /// Return wheather line intersects with this line.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when lines are crossed or are the same.</returns>
        public virtual bool Intersects(Line line, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            intersec2 = Vector2.Zero;
            intersec1 = Vector2.Zero;

            MatrixND mat = new MatrixND(new float[,] { { a, b, -c }, { line.a, line.b, -line.c } });

            if (mat.Elimine())
            {
                intersec1.X = mat[0, 2];
                intersec1.Y = mat[1, 2];

                cs = CrossState.Intersecting1Point;
                return true;
            }
            else
                if (mat.GetRank(1) < mat.GetRank(2))
                {
                    cs = CrossState.Paralel;
                    return false;
                }
                else
                {
                    cs = CrossState.Identical;
                    return false;
                }
        }

        /// <summary>
        /// Return wheather elipse intersects with this line.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when line intersects with elipse.</returns>
        public virtual bool Intersects(Elipse elipse, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            bool res = elipse.Intersects(this, out intersec1, out intersec2, out cs);
            if (res)
                if (intersec1 == intersec2)
                    cs = CrossState.Intersecting1Point;
                else
                    cs = CrossState.Intersecting2Point;
            else
                cs = CrossState.DontIntersecting;

            return res;
        }

        /// <summary>
        /// Return wheather polygon intersects with this line.
        /// </summary>
        /// <param name="polygon">Polygon to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when line intersects with polygon.</returns>
        public virtual bool Intersects(ConvexPolygon polygon, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            return polygon.Intersects(this, out intersec1, out intersec2, out cs);
        }

        #endregion
    }
}
