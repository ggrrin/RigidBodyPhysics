using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System.Collections.Generic;

namespace PhysicsSimulator.GUI.Menu
{
    /// <summary>
    /// Třída reprezentujcí horní panel ve hře.
    /// </summary>
    class InformationPanel : Fence
    {

        /// <summary>
        /// seznam hracovych kamenu
        /// </summary>
        public List<Button> Stones { get; set; }

        private UseKeyboard keyboardBox;

        public Button freelook, boom;

        /// <summary>
        /// inicializuje informacni panel
        /// </summary>
        public InformationPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// delegat pro udalost vyberu daneho hracova kamenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SelectedEventHandler(object sender, ExitEventArgs e);

        /// <summary>
        /// udalost zavolana pri vyberu hracova kamenu
        /// </summary>
        public event SelectedEventHandler OnStoneSelected;


        /// <summary>
        /// inicializace vnitrich komponent 
        /// </summary>
        private void InitializeComponent()
        {
            Stones = new List<Button>();

            keyboardBox = new UseKeyboard();
            keyboardBox.OnItemSelected += (object sender, ExitEventArgs e) => { if (OnStoneSelected != null) OnStoneSelected(this, e); };

            //keyboardBox
            Controls.Add(keyboardBox);

            freelook = new Button();
            boom = new Button();

            const float yOffset = 25;

            //freelook            
            freelook.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            freelook.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            freelook.Location = new Vector2(1700, yOffset);
            freelook.Size = new Vector2(150, 45);
            freelook.TextSize = 30;
            freelook.Text = "freelook";
            //freelook.OnClick += new EventHandler(freelook_OnClick);
            Controls.Add(freelook);

            //boom            
            boom.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            boom.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            boom.Location = new Vector2(850, 5);
            boom.Size = new Vector2(200, 90);
            boom.TextSize = 50;
            boom.Text = "BOOOM!";
            //boom.OnClick += new EventHandler(boom_OnClick);
            Controls.Add(boom);

            //InformationPanel
            this.Size = new Vector2(1921, 100);
            this.Location = new Vector2(0, 0);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\InformationPanel");

        }

        /// <summary>
        /// inicializace hracovych kamenu do gui
        /// </summary>
        /// <param name="count"></param>
        public void Initialize(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var b = new Button();
                //b
                keyboardBox.Add(b);
                b.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\stone");
                b.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\stoneSelected");
                b.Location = new Vector2(10 + i * 95, 5);
                b.Size = new Vector2(90, 90);
                b.TextSize = 30;
                b.Index = i;
                b.Text = "";
                Controls.Add(b);

                Stones.Add(b);
            }
        }
    }
}
