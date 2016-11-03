using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.MathAddition;
using System;

namespace PhysicsSimulator.Engine.Interaction.Core
{
    /// <summary>
    /// Třída realizující úsečku a veškeré interakce s ostatními IInteractable objekty.
    /// </summary>
    [Serializable]
    class Segment : Line, IInteractable
    {

        #region "Constructors"

        /// <summary>
        /// Create new instance of segment.
        /// </summary>
        /// <param name="v1">First point, determining segment.</param>
        /// <param name="v2">Second point, determining segment.</param>
        /// <param name="color">Color of segment.</param>
        public Segment(Vector2 v1, Vector2 v2, Color color)
            : base(v1, v2, color)
        { }

        #endregion

        #region "Update"



        /// <summary>
        /// Update vertices for drawing.
        /// </summary>
        protected override void UpdateVertices()
        {
            if (base.vertices == null)
                base.UpdateVertices();


            base.vertices[0].Position = new Vector3(base.Point1, 0);
            base.vertices[1].Position = new Vector3(base.Point2, 0);
            base.vertices[0].Color = color;
            base.vertices[1].Color = color;
        }

        #endregion

        #region "Intersection"



        /// <summary>
        /// Return wheather point intersects with point.
        /// </summary>
        /// <param name="point">Point to test intersection.</param>
        /// <returns>Returns true, when point lies on the point.</returns>
        public override bool Intersects(Vector2 point)
        {
            //TODO: BullShit


            if (!base.Intersects(point))
                return false;
            else
                return Helper.IsInSquareInterval(Point1, Point2, point);
        }

        /// <summary>
        /// Return wheather line intersects with this segment.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        /// <param name="intersec1">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="intersec2">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when lines are crossed or are the same.</returns>
        public override bool Intersects(Line line, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            if (!base.Intersects(line, out intersec1, out intersec2, out cs))
                return false;
            else
                if (this.Intersects(intersec1))
                    return true;
                else
                {
                    cs = CrossState.DontIntersecting;
                    return false;
                }
        }

        /// <summary>
        /// Return wheather elipse intersects with this segment.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when segment intersects with elipse.</returns>
        public override bool Intersects(Elipse elipse, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            if (!base.Intersects(elipse, out intersec1, out intersec2, out cs))
                return false;
            else
            {
                bool l = this.Intersects(intersec1);
                bool g = this.Intersects(intersec1);

                if (l && g)
                    return true;
                else if (l)
                {
                    intersec2 = intersec1;
                    cs = CrossState.Intersecting1Point;
                    return true;
                }
                else if (g)
                {
                    intersec1 = intersec2;
                    cs = CrossState.Intersecting1Point;
                    return true;
                }
                //alepson jeden kraj usečky je uvnitře
                else if (elipse.Intersects(this.Point1) || elipse.Intersects(this.Point2))
                {
                    cs = CrossState.Identical;
                    return true;
                }
                else
                {
                    cs = CrossState.DontIntersecting;
                    return false;
                }
            }
        }

        #endregion
    }
}
