using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PhysicsSimulator.GUI
{
    /// <summary>
    /// Třída reprezentujcí okénko GUI. Podporuje obsah dalších IFenceObjectů, pozicovaných od pozice daného okna.
    /// </summary>
    class Fence : IFenceObject
    {
        //Graphic
        private Color backColor;
        private Texture2D backTexture;
        private Image backgroundImage;
        private Vector2 location;
        private Vector2 size;
        private bool visible;
        private float layerDepth;

        //
        private List<IFenceObject> controls;
        private Fence innerFence;

        private GraphicsDevice graphic;

        //events
        public event EventHandler OnUpdate;

        #region "Properties"

        bool enabled = true;

        /// <summary>
        /// urcuje zda je okenko povoleno coz v tomot pripade znamena false => nezobrazuji se jeho vnitri prvkyy ; true => vnitrni prvky se zobrazuji
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        /// <summary>
        /// textura pozadi okna
        /// </summary>
        public Texture2D BackImage
        {
            get { return backgroundImage.Texture; }
            set { backgroundImage.Texture = value; }

        }

        /// <summary>
        /// jak zachazet s tekturou na pozadi
        /// </summary>
        public SizeMode BackImageSizemode
        {
            get { return backgroundImage.SizeMode; }
            set { backgroundImage.SizeMode = value; }

        }


        /// <summary>
        /// vrstava vykreslovani
        /// </summary>
        public float LayerDepth
        {
            get { return layerDepth; }
            set
            {
                layerDepth = value;
                AktualizeLayers();
                if (backgroundImage != null)
                {
                    backgroundImage.LayerDepth = value + 0.001f;
                }
            }
        }


        /// <summary>
        /// vnitrni dialog
        /// </summary>
        public virtual Fence InnerFence
        {
            get { return innerFence; }
            set { innerFence = value; }
        }


        /// <summary>
        /// zda vykreslovat nebo ne
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }


        /// <summary>
        /// barva pozadi
        /// </summary>
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                backTexture = new Texture2D(graphic, 1, 1, true, SurfaceFormat.Color);
                Color[] c = new Color[1];
                c[0] = backColor;
                backTexture.SetData<Color>(c);
            }
        }



        /// <summary>
        /// pozice okna
        /// </summary>
        public Vector2 Location
        {
            get { return location; }
            set
            {
                location = value;
                location = new Vector2(location.X, location.Y);
            }
        }


        /// <summary>
        /// rozmery okna
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = new Vector2(value.X, value.Y);
                backgroundImage.Width = (int)value.X;
                backgroundImage.Height = (int)value.Y;
            }
        }


        /// <summary>
        /// seznam vnitrnich prvku okna
        /// </summary>
        public List<IFenceObject> Controls
        {
            get { return controls; }
            set { controls = value; }
        }

        #endregion

        /// <summary>
        /// inicializace zakladniho okna
        /// </summary>
        public Fence()
        {
            this.enabled = true;
            this.backgroundImage = new Image();
            this.graphic = Configuration.device;
            this.controls = new List<IFenceObject>();
            this.Location = new Vector2(0, 0);
            this.Size = new Vector2(800, 600);
            this.Visible = true;
            this.LayerDepth = 0.60f;
        }


        /// <summary>
        /// aktualizace vseho potrebneho pro vykresleni a interakci
        /// </summary>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public virtual void Update(GameTime time, Vector2 margin)
        {
            if (Enabled)
                if (visible)
                {
                    if (OnUpdate != null)
                        OnUpdate(this, new EventArgs());

                    if (innerFence != null)
                    {
                        innerFence.Update(time, margin + location);
                    }
                    else
                    {
                        foreach (IFenceObject a in controls)
                        {
                            a.Update(time, margin + location);
                        }
                    }
                }
        }

        /// <summary>
        /// vykresleni okna se vsemi jeho vnitrnimi prvky 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public virtual void Draw(SpriteBatch sprite, GameTime time, Vector2 margin)
        {
            if (visible)
            {
                if (backTexture != null)
                    sprite.Draw(backTexture, new Rectangle((int)((location.X + margin.X) * Configuration.menuScale), (int)((location.Y + margin.Y) * Configuration.menuScale), (int)(size.X * Configuration.menuScale), (int)(size.Y * Configuration.menuScale)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);

                if (backgroundImage != null)
                    backgroundImage.Draw(sprite, time, margin + location);
                if (Enabled)
                    foreach (IFenceObject a in controls)
                    {
                        a.Draw(sprite, time, margin + location);
                    }

                if (innerFence != null)
                {
                    innerFence.Draw(sprite, time, margin + location);
                }
            }

        }

        /// <summary>
        /// zavreni vnitrniho dialogu
        /// </summary>
        private void CloseInnerFence()
        {
            this.innerFence = null;
        }

        /// <summary>
        /// aktualizovat vrstvu vykreslovani tak aby vnitrni prvky byli vykreslovany na okno
        /// </summary>
        private void AktualizeLayers()
        {
            foreach (IFenceObject i in controls)
            {
                i.LayerDepth = layerDepth;
            }
        }

    }



}
