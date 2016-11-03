using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsSimulator.GUI
{
    /// <summary>
    /// Interface určující základní vlastnosti prvků GUI.
    /// </summary>
    interface IFenceObject
    {
        bool Enabled { get; set; }
        float LayerDepth { get; set; }
        void Update(GameTime time, Vector2 margin);
        void Draw(SpriteBatch sprite, GameTime time, Vector2 margin);
    }
}
