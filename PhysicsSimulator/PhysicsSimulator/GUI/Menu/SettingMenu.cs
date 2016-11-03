using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhysicsSimulator.GUI.Menu
{
    /// <summary>
    /// Třída reprezentující okenko s nastavením rozlišení aplikace v hlavním menu.
    /// </summary>
    class SettingMenu : Fence
    {
        #region "Declaration"

        //Buttons
        private Button exitButton;
        private Button previousResulutionButton;
        private Button nextResulutionButton;

        private Button previousWindowButton;
        private Button nextWindowButton;

        private Button applyButton;
        private Button changeIsfullscreen;

        //Labels
        private Label titleLabel;
        private Label isFullscreenLable;
        private Label resolutionLabel;

        private UseKeyboard keyboardBox;

        //events
        /// <summary>
        /// udalost zaverni okna
        /// </summary>
        public event EventHandler Exit;

        #endregion


        /// <summary>
        /// inicializace okna
        /// </summary>
        public SettingMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// inicializace vnitrnich komponent
        /// </summary>
        private void InitializeComponent()
        {
            //create new intance            
            keyboardBox = new UseKeyboard();

            exitButton = new Button();
            previousResulutionButton = new Button();
            nextResulutionButton = new Button();
            applyButton = new Button();
            changeIsfullscreen = new Button();
            previousWindowButton = new Button();
            nextWindowButton = new Button();

            titleLabel = new Label();
            isFullscreenLable = new Label();
            resolutionLabel = new Label();

            //keyboard
            Controls.Add(keyboardBox);

            //previousResulutionButton
            keyboardBox.Add(previousResulutionButton);
            previousResulutionButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartLeft0");
            previousResulutionButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartLeft1");
            previousResulutionButton.Location = new Vector2(320, 140);
            previousResulutionButton.Size = new Vector2(100, 100);
            previousResulutionButton.TextSize = 30;
            previousResulutionButton.Index = 0;
            previousResulutionButton.Text = "";
            previousResulutionButton.OnClick += new EventHandler(previousResulutionButton_OnClick);

            //nextResulutionButton
            keyboardBox.Add(nextResulutionButton);
            nextResulutionButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartRight0");
            nextResulutionButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartRight1");
            nextResulutionButton.Location = new Vector2(690, 140);
            nextResulutionButton.Size = new Vector2(100, 100);
            nextResulutionButton.TextSize = 30;
            nextResulutionButton.Index = 1;
            nextResulutionButton.Text = "";
            nextResulutionButton.OnClick += new EventHandler(nextResulutionButton_OnClick);

            //previousWindowButton
            keyboardBox.Add(previousWindowButton);
            previousWindowButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartLeft0");
            previousWindowButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartLeft1");
            previousWindowButton.Location = new Vector2(320, 240);
            previousWindowButton.Size = new Vector2(100, 100);
            previousWindowButton.TextSize = 30;
            previousWindowButton.Index = 2;
            previousWindowButton.Text = "";
            previousWindowButton.OnClick += new EventHandler(previousWindowButton_OnClick);

            //nextWindowButton
            keyboardBox.Add(nextWindowButton);
            nextWindowButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartRight0");
            nextWindowButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\DartRight1");
            nextWindowButton.Location = new Vector2(690, 240);
            nextWindowButton.Size = new Vector2(100, 100);
            nextWindowButton.TextSize = 30;
            nextWindowButton.Index = 3;
            nextWindowButton.Text = "";
            nextWindowButton.OnClick += new EventHandler(previousWindowButton_OnClick);

            //exitbutton
            keyboardBox.Add(exitButton);
            exitButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            exitButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            exitButton.Location = new Vector2(70, 350);
            exitButton.Size = new Vector2(436, 98);
            exitButton.TextSize = 60;
            exitButton.Index = 4;
            exitButton.Text = "OK";
            exitButton.OnClick += new EventHandler(exitButton_OnClick);

            //applyButton
            keyboardBox.Add(applyButton);
            applyButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            applyButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            applyButton.Location = new Vector2(594, 350);
            applyButton.Size = new Vector2(436, 98);
            applyButton.TextSize = 60;
            applyButton.Index = 5;
            applyButton.Text = "Apply";
            applyButton.OnClick += new EventHandler(applyButton_OnClick);

            //title
            Controls.Add(titleLabel);
            titleLabel.Text = "Nastavení Grafiky";
            titleLabel.IsMultiLine = false;
            titleLabel.IsCenter = true;
            titleLabel.Location = new Vector2(275, 30);
            titleLabel.Size = new Vector2(550, 100);
            titleLabel.TextSize = 80;

            //isFullscreenLable
            Controls.Add(isFullscreenLable);
            isFullscreenLable.Text = Configuration.settingG.IsFullScreen ? "full screen" : "v okně";
            isFullscreenLable.IsMultiLine = false;
            isFullscreenLable.IsCenter = true;
            isFullscreenLable.Location = new Vector2(420, 240);
            isFullscreenLable.Size = new Vector2(270, 100);
            isFullscreenLable.TextSize = 55;

            //Resolution
            Controls.Add(resolutionLabel);
            resolutionLabel.Text = Configuration.settingG.GetResolution().ToString();
            resolutionLabel.IsMultiLine = false;
            resolutionLabel.IsCenter = true;
            resolutionLabel.Location = new Vector2(420, 140);
            resolutionLabel.Size = new Vector2(270, 100);
            resolutionLabel.TextSize = 55;

            //this form
            Size = new Vector2(1100, 550);
            Location = new Vector2(100, 100);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMainMenu");
            this.BackImageSizemode = SizeMode.extend;
            LayerDepth = 0.65f;
        }

        #region "Events"

        /// <summary>
        /// pri opusteni okna ulozit rozliseni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton_OnClick(object sender, EventArgs e)
        {
            Configuration.settingG.SaveResolution();
            Configuration.ReinitializeGraphics();

            if (Exit != null)
                Exit(this, new EventArgs());
        }


        /// <summary>
        /// aktualizovat rozliseni podle vybranych dat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyButton_OnClick(object sender, EventArgs e)
        {
            Configuration.settingG.SaveResolution();
            Configuration.ReinitializeGraphics();
        }


        /// <summary>
        /// vybrat predchozi rozliseni v poradi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previousResulutionButton_OnClick(object sender, EventArgs e)
        {
            Configuration.settingG.PreviousResolution();
            resolutionLabel.Text = Configuration.settingG.GetResolution().ToString();
        }


        /// <summary>
        /// vybrat nasledujici rozliseni v poradi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextResulutionButton_OnClick(object sender, EventArgs e)
        {
            Configuration.settingG.NextResolution();
            resolutionLabel.Text = Configuration.settingG.GetResolution().ToString();
        }


        /// <summary>
        /// nastaveni fullscreen modu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previousWindowButton_OnClick(object sender, EventArgs e)
        {
            if (Configuration.settingG.IsFullScreen)
            {
                Configuration.settingG.IsFullScreen = false;
                isFullscreenLable.Text = "v okně";
            }
            else
            {
                Configuration.settingG.IsFullScreen = true;
                isFullscreenLable.Text = "full screen";
            }
        }

        #endregion
    }
}
