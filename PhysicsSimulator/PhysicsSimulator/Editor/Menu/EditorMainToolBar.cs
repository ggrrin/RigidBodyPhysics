using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.Engine;
using PhysicsSimulator.GUI;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator.Editor.Menu
{

    /// <summary>
    /// Třída realizující horní toolbar v Editoru pro ukladani, otevirani, startu simulace...
    /// </summary>
    class EditorMainToolBar : Fence
    {
        /// <summary>
        /// instance editoru se kterym se pracuje
        /// </summary>
        protected MapEditor editor;

        protected const float BUTTON_WIDTH = 150;
        protected const float BUTTON_HEIGHT = 50;
        protected const float TEXT_SIZE = 30;

        /// <summary>
        /// podora vyskaovavacich dialogu
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

        Button leave;
        Button open;
        Button save;
        Button clean;
        public Button simulate;

        /// <summary>
        /// inicializace panulu a svazani s danym editorum
        /// </summary>
        /// <param name="editor">dany editor</param>
        public EditorMainToolBar(MapEditor editor)
        {
            this.editor = editor;

            simulate = new Button();
            leave = new Button();
            open = new Button();
            clean = new Button();
            save = new Button();

            /* //title
             title.Text = "Teleso";
             title.IsMultiLine = false;
             title.IsCenter = false;
             title.Location = new Vector2(LABEL_MARGIN, 2);
             title.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
             title.TextSize = TEXT_SIZE;
             Controls.Add(title);*/


            //simulate
            simulate.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            simulate.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            simulate.Location = new Vector2(1110, 10);
            simulate.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            simulate.TextSize = 30;
            simulate.Text = "simulate";
            simulate.OnClick += new EventHandler(simulate_OnClick);
            Controls.Add(simulate);


            //leave
            leave.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            leave.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            leave.Location = new Vector2(1270, 10);
            leave.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            leave.TextSize = TEXT_SIZE;
            leave.Text = "leave ";
            leave.OnClick += new EventHandler(leave_OnClick);
            Controls.Add(leave);

            //open
            open.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            open.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            open.Location = new Vector2(1430, 10);
            open.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            open.TextSize = TEXT_SIZE;
            open.Text = "open map";
            open.OnClick += new EventHandler(open_OnClick);
            Controls.Add(open);

            //close
            clean.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            clean.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            clean.Location = new Vector2(1590, 10);
            clean.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            clean.TextSize = TEXT_SIZE - 5;
            clean.Text = "clean/close map";
            clean.OnClick += new EventHandler(clean_OnClick);
            Controls.Add(clean);

            //save
            save.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton0");
            save.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenuButton1");
            save.Location = new Vector2(1750, 10);
            save.Size = new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT);
            save.TextSize = TEXT_SIZE;
            save.Text = "Save map";
            save.OnClick += new EventHandler(save_OnClick);
            Controls.Add(save);

            //ItemPad
            this.Visible = true;
            this.Location = new Vector2(0, 0);
            this.Size = new Vector2(1920, 100);
            this.BackImage = Configuration.content.Load<Texture2D>("Textures\\Menu\\SubMenubackground");
            this.BackImageSizemode = SizeMode.extend;
        }

        /// <summary>
        /// obsluha udalosti opusteni editoru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leave_OnClick(object sender, EventArgs e)
        {
            editor.LeaveEditor();
        }


        /// <summary>
        /// obsluha udalosti simulace zaptunti/vypnuti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simulate_OnClick(object sender, EventArgs e)
        {
            this.editor.Running ^= true;
            if (editor.Running)
                simulate.Text = "Stop";
            else
                simulate.Text = "Simulate";

            this.editor.toolbar.Enabled = !this.editor.Running;
            this.editor.itemPad.Enabled = false;

            foreach (var i in Controls)
                if (i != sender)
                    i.Enabled = !this.editor.Running;
        }

        /// <summary>
        /// zoprazi dialog a podle vstupu otevre danou mapu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void open_OnClick(object sender, EventArgs e)
        {
            var dialog = new EditorInputDialog();
            dialog.Exit += delegate(object sen, ExitEventArgs args)
            {
                if (args.Output != null)
                {
                    try
                    {
                        editor.CleanMap();
                        editor.Map = GameMap.Deserialize(args.Output);
                        editor.LoadMapToEditor();
                    }
                    catch
                    {
                        editor.l.Text = "Takova mapa neexistuje";
                    }
                }
                this.InnerFence = null;

            };
            this.InnerFence = dialog;
        }


        /// <summary>
        /// obsluha udalosti vycisteni mapy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clean_OnClick(object sender, EventArgs e)
        {
            editor.CleanMap();
        }



        /// <summary>
        /// ulozeni dane mapy do souboru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_OnClick(object sender, EventArgs e)
        {
            bool save = false;
            foreach (var i in editor.Bodies)
                if (i.Player)
                    save = true;

            if (save)
            {
                var dialog = new EditorInputDialog();
                dialog.Exit += delegate(object sen, ExitEventArgs args)
                {
                    if (args.Output != null)
                    {
                        try
                        {
                            editor.BackUpMap();
                            editor.Map.Serialize(args.Output.ToString());
                        }
                        catch
                        {
                            editor.l.Text = "Nepodarilo se ulozit mapu";
                        }
                    }
                    this.InnerFence = null;

                };
                this.InnerFence = dialog;

            }
            else
            {
                editor.l.Text = "Nebyl nalezen zadny hracuv kamen => mapa nemuze byt ulozena.";
            }
        }

    }

}
