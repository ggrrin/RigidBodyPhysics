using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.GUI;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.Editor.Menu
{
    /// <summary>
    /// Třída realizující toolbar pro výběr toolů v editoru.
    /// </summary>
    class EditorToolBar : Fence
    {
        /// <summary>
        /// editor se kterym se pracuje
        /// </summary>
        private MapEditor editor;

        public Label title, st;
        private Button convex, selector;
        private Button elipse, box;
        Button simulate;

        /// <summary>
        /// vybrany tool
        /// </summary>
        public Tools selectedTool = Tools.None;
        /// <summary>
        /// delegat pro udalost vyber toolu
        /// </summary>
        /// <param name="sendr"></param>
        /// <param name="args"></param>
        public delegate void ToolsSelected(object sendr, ToolsSelectedEventArgs args);

        /// <summary>
        /// udalost volana pri vyberu toolu
        /// </summary>
        public event ToolsSelected OnToolSelected;


        /// <summary>
        /// inicializace toolboxu a svazani s danym editorm
        /// </summary>
        /// <param name="editor">dany editor</param>
        public EditorToolBar(MapEditor editor)
        {
            this.editor = editor;
            InitializeComponent();
        }


        /// <summary>
        /// Inicializuje GUI componenty
        /// </summary>
        private void InitializeComponent()
        {
            OnToolSelected += (object sender, ToolsSelectedEventArgs e) => { this.selectedTool = e.tool; };

            //Created new instancs
            title = new Label();
            selector = new Button();
            convex = new Button();
            elipse = new Button();
            box = new Button();
            st = new Label();
            simulate = new Button();

            //st
            st.Text = Vector2.Zero.ToString();
            st.IsMultiLine = true;
            st.IsCenter = false;
            st.Location = new Vector2(0, 0);
            st.Size = new Vector2(140, 60);
            st.TextSize = 25;
            Controls.Add(st);

            //title
            title.Text = "Telesa:";
            title.IsMultiLine = false;
            title.IsCenter = true;
            title.Location = new Vector2(10, 25);
            title.Size = new Vector2(150, 55);
            title.TextSize = 30;
            Controls.Add(title);

            //selector            
            selector.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            selector.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            selector.Location = new Vector2(10, 80);
            selector.Size = new Vector2(150, 45);
            selector.TextSize = 30;
            selector.Text = "selector";
            selector.OnClick += new EventHandler(selector_OnClick);
            Controls.Add(selector);

            //convex            
            convex.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            convex.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            convex.Location = new Vector2(10, 130);
            convex.Size = new Vector2(150, 45);
            convex.TextSize = 30;
            convex.Text = "convex";
            convex.OnClick += new EventHandler(convex_OnClick);
            Controls.Add(convex);

            //elipse
            elipse.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            elipse.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            elipse.Location = new Vector2(10, 180);
            elipse.Size = new Vector2(150, 45);
            elipse.TextSize = 30;
            elipse.Text = "elipse";
            elipse.OnClick += new EventHandler(elipse_OnClick);
            Controls.Add(elipse);

            //box
            box.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            box.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            box.Location = new Vector2(10, 230);
            box.Size = new Vector2(150, 45);
            box.TextSize = 30;
            box.Text = "box";
            box.OnClick += new EventHandler(box_OnClick);
            Controls.Add(box);

            //GameSubMenu
            this.Visible = true;
            this.Location = new Vector2(0, 100);
            this.Size = new Vector2(170, 1500);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenubackground");
            this.BackImageSizemode = SizeMode.extend;
            //this.Close += new EventHandler(GameSubMenu_Close);
        }

        /// <summary>
        /// zpane nastroj event box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void box_OnClick(object sender, EventArgs e)
        {
            title.Text = box.Text;
            var arg = new ToolsSelectedEventArgs(Tools.Box);
            if (OnToolSelected != null)
                OnToolSelected(this, arg);
        }

        /// <summary>
        /// zapne nastroj pro vyber opektu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selector_OnClick(object sender, EventArgs e)
        {
            title.Text = selector.Text;
            var arg = new ToolsSelectedEventArgs(Tools.Selector);
            if (OnToolSelected != null)
                OnToolSelected(this, arg);
        }


        /// <summary>
        /// zapne nastroj pro tvorbu konvexni teles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void convex_OnClick(object sender, EventArgs e)
        {
            title.Text = convex.Text;
            var arg = new ToolsSelectedEventArgs(Tools.Convex);
            if (OnToolSelected != null)
                OnToolSelected(this, arg);
        }


        /// <summary>
        /// zapne nastroj pro tvorbu elips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elipse_OnClick(object sender, EventArgs e)
        {
            title.Text = elipse.Text;
            var arg = new ToolsSelectedEventArgs(Tools.Elipse);
            if (OnToolSelected != null)
                OnToolSelected(this, arg);
        }
    }


    /// <summary>
    /// seznam nastroju editoru
    /// </summary>
    enum Tools
    {
        /// <summary>
        /// zady nastroj
        /// </summary>
        None,

        /// <summary>
        /// vybira objekty na mape
        /// </summary>
        Selector,

        /// <summary>
        /// tvorba convexnich teles
        /// </summary>
        Convex,

        /// <summary>
        /// tvorba teles tvaru elipsy
        /// </summary>
        Elipse,

        /// <summary>
        /// kresleni vektru teles daneho telesa
        /// </summary>
        VelocityDrawer,

        /// <summary>
        /// tvorba event boxu
        /// </summary>
        Box,

        /// <summary>
        /// tool pro posouvani telse po mape
        /// </summary>
        Positioner,

        /// <summary>
        /// toool pro otaceni teles
        /// </summary>
        Rotator,

        /// <summary>
        /// tool pro kopirovani teles
        /// </summary>
        Cloner,

        /// <summary>
        /// tool pro vyber cilove lokace teleportu
        /// </summary>
        LocationPicker,
    }
}
