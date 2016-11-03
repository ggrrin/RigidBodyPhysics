using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhysicsSimulator.GUI.Menu
{
    /// <summary>
    /// Třída reprezentující okno submenu hry.
    /// </summary>
    class GameSubMenu : Fence
    {
        #region "Declaration"

        private Label title;

        private Button saveGameButton;
        private Button mainMenuButton;
        private Button resetLevelButton;


        private UseKeyboard keyboardBox;

        #endregion

        public event EventHandler Close;
        public event EventHandler GoToMainMenu;
        public event EventHandler PlayAgain;


        /// <summary>
        /// inicializuje submenu
        /// </summary>
        public GameSubMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// inicializuje komponenty submenu
        /// </summary>
        private void InitializeComponent()
        {
            //Created new instancs
            title = new Label();

            saveGameButton = new Button();
            mainMenuButton = new Button();
            resetLevelButton = new Button();

            keyboardBox = new UseKeyboard();

            //useKeyboard
            Controls.Add(keyboardBox);

            //title
            title.Text = "Herní menu";
            title.IsMultiLine = false;
            title.IsCenter = true;
            title.Location = new Vector2(32, 25);
            title.Size = new Vector2(436, 55);
            title.TextSize = 55;
            Controls.Add(title);



            //resetLevel
            keyboardBox.Add(resetLevelButton);
            resetLevelButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            resetLevelButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            resetLevelButton.Location = new Vector2(32, 190);
            resetLevelButton.Size = new Vector2(436, 98);
            resetLevelButton.TextSize = 45;
            resetLevelButton.Index = 1;
            resetLevelButton.Text = "Restartovat level";
            resetLevelButton.OnClick += new EventHandler(resetLevel_OnClick);

            //mainMenuButton
            keyboardBox.Add(mainMenuButton);
            mainMenuButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            mainMenuButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            mainMenuButton.Location = new Vector2(32, 300);
            mainMenuButton.Size = new Vector2(436, 98);
            mainMenuButton.TextSize = 45;
            mainMenuButton.Index = 2;
            mainMenuButton.Text = "Hlavní menu";
            mainMenuButton.OnClick += new EventHandler(mainMenuButton_OnClick);

            //GameSubMenu
            this.Visible = false;
            this.Location = new Vector2(715, 200);
            this.Size = new Vector2(500, 700);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenubackground");
            this.BackImageSizemode = SizeMode.extend;
            this.Close += new EventHandler(GameSubMenu_Close);
        }


        /// <summary>
        /// obsluha zavreni submenu udalosti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameSubMenu_Close(object sender, EventArgs e)
        {
            Visible = false;
        }


        /// <summary>
        /// obsluha tlacitka pro navrat do hlavniho menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainMenuButton_OnClick(object sender, EventArgs e)
        {
            if (GoToMainMenu != null)
                GoToMainMenu(this, new EventArgs());

        }


        /// <summary>
        /// obsluha udalosti pro restartovani levelu 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetLevel_OnClick(object sender, EventArgs e)
        {
            if (PlayAgain != null)
                PlayAgain(this, new EventArgs());
        }
    }
}
