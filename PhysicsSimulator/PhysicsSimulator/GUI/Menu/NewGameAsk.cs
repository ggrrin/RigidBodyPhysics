using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.GUI.Menu
{
    /// <summary>
    /// Třída reprezentuje okénko pro dotaz zda chce uživatel přemazat stávající proil při startu nové hry.
    /// </summary>
    class NewGameAsk : Fence
    {
        private Label text;
        private Button yes;
        private Button no;
        private UseKeyboard keyboardBox;


        /// <summary>
        /// delegat pro udlost exti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ExitEventHandler(object sender, ExitEventArgs e);

        /// <summary>
        /// udalost ukonceni okna
        /// </summary>
        public event ExitEventHandler Exit;


        /// <summary>
        /// inicializuje okno
        /// </summary>
        public NewGameAsk()
        {
            InitializeComponent();
        }


        /// <summary>
        /// inicializuje vnitrni komponennty
        /// </summary>
        private void InitializeComponent()
        {
            //Create new instance
            text = new Label();
            yes = new Button();
            no = new Button();

            keyboardBox = new UseKeyboard();

            //kyeboardBox
            Controls.Add(keyboardBox);

            //text
            Controls.Add(text);
            text.Text = "Opravdu chcete začít novou hru? Předchozí hra bude ztracena!";
            text.IsMultiLine = true;
            text.IsMultiLine = false;
            text.IsCenter = true;
            text.Location = new Vector2(10, 90);
            text.Size = new Vector2(880, 40);
            text.TextSize = 35;

            //yes
            keyboardBox.Add(yes);
            yes.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            yes.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            yes.Location = new Vector2(150, 150);
            yes.Size = new Vector2(250, 70);
            yes.TextSize = 35;
            yes.Index = 0;
            yes.Text = "Ano";
            yes.OnClick += new EventHandler(yes_OnClick);

            //no
            keyboardBox.Add(no);
            no.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            no.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            no.Location = new Vector2(500, 150);
            no.Size = new Vector2(250, 70);
            no.TextSize = 35;
            no.Index = 1;
            no.Text = "Ne";
            no.OnClick += new EventHandler(no_OnClick);

            //this Fenece
            this.Size = new Vector2(900, 300);
            this.Location = new Vector2(510, 390);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMainMenu");
            this.BackImageSizemode = SizeMode.extend;
            LayerDepth = 0.65f;
        }

        /// <summary>
        /// nastavi argumenty udalosti pro opusteni  okna
        /// </summary>
        /// <param name="isContinue"></param>
        private void PrepareExitEventArgs(bool isContinue)
        {
            ExitEventArgs e = new ExitEventArgs();
            e.IsContinue = isContinue;
            if (Exit != null)
            {
                Exit(this, e);
            }
        }

        #region "Events"

        /// <summary>
        /// obsluha udlaosti zamitnuti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void no_OnClick(object sender, EventArgs e)
        {
            PrepareExitEventArgs(false);
        }

        /// <summary>
        /// obsluha udalosti potvrzeni akce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yes_OnClick(object sender, EventArgs e)
        {
            PrepareExitEventArgs(true);
        }

        #endregion
    }

}
