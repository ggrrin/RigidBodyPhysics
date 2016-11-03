using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PhysicsSimulator
{

    /// <summary>
    /// Třída reprezentující data o rozlišení okna aplikace.
    /// </summary>
    [Serializable]
    public class GraphicConnfiguration
    {
        private Resolution[] resolutios;
        private uint selectResolution;

        /// <summary>
        /// vrati pole vsech moznych rozliseni
        /// </summary>
        public Resolution[] Resolution
        {
            get { return resolutios; }
        }

        /// <summary>
        /// vrati index vybraneho rozliseni
        /// </summary>
        public uint SelectResolution
        {
            get { return selectResolution; }
        }

        /// <summary>
        /// vrati zda je bezet ve fullscreen
        /// </summary>
        public bool IsFullScreen
        { get; set; }

        /// <summary>
        /// inicializuju pole moznych rozliseni
        /// </summary>
        public GraphicConnfiguration()
        {
            IsFullScreen = false;
            resolutios = new Resolution[18];
            resolutios[0] = new Resolution(640, 480);
            resolutios[1] = new Resolution(720, 480);
            resolutios[2] = new Resolution(720, 576);
            resolutios[3] = new Resolution(800, 600);
            resolutios[4] = new Resolution(844, 480);
            resolutios[5] = new Resolution(1024, 768);
            resolutios[6] = new Resolution(1152, 720);
            resolutios[7] = new Resolution(1152, 864);
            resolutios[8] = new Resolution(1280, 720);
            resolutios[9] = new Resolution(1280, 768);
            resolutios[10] = new Resolution(1280, 800);
            resolutios[11] = new Resolution(1280, 960);
            resolutios[12] = new Resolution(1280, 1024);
            resolutios[13] = new Resolution(1360, 768);
            resolutios[14] = new Resolution(1360, 1024);
            resolutios[15] = new Resolution(1600, 900);
            resolutios[16] = new Resolution(1680, 1050);
            resolutios[17] = new Resolution(1920, 1080);

            selectResolution = 17;
        }

        /// <summary>
        /// vrati vybrane rozliseni
        /// </summary>
        /// <returns></returns>
        public Resolution GetResolution()
        {
            return resolutios[selectResolution];
        }


        /// <summary>
        /// vybere urozliseni polde undexu
        /// </summary>
        /// <param name="index">index rozliseni</param>
        public void SetResolution(uint index)
        {
            if (index < resolutios.Length)
                selectResolution = index;
            else
                throw new Exception("You set index, which doesn't implement resolution. ");
        }

        /// <summary>
        /// vrati nasledujci rozliseni v poli
        /// </summary>
        public void NextResolution()
        {
            if (selectResolution < resolutios.Length - 1)
            {
                selectResolution++;
            }
        }

        /// <summary>
        /// vrati predchozi rozliseni v poli
        /// </summary>
        public void PreviousResolution()
        {
            if (selectResolution > 0)
            {
                selectResolution--;
            }
        }

        /// <summary>
        ///uloží informace osoučasném rozlišení na disk
        /// </summary>
        public void SaveResolution()
        {
            using (Stream lStream = new FileStream("Content\\Configuration\\GraphicConnfiguration.hmc", FileMode.Create))
            {
                IFormatter lFormatter = new BinaryFormatter();
                lFormatter.Serialize(lStream, this);
            }
        }

    }

    /// <summary>
    /// Struktura uchovavajici informace o rozliseni
    /// </summary>
    [Serializable]
    public struct Resolution
    {
        private int width;
        private int height;

        /// <summary>
        /// sířka
        /// </summary>
        public int Width
        { get { return width; } set { width = value; } }

        /// <summary>
        /// výška
        /// </summary>
        public int Height
        { get { return height; } set { height = value; } }

        /// <summary>
        /// inicilizuje rozlišeni podle parametru
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Resolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// prevede rozlišeni do textove podoby
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} x {1}", width, height);
        }
    }
}
