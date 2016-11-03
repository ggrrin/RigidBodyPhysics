using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsSimulator.Engine.MathAddition;
using System;
using System.Collections.Generic;

namespace PhysicsSimulator
{
    /// <summary>
    /// Třída usnadňující vykreslování vektrů a bodů.
    /// </summary>
    [Serializable]
    class VectorDrawer
    {
        /// <summary>
        /// Vertex buffer
        /// </summary>
        List<VertexPositionColor> vertices;

        /// <summary>
        /// inicializuje tridu
        /// </summary>
        public VectorDrawer()
        {
            vertices = new List<VertexPositionColor>();
        }


        /// <summary>
        /// prida k vykreslovani bod danych vlastnosti dle parametru
        /// </summary>
        /// <param name="point">pozice</param>
        /// <param name="size">velikost krizku</param>
        /// <param name="rotation">pooteceni krizku</param>
        /// <param name="color">barva</param>
        public void Add(Vector2 point, float size, float rotation, Color color)
        {
            MatrixND rotationTranslation = Helper.CreateRotationMatrix(rotation);

            Vector2[] points = {
                                   point + size * Vector2.UnitX,
                                   point + -size * Vector2.UnitX,
                                   point + size * Vector2.UnitY,
                                   point + -size * Vector2.UnitY,
                               };

            foreach (Vector2 v in points)
                vertices.Add(new VertexPositionColor(new Vector3(Helper.Transform(rotationTranslation, v - point) + point, 0), color));
        }


        /// <summary>
        /// prida k vykreslovani dany vektor podle parametru
        /// </summary>
        /// <param name="orign">pocatek vektoru</param>
        /// <param name="vector">vektor k vykresleni</param>
        /// <param name="color">barva vektoru</param>
        public void Add(Vector2 orign, Vector2 vector, Color color)
        {
            vertices.Add(new VertexPositionColor(new Vector3(orign, 0), color));
            vertices.Add(new VertexPositionColor(new Vector3(orign + vector, 0), color));
        }

        /// <summary>
        /// vymaze vsechny veci k vykresleni
        /// </summary>
        public void Clear()
        {
            this.vertices.Clear();
        }


        /// <summary>
        /// vykresli vsechny veci v burferu
        /// </summary>
        public void Draw()
        {


            if (vertices.Count != 0)
            {
                VertexPositionColor[] fffff = vertices.ToArray();

                BasicEffect effect = Configuration.effect;
                GraphicsDevice device = Configuration.device;

                effect.VertexColorEnabled = true;
                effect.TextureEnabled = false;
                effect.LightingEnabled = false;
                effect.DirectionalLight0.Enabled = false;

                device.BlendState = BlendState.Opaque;
                device.DepthStencilState = DepthStencilState.Default;
                device.SamplerStates[0] = SamplerState.LinearWrap;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    try
                    {
                        device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, fffff, 0, fffff.Length / 2);
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

                device.BlendState = BlendState.Opaque;
                device.DepthStencilState = DepthStencilState.Default;
                device.SamplerStates[0] = SamplerState.LinearWrap;



            }
        }
    }
}
