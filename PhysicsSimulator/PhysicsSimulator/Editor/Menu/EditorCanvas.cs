using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.GUI;
using System;

namespace PhysicsSimulator.Editor.Menu
{
    /// <summary>
    /// Třída podporujcí odchytávaní kliknutí myši v určité oblasi.
    /// </summary>
    class EditorCanvas : Button
    {
        /// <summary>
        /// inicializuje platno
        /// </summary>
        public EditorCanvas()
        {
            InitializeComponents();
        }


        /// <summary>
        /// inicializuje komponety platna
        /// </summary>
        private void InitializeComponents()
        {
            //GameSubMenu
            base.useEnter = false;
            this.Visible = true;
            this.Location = new Vector2(500, 100);
            this.Size = new Vector2(1420, 1500);
            this.BackTexture = Configuration.content.Load<Texture2D>("Textures\\Menu\\canvas");
            this.BackTextureHover = Configuration.content.Load<Texture2D>("Textures\\Menu\\canvas");
            this.MouseEnterSound = null;
            this.Text = "";
            this.TingeHover = new Color(0, 255, 0, 255);
        }

        /// <summary>
        /// obsluha udalosti kliknuti na platno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Button_OnClick(object sender, EventArgs e)
        {
            if (MapEditor.NoDialog)
                base.Button_OnClick(sender, e);
        }


    }
}
