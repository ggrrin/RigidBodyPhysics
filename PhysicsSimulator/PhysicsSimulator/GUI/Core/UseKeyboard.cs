using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System.Collections.Generic;

namespace PhysicsSimulator.GUI
{
    /// <summary>
    /// Třída umožňující pohybovat se mezi danými tlačítky pomocí šipek či kolečka myši.
    /// </summary>
    class UseKeyboard : IFenceObject
    {
        private KeyboardState keyboard;
        private KeyboardState previousKeyboard;

        private MouseState mouse;
        private MouseState previousMouse;

        private List<Button> controls;

        private int selectIndex;
        private float layerDepth;

        #region "Properties"

        bool enabled = true;

        /// <summary>
        /// nic nedela jen implemntuje rozhrani
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// nic nedela pouze implementuje rozhrani
        /// </summary>
        public float LayerDepth
        {
            get { return layerDepth; }
            set
            {
                layerDepth = value;
                AktualizeLayers();

            }
        }

        /// <summary>
        /// nastaveni vybreaneho indexu (buttonu)
        /// </summary>
        public int SelectIndex
        {
            get { return selectIndex; }
            set
            {
                selectIndex = value;
                if (OnItemSelected != null)
                    OnItemSelected(this, new ExitEventArgs(value));
            }
        }

        #endregion

        /// <summary>
        /// handler pro udlost vyberu urciteho tlacitka
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SelectedEventHandler(object sender, ExitEventArgs e);

        /// <summary>
        /// udalost tlacitko vybrano 
        /// </summary>
        public event SelectedEventHandler OnItemSelected;

        /// <summary>
        /// inicializuje usekyboard
        /// </summary>
        public UseKeyboard()
        {
            controls = new List<Button>();
        }


        /// <summary>
        /// obsluha komponenty
        /// </summary>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public void Update(GameTime time, Vector2 margin)
        {
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            if (((keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.Right)) && (previousKeyboard.IsKeyUp(Keys.Down) && previousKeyboard.IsKeyUp(Keys.Right))) || (mouse.ScrollWheelValue == previousMouse.ScrollWheelValue - 120))
            {
                if (SelectIndex + 1 < controls.Count)
                    SelectIndex++;
            }
            else if (((keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.Left)) && (previousKeyboard.IsKeyUp(Keys.Up) && previousKeyboard.IsKeyUp(Keys.Left))) || (mouse.ScrollWheelValue == previousMouse.ScrollWheelValue + 120))
            {
                if (SelectIndex - 1 >= 0)
                    SelectIndex--;
            }

            foreach (IFenceObject a in controls)
            {
                a.Update(time, margin);
            }

            previousKeyboard = keyboard;
            previousMouse = mouse;
        }

        /// <summary>
        /// vykresleni vsech vnitrnich prvku 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        /// <param name="margin"></param>
        public void Draw(SpriteBatch sprite, GameTime time, Vector2 margin)
        {
            foreach (IFenceObject a in controls)
            {
                a.Draw(sprite, time, margin);
            }
        }


        /// <summary>
        /// prida tlacitko do seznamu tlacitek ovladatelneho klavesnici
        /// </summary>
        /// <param name="button"></param>
        public void Add(Button button)
        {
            controls.Add(button);
            button.UseKeyboard = this;
        }


        /// <summary>
        /// aktualizuje vykreslovaci vrstvy aby byli tlacitka na
        /// </summary>
        private void AktualizeLayers()
        {
            foreach (IFenceObject i in controls)
            {
                i.LayerDepth = layerDepth;
            }
        }

    }
}
