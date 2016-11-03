using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PhysicsSimulator
{
    /// <summary>
    /// Třída shrnujcí základní věci, které jsou potřeba na ruzných místech v celé aplikaci.
    /// </summary>
    static class Configuration
    {
        /// <summary>
        /// graficke zarizeni
        /// </summary>
        public static GraphicsDevice device;
        /// <summary>
        /// effect pouzivany pro vykreslovani
        /// </summary>
        public static BasicEffect effect;

        /// <summary>
        /// Limit pro vykreslovani nekonecny dlouhych primek
        /// </summary>
        public static Vector2 rightTopOfScreen = new Vector2(10000, 10000);
        /// <summary>
        /// Limit pro vykreslovani nekonecny dlouhych primek
        /// </summary>
        public static Vector2 leftBottomOfScreen = -rightTopOfScreen;

        /// <summary>
        /// maximalni úhlová rychlost objektů
        /// </summary>
        public const float maxRotation = 5f;
        /// <summary>
        /// maximalní rychlost objektů
        /// </summary>
        public const float maxSpeed = 80;

        /// <summary>
        /// Nastavení rozlišení okna/obrazovky
        /// </summary>
        public static GraphicConnfiguration settingG;

        /// <summary>
        /// používaný profil s informaci o splněných levelech
        /// </summary>
        public static Profile useProfile;

        /// <summary>
        /// počet pixelů ve výšsce aktualního okna
        /// </summary>
        public static float height;

        /// <summary>
        /// Počet pixelnů na šířce aktuálního okna.
        /// </summary>
        public static float width;

        /// <summary>
        /// Určuje jak scalovat GUI podle rozlišení. Uděláno tak že na každém monitoru je pomer šířky objektu ku šířky monitoru stejný
        /// </summary>
        public static float menuScale;

        /// <summary>
        /// content manager pro nacitani kompilovanych dat
        /// </summary>
        public static ContentManager content;

        /// <summary>
        /// graphic device manager
        /// </summary>
        public static GraphicsDeviceManager graphic;


        /// <summary>
        /// Hustota nehybnych objektů
        /// </summary>
        public const float stuckDensity = 999999999999999999;

        /// <summary>
        /// Inicializuje Configurační třídu
        /// </summary>
        /// <param name="graphics">graphic device manager</param>
        /// <param name="Service">service</param>
        /// <param name="eff">effect pro vykreslovani</param>
        public static void Initialize(GraphicsDeviceManager graphics, IServiceProvider Service, BasicEffect eff)
        {
            graphic = graphics;
            device = graphic.GraphicsDevice;
            effect = eff;
            //graphicDivice = graphics.GraphicsDevice;

            if (!Directory.Exists("Content\\Profiles"))
                Directory.CreateDirectory("Content\\Profiles");

            if (!Directory.Exists("Content\\Maps"))
                Directory.CreateDirectory("Content\\Maps");

            if (!Directory.Exists("Content\\Configuration"))
                Directory.CreateDirectory("Content\\Configuration");

            if (Directory.GetFiles("Content\\Profiles").Length == 0)
            {
                useProfile = new Profile();
                SaveProfile();
            }
            else
            {
                using (Stream lStream = new FileStream("Content\\Profiles\\profile.hmp", FileMode.Open))
                {
                    IFormatter lFormatter = new BinaryFormatter();
                    useProfile = (Profile)lFormatter.Deserialize(lStream);
                }
            }

            if (Directory.GetFiles("Content\\Configuration").Length == 0)
            {
                settingG = new GraphicConnfiguration();
                settingG.SaveResolution();
            }
            else
            {
                using (Stream lStream = new FileStream("Content\\Configuration\\GraphicConnfiguration.hmc", FileMode.Open))
                {
                    IFormatter lFormatter = new BinaryFormatter();
                    settingG = (GraphicConnfiguration)lFormatter.Deserialize(lStream);
                }
            }

            graphic.PreferredBackBufferWidth = settingG.GetResolution().Width;
            graphic.PreferredBackBufferHeight = settingG.GetResolution().Height;
            graphic.IsFullScreen = settingG.IsFullScreen;

            content = new ContentManager(Service, "Content");
            height = graphic.PreferredBackBufferHeight;
            width = graphic.PreferredBackBufferWidth;


            menuScale = (float)width / 1920f;

            graphic.ApplyChanges();
        }

        /// <summary>
        /// Aktualizuje data v teto tride podle novych dat z graphic
        /// </summary>
        public static void ReinitializeGraphics()
        {
            graphic.PreferredBackBufferWidth = settingG.GetResolution().Width;
            graphic.PreferredBackBufferHeight = settingG.GetResolution().Height;
            graphic.IsFullScreen = settingG.IsFullScreen;

            height = graphic.PreferredBackBufferHeight;
            width = graphic.PreferredBackBufferWidth;

            menuScale = (float)width / 1920f;



            graphic.ApplyChanges();
        }


        /// <summary>
        /// Uloží profil s informaci o levelech
        /// </summary>
        public static void SaveProfile()
        {
            using (Stream lStream = new FileStream("Content\\Profiles\\profile.hmp", FileMode.Create))
            {
                IFormatter lFormatter = new BinaryFormatter();
                lFormatter.Serialize(lStream, useProfile);
            }
        }


    }


}
