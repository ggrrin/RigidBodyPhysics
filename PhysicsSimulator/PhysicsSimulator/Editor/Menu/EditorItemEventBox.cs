using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.Engine.Game;
using PhysicsSimulator.GUI;
using System;

namespace PhysicsSimulator.Editor.Menu
{
    /// <summary>
    /// Třída realizující pad pro nastavení vlastností EventBoxů v editoru.
    /// </summary>
    class EditorItemEventBox : EditorItemPad
    {
        private const int LABEL_COUNT = 5;

        private EventBoxEnum boxType;

        /// <summary>
        /// event box type s kterym se aktualne pracuje
        /// </summary>
        public EventBoxEnum BoxType
        {
            get { return boxType; }
            set
            {
                boxType = value;

                teleportLocation.Enabled = value == EventBoxEnum.Teleport;
            }
        }


        /// <summary>
        /// event box s kterym se pracuje
        /// </summary>
        private EventBox box;


        /// <summary>
        /// event box s kterym se pracuje
        /// </summary>
        public EventBox @Box
        {
            get { return box; }
            set
            {
                box = value;
                this.BoxType = box.EvType;
            }
        }


        protected override int labelCount
        {
            get { return LABEL_COUNT; }
        }

        /// <summary>
        /// komponenty
        /// </summary>
        Button speeder, slower, rotator, teleport, finish, teleportLocation;


        /// <summary>
        /// inicializace
        /// </summary>
        /// <param name="editor"> pouzivany editoro</param>
        public EditorItemEventBox(MapEditor editor)
            : base(editor)
        { }


        /// <summary>
        /// inicializuje komponenty
        /// </summary>
        protected override void InitializeComponent()
        {
            Enabled = true;
            boxType = EventBoxEnum.Speeder;
            speeder = new Button();
            slower = new Button();
            rotator = new Button();
            teleport = new Button();
            finish = new Button();
            teleportLocation = new Button();

            base.labelsText = new string[LABEL_COUNT];
            for (int i = 0; i < LABEL_COUNT; i++)
            {
                labelsText[i] = ((EventBoxEnum)i).ToString();
            }

            base.InitializeLabeks();

            //teleportLocation
            teleportLocation.Enabled = false;
            teleportLocation.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            teleportLocation.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            teleportLocation.Location = new Vector2(BUTTON_MARGIN, 330);
            teleportLocation.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            teleportLocation.TextSize = TEXT_SIZE;
            teleportLocation.Index = 0;
            teleportLocation.Text = "Location";
            teleportLocation.OnClick += new EventHandler(loc_OnClick);
            Controls.Add(teleportLocation);

            //speeder
            speeder.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            speeder.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            speeder.Location = new Vector2(BUTTON_MARGIN, 80);
            speeder.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            speeder.TextSize = TEXT_SIZE;
            speeder.Index = 0;
            speeder.Text = "this";
            speeder.OnClick += new EventHandler(val_OnClick);
            Controls.Add(speeder);

            //slower
            slower.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            slower.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            slower.Location = new Vector2(BUTTON_MARGIN, 130);
            slower.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            slower.TextSize = TEXT_SIZE;
            slower.Index = 1;
            slower.Text = "0";
            slower.OnClick += new EventHandler(val_OnClick);
            Controls.Add(slower);

            //rotator
            rotator.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            rotator.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            rotator.Location = new Vector2(BUTTON_MARGIN, 180);
            rotator.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            rotator.TextSize = TEXT_SIZE;
            rotator.Index = 2;
            rotator.Text = "0";
            rotator.OnClick += new EventHandler(val_OnClick);
            Controls.Add(rotator);

            //teleport
            teleport.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            teleport.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            teleport.Location = new Vector2(BUTTON_MARGIN, 230);
            teleport.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            teleport.TextSize = TEXT_SIZE;
            teleport.Index = 3;
            teleport.Text = "0";
            teleport.OnClick += new EventHandler(val_OnClick);
            Controls.Add(teleport);

            //finish
            finish.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            finish.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            finish.Location = new Vector2(BUTTON_MARGIN, 280);
            finish.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            finish.TextSize = TEXT_SIZE;
            finish.Index = 4;
            finish.Text = "0";
            finish.OnClick += new EventHandler(val_OnClick);
            Controls.Add(finish);

            this.title.Text = "Event Box";
        }

        /// <summary>
        /// obsluha udalsti vyberu toolu pro vyber lokace teleportu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loc_OnClick(object sender, EventArgs e)
        {
            base.InvokeToolSelected(Tools.LocationPicker);

        }


        /// <summary>
        /// aktualizuje data z event boxu do GUI
        /// </summary>
        public override void SetupFields()
        {
            ClearButtonText();

            foreach (var i in Controls)
                if (i as Button != null)
                    if ((i as Button).Index == (int)this.BoxType)
                        (i as Button).Text = "this";
        }


        /// <summary>
        /// nastavi vsem komponentam GUI vychozi hodnotu
        /// </summary>
        private void ClearButtonText()
        {
            speeder.Text = "0";
            slower.Text = "0";
            rotator.Text = "0";
            teleport.Text = "0";
            finish.Text = "0";
        }


        /// <summary>
        /// vybere typ evenboxu a ulozi 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void val_OnClick(object sender, EventArgs e)
        {
            ClearButtonText();
            this.BoxType = (EventBoxEnum)(sender as Button).Index;
            (sender as Button).Text = "this";

            Save(box.Min, box.Max);
        }



        /// <summary>
        /// ulozi eventbox podle parametru
        /// </summary>
        /// <param name="Min">jeden vrchol urcujici box</param>
        /// <param name="Max">druhy vrchol urcujici box</param>
        public void Save(Vector2 Min, Vector2 Max)
        {

            if (box != null)
            {
                this.editor.EventBoxes.Remove(box);
            }

            switch (this.BoxType)
            {
                case EventBoxEnum.Speeder:
                    this.editor.EventBoxes.Add(box = new SpeederEventBox(Min, Max));
                    break;
                case EventBoxEnum.Slower:
                    this.editor.EventBoxes.Add(box = new SlowerEventBox(Min, Max));
                    break;
                case EventBoxEnum.Rotator:
                    this.editor.EventBoxes.Add(box = new RotatorEventBox(Min, Max));
                    break;
                case EventBoxEnum.Teleport:
                    this.editor.EventBoxes.Add(box = new TeleportEventBox(Min, Max));
                    break;
                case EventBoxEnum.Finish:
                    this.editor.EventBoxes.Add(box = new FinishEventBox(Min, Max));
                    break;
            }
        }


        /// <summary>
        /// vymaze dany event box z mapy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void delete_OnClick(object sender, EventArgs e)
        {
            this.editor.EventBoxes.Remove(box);
            this.Enabled = false;
        }
    }
}
