using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction.Core;
using PhysicsSimulator.Engine.MathAddition;
using System;

namespace PhysicsSimulator.Engine.Interaction
{
    /// <summary>
    /// Třída realizujcí poloroviny a jejich kolize s ostatními IInteractable objekty. Složí jako základ třídě PolygonLink.
    /// </summary>
    [Serializable]
    class HalfSurface : Line, IInteractable
    {
        #region "Constructors"

        /// <summary>
        /// Create new instance of halfSurface.
        /// </summary>
        /// <param name="color">Color of halfSurface.</param>
        /// <param name="normal">Normal specific for halfSurface.</param>
        /// <param name="point">Point specific for halfSurface.</param>
        public HalfSurface(Color color, Vector2 normal, Vector2 point)
            : base(point, point + new Vector2(-normal.Y, normal.X), color)
        {
            if (normal.Length() == 0) throw new Exception("Normal can not have length 0.");

            //base.a = normal.X;
            //base.b = normal.Y; myslim ze je to zbytecny
            base.Normal = Vector2.Normalize(base.Normal);
        }

        /// <summary>
        /// Create new instance of halfSurface.
        /// </summary>
        /// <param name="point1">First point specific for line.</param>
        /// <param name="point2">First point specific for line.</param>
        /// <param name="pointIn">Point which determinate on which side of line is halfsruface.</param>
        /// <param name="color">Color of halfSurface.</param>
        public HalfSurface(Vector2 point1, Vector2 point2, Vector2 pointIn, Color color)
            : base(point1, point2, color)
        {
            if (base.Intersects(pointIn)) throw new Exception("These three points does not deteremine halfsurface.");
            if (base.Normal == Vector2.Zero) throw new Exception("Normal can not have length 0.");

            if (!this.Intersects(pointIn))
            {
                base.a *= -1;
                base.b *= -1;
                base.c *= -1;
            }
            if (!this.Intersects(pointIn)) throw new Exception("Unexpect exception.");
        }


        #endregion

        #region "Intersection"


        /// <summary>
        /// Return wheather point intersects with halfsurface.
        /// </summary>
        /// <param name="point">Point to test intersection.</param>
        /// <returns>Returns true, when point lies on the halfsurface.</returns>
        public override bool Intersects(Vector2 point)
        {
            if (base.Intersects(point)) return true;

            float dist = base.OrientedDistance(point);

            float dist1 = base.OrientedDistance(point + 2 * base.Distance(point) * Vector2.Normalize(base.Normal));

            if ((dist < 0 && dist1 < 0) || (dist > 0 && dist1 > 0))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Return wheather line intersects with halfsurface.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when lines are crossed or are the same.</returns>
        public override bool Intersects(Line line, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            if (base.Intersects(line, out intersec1, out intersec2, out cs))
                return true;
            else
                if (cs == CrossState.Paralel)
                    return this.Intersects(line.GetPointOnTheLine(new Vector2(0, 0)));
                else
                    return false;
        }

        /// <summary>
        /// Return wheather elipse intersects with halfsurface.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when halfsurface intersects with elipse.</returns>
        public override bool Intersects(Elipse elipse, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            if (base.Intersects(elipse, out intersec1, out intersec2, out cs)) // pokud  primka urcena polorovinou koliduje 
                return true;
            else
                if (this.Intersects(elipse.Position)) // pokud je stred elipsy nekde v polorovine je tam i jista cast elipsy
                {

                    /* Line l = new Line(elipse.Position, elipse.Position + new Vector2(1,-1), Color.PaleVioletRed);

                     Vector2 o1, o2; CrossState  ggg;

                     l.Intersects(this,out o1,out o2,out ggg);
                     float hhh = Vector2.Distance(elipse.Position, o1) - Vector2.Distance(elipse.Position, elipse.APoint);
                     if (hhh < 0)
                     {
 
                     }
                     base.GetPointOnTheLine(elipse.Position);*/

                    cs = CrossState.Intersecting1Point;

                    return true;
                }
                else
                    return false;
        }

        #endregion
    }
}
