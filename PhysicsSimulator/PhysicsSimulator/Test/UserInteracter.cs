using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsSimulator.Engine.Interaction;
using PhysicsSimulator.Engine.Interaction.Core;
using System.Collections.Generic;

namespace PhysicsSimulator.Test
{
    /// <summary>
    /// Třída pro testování interakčních objektů ovládaných uživatelem.
    /// </summary>
    class UserInteracter : Game
    {
        const float k = 5f;
        GraphicsDeviceManager graphics;
        BasicEffect effect;
        KeyboardState prevK;

        float speedFactor = 0.1f;
        int prevSroll = 0;

        Vector2 direction;

        Segment directionPointer;

        Elipse player;
        List<IInteractable> interactableObj;

        public UserInteracter()
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

            this.InitializeUIObj();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(((float)mouse.X / ((float)graphics.PreferredBackBufferWidth / (2f * k))) - k, (-(float)mouse.Y / ((float)graphics.PreferredBackBufferHeight / (2f * k))) + k);
            direction = mousePos - player.Position;
            directionPointer = new Segment(player.Position, player.Position + direction, Color.Black);
            direction.Normalize();

            speedFactor += (mouse.ScrollWheelValue - prevSroll) / 10000f;
            MathHelper.Clamp(speedFactor, float.Epsilon, 1);

            direction *= speedFactor;

            prevSroll = mouse.ScrollWheelValue;
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W) || (keyboard.IsKeyDown(Keys.S) && prevK.IsKeyUp(Keys.S)))
            {
                player.Translate(direction);
                player.Rotate(player.Position, MathHelper.ToRadians(5f));
            }

            foreach (IInteractable ko in interactableObj)
            {
                Vector2 v1, v2; CrossState cs;
                if (ko.Intersects(player, out v1, out v2, out cs))
                {
                    int i = 0;
                    do
                    {
                        player.Translate(-0.1f * direction);
                        player.Rotate(player.Position, MathHelper.ToRadians(-0.1f * 5f));
                        if (i++ > 10)
                            break;
                    }
                    while (ko.Intersects(player, out v1, out v2, out cs));

                }
            }


            prevK = keyboard;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            player.Draw();

            foreach (IInteractable i in interactableObj)
                i.Draw();

            directionPointer.Draw();
        }

        private void InitializeUIObj()
        {
            player = new Elipse(Color.Green, 2 * Vector2.UnitY, 0.5f, 0.7f, 2f);
            //player.Position = Vector2.Zero;
            player.Translate(2 * Vector2.UnitY);


            interactableObj = new List<IInteractable>();
            interactableObj.Add(new HalfSurface(Color.Red, Vector2.UnitY, Vector2.UnitY * -3));
            interactableObj.Add(new ConvexPolygon(Color.Bisque, 3 * Vector2.UnitX, 2 * Vector2.UnitY + 3 * Vector2.UnitX, 0.91f * Vector2.UnitX));
            interactableObj.Add(new ConvexPolygon(Color.Black, new Vector2(-0.5f, 0.5f), new Vector2(-0.1f, 0.5f), new Vector2(0.1f, -0.3f), new Vector2(-0.3f, -0.2f)));
        }


    }
}
