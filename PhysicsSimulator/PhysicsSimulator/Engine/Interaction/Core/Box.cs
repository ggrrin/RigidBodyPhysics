using Microsoft.Xna.Framework;
using PhysicsSimulator.Engine.MathAddition;
using System;

namespace PhysicsSimulator.Engine.Interaction.Core
{
    /// <summary>
    /// Třída realizujcí základní interakční objekt = obdelník, který má strany rovnoběžné s osami x a y. Výpočet kolize dvou Boxů je rychlý a jednoduchý a proto zvyšuje efektivitu, protože v drtivé většině případu zamezuje řešení složitějších kolizí. 
    /// </summary>
    [Serializable]
    class Box
    {
        /// <summary>
        /// prvni bod urcujci obdelnik
        /// </summary>
        private Vector2 min;
        /// <summary>
        /// druhy bod urcujci obdelnik
        /// </summary>
        private Vector2 max;

        /// <summary>
        /// pomocny vykreslovac
        /// </summary>
        private VectorDrawer vd;

        /// <summary>
        /// druhy bod urcujci obdelnik
        /// </summary>
        public Vector2 Max
        {
            get { return max; }
            set { max = value; }
        }



        /// <summary>
        /// prvni bod urcujci obdelnik
        /// </summary>
        public Vector2 Min
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// barva pri vykreslovani
        /// </summary>
        private Color color;


        /// <summary>
        /// barva pri vykreslovani
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; Update(); }
        }

        /// <summary>
        /// zda se bude vykreslovat ci ne
        /// </summary>
        private bool drawAble = false;


        /// <summary>
        /// zda se bude vykreslovat ci ne
        /// </summary>
        public bool DrawAble
        {
            get { return drawAble; }
            set { drawAble = value; }
        }

        /// <summary>
        /// Inicializuje novy obdelnik
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Box(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
            vd = new VectorDrawer();
            color = Color.Peru;
            vd.Add(min, max - min, Color.Peru);
            vd.Add(min, new Vector2(max.X, min.Y) - min, Color.Peru);
            vd.Add(min, new Vector2(min.X, max.Y) - min, Color.Peru);

        }

        /// <summary>
        /// otestuje zda tento obdelnik interaguje s tim v parametru
        /// </summary>
        /// <param name="box"></param>
        /// <returns>pokud ano true</returns>
        private bool Check(Box box)
        {
            if (Helper.IsInInterval(new Vector2(min.X, max.X), box.min.X) || Helper.IsInInterval(new Vector2(min.X, max.X), box.max.X))
                if (Helper.IsInInterval(new Vector2(min.Y, max.Y), box.min.Y) || Helper.IsInInterval(new Vector2(min.Y, max.Y), box.max.Y))
                    return true;
                else
                    return false;
            else
                return false;
        }


        /// <summary>
        /// Otestuje jestli tenhle interaguje stamtim nebo tamtem s timhle 
        /// </summary>
        /// <param name="box"></param>
        /// <returns>kdyz jo true</returns>
        public virtual bool Intersects(Box box)
        {
            return this.Check(box) || box.Check(this);
        }

        /// <summary>
        /// Vypocita zda interaguje s podem
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual bool Intersects(Vector2 v)
        {
            return Helper.IsInInterval(new Vector2(min.X, max.X), v.X) && Helper.IsInInterval(new Vector2(min.Y, max.Y), v.Y);
        }


        /// <summary>
        /// Aktualizuje data pro vykreslovani
        /// </summary>
        private void Update()
        {
            vd.Clear();
            vd.Add(min, max - min, color);
            vd.Add(min, new Vector2(max.X, min.Y) - min, color);
            vd.Add(min, new Vector2(min.X, max.Y) - min, color);
        }


        /// <summary>
        /// vykresli
        /// </summary>
        public virtual void Draw()
        {
            //if (drawAble) //TODO dat na zpet
            vd.Draw();

        }


    }
}
