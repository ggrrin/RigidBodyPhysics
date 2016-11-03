using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.Editor;
using PhysicsSimulator.Engine;
using PhysicsSimulator.GUI.Menu;
using PhysicsSimulator.GUI.Menu.CustomEventArgs;
using System;

namespace PhysicsSimulator
{
    /// <summary>
    /// Třída reprezentující vstupní bod programu.
    /// </summary>
    public class GameCore : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// graphic manager
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// spritebatch pro vykreslovani 2D objektů ´_ GUI
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// Effekt používany pri vykrelsovani
        /// </summary>
        BasicEffect effect;

        /// <summary>
        /// Hlavní menu
        /// </summary>
        MainMenu menu;

        /// <summary>
        /// Samotna hra
        /// </summary>
        GameMap game;

        /// <summary>
        /// Editor map
        /// </summary>
        MapEditor editor;



        /// <summary>
        /// Inicializuje Aplikaci
        /// </summary>
        public GameCore()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.View = Matrix.Identity;
            effect.World = Matrix.Identity;
            const float k = 15;

            effect.Projection = Matrix.CreateOrthographicOffCenter(-k * GraphicsDevice.Viewport.AspectRatio, k * GraphicsDevice.Viewport.AspectRatio, -k, k, -1f, 10f);

            /////////////////////////////////////////////////////
            Exiting += new EventHandler<EventArgs>(Game_Exiting);
            Configuration.Initialize(graphics, Services, effect);

            InitializeMainMenu();
            //editor = new MapEditor();
            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            ////////////////////////////////////////////////////////////////////

            if (game != null)
                game.Update(gameTime);
            if (menu != null)
                menu.Update(gameTime, new Vector2(0, 0));
            if (editor != null)
                editor.Update(gameTime);

            base.Update(gameTime);


            ////////////////////////////////////////////////////////////////////

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 30, 30));

            if (game != null)
                game.Draw(spriteBatch, gameTime);

            if (editor != null)
                editor.Draw(spriteBatch, gameTime);


            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            if (menu != null)
                menu.Draw(spriteBatch, gameTime, new Vector2(0, 0));

            if (editor != null)
                editor.DrawUI(spriteBatch, gameTime);

            if (game != null)
                game.DrawUI(spriteBatch, gameTime);

            spriteBatch.End();

            base.Draw(gameTime);

        }

        #region "Events"

        /// <summary>
        /// obsluha události ukončení aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_OnExit(object sender, EventArgs e)
        {
            this.Exit();
        }


        /// <summary>
        /// Obsluha udalsoti startu hry. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menu_OnStartGame(object sender, ExiEventArgs e)
        {
            InitializeGame(e);
            menu = null;
        }

        /// <summary>
        /// Obsluha udalsoti navratu ze hry do menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void game_GotoMenu(object sender, EventArgs e)
        {
            InitializeMainMenu();
            game = null;
        }

        /// <summary>
        /// Inicizlizuje vse potrepne pro vytovoreni hlavniho menu
        /// </summary>
        private void InitializeMainMenu()
        {
            menu = new MainMenu();
            menu.OnExit += new EventHandler(menu_OnExit);
            menu.OnStartGame += new MainMenu.StarGameEventHandler(menu_OnStartGame);
            menu.OnStartEditor += menu_OnStartEditor;
        }

        /// <summary>
        /// Obsluha udalsoti startu editoru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menu_OnStartEditor(object sender, EventArgs e)
        {
            menu = null;
            editor = new MapEditor();
            editor.GoToMainMenu += delegate(object s, EventArgs arg)
            {
                editor = null;
                InitializeMainMenu();
            };
        }

        /// <summary>
        /// Inicializuje vse potrepne pro start hry
        /// </summary>
        /// <param name="e"></param>
        private void InitializeGame(ExiEventArgs e)
        {
            game = GameMap.Deserialize(e.LevelPath.ToString());
            game.GotoMenu += game_GotoMenu;
            game.PlayNextMap += game_PlayNextMap;
            game.RestartLevel += game_RestartLevel;
        }

        /// <summary>
        /// Při ukoncovani hry ulozi profil s info o levelech
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Game_Exiting(object sender, EventArgs e)
        {
            Configuration.SaveProfile();
        }

        /// <summary>
        /// Spusti dalsi level hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void game_PlayNextMap(object sender, ExiEventArgs e)
        {
            InitializeGame(e);
        }

        /// <summary>
        /// Restartuje level hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void game_RestartLevel(object sender, ExiEventArgs e)
        {
            InitializeGame(e);
        }

        #endregion
    }
}
