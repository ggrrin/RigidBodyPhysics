using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PhysicsSimulator.GUI
{
    /// <summary>
    /// Třída reprezentující tlačítko v GUI. Poskytuje zakladní věci jako kliknutí, prejetí myší, text, pozadí v různých stavech.
    /// </summary>
    class Button : IFenceObject
    {
        private UseKeyboard useKeyboard;
        protected bool useEnter;

        //mouse
        private MouseState mouse;
        private MouseState previousMouse;
        private Rectangle mouseRec;

        //keyboard
        private KeyboardState keyboard;
        private KeyboardState previousKeyboard;

        //Graphic
        private Texture2D backTexture;
        private Texture2D backTextureHover;
        private Color tinge;
        private Color tingeHover;
        private Vector2 location;
        private Vector2 size;
        private Label label;
        private int index;
        private bool visible;

        //Events
        public event EventHandler OnClick;
        public event EventHandler MouseEnter;
        public event EventHandler TextChanged;

        //using
        private Texture2D usingTexture;
        private Color usingTinge;
        private float layerDepth;
        private bool isSelect;

        //sound
        private SoundEffect mouseEnterSound;
        private SoundEffect onClickSound;


        #region "Properties"

        bool enabled = true;

        /// <summary>
        /// zda je button klikatelny pokud je false zesedne
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// zvuk pri kliknuti na tlacitko
        /// </summary>
        public SoundEffect OnClickSound
        {
            get { return onClickSound; }
            set { onClickSound = value; }
        }

        /// <summary>
        /// zvuk pro prejeti mysi po tlacitku
        /// </summary>

        public SoundEffect MouseEnterSound
        {
            get { return mouseEnterSound; }
            set { mouseEnterSound = value; }
        }


        /// <summary>
        /// vrstva vygreslovani
        /// </summary>
        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value + 0.01f; label.LayerDepth = this.layerDepth + 0.01f; }
        }

        /// <summary>
        /// dany usekybor t ktereho je button soucasti
        /// </summary>

        public UseKeyboard UseKeyboard
        {
            get { return useKeyboard; }
            set { useKeyboard = value; }
        }


        /// <summary>
        /// urcuje zda zobrazovat tlacitko
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; label.Visible = value; }
        }


        /// <summary>
        /// urcuje poradi button pokud je pozivano useKeyboard
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }


        /// <summary>
        /// urcuje texturu pozdi buton standartni
        /// </summary>
        public Texture2D BackTexture
        {
            get { return backTexture; }
            set { backTexture = value; }
        }


        /// <summary>
        /// urcuje texturu buttonu po prejeti mysi
        /// </summary>
        public Texture2D BackTextureHover
        {
            get { return backTextureHover; }
            set { backTextureHover = value; }
        }


        /// <summary>
        /// priparveni tlitka danou barvou (chytne odstin dane barvy)
        /// </summary>
        public Color Tinge
        {
            get { return tinge; }
            set { tinge = value; }
        }


        /// <summary>
        /// pribarveni po prejeti mysi
        /// </summary>
        public Color TingeHover
        {
            get { return tingeHover; }
            set { tingeHover = value; }
        }


        /// <summary>
        /// text zobrazovany na tlacitku
        /// </summary>
        public string Text
        {
            get { return label.Text; }
            set
            {
                label.Text = value;
                if (TextChanged != null)
                    TextChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// velikost pisma
        /// </summary>
        public float TextSize
        {
            get
            {
                return label.TextSize;
            }
            set
            {
                label.TextSize = value;
            }
        }

        /// <summary>
        /// font ktery pouzit na text tlacitka
        /// </summary>
        public SpriteFont Font
        {
            get { return label.Font; }
            set { label.Font = value; }
        }

        /// <summary>
        /// barva textu kterou vykreslit text
        /// </summary>
        public Color TextColor
        {
            get { return label.Color; }
            set { label.Color = value; }
        }


        /// <summary>
        /// pozice tlacitka vzhledem k objektu ve kterem je
        /// </summary>
        public Vector2 Location
        {
            get { return location; }
            set { location = value; location = new Vector2(location.X, location.Y); }
        }


        /// <summary>
        /// rozmery tlacitka
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = new Vector2(value.X, value.Y);
                label.Size = value;
            }
        }

        #endregion


        /// <summary>
        /// inicializuje zakladni vzhled buttonu
        /// </summary>
        public Button()
        {
            useEnter = true;
            label = new Label();
            BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton0");
            BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton0");
            MouseEnterSound = Configuration.content.Load<SoundEffect>("Sounds\\Menus\\button1");
            OnClickSound = Configuration.content.Load<SoundEffect>("Sounds\\Menus\\button2");
            usingTexture = backTexture;
            tinge = Color.White;
            tingeHover = Color.White;
            Size = new Vector2(150, 50);
            Location = new Vector2(0, 0);
            label.Text = "Button";
            label.IsCenter = true;
            label.Size = size;
            index = -1;
            Visible = true;
            previousMouse = Mouse.GetState();
            previousKeyboard = Keyboard.GetState();
            layerDepth = 0.61f;

            MouseEnter += new EventHandler(Button_MouseEnter);
            OnClick += new EventHandler(Button_OnClick);

        }


        /// <summary>
        /// aktualizacu informaci v buttonu nutna pro vykresleni
        /// </summary>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public void Update(GameTime time, Vector2 margin)
        {
            if (visible)
            {
                if (!enabled)
                {
                    label.Color = Color.LightGray;
                    label.Update(time, new Vector2(location.X + margin.X, location.Y + margin.Y));
                    usingTexture = BackTexture;
                    usingTinge = Color.Gray;
                    return;
                }
                else
                {
                    label.Color = TextColor;
                }



                //mouse
                mouse = Mouse.GetState();
                mouseRec = new Rectangle(mouse.X, mouse.Y, 1, 1);

                //keyboard
                keyboard = Keyboard.GetState();

                bool use = false;

                if (useKeyboard != null)
                {
                    if (index == useKeyboard.SelectIndex)
                        use = true;
                }

                bool isMouseRec = new Rectangle((int)((location.X + margin.X) * Configuration.menuScale), (int)((location.Y + margin.Y) * Configuration.menuScale), (int)(size.X * Configuration.menuScale), (int)(size.Y * Configuration.menuScale)).Intersects(mouseRec);

                if (isMouseRec || use)
                {
                    if (useKeyboard != null)
                        useKeyboard.SelectIndex = index;


                    if (!isSelect)
                    {
                        isSelect = true;
                        if (MouseEnter != null)
                            MouseEnter(this, new EventArgs());

                    }

                    usingTexture = backTextureHover;
                    usingTinge = tingeHover;

                    if ((mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released && isMouseRec) || (useEnter ? (keyboard.IsKeyDown(Keys.Enter) && previousKeyboard.IsKeyUp(Keys.Enter)) : false))
                        if (OnClick != null)
                            OnClick(this, new EventArgs());
                }
                else
                {
                    usingTexture = backTexture;
                    usingTinge = tinge;
                    isSelect = false;
                }

                label.Update(time, new Vector2(location.X + margin.X, location.Y + margin.Y));

                previousMouse = mouse;
                previousKeyboard = keyboard;
            }
        }

        /// <summary>
        /// vykresleni button podle jeho vlastnosti
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public void Draw(SpriteBatch sprite, GameTime time, Vector2 margin)
        {
            if (visible)
            {
                sprite.Draw(usingTexture, new Rectangle((int)((location.X + margin.X) * Configuration.menuScale), (int)((location.Y + margin.Y) * Configuration.menuScale), (int)(size.X * Configuration.menuScale), (int)(size.Y * Configuration.menuScale)), null, usingTinge, 0, Vector2.Zero, SpriteEffects.None, layerDepth);
                label.Draw(sprite, time, new Vector2(margin.X + location.X, margin.Y + location.Y));
            }
        }


        /// <summary>
        /// pridan zvuk pri najeti na tlacitku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (mouseEnterSound != null)
                mouseEnterSound.Play();
        }


        /// <summary>
        /// pridani zvuku pri kliku na tlacitko
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Button_OnClick(object sender, EventArgs e)
        {
            onClickSound.Play();
        }
    }
}
