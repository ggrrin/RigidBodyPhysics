using Microsoft.Xna.Framework;
using System;

namespace PhysicsSimulator.Engine.Interaction.Core
{
    /// <summary>
    /// Třída realizujcí základní stavební kámen Třídy ConvexPolygon, tedy jeho stranu. Slouží hlavně k výpočtu interakcí.
    /// </summary>
    [Serializable]
    class PolygonLink : HalfSurface, IInteractable
    {

        /// <summary>
        /// usecka pro zjisteni pozice pruniku
        /// </summary>
        private Segment s;

        /// <summary>
        /// qvadrat ve kterem je linearni funkce urce primkou (kvuli testovani zda je potom konvexni )
        /// </summary>
        public int Quadrant
        {
            get
            {
                if (Directive == float.PositiveInfinity)
                    return 2;

                if (Directive == float.NegativeInfinity)
                    return 4;

                if (Directive >= 0)
                    if (Point1.X <= Point2.X)
                        return 1;
                    else
                        return 3;
                else
                    if (Point1.Y <= Point2.Y)
                        return 2;
                    else
                        return 4;
            }
        }

        /// <summary>
        /// smernice primky
        /// </summary>
        public float Directive
        {
            get { return -base.a / base.b; }
        }

        /// <summary>
        /// barava
        /// </summary>
        public override Color Color
        {
            get
            {
                return base.Color;
            }
            set
            {
                base.Color = value;
                s.Color = value;


            }
        }


        /// <summary>
        /// usecka
        /// </summary>
        public Segment Seqm
        {
            get { return s; }
        }

        /// <summary>
        /// Create new instance of PolygonLink.
        /// </summary>
        /// <param name="point1">First point specific for line.</param>
        /// <param name="point2">First point specific for line.</param>
        /// <param name="pointIn">Point which determinate on which side of line is PolygonLink.</param>
        /// <param name="color">Color of PolygonLink.</param>
        public PolygonLink(Vector2 point1, Vector2 point2, Vector2 pointIn, Color color)
            : base(point1, point2, pointIn, color)
        {
            this.s = new Segment(point1, point2, color);
        }

        /// <summary>
        /// Translate polygonlink.
        /// </summary>
        /// <param name="translation">Translation vector.</param>
        public override void Translate(Vector2 translation)
        {
            base.Translate(translation);
            s.Translate(translation);
        }

        /// <summary>
        /// Rotate polygonlink by specific angle due to specific orign.
        /// </summary>
        /// <param name="orign">Orign of rotationg.</param>
        /// <param name="angle">Angle of rotationg.</param>
        public override void Rotate(Vector2 orign, float angle)
        {
            base.Rotate(orign, angle);
            s.Rotate(orign, angle);
        }

        /// <summary>
        /// Draw polygon link.
        /// </summary>
        public override void Draw()
        {
            s.Draw();
        }

        /// <summary>
        /// Return wheather line intersects with this PolygonLink.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        /// <param name="intersection">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when lines are crossed or are the same.</returns>
        public bool IntersectsSegment(Line line, out Vector2 intersection, out CrossState cs)
        {
            Vector2 a;
            return s.Intersects(line, out intersection, out a, out cs);
        }


    }
}
