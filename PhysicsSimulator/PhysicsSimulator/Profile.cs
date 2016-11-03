using System;

namespace PhysicsSimulator
{
    /// <summary>
    /// Třída pro uložení levelu hry.
    /// </summary>
    [Serializable]
    class Profile
    {
        /// <summary>
        /// pole s informaci jaky level je hotov
        /// </summary>
        private bool[] levels;
        /// <summary>
        /// index v poli posldniho levelu 
        /// </summary>
        private int lastLevel;

        /// <summary>
        /// index v poli posldniho levelu 
        /// </summary>
        public int LastLevel
        {
            get { return lastLevel; }
        }

        /// <summary>
        /// inicializuje profil
        /// </summary>
        public Profile()
        {
            levels = new bool[System.IO.Directory.GetFiles("Content\\Maps").Length];
            lastLevel = 0;
        }

        /// <summary>
        /// ulozi ze dany level byl udelan
        /// </summary>
        /// <param name="level">ten level</param>
        public void CompleteLevel(int level)
        {
            levels[level] = true;

            if (level + 1 < levels.Length)
                lastLevel = level + 1;
        }
    }
}
