using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.Interaction.Core;
using PhysicsSimulator.Engine.Physics;

namespace PhysicsSimulator.Test
{
    /// <summary>
    /// Třida jen pro testování něčeho ..
    /// </summary>
    class TestPhysics : Game
    {
        const float k = 5f;
        GraphicsDeviceManager graphics;
        BasicEffect effect;


        ConvexBody test1;
        ConvexBody test2;

        Elipse ellipsa;

        Line horizontal;
        Line vertical;


        public TestPhysics()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.View = Matrix.Identity;
            effect.World = Matrix.Identity;
            effect.Projection = Matrix.CreateOrthographicOffCenter(-k * GraphicsDevice.Viewport.AspectRatio, k * GraphicsDevice.Viewport.AspectRatio, -k, k, -1f, 10f);
            Configuration.Initialize(graphics, Services, effect);

            test1 = new ConvexBody(Color.Black, Vector2.Zero, 1000f, 1, Vector2.UnitX, 2f, 0, /*points*/ -3 * Vector2.UnitX, 2 * Vector2.UnitY, Vector2.UnitX, -Vector2.UnitY);
            test2 = new ConvexBody(Color.Red, Vector2.Zero, 1000f, 1, Vector2.UnitX, -3.14f, 0,/*points*/ -3 * Vector2.UnitX, 2 * Vector2.UnitY, Vector2.UnitX, -Vector2.UnitY);

            ellipsa = new Elipse(Color.Yellow, new Vector2(2, 3), 1, 1.5f, 0.8f);

            horizontal = new Line(Vector2.Zero, Vector2.UnitX, Color.Honeydew);
            vertical = new Line(Vector2.Zero, Vector2.UnitY, Color.Purple);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //test1.Rotate(Vector2.Zero, 0.01f);

            ellipsa.Rotate(Vector2.Zero, 0.01f);
            ellipsa.Rotate(ellipsa.Position, 0.01f);
            //ellipsa.Translate(new Vector2(-0.01f, -0.01f));
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //test1.Draw();
            //test2.Draw();

            ellipsa.Draw();

            horizontal.Draw();
            vertical.Draw();

        }

    }
}
