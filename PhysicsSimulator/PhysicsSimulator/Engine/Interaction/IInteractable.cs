using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.Interaction.Core;
using System;

namespace PhysicsSimulator.Engine.Interaction
{
    /// <summary>
    /// Rozhraní určujcí základní vlastnosti, které by měly splňovat interakční objekty.
    /// </summary>
    interface IInteractable
    {
        /// <summary>
        /// Vykresli objekt
        /// </summary>
        void Draw();

        Color @Color { get; set; }

        /// <summary>
        /// Translate obj by translation vector.
        /// </summary>
        /// <param name="transaltion">Translation, which determine direction and length.</param>
        void Translate(Vector2 transaltion);

        /// <summary>
        /// Rotate obj by angle.
        /// </summary>
        /// <param name="orign">Orign of rotationg.</param>
        /// <param name="angle">Angle in counterclockwise.</param>
        void Rotate(Vector2 orign, float angle);

        #region "Intersection"

        #region "Just bool"

        //bool Intersects<T>(T obj) where T : IInteractable;


        /// <summary>
        /// Return wheather Line intersects with this IInteractable.
        /// </summary>
        /// <param name="line">IInteractable to test intersection.</param>
        /// <returns>Returns true, when IInteractables are crossed or are the same.</returns>
        bool Intersects(Line line);

        /// <summary>
        /// Return wheather Segment intersects with this IInteractable.
        /// </summary>
        /// <param name="segment">IInteractable to test intersection.</param>
        /// <returns>Returns true, when IInteractables are crossed or are the same.</returns>
        bool Intersects(Segment segment);

        /// <summary>
        /// Return wheather Segment intersects with this IInteractable.
        /// </summary>
        /// <param name="halfSurface">IInteractable to test intersection.</param>
        /// <returns>Returns true, when IInteractables are crossed or are the same.</returns>
        bool Intersects(HalfSurface halfSurface);

        /// <summary>
        /// Return wheather elipse intersects with this IInteractable.
        /// </summary>
        /// <param name="polygon">Polygon to test intersection.</param>
        /// <returns>Returns true, when IInteractable intersects with polygon.</returns>
        bool Intersects(ConvexPolygon polygon);

        /// <summary>
        /// Return wheather elipse intersects with this IInteractable.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        /// <returns>Returns true, when IInteractable intersects with elipse.</returns>
        bool Intersects(Elipse elipse);

        #endregion

        bool Intersects(IInteractable obj, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs);

        /// <summary>
        /// Return wheather point intersects with IInteractable.
        /// </summary>
        /// <param name="point">Point to test intersection.</param>
        /// <returns>Returns true, when point lies on the IInteractable.</returns>
        bool Intersects(Vector2 point);

        /// <summary>
        /// Return wheather Line intersects with this IInteractable.
        /// </summary>
        /// <param name="line">IInteractable to test intersection.</param>
        /// <param name="intersection">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when IInteractables are crossed or are the same.</returns>
        bool Intersects(Line line, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs);

        /// <summary>
        /// Return wheather Segment intersects with this IInteractable.
        /// </summary>
        /// <param name="segment">IInteractable to test intersection.</param>
        /// <param name="intersection">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when IInteractables are crossed or are the same.</returns>
        bool Intersects(Segment segment, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs);

        /// <summary>
        /// Return wheather Segment intersects with this IInteractable.
        /// </summary>
        /// <param name="halfSurface">IInteractable to test intersection.</param>
        /// <param name="intersection">Feed back vector contains intersection, wheater it intersects.</param>
        /// <param name="cs">Feed back for CrossState</param>
        /// <returns>Returns true, when IInteractables are crossed or are the same.</returns>
        bool Intersects(HalfSurface halfSurface, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs);

        /// <summary>
        /// Return wheather elipse intersects with this IInteractable.
        /// </summary>
        /// <param name="polygon">Polygon to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when IInteractable intersects with polygon.</returns>
        bool Intersects(ConvexPolygon polygon, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs);

        /// <summary>
        /// Return wheather elipse intersects with this IInteractable.
        /// </summary>
        /// <param name="elipse">Elipse to test intersection.</param>
        /// <param name="intersec1">Feed back, first intersection.</param>
        /// <param name="intersec2">Feed back, second intersection.</param>
        /// <param name="cs">Feed back, CrossState</param>
        /// <returns>Returns true, when IInteractable intersects with elipse.</returns>
        bool Intersects(Elipse elipse, out Vector2 intersec1, out Vector2 intersec2, out CrossState cs);

        #endregion

    }

    /// <summary>
    /// Výsledný stav kolizí
    /// </summary>

    [Serializable]
    enum CrossState
    {
        /// <summary>
        /// Objekty interaguji pravě v jednom bodě.
        /// </summary>
        Intersecting1Point,

        /// <summary>
        /// Objekty interaguji alespoň ve dvou bodech určených průnikem hran objektů.
        /// </summary>
        Intersecting2Point,

        /// <summary>
        /// Objekty neinteragují v žádném bodě.
        /// </summary>
        DontIntersecting,
        /// <summary>
        /// Objekty se překrývají. (pouze u přímek)
        /// </summary>
        Identical,
        /// <summary>
        /// Objekty jsou rovnoběžné. (pouze u přímek)
        /// </summary>
        Paralel
    }
}
