using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.GUI.Menu
{
    /// <summary>
    /// Třída reprezentujcí hlavní menu celé aplikace.
    /// </summary>
    class MainMenu : Fence
    {

        #region "Declaration"

        private Label text;

        private Button resumeButton;
        private Button newGameButton;
        private Button settingButton;
        private Button exitButton;
        private Button editorButton;

        private UseKeyboard keyboardBox;

        #endregion

        //events
        /// <summary>
        /// udalost volana pri ukonceni hlavniho menu
        /// </summary>
        public event EventHandler OnExit;

        /// <summary>
        /// delegat pro udalost startu hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void StarGameEventHandler(object sender, ExiEventArgs e);

        /// <summary>
        /// zavola udalost stratu hry
        /// </summary>
        public event StarGameEventHandler OnStartGame;


        /// <summary>
        /// udalost startu editoru
        /// </summary>
        public event EventHandler OnStartEditor;

        /// <summary>
        /// inicializace hlavniho menu
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// inicializace vnitrich komponent
        /// </summary>
        private void InitializeComponent()
        {
            //Created new instancs
            text = new Label();

            resumeButton = new Button();
            newGameButton = new Button();
            settingButton = new Button();
            exitButton = new Button();
            editorButton = new Button();
            keyboardBox = new UseKeyboard();

            //text
            text.Text = "Takze cilem hry je dostat  svoje \"sutraky\" do finish lokalit. To jsou zlute \"skoro obdelniky\". Sutraku muze byt obecne n a kazda finsh lokalita pojme jen jeden. Mezi kameny se prepina vlevo nahore prejetim na ikony nebo sipkama. Kazdemu kamenu jde nastavit pocatecni rychlost kliknutim na platno (modra cara je pak vektor rychlosti kamenu), dale lze kazdemu kamenu nastavit pocatecni rotace a to posuvnikem vpravo (prvni kliknuti pro moznost posouvani, druhe ulozeni hodnoty). Po nastaveni pocatecnich hodnot uz je staci uderit do BOOOM a cekat jestli kameny dopluji do svych domecku. Pokud nikoliv a utikaji nenavratne pryc, tak ESC -> Restartovat level. Klavesami - +  lze oddalovat/priblizovat. Ve freelook modu se lze sipkamy pohybovat po mape. Kromne finish lokalit existuji take dalsi, speeder, slower, teleport, rotator. (barvy jednotlivych \"obdelniku\" si lze zjistit v editoru) Teleport teleportuje na pro hrace nezname misto. Teleport muzou pouzit pouze hracovy kameny. V editoru obecne plati ze po vybrani nastroje se ENTERem dokocuje akce a ESC rusi dany tool. Mapy se ukladaji primo do slozky s temi hernimy, takze si je lze prepsat. Dialog podporuje psani pouze cisel( carky a minus) a to opet na nenumericke klavesnici => jmena souboru(map) je jen cislo. BackSpacem  lze v editor mazat body.";
            text.Color = Color.Green;
            text.IsMultiLine = true;
            text.Location = new Vector2(100, 100);
            text.Size = new Vector2(1100, 900);
            text.TextSize = 40;

            //resumeButton
            keyboardBox.Add(resumeButton);
            resumeButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton0");
            resumeButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton1");
            resumeButton.Location = new Vector2(1300, 50);
            resumeButton.Size = new Vector2(475, 122);
            resumeButton.TextSize = 55;
            resumeButton.Index = 0;
            resumeButton.Text = "Pokračovat";
            resumeButton.OnClick += new EventHandler(resumeButton_OnClick);

            //newGameButton
            keyboardBox.Add(newGameButton);
            newGameButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton0");
            newGameButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton1");
            newGameButton.Location = new Vector2(1300, 175);
            newGameButton.Size = new Vector2(475, 122);
            newGameButton.TextSize = 55;
            newGameButton.Index = 1;
            newGameButton.Text = "Nová Hra";
            newGameButton.OnClick += new EventHandler(newGameButton_OnClick);

            //settingButton
            keyboardBox.Add(settingButton);
            settingButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton0");
            settingButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton1");
            settingButton.Location = new Vector2(1300, 300);
            settingButton.Size = new Vector2(475, 122);
            settingButton.TextSize = 55;
            settingButton.Index = 2;
            settingButton.Text = "Nastavení";
            settingButton.OnClick += new EventHandler(settingButton_OnClick);

            //editorButton
            keyboardBox.Add(editorButton);
            editorButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton0");
            editorButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton1");
            editorButton.Location = new Vector2(1300, 425);
            editorButton.Size = new Vector2(475, 122);
            editorButton.TextSize = 55;
            editorButton.Index = 4;
            editorButton.Text = "Editor";
            editorButton.OnClick += new EventHandler(editorButton_OnClick);

            //exitButton
            keyboardBox.Add(exitButton);
            exitButton.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton0");
            exitButton.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainButton1");
            exitButton.Location = new Vector2(1300, 550);
            exitButton.Size = new Vector2(475, 122);
            exitButton.TextSize = 55;
            exitButton.Index = 3;
            exitButton.Text = "Ukončit";
            exitButton.OnClick += new EventHandler(exitButton_OnClick);


            //keyboardBox
            Controls.Add(keyboardBox);
            Controls.Add(text);

            //Mainmenu 
            Size = new Vector2(1921, 1536);
            Location = new Vector2(0, 0);
            BackColor = new Color(166, 218, 232);
            BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\MainMenuBackground");
            this.BackImageSizemode = SizeMode.extedToWidth;
        }

        /// <summary>
        /// zavaolani udalosti startu hry
        /// </summary>
        /// <param name="level"></param>
        private void SetStartGameEventArgs(int level)
        {
            if (OnStartGame != null)
            {
                ExiEventArgs e = new ExiEventArgs();
                e.LevelPath = level;
                OnStartGame(this, e);
            }
        }

        #region "Events"


        /// <summary>
        /// obsluha udalosti ukonceni programu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton_OnClick(object sender, EventArgs e)
        {
            if (OnExit != null)
                OnExit(this, new EventArgs());
        }

        /// <summary>
        /// obsluha udalosti startnu nove hry zoprazeni upozorneni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGameButton_OnClick(object sender, EventArgs e)
        {
            NewGameAsk newGameAsk = new NewGameAsk();
            newGameAsk.Exit += new NewGameAsk.ExitEventHandler(newGameAsk_Exit);
            InnerFence = newGameAsk;
        }

        /// <summary>
        /// obsluha udalosti pokracovani v predchozi hre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resumeButton_OnClick(object sender, EventArgs e)
        {
            SetStartGameEventArgs(Configuration.useProfile.LastLevel);
        }


        /// <summary>
        /// obsluha odalosti pro zobrazeni okenka sn nastavenim rozliseni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingButton_OnClick(object sender, EventArgs e)
        {
            SettingMenu settingMenu = new SettingMenu();
            settingMenu.Exit += new EventHandler(settingMenu_Exit);
            this.InnerFence = settingMenu;
        }

        /// <summary>
        /// obsluha udalosti ukonceni okna s nastavenim 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingMenu_Exit(object sender, EventArgs e)
        {
            InnerFence = null;
        }


        /// <summary>
        /// obsluha udalosti spusteni eidtoru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editorButton_OnClick(object sender, EventArgs e)
        {
            if (OnStartEditor != null)
                OnStartEditor(this, new EventArgs());
        }

        /// <summary>
        /// obsluha udalosti  startu hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGameAsk_Exit(object sender, ExitEventArgs e)
        {
            InnerFence = null;

            if (e.IsContinue)
            {
                Configuration.useProfile = new Profile();
                Configuration.SaveProfile();
                SetStartGameEventArgs(Configuration.useProfile.LastLevel);
            }

        }

        #endregion
    }
}
