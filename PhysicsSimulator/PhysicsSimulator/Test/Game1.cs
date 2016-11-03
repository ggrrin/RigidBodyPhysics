using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.Interaction.Core;
using System;

namespace PhysicsSimulator.Test
{
    /// <summary>
    /// Zastaralá třída, používána jen při testování interakčních objektů.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect effect;
        Line hf;
        Elipse[] e;
        Line[] l;
        ConvexPolygon poi;
        ConvexPolygon pio;
        Segment sss;
        ConvexPolygon[] ul;

        public Game1()
        {
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
            const int k = 3;
            effect.Projection = Matrix.CreateOrthographicOffCenter(-k * GraphicsDevice.Viewport.AspectRatio, k * GraphicsDevice.Viewport.AspectRatio, -k, k, -1f, 10f);


            Configuration.Initialize(graphics, Services, effect);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ul = new ConvexPolygon[3];

            //error opakování kvadratù -> nekonvexní
            ul[0] = new ConvexPolygon(false, Color.Black, new Vector2(-1f, 0f), new Vector2(0, 1f), new Vector2(0.3f, 0f), new Vector2(1f, 1f), new Vector2(0.3f, -1), new Vector2(0.1f, 1));

            //error všechno ok ale nìco se pøekøíží
            ul[1] = new ConvexPolygon(false, Color.Green,
                new Vector2(-1, 0f),
                new Vector2(0, 1f),
                new Vector2(1f, 0),
                new Vector2(-0.9f, 0.5f)
                );
            ul[1].Translate(Vector2.UnitX * 2);

            //error v v jednom kvadratu špatné uspoøádání
            ul[2] = new ConvexPolygon(false, Color.Red,
                new Vector2(0.9f, -0.5f),
                new Vector2(-1, 0f),
                new Vector2(0, 0.5f),
                new Vector2(0.5f, -0.25f)
                );

            ul[2].Translate(Vector2.UnitX * -2);




            poi = new ConvexPolygon(Color.Bisque, 3 * Vector2.UnitX, 2 * Vector2.UnitY + 3 * Vector2.UnitX, 0.91f * Vector2.UnitX);
            pio = new ConvexPolygon(Color.Black, new Vector2(-0.5f, 0.5f), new Vector2(-0.1f, 0.5f), new Vector2(0.1f, -0.3f), new Vector2(-0.3f, -0.2f));

            Vector2 u, v;
            CrossState cs;

            bool adfadf = poi.Intersects(pio, out v, out u, out cs);

            Console.WriteLine("{0},{1},{2}", v, u, cs);
            /*Segment s = new Segment(Vector2.Zero, Vector2.UnitX + Vector2.UnitY, Color.AliceBlue);

            Segment d = new Segment(Vector2.UnitY, Vector2.UnitX - Vector2.UnitY, Color.AliceBlue);

            Vector2 g; CrossState fsa;
            bool hjoi = s.Intersects(d, out g, out fsa);*/


            l = new Line[0];
            e = new Elipse[2];

            /*
            HalfLine h = new HalfLine(Color.Red, Vector2.Zero, Vector2.UnitY);
            */
            e[0] = new Elipse(Color.Red, new Vector2(1, 1), 2, 1f, MathHelper.ToRadians(0));

            e[1] = new Elipse(Color.Blue, new Vector2(1, -1), 1.5f, 0.7f, MathHelper.ToRadians(-45));
            //e[0].Rotate(new Vector2(0.0f,0), MathHelper.ToRadians(45));
            //e[0].Translate(Vector2.UnitX + Vector2.UnitY);

            e[0].Intersects(poi, out v, out u, out cs);
            Console.WriteLine("{0},{1},{2}", v, u, cs);
            //..-/
            e[0].Intersects(e[1], out v, out u, out cs);
            Console.WriteLine("{0},{1},{2}", v, u, cs);

            e[1].Intersects(e[0], out v, out u, out cs);
            Console.WriteLine("{0},{1},{2}", v, u, cs);

            sss = new Segment(new Vector2(-0.4454086f, 0.6455005f), new Vector2(0.394783f, 0.08888465f), Color.BurlyWood);

            hf = new Line(new Vector2(-0.4142136f, -0.4142136f), new Vector2(-0.5636416f, 0.249474f)/*, new Vector2(-0.4454086f, 0.6455005f)*/, Color.Black);



            /*Vector2 p, pl;
            CrossState cs;
            h.Intersects(e[0],out p, out pl,out cs);
            */

            //[0] = new Segment(new Vector2(-0.4142136f, -0.4142136f), new Vector2(-0.4142136f, -0.4142136f) + new Vector2( -0.9755788f, -0.219649762f) , Color.Green);

            //l[0].Intersects(Vector2.Zero);

            //l[1] = new Line(new Vector2(0, 0f), Vector2.UnitY, Color.Orange);
            //l[2] = new Line(new Vector2(0, 0f), Vector2.UnitX, Color.Yellow);

            /*l[1] = new Line(Vector2.Zero, Vector2.UnitX, Color.Red);

            l[0].Intersects(l[1]);*/

            //e[0] = new ConvexObliqueElement(Color.Red, new Vector2( -1f, -0.5f), new Vector2( -0.5f, 0.4f), new Vector2( -0.35f, 0.6f), new Vector2( -0.1f, 0.7f), new Vector2( 0.4f, 0.6f), new Vector2( 0.4f, 0.4f), new Vector2( -0.3f, -0.3f));
            //e[1] = new ConvexObliqueElement(Color.Green, new Vector2( 0.20f, 0.7f), new Vector2( 0.25f, 0.8f), new Vector2( 0.8f, 0.8f), new Vector2( 0.42f, 0.3f));

            //f[0] = new ElipseElement(Color.SpringGreen, new Vector2( 1f, 1.0f), 1.5f, 0.75f,  MathHelper.PiOver4);
            //f[0] = new ElipseElement(Color.SpringGreen, new Vector2( -0.4f, 0.1f), 0.5f, 0.5f, MathHelper.PiOver4);
            //bool i = f.Intersects(new ElementLink(Vector2.Zero, Vector2.UnitX + Vector2.UnitY));

            //l[1] = new Line(Vector2.Zero, Vector2.UnitY , Color.Yellow);
            //l[0] = new Line(Vector2.Zero, Vector2.UnitY, Color.YellowGreen);
            //l[3] = new Line(Vector2.Zero + Vector2.UnitY, Vector2.UnitX + Vector2.UnitY, Color.Yellow);
            //l[4] = new Line(Vector2.Zero + Vector2.UnitX, Vector2.UnitY + Vector2.UnitX, Color.YellowGreen);

            //l[0].Rotate(new Vector2( 0,0), MathHelper.ToRadians(10));



            //l[0].Intersects<int>(e[0]);

            //bool fer = poi.Intersects(l[0]);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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


            Vector2 u, v;
            CrossState cs;
            //e[0].Translate(-0.01f * Vector2.UnitX);
            e[0].Rotate(Vector2.Zero, MathHelper.ToRadians(0.9f));
            e[0].Intersects(poi, out v, out u, out cs);

            pio.Rotate(Vector2.Zero, MathHelper.ToRadians(0.9f));
            poi.Rotate(Vector2.Zero, MathHelper.ToRadians(0.9f));

            foreach (Line k in l)
            {
                k.Rotate(Vector2.Zero, MathHelper.ToRadians(0.9f));

            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Console.WriteLine("{0},{1},{2}", v, u, cs);
                Console.WriteLine(e[0].Position);
            }
            // TODO: Add your update logic he*/



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (ConvexPolygon p in ul)
                p.Draw();
            foreach (Elipse h in e)
                h.Draw();

            foreach (Line k in l)
                k.Draw();


            poi.Draw();

            hf.Draw();
            sss.Draw();
            pio.Draw();

            base.Draw(gameTime);
        }
    }
}
