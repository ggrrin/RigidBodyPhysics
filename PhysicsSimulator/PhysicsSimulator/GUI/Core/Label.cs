using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;

namespace PhysicsSimulator.GUI
{
    /// <summary>
    /// Třída realizujcí výpis textu na obrazovku v určitém prostoru o určité velikosti. Podporuje více řádkové vypisování a zarovnání na střed.
    /// </summary>
    class Label : IFenceObject
    {
        private string text;
        private bool isMultiLine;
        private Vector2 location;
        private Vector2 size;
        private float textScale;
        private float textSize;
        private bool isCenter;
        private bool visible;
        private float layerDepth;


        //Graphic
        private SpriteFont font;
        private Color color;

        //toUse
        private List<string> rows;
        private Vector2 margin;

        #region "Properties"

        bool enabled = true;

        /// <summary>
        /// u popisku nic nedela jen implmentuje rozhrani
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// vrstva do ktere vykreslovat
        /// </summary>
        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value + 0.01f; }
        }

        /// <summary>
        /// zda je videt ci ne
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        /// <summary>
        /// text popisku 
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }


        /// <summary>
        /// velikost pouzivaneho textu
        /// </summary>
        public float TextSize
        {
            get
            {
                return textSize;
            }
            set
            {
                textSize = value;
                textScale = textSize / font.MeasureString("A").Y;
            }
        }

        /// <summary>
        /// centrovat text na stred
        /// </summary>
        public bool IsCenter
        {
            get { return isCenter; }
            set { isCenter = value; }
        }


        /// <summary>
        /// je viceradkovu 
        /// </summary>
        public bool IsMultiLine
        {
            get { return isMultiLine; }
            set
            {
                isMultiLine = value;
            }
        }

        /// <summary>
        /// pozice labelu 
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
        /// rozmery labelu
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                size = new Vector2(size.X, size.Y);
            }
        }

        /// <summary>
        /// pouzity font
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                textScale = textSize / font.MeasureString("A").Y;
            }
        }

        /// <summary>
        /// pouzita barva
        /// </summary>
        public Color @Color
        {
            get { return color; }
            set { color = value; }
        }

        #endregion


        /// <summary>
        /// inicializuje zakladni label
        /// </summary>
        public Label()
        {
            rows = new List<string>();
            font = Configuration.content.Load<SpriteFont>("Fonts\\font");

            TextSize = 10;
            Text = "Label";
            IsMultiLine = false;
            Location = new Vector2(0, 0);
            Size = new Vector2(font.MeasureString(text).X, font.MeasureString(text).Y);
            Color = Color.White;
            IsCenter = false;
            Visible = true;
            layerDepth = 0.62f;

        }

        /// <summary>
        /// aktualizuje vse potrebne pro vykresleni
        /// </summary>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public void Update(GameTime time, Vector2 margin)
        {
            if (visible)
            {
                EditText();
            }
        }


        /// <summary>
        /// vykresli popisek
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public void Draw(SpriteBatch sprite, GameTime time, Vector2 margin)
        {
            if (visible)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    sprite.DrawString(font, rows[i].ToString(), (this.margin + margin + location + new Vector2(0, i * textSize * Configuration.menuScale)) * Configuration.menuScale, color, 0.0f, Vector2.Zero, textScale * Configuration.menuScale, SpriteEffects.None, layerDepth);
                }
            }

        }

        /// <summary>
        /// pripravy text pro vykrleseni tak aby byl centrovany pripadne vice radkovy 
        /// </summary>
        private void EditText()
        {
            if (text != string.Empty)
            {
                rows.Clear();

                if (textSize > size.Y)
                {
                    return;
                }
                else if (font.MeasureString(text).X * textScale > size.X)
                {
                    if (!isMultiLine)
                    {
                        string subString;
                        int i = 1;
                        do
                        {
                            subString = text.Substring(0, text.Length - i);
                            i++;
                        }
                        while (font.MeasureString(subString).X * textScale > size.X);
                        rows.Add(subString);
                        return;
                    }
                    else
                    {
                        int allowRows = (int)(size.Y / textSize);
                        int rowCount = 0;
                        string[] words = text.Split(' ');
                        StringBuilder s = new StringBuilder();
                        foreach (string word in words)
                        {
                            if (font.MeasureString(word).X * textScale > size.X)
                            {
                                string subString;
                                int i = 1;
                                do
                                {
                                    subString = text.Substring(0, text.Length - i);
                                    i++;
                                }
                                while (font.MeasureString(subString).X * textScale > size.X);
                                rows.Add(subString);
                            }
                            else if (font.MeasureString(s.ToString()).X * textScale + font.MeasureString(word).X * textScale < size.X)
                            {
                                s.Append(word);
                                s.Append(' ');
                            }
                            else
                            {
                                rows.Add(s.ToString());
                                s.Clear();
                                s.Append(word);
                                s.Append(' ');
                                rowCount++;
                            }
                            if (rowCount == allowRows)
                                return;
                        }
                        rows.Add(s.ToString());
                        return;
                    }
                }
                rows.Add(text);
                if (isCenter)
                {
                    margin = new Vector2((size.X - font.MeasureString(rows[0]).X * textScale) / 2, (size.Y - font.MeasureString(rows[0]).Y * textScale) / 2);
                }
            }
            else
            {
                rows = new List<string>();
                rows.Add("");
            }
        }


    }
}
