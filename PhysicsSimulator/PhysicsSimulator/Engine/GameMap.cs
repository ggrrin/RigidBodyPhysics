using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.Engine.Game;
using PhysicsSimulator.Engine.Physics;
using PhysicsSimulator.GUI;
using PhysicsSimulator.GUI.Menu;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PhysicsSimulator.Engine
{

    /// <summary>
    /// Třída realizující samotnou hru.
    /// </summary>
    [Serializable]
    class GameMap : PhysicCore
    {
        /// <summary>
        /// vychozi rychlost pohybu platna
        /// </summary>
        private const float CANVAS_MOVEMENT_SPEED = 0.6f;

        /// <summary>
        /// vychozi rychlost scalovani platna
        /// </summary>
        private const float CANVAS_SCALE_SPEED = 1.1f;


        /// <summary>
        /// pomocny vykreslovac vektoru a bodu
        /// </summary>
        VectorDrawer vdraw;

        //Map
        /// <summary>
        /// dany level
        /// </summary>
        private int level;

        /// <summary>
        /// zda je hra ve freelook modu
        /// </summary>
        private bool freeLookMode = false;
        //Player

        //SubMenus
        /// <summary>
        /// submenu
        /// </summary>
        private GameSubMenu subMenu;

        /// <summary>
        /// menu po dokonceni levelu
        /// </summary>
        private LevelComplete levelComplete;

        /// <summary>
        /// panel pro ovladani hry
        /// </summary>
        private InformationPanel informationPanel;

        /// <summary>
        /// panel pro urcovani uhlove rychlosti teles
        /// </summary>
        private RotationBar rot;
        //Update


        /// <summary>
        /// predchozi hodnota stoped
        /// </summary>
        private bool stopedPrev = true;

        /// <summary>
        /// nynejsi hodnota stoppd
        /// </summary>
        private bool stopped = true;

        /// <summary>
        /// nynejsi hodnota stopped.... zda bezi simulace
        /// </summary>
        private bool Stopped
        {
            get { return stopped; }
            set
            {
                stopedPrev = stopped;
                stopped = value;
            }
        }

        /// <summary>
        /// VSTUP/VYSTUP keyboard
        /// </summary>
        private KeyboardState keyboard, previousKeyboard;

        /// <summary>
        /// vstup vystup mys
        /// </summary>
        private MouseState mouse, previousMouse;


        //Events
        /// <summary>
        /// delegat pro start nove hry => dalsiho leverlu/ restartu levelu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void StarGameEventHandler(object sender, ExiEventArgs e);

        /// <summary>
        /// event pro vstup do hlavniho menu
        /// </summary>
        public event EventHandler GotoMenu;

        /// <summary>
        /// event pro spustenin dalsi mapy
        /// </summary>
        public event StarGameEventHandler PlayNextMap;

        /// <summary>
        /// event pro restartovani levelu
        /// </summary>
        public event StarGameEventHandler RestartLevel;

        /// <summary>
        /// vychozi nastacalovani mapy
        /// </summary>
        public float unitsOnWidth = 20;

        /// <summary>
        /// platno chytajci klikani mysi
        /// </summary>
        Button clickableCanvas;


        /// <summary>
        /// nastaveni scalu mapy
        /// </summary>
        public float UnitsOnScreen
        {
            get { return unitsOnWidth; }
            set { unitOnWidth = value; }
        }


        /// <summary>
        /// seznam teles
        /// </summary>
        public List<IPhysicBody> Bodies
        {
            get { return base.bodies; }
            set { base.bodies = value; }
        }

        /// <summary>
        /// seznam eventboxu
        /// </summary>
        public List<EventBox> EventBoxes
        {
            get { return base.eventBoxes; }
            set { base.eventBoxes = value; }
        }

        /// <summary>
        /// seznam teles ovladatelnych hracem
        /// </summary>
        private List<IPhysicBody> playerBodies;

        /// <summary>
        /// aktualne vybrane hracovo teleso
        /// </summary>
        private int selectedPlayer = 0;

        /// <summary>
        /// jen inicializuje tridu. neni urceno pro start hry!! (pouze pro editor) 
        /// </summary>
        public GameMap()
        {

        }

        /// <summary>
        /// start hry podle dane ulozene mapy
        /// </summary>
        /// <param name="data">ulozena mapa</param>
        public GameMap(MapData data)
        {
            this.level = data.Level;
            this.unitOnWidth = data.UnitsOnWidth;
            this.bodies = data.Bodies;
            this.eventBoxes = data.EventBoxes;

            vdraw = new VectorDrawer();

            clickableCanvas = new Button();
            clickableCanvas.Location = new Vector2(0, 100);
            clickableCanvas.Size = new Vector2(1670, 1820);
            clickableCanvas.Text = "";
            clickableCanvas.BackTexture = new Texture2D(Configuration.device, 1, 1);
            clickableCanvas.BackTextureHover = new Texture2D(Configuration.device, 1, 1);
            clickableCanvas.MouseEnterSound = null;
            clickableCanvas.OnClick += delegate(object s, EventArgs e)
            {
                playerBodies[selectedPlayer].Velocity = MousePosition() - playerBodies[selectedPlayer].Position;
            };

            //subMenu
            this.subMenu = new GameSubMenu();
            this.subMenu.GoToMainMenu += new EventHandler(subMenu_GoToMainMenu);
            this.subMenu.PlayAgain += new EventHandler(levelUncomplete_playAgain);
            this.subMenu.Close += delegate(object sender, EventArgs e)
            {
                this.subMenu.Visible ^= true;
                Stopped = stopedPrev;
            };

            rot = new RotationBar();
            rot.OnValSet += delegate(object sender, ExitEventArgs e)
            {
                playerBodies[selectedPlayer].AngularVelocity = new Vector3(0, 0, Configuration.maxRotation * e.NumberFL);
            };

            //levelComplete
            this.levelComplete = new LevelComplete();
            this.levelComplete.PlayNext += new EventHandler(levelComplete_playNext);
            this.levelComplete.exit += new EventHandler(levelComplete_exit);
            //InformationPanel
            informationPanel = new InformationPanel();
            informationPanel.boom.OnClick += boom_OnClick;
            informationPanel.freelook.OnClick += freelook_OnClick;

            InitializeMap();
        }

        /// <summary>
        /// zmena freelok modu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void freelook_OnClick(object sender, EventArgs e)
        {
            freeLookMode ^= true;

            foreach (var i in informationPanel.Stones)
                i.Enabled = !freeLookMode;

            rot.Enabled = !freeLookMode;

        }


        /// <summary>
        /// spistit simulaci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void boom_OnClick(object sender, EventArgs e)
        {
            Stopped = false;
            (sender as Button).Enabled = false;
            rot.Enabled = false;
        }


        /// <summary>
        /// inicializovat hracovi kameny
        /// </summary>
        private void InitializeMap()
        {
            // separovat hracem ovladane objekty
            playerBodies = new List<IPhysicBody>();
            foreach (var i in bodies)
                if (i.Player)
                    playerBodies.Add(i);

            informationPanel.Initialize(playerBodies.Count);
            informationPanel.OnStoneSelected += informationPanel_OnStoneSelected;
        }


        /// <summary>
        /// zmenit kamen pri vybrani hracova kamenu v menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void informationPanel_OnStoneSelected(object sender, ExitEventArgs e)
        {
            selectedPlayer = e.Number;
            if (!rot.slidingMode)
                rot.Value = playerBodies[selectedPlayer].AngularVelocity.Z / Configuration.maxRotation;
        }


        /// <summary>
        /// otestovani zda do kaze finih zony dojel hracuv kamen pokud ano => dalsi level
        /// </summary>
        private void CheckPlayerFinishZones()
        {
            bool levelDone = true;

            foreach (var i in playerBodies)
                if (i.Player)
                    levelDone = false;

            if (levelDone)
            {
                Configuration.useProfile.CompleteLevel(level);
                stopped = true;
                levelComplete.Visible = true;
            }
        }

        /// <summary>
        /// zda bylo kliknutu na nejakou klavesu
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool KeyPressed(Keys key)
        {
            return keyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }


        /// <summary>
        /// Update hry
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            vdraw.Clear();
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            //iNPUT
            if (KeyPressed(Keys.Escape))
            {
                Stopped = this.subMenu.Visible ^= true;
            }

            if (!freeLookMode)
            {
                base.cameraPosition = playerBodies[selectedPlayer].Position;
                vdraw.Add(cameraPosition, 0.2f, 0.2f, Color.Green);
                vdraw.Add(cameraPosition, 0.2f, 0.4f, Color.Green);
                if (Stopped)
                    vdraw.Add(playerBodies[selectedPlayer].Position, MousePosition() - playerBodies[selectedPlayer].Position, Color.Blue);

                clickableCanvas.Update(time, Vector2.Zero);


            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Up))
                    cameraPosition.Y += CANVAS_MOVEMENT_SPEED;
                else if (keyboard.IsKeyDown(Keys.Down))
                    cameraPosition.Y -= CANVAS_MOVEMENT_SPEED;

                if (keyboard.IsKeyDown(Keys.Left))
                    cameraPosition.X -= CANVAS_MOVEMENT_SPEED;
                else if (keyboard.IsKeyDown(Keys.Right))
                    cameraPosition.X += CANVAS_MOVEMENT_SPEED;
            }

            if (keyboard.IsKeyDown(Keys.OemPlus))
                unitOnWidth /= CANVAS_SCALE_SPEED;
            else if (keyboard.IsKeyDown(Keys.OemMinus))
                unitOnWidth *= CANVAS_SCALE_SPEED;

            ///////////////////////////////////////////////
            CheckPlayerFinishZones();

            if (!Stopped)
                base.Update(time);

            UpdateCanvasPositionLocation();

            if (stopped)
                foreach (var i in playerBodies)
                    vdraw.Add(i.Position, i.Velocity, Color.Green);

            subMenu.Update(time, new Vector2(0, 0));
            levelComplete.Update(time, new Vector2(0, 0));
            informationPanel.Update(time, new Vector2(0, 0));
            rot.Update(time, Vector2.Zero);

            //PREVIOUS INPUT
            previousKeyboard = keyboard;
            previousMouse = mouse;
        }


        /// <summary>
        /// vykresleni hernich obektu
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        public override void Draw(SpriteBatch sprite, GameTime time)
        {
            base.Draw(sprite, time);
            vdraw.Draw();
        }


        /// <summary>
        /// vykresleni GUI
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="time"></param>
        public void DrawUI(SpriteBatch sprite, GameTime time)
        {
            ///////////////////////////////////////////////
            var health = new Label();
            //Controls.Add(health);
            health.Location = new Vector2(2000, 0);
            health.Size = new Vector2(350, 100);
            health.Text = "Health ";
            health.IsCenter = true;
            health.TextSize = 30;

            health.Update(time, Vector2.Zero);
            health.Draw(sprite, time, Vector2.Zero);
            //kdyz to tady je tak nevyskakuje NotSupportedException... I have no idea Why! !  that implies =>> dount knou wot
            ///////////////////////////////////////////////////////

            subMenu.Draw(sprite, time, new Vector2(0, 0));
            levelComplete.Draw(sprite, time, new Vector2(0, 0));
            //levelUncomplete.Draw(sprite, time, new Vector2(0, 0));
            informationPanel.Draw(sprite, time, new Vector2(0, 0));
            rot.Draw(sprite, time, Vector2.Zero);
        }

        #region Input Output

        /// <summary>
        /// serializuje(ulozi danou instanci tridy do soboru jmena podle parametru)
        /// </summary>
        /// <param name="mapName">jmeno souboru mapy</param>
        public void Serialize(string mapName)
        {
            string directory = "Content\\Maps\\";

            var data = new MapData(int.Parse(mapName), unitsOnWidth, bodies, EventBoxes);

            using (Stream file = new FileStream(Path.Combine(directory, mapName), FileMode.Create))
            {
                IFormatter formater = new BinaryFormatter();
                formater.Serialize(file, data);
            }
        }

        /// <summary>
        /// nahraje mapu ze souboru a vytvori novou instanci hry 
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public static GameMap Deserialize(string mapName)
        {
            string directory = "Content\\Maps\\";
            using (Stream file = new FileStream(Path.Combine(directory, mapName), FileMode.Open))
            {
                IFormatter formater = new BinaryFormatter();
                return new GameMap(formater.Deserialize(file) as MapData);
            }
        }

        #endregion

        #region "Events"

        /// <summary>
        /// obsluha udlosti pro vstup do hlavniho menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subMenu_GoToMainMenu(object sender, EventArgs e)
        {
            Stopped = true;
            if (GotoMenu != null)
                GotoMenu(this, new EventArgs());
        }

        /// <summary>
        /// obsluha udalosti pro vstup do dalsiho levelu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelComplete_playNext(object sender, EventArgs e)
        {
            if (PlayNextMap != null)
            {

                ExiEventArgs eventArgs = new ExiEventArgs();
                eventArgs.LevelPath = Configuration.useProfile.LastLevel;
                PlayNextMap(this, eventArgs);
            }
        }

        /// <summary>
        /// obsluha udalosti ukonceni hry pro splneni levelu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelComplete_exit(object sender, EventArgs e)
        {
            if (this.GotoMenu != null)
                GotoMenu(this, new EventArgs());
        }


        /// <summary>
        /// obsluha udalosti restartovat level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelUncomplete_playAgain(object sender, EventArgs e)
        {
            if (RestartLevel != null)
            {
                ExiEventArgs eventArgs = new ExiEventArgs();
                eventArgs.LevelPath = level;
                RestartLevel(this, eventArgs);
            }
        }


        #endregion
    }
}
