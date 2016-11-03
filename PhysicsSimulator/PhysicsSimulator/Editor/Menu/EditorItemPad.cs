using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.GUI;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.Editor.Menu
{
    /// <summary>
    /// Abstraktní třída realizujcí společnou část pro pad, která se dále dělí na pad pro upravy Iphysic objektu a EventBoxů.
    /// </summary>
    abstract class EditorItemPad : Fence
    {
        /// <summary>
        /// instance editoru
        /// </summary>
        protected MapEditor editor;

        protected abstract int labelCount { get; }

        protected const float BUTTON_WIDTH = 140;
        protected const float BUTTON_HEIGHT = 45;
        protected const int TEXT_SIZE = 30;
        protected const float BUTTON_MARGIN = 160;
        protected const float LABEL_MARGIN = 10;

        /// <summary>
        /// mistecko pro vyskakovaci dialog a jeho ovladani
        /// </summary>
        public override Fence InnerFence
        {
            get { return base.InnerFence; }
            set
            {
                base.InnerFence = value;
                if (value == null)
                    MapEditor.NoDialog = true;
                else
                    MapEditor.NoDialog = false;
            }
        }

        /// <summary>
        /// vybrany tool
        /// </summary>
        public Tools selectedTool = Tools.None;

        /// <summary>
        /// delegat pro vyber toolu
        /// </summary>
        /// <param name="sendr"></param>
        /// <param name="args"></param>
        public delegate void ToolsSelected(object sendr, ToolsSelectedEventArgs args);

        /// <summary>
        /// udalost tool byl vybran
        /// </summary>
        public event ToolsSelected OnToolSelected;


        public Label title;
        //public Button save;
        public Button delete;
        protected Label[] labels;
        protected string[] labelsText;


        /// <summary>
        /// Inicializuje pad
        /// </summary>
        /// <param name="editor"></param>
        public EditorItemPad(MapEditor editor)
        {
            this.editor = editor;

            Enabled = false;
            OnToolSelected += (object sender, ToolsSelectedEventArgs e) => { this.selectedTool = e.tool; };

            title = new Label();
            //save = new Button();
            delete = new Button();

            //title
            title.Text = "Teleso";
            title.IsMultiLine = false;
            title.IsCenter = false;
            title.Location = new Vector2(LABEL_MARGIN, 2);
            title.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            title.TextSize = TEXT_SIZE;
            Controls.Add(title);

            ////save
            //save.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            //save.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            //save.Location = new Vector2(LABEL_MARGIN, 30);
            //save.Size = new Vector2(BUTTON_WIDTH , BUTTON_HEIGHT);
            //save.TextSize = TEXT_SIZE;
            //save.Text = "Save";
            //save.OnClick += new EventHandler(save_OnClick);
            //Controls.Add(save);

            //delete
            delete.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            delete.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            delete.Location = new Vector2(BUTTON_MARGIN, 30);
            delete.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            delete.TextSize = TEXT_SIZE;
            delete.Text = "delete";
            delete.OnClick += new EventHandler(delete_OnClick);
            Controls.Add(delete);

            InitializeComponent();

            //ItemPad
            this.Visible = true;
            this.Location = new Vector2(170, 100);
            this.Size = new Vector2(330, 1500);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenubackground");
            this.BackImageSizemode = SizeMode.extend;
        }



        public override void Update(GameTime time, Vector2 margin)
        {
            base.Update(time, margin);
        }

        /// <summary>
        /// obshluha udalosti vymazani daneho objekut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void delete_OnClick(object sender, EventArgs e);

        /// <summary>
        /// zavre pad
        /// </summary>
        protected void ClosePad()
        {
            this.selectedTool = Tools.None;
            this.Enabled = false;
        }

        /// <summary>
        /// inicialize component ze struktury
        /// </summary>
        public abstract void SetupFields();



        /// <summary>
        /// inicializace popisku
        /// </summary>
        protected void InitializeLabeks()
        {
            labels = new Label[labelCount];
            for (int i = 0; i < labelCount; i++)
            {
                labels[i] = new Label();
                labels[i].Text = labelsText[i];
                labels[i].IsMultiLine = false;
                labels[i].IsCenter = true;
                labels[i].Location = new Vector2(LABEL_MARGIN, 80 + i * 50);
                labels[i].Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
                labels[i].TextSize = TEXT_SIZE;
                Controls.Add(labels[i]);
            }
        }

        /// <summary>
        /// posle editoru zpravu ze byl vybran dany tool
        /// </summary>
        /// <param name="tool">dany tool</param>
        protected void InvokeToolSelected(Tools tool)
        {
            var arg = new ToolsSelectedEventArgs(tool);
            if (OnToolSelected != null)
                OnToolSelected(this, arg);
        }


        /// <summary>
        /// Inicializace GUI component
        /// </summary>
        protected abstract void InitializeComponent();


    }
}
