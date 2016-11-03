using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.GUI.Menu
{
    /// <summary>
    /// Okenko ve hře umožňujcí nastavit rotaci daného objektu pomocí posuvníku.
    /// </summary>
    class RotationBar : Fence
    {
        Button roller;

        const float ROLlER_MAX = 10;
        const float ROLLER_MIN = 970;
        public bool slidingMode = false;


        /// <summary>
        /// delegat pro nastaveni zmene hodnoty udalosti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OnValSetEventHandler(object sender, ExitEventArgs e);

        /// <summary>
        /// udalost oznamujci zmenu hodnoty rolleru 
        /// </summary>
        public event OnValSetEventHandler OnValSet;

        /// <summary>
        /// nastaveni hodnoty do GUI
        /// </summary>
        public float Value
        {
            get { return (roller.Location.Y - 490) / 480; }
            set { roller.Location = new Vector2(roller.Location.X, (value * 480) + 490); }
        }

        Label l;

        /// <summary>
        /// inicializace okna s rollerem
        /// </summary>
        public RotationBar()
        {
            l = new Label();
            l.Location = new Vector2(10, 480);
            l.Size = new Vector2(300, 50);
            l.Text = "0";
            l.IsCenter = false;
            l.TextSize = 50;
            Controls.Add(l);

            roller = new Button();

            //restart            
            roller.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            roller.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            roller.Location = new Vector2(50, 490);
            roller.Size = new Vector2(70, 30);
            roller.TextSize = 30;
            roller.Text = "";
            roller.OnClick += new EventHandler(roller_OnClick);
            Controls.Add(roller);

            //fence
            Location = new Vector2(1790, 100);
            Size = new Vector2(250, 1920);
            BackImage = null;
            BackColor = Color.Black;
        }


        /// <summary>
        /// potrebny update pozice loreru
        /// </summary>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public override void Update(GameTime time, Vector2 margin)
        {
            base.Update(time, margin);


            if (slidingMode)
                roller.Location = new Vector2(roller.Location.X, Math.Max(Math.Min((Mouse.GetState().Y / Configuration.menuScale) - this.Location.Y - 15, ROLLER_MIN), ROLlER_MAX));
            //l.Text = (roller.Location.Y - 490).ToString();
        }


        /// <summary>
        /// obsluha udalosti kliknuti na roler ulozeni/zmena hodnoty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void roller_OnClick(object sender, EventArgs e)
        {
            slidingMode ^= true;

            if (!slidingMode)
                if (OnValSet != null)
                    OnValSet(this, new ExitEventArgs((roller.Location.Y - 490) / 480));
        }


    }
}
