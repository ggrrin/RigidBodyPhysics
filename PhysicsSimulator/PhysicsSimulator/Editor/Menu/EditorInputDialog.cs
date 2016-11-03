using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.GUI;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.Editor.Menu
{

    /// <summary>
    /// Třída realizující vyskakovací dialog s možností zdání v stupních hodnot z {0123456789,-} na NEnumerické klavesnici v Editoru
    /// </summary>
    class EditorInputDialog : Fence
    {
        /// <summary>
        /// input
        /// </summary>
        KeyboardState keyboard, previousKeyboard;


        /// <summary>
        /// zakazani vystupni hodnoty nula
        /// </summary>
        public bool NoZero { get; set; }


        /// <summary>
        /// popisek dialogu
        /// </summary>
        public string Text
        {
            get { return title.Text; }
            set { title.Text = value; }
        }



        Label title;
        Label value;
        Label warning;

        Button ok;
        Button cancel;
        private const float BUTTON_HEIGHT = 45;
        private const float BUTTON_WIDTH = 100;
        private const float TEXT_SIZE = 30;


        public delegate void DialogExitEventHandler(object sender, ExitEventArgs args);
        public event DialogExitEventHandler Exit;

        public EditorInputDialog()
        {
            NoZero = false;
            InitializeComponents();
        }

        /// <summary>
        /// Inicializuje komponenty
        /// </summary>
        private void InitializeComponents()
        {
            title = new Label();
            value = new Label();

            warning = new Label();

            ok = new Button();
            cancel = new Button();

            //title
            title.Text = "Zadejte hodnotu :";
            title.IsMultiLine = false;
            title.IsCenter = true;
            title.Location = new Vector2(25, 25);
            title.Size = new Vector2(330, 45);
            title.TextSize = TEXT_SIZE;
            Controls.Add(title);

            //warning
            warning.Text = "";
            warning.IsMultiLine = true;
            warning.IsCenter = false;
            warning.Location = new Vector2(25, 145);
            warning.Size = new Vector2(150, 90);
            warning.IsMultiLine = true;
            warning.TextSize = TEXT_SIZE - 5;
            Controls.Add(warning);


            //value
            value.Text = "";
            value.IsMultiLine = true;
            value.IsCenter = false;
            value.Location = new Vector2(50, 100);
            value.Size = new Vector2(200, 100);
            value.TextSize = TEXT_SIZE;
            Controls.Add(value);

            //velocityX            
            ok.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            ok.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            ok.Location = new Vector2(50, 200);
            ok.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            ok.TextSize = TEXT_SIZE;
            ok.Text = "OK";
            ok.OnClick += new EventHandler(ok_OnClick);
            Controls.Add(ok);

            //velocityY
            cancel.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            cancel.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            cancel.Location = new Vector2(170, 200);
            cancel.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            cancel.TextSize = TEXT_SIZE;
            cancel.Text = "Cancel";
            cancel.OnClick += new EventHandler(cancel_OnClick);
            Controls.Add(cancel);

            //GameSubMenu
            this.Visible = true;
            this.Location = new Vector2(500, 200);
            this.Size = new Vector2(330, 300);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenubackground");
            this.BackImageSizemode = SizeMode.extend;
            //this.LayerDepth = 0.61f; //This goona bloww my miind No idea why ? but if greather then 0.61 it throw exceptino vhen drawing line....brrr
        }

        /// <summary>
        /// nacte status input zarizeni
        /// </summary>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public override void Update(GameTime time, Vector2 margin)
        {
            base.Update(time, margin);
            keyboard = Keyboard.GetState();
            FillLabel();
            previousKeyboard = keyboard;
        }

        public override void Draw(SpriteBatch sprite, GameTime time, Vector2 margin)
        {
            base.Draw(sprite, time, margin);
        }


        /// <summary>
        /// zapisuje vstup z klavesnice na dialog
        /// </summary>
        private void FillLabel()
        {
            if (KeyPressed(Keys.D0))
                value.Text += "0";
            else if (KeyPressed(Keys.D1))
                value.Text += "1";
            else if (KeyPressed(Keys.D2))
                value.Text += "2";
            else if (KeyPressed(Keys.D3))
                value.Text += "3";
            else if (KeyPressed(Keys.D4))
                value.Text += "4";
            else if (KeyPressed(Keys.D5))
                value.Text += "5";
            else if (KeyPressed(Keys.D6))
                value.Text += "6";
            else if (KeyPressed(Keys.D7))
                value.Text += "7";
            else if (KeyPressed(Keys.D8))
                value.Text += "8";
            else if (KeyPressed(Keys.D9))
                value.Text += "9";
            else if (KeyPressed(Keys.OemComma))
                value.Text += ",";
            else if (KeyPressed(Keys.OemMinus))
                value.Text += "-";
            else if (KeyPressed(Keys.Back))
                if (value.Text.Length > 1)
                    value.Text = value.Text.Substring(0, value.Text.Length - 1);
                else if (value.Text.Length == 1)
                    value.Text = "";
        }

        /// <summary>
        /// zda byla dana klavesa stisknuta
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool KeyPressed(Keys key)
        {
            return keyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }


        /// <summary>
        /// zruseni dialogu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_OnClick(object sender, EventArgs e)
        {
            if (Exit != null)
                Exit(this, new ExitEventArgs());
        }


        /// <summary>
        /// potvrzeni dialogu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ok_OnClick(object sender, EventArgs e)
        {
            float res;
            if (float.TryParse(value.Text, out res))
            {
                if (NoZero && res == 0)
                {
                    warning.Text = "Nula neprijatelna!";
                }
                else if (Exit != null)
                    Exit(this, new ExitEventArgs(value.Text));
            }
            else
            {
                warning.Text = "Nelze zkonvertovat do float";
            }
        }




    }
}
