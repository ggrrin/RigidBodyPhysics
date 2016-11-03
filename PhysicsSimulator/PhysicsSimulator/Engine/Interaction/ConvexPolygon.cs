using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction.Core;
using PhysicsSimulator.Engine.MathAddition;
using System;
using System.Collections.Generic;
using System.Text;


namespace PhysicsSimulator.Engine.Interaction
{
    /// <summary>
    /// Třída realizujcí konvexní mnohoúhelníky a jejich kolize s ostatními IInteractable objekty.
    /// </summary>
    [Serializable]
    class ConvexPolygon : IInteractable
    {
        /// <summary>
        /// seznam hran 
        /// </summary>
        private List<PolygonLink> polygonLinks;

        /// <summary>
        /// seznam hran 
        /// </summary>
        public List<PolygonLink> PolygonLinks
        {
            get { return polygonLinks; }
        }

        /// <summary>
        /// barva
        /// </summary>
        private Color color;


        /// <summary>
        /// barva
        /// </summary>
        public Color @Color
        {
            get { return color; }
            set
            {
                color = value;
                foreach (var i in polygonLinks)
                    i.Color = value;
            }
        }

        /// <summary>
        /// pozice
        /// </summary>
        protected Vector2 position;

        /// <summary>
        /// pozice
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }

        #region "Constructors"



        /// <summary>
        /// Create polygon determined by specific points
        /// </summary>
        /// <param name="checkInput">True, if you want to check convexivity.</param>
        /// <param name="color">Color of polygon</param>
        /// <param name="points">List of points in tandem.</param>
        public ConvexPolygon(bool checkInput, Color color, params Vector2[] points)
        {
            this.color = color;

            if (points.Length < 3 || points == null) throw new Exception("Polygon has to have at least 3 points.");

            polygonLinks = new List<PolygonLink>();

            for (int i = 0; i < points.Length; i++)
            {
                PolygonLink ln = new PolygonLink(points[i % points.Length], points[(i + 1) % points.Length], points[(i + 2) % points.Length], color);
                polygonLinks.Add(ln);
            }

            if (checkInput) this.CheckInput();
        }

        /// <summary>
        /// Create polygon determined by specific points and check if it is convex, otherwise throws exception.
        /// </summary>
        /// <param name="color">Color of polygon</param>
        /// <param name="points">List of points in tandem.</param>
        public ConvexPolygon(Color color, params Vector2[] points)
        {
            this.color = color;

            if (points.Length < 3 || points == null) throw new Exception("Polygon has to have at least 3 points.");

            polygonLinks = new List<PolygonLink>();

            for (int i = 0; i < points.Length; i++)
            {
                PolygonLink ln = new PolygonLink(points[i % points.Length], points[(i + 1) % points.Length], points[(i + 2) % points.Length], color);
                polygonLinks.Add(ln);
            }

            this.CheckInput();
        }

        #endregion

        #region "Update"




        public virtual bool Intersects(IInteractable obj, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs)
        {
            return obj.Intersects(this, out intersec1, out intersec2, out cs);
        }

        /// <summary>
        /// Translate ConvexPolygon by specific translation.
        /// </summary>
        /// <param name="translation">Translation vector.</param>
        public virtual void Translate(Vector2 translation)
        {
            foreach (PolygonLink pl in polygonLinks)
                pl.Translate(translation);
            position += translation;
        }

        /// <summary>
        /// Rotate ConvexPolygon by specific angle due to specific orign.
        /// </summary>
        /// <param name="orign">Orign of rotationg.</param>
        /// <param name="angle">Angle of rotationg.</param>
        public virtual void Rotate(Vector2 orign, float angle)
        {
            foreach (PolygonLink pl in polygonLinks)
                pl.Rotate(orign, angle);

            this.position -= orign;
            this.position = Helper.Transform(Helper.CreateRotationMatrix(angle), position);
            this.position += orign;
        }

        /// <summary>
        /// Draws polygon.
        /// </summary>
        public virtual void Draw()
        {
            foreach (PolygonLink h in polygonLinks)
                h.Draw();
        }

        #endregion

        #region "Intersection"

        #region "just bool"

        /// <summary>
        /// Return wheather line intersects with this ConvexPolygon.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        public virtual bool Intersects(Line line)
        {
            Vector2 mk, ko;
            CrossState cs;
            return this.Intersects(line, out  ko, out mk, out cs);
        }

        /// <summary>
        /// Return wheather ConvexPolygon intersects with this halfSurface.
        /// </summary>
        /// <param name="halfSurface">halfSurface to test intersection.</param>
        public virtual bool Intersects(HalfSurface halfSurface)
        {
            Vector2 a, b;
            CrossState cs;
            return this.Intersects((Line)halfSurface, out a, out b, out cs);
        }

        /// <summary>
        /// Return wheather ConvexPolygon intersects with this segment.
        /// </summary>
        /// <param name="segment">segment to test intersection.</param>
        public virtual bool Intersects(Segment segment)
        {
            Vector2 a, b;
            CrossState cs;
            return this.Intersects(segment, out a, out b, out cs);
        }

        /// <summary>
        /// Return wheather elipse intersects with this ConvexPolygon.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        public virtual bool Intersects(Elipse elipse)
        {
            Vector2 a, b;
            CrossState cs;
            return this.Intersects(elipse, out a, out b, out cs);
        }

        /// <summary>
        /// Return wheather polygon intersects with this ConvexPolygon.
        /// </summary>
        /// <param name="elipse">Polygon to test intersection.</param>
        public virtual bool Intersects(ConvexPolygon polygon)
        {
            Vector2 a, b;
            CrossState cs;
            return this.Intersects(polygon, out a, out b, out cs);
        }

        #endregion

        /// <summary>
        /// Return wheather point intersects with ConvexPolygon.
        /// </summary>
        /// <param name="point">Point to test intersection.</param>
        /// <returns>Returns true, when point lies on the line.</returns>
        public virtual bool Intersects(Vector2 point)
        {
            foreach (PolygonLink h in polygonLinks)
                if (!h.Intersects(point))
                    return false;
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PolygonLink p in polygonLinks)
            {
                sb.Append(p.Point1.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Return wheather line intersects with this Convexpolygon.
        /// </summary>
        /// <param name="line">Line to test intersection.</param>
        /// <param name="intersection">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when some of the polygonlinks are crossed or are the same.</returns>
        public virtual bool Intersects(Line line, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            int counter = 0;

            Vector2 intersection = Vector2.Zero;
            inters1 = intersection; inters2 = intersection;
            CrossState crossSeg;

            foreach (PolygonLink h in polygonLinks)
                if (h.IntersectsSegment(line, out intersection, out crossSeg))
                {
                    if (counter == 0)
                        inters1 = intersection;

                    if (counter == 1)
                        inters2 = intersection;

                    if (++counter == 2) break;

                }

            if (counter == 0)
            {
                cs = CrossState.DontIntersecting;
                return false;
            }
            else if (counter == 1)
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

        /// <summary>
        /// Return wheather halfSurface intersects with this Convexpolygon.
        /// </summary>
        /// <param name="halfSurface">halfSurface to test intersection.</param>
        /// <param name="intersection">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when some of the polygonlinks are crossed or are the same.</returns>
        public virtual bool Intersects(HalfSurface halfSurface, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            return halfSurface.Intersects(this, out inters1, out inters2, out cs);
        }

        /// <summary>
        /// Return wheather elipse intersects with this ConvexPolygon.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when ConvexPolygon intersects with elipse.</returns>
        public virtual bool Intersects(Elipse elipse, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            return elipse.Intersects(this, out inters1, out inters2, out cs);

        }

        /// <summary>
        /// Return wheather polygon intersects with this ConvexPolygon.
        /// </summary>
        /// <param name="polygon">Polygon to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when ConvexPolygon intersects with polygon.</returns>
        public virtual bool Intersects(ConvexPolygon polygon, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            if (PointsIntersects(polygon, out inters1, out inters2) || polygon.PointsIntersects(this, out inters1, out inters2))
            {
                if (inters1 == inters2)
                    cs = CrossState.Intersecting1Point;
                else
                    cs = CrossState.Intersecting2Point;
                return true;
            }
            else
            {
                cs = CrossState.DontIntersecting;
                return false;
            }
        }

        /// <summary>
        /// Return wheather segment intersects with this ConvexPolygon.
        /// </summary>
        /// <param name="segment">segment to test intersection.</param>
        /// <param name="intersection">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when some of the polygonlinks are crossed or are the same.</returns>
        public virtual bool Intersects(Segment segment, out Vector2 inters1, out Vector2 inters2, out CrossState cs)
        {
            if (!this.Intersects((Line)segment, out inters1, out inters2, out cs))
                return false;
            else
            {
                bool p = segment.Intersects(inters1);
                bool q = segment.Intersects(inters2);

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

        #region "help intersection"

        /// <summary>
        /// Return wheather polygon intersects with this ConvexPolygon NOT OTHER WAY.
        /// </summary>
        /// <param name="polygon">Polygon to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when ConvexPolygon intersects with polygon.</returns>
        private bool PointsIntersects(ConvexPolygon polygon, out Vector2 inters1, out Vector2 inters2)
        {
            //jestli interagují samotné body
            for (int i = 0; i < polygon.polygonLinks.Count; i++)//PolygonLink link in polygon.SegmentLines)
                if (this.Intersects(polygon.polygonLinks[i].Point2))
                {
                    Vector2 z; CrossState cs;
                    //najdi pruseciky
                    this.Intersects(polygon.polygonLinks[i].Seqm, out inters1, out z, out cs);


                    if (cs == CrossState.DontIntersecting)
                    {
                        //TODO: dfdfjuuu
                        inters1 = polygon.polygonLinks[i].Point2;
                        inters2 = inters1;
                        cs = CrossState.Intersecting1Point;
                        return true;
                    }

                    while (!this.Intersects(polygon.polygonLinks[(++i) % polygon.polygonLinks.Count].Seqm, out inters2, out z, out cs)) ;

                    return true;
                }

            inters1 = Vector2.Zero;
            inters2 = Vector2.Zero;
            return false;
        }

        #endregion

        #endregion

        #region "Check Input"

        /// <summary>
        /// zjištění, zda je psáno clockwise nebo counter clockwise
        /// </summary>
        /// <returns>wheather is clockwise true else flase</returns>
        private bool IsClockWise()
        {
            Vector2 n = this.polygonLinks[0].Normal;

            if ((n.X < 0 && n.Y < 0) || (n.X < 0 && n.Y > 0))
                if (this.polygonLinks[0].Point1.Y < this.polygonLinks[0].Point2.Y)
                    return true;
                else
                    return false;
            else if ((n.X > 0 && n.Y > 0) || (n.X > 0 && n.Y < 0))
            {
                if (this.polygonLinks[0].Point1.Y > this.polygonLinks[0].Point2.Y)
                    return true;
                else
                    return false;
            }
            else if (n.X == 0)
                if (n.Y > 0)
                    if (this.polygonLinks[0].Point1.X < this.polygonLinks[0].Point2.X)
                        return true;
                    else
                        return false;
                else
                    if (this.polygonLinks[0].Point1.X > this.polygonLinks[0].Point2.X)
                        return true;
                    else
                        return false;
            else
                if (n.X > 0)
                    if (this.polygonLinks[0].Point1.X > this.polygonLinks[0].Point2.X)
                        return true;
                    else
                        return false;
                else
                    if (this.polygonLinks[0].Point1.X < this.polygonLinks[0].Point2.X)
                        return true;
                    else
                        return false;
        }


        /// <summary>
        /// Returns true if input is ok else thow exeption. ALL THE VARIABLES HAS TO BE SET!!!!!
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            bool isCockwise = this.IsClockWise();

            //najdi přechody mezi kvadranty honota rovná se index v poli links začátek kvadrantu
            int[] changeQ = new int[] { -1, -1, -1, -1 };

            int k = 0; // přechod -index pole changeQ -- na konci je to počet kvadrantů 

            int g = 0; // procházení links

            while ((g + 1) % polygonLinks.Count != changeQ[0])//nastav do pole přechody mezi kvadranty -> pokud jsme došli k indexu prního kvadrantu našli jsme všechny přechody
            {
                try
                {
                    if (changeQ[k % 4] == -1 && polygonLinks[g % polygonLinks.Count].Quadrant != polygonLinks[(g + 1) % polygonLinks.Count].Quadrant) // pokud jsem na přechodu mezi kvadranty tak to zapiš
                    {
                        changeQ[k++] = (g + 1) % polygonLinks.Count;
                    }
                }
                catch { throw new Exception("Element is not convex! It contains more then four separated qadrants!"); }

                g++;
            }
            if (k < 2) throw new Exception("This can not be convex elemnt! It has only two different directives.");


            //test zda jsou směrnice v jednotlivých kvadrantech ve sprváném pořadí
            for (int j = 0; j < k; j++)//projdeme postupně kvadranty
            {
                int max;//počet úseček v kvadrantu
                if (changeQ[j] > changeQ[(j + 1) % k])
                    max = polygonLinks.Count - changeQ[j] + changeQ[(j + 1) % k];
                else
                    max = changeQ[(j + 1) % k] - changeQ[j];

                for (int i = 0; i < max - 1 /*kontroluju po dvouch pokud je v  intervalu jen jedna funguje*/ ; i++)// projdeme prvky kvadrantů
                {
                    int linkIndex = (changeQ[j] + i) % polygonLinks.Count;
                    bool isDecresing = polygonLinks[linkIndex].Directive >= polygonLinks[(linkIndex + 1) % polygonLinks.Count].Directive;// pokud klesá
                    if (!isCockwise ^ !isDecresing)//pokud jde CCLW tak přehod hodnotu isDecresing
                    {
                        throw new Exception(String.Format("This can not be convex elemnt! In {0}th quadrant is incorrect order of directives", polygonLinks[linkIndex].Quadrant));
                    }
                }
            }

            Vector2 w; CrossState cs;

            //test zda se některé linky neprotínají
            for (int i = 2; i < polygonLinks.Count - 1; i++)
            {
                for (int x = 0; x < i - 1; x++)
                    if (polygonLinks[x].IntersectsSegment(polygonLinks[i], out w, out cs)) throw new Exception(String.Format("At least two links are corssed ({0}. and {1}.) Crossing is forbiden! ", i, x));
            }

            return true;
        }

        #endregion

    }
}
