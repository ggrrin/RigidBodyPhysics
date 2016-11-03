using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsSimulator.GUI
{
    /// <summary>
    /// Třída realizujcí obrázek jako prvek GUI.
    /// </summary>
    class Image : IFenceObject
    {
        private Texture2D texture;
        private int width;
        private int height;
        private SizeMode sizeMode;
        private Rectangle rectangle;
        private Rectangle cutRectangle;
        private Vector2 margin;
        private Color tinge;
        private float rotation;
        private Vector2 origin;
        private SpriteEffects spriteEffects;
        private float layerDepth;

        #region "Properties"

        bool enabled = true;

        /// <summary>
        /// u obrazku nedela nic jen implemntuje rozhrani
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// textura daneho robrazku
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                if (value != null) SetRectangles();
            }
        }

        /// <summary>
        /// siraka 
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = (int)((float)value); SetRectangles(); }
        }

        /// <summary>
        /// vyska
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = (int)((float)value); SetRectangles(); }
        }


        /// <summary>
        /// okraje
        /// </summary>
        public Vector2 Margin
        {
            get { return margin; }
            set { margin = value; }
        }

        /// <summary>
        /// jak zachazet s rozmery obrazku pri scalovani
        /// </summary>
        public SizeMode SizeMode
        {
            get { return sizeMode; }
            set { sizeMode = value; SetRectangles(); }
        }


        /// <summary>
        /// pribarveni obrazku danou barvou
        /// </summary>
        public Color Tinge
        {
            get { return tinge; }
            set { tinge = value; }
        }


        /// <summary>
        /// otoceni obrazku
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// bod otoceni obrazku
        /// </summary>
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        /// <summary>
        /// dany effect
        /// </summary>
        public SpriteEffects SpriteEffects
        {
            get { return spriteEffects; }
            set { spriteEffects = value; }
        }

        /// <summary>
        /// vrstva vykreslovani
        /// </summary>
        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value; }
        }

        #endregion


        /// <summary>
        /// inicializuje zakaladni obrazek
        /// </summary>
        public Image()
        {
            this.texture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMainMenu");
            width = texture.Width;
            height = texture.Height;
            sizeMode = SizeMode.noExtend;
            SetRectangles();
            margin = Vector2.Zero;
            rotation = 0f;
            tinge = Color.White;
            origin = Vector2.Zero;
            spriteEffects = SpriteEffects.None;
            layerDepth = 0.605f;
        }

        public void Update(GameTime time, Vector2 margin)
        {

        }

        /// <summary>
        /// vykresli obrazek
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public void Draw(SpriteBatch sprite, GameTime time, Vector2 margin)
        {
            if (Texture != null)
                sprite.Draw(texture, new Rectangle((int)((rectangle.X + margin.X + this.margin.X) * Configuration.menuScale), (int)((rectangle.Y + margin.Y + this.margin.Y) * Configuration.menuScale), (int)(rectangle.Width * Configuration.menuScale), (int)(rectangle.Height * Configuration.menuScale)), cutRectangle, tinge, rotation, origin, spriteEffects, layerDepth);
        }

        /// <summary>
        /// nastaveni dat pro vykresleni podle sizemode
        /// </summary>
        private void SetRectangles()
        {
            if (sizeMode == SizeMode.extedToHeight)
            {
                rectangle = new Rectangle(0, 0, (int)(texture.Width * ((float)height / (float)texture.Height)) > this.width ? this.width : (int)(texture.Width * ((float)height / (float)texture.Height)), height);
                cutRectangle = new Rectangle(0, 0, (int)(texture.Width * ((float)width / (float)(texture.Width * ((float)height / (float)texture.Height)))) > texture.Width ? texture.Width : (int)(texture.Width * ((float)width / (float)(texture.Width * ((float)height / (float)texture.Height)))), texture.Height);
            }
            else if (sizeMode == SizeMode.extedToWidth)
            {
                rectangle = new Rectangle(0, 0, width, (int)(texture.Height * ((float)width / (float)texture.Width)) > this.height ? this.height : (int)(texture.Height * ((float)width / (float)texture.Width)));
                cutRectangle = new Rectangle(0, 0, texture.Width, (int)(texture.Height * ((float)height / (float)(texture.Height * ((float)width / (float)texture.Width)))) > texture.Height ? texture.Height : (int)(texture.Height * ((float)height / (float)(texture.Height * ((float)width / (float)texture.Width)))));
            }
            else if (sizeMode == SizeMode.extend)
            {
                rectangle = new Rectangle(0, 0, width, height);
                cutRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }
            else if (sizeMode == SizeMode.noExtend)
            {
                rectangle = new Rectangle(0, 0, texture.Width > width ? width : texture.Width, texture.Height > height ? height : texture.Height);
                cutRectangle = rectangle;
            }

        }
    }

    /// <summary>
    /// Enum určujcí jak se zachovat k objektu pri scalování
    /// </summary>
    enum SizeMode
    {
        /// <summary>
        /// Obrázek se neroztáhne zustane jako originál
        /// </summary>
        noExtend,
        /// <summary>
        /// Obrázek se roztáhne a zdeformuje
        /// </summary>
        extend,
        /// <summary>
        /// Obrázek se roztáhne podle výšky a zachová poměr stran
        /// </summary>
        extedToHeight,
        /// <summary>
        /// Obrázek se rotáhne podle šířky a zachová poměr stran
        /// </summary>
        extedToWidth
    }
}
