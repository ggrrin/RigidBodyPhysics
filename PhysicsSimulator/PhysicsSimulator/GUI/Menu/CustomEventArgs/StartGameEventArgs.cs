
namespace PhysicsSimulator.GUI.Menu.CustomEventArgs
{
    /// <summary>
    /// Třida Eventargs poskytujcí informaci o určité mapě.
    /// </summary>
    class ExiEventArgs
    {
        private int levelPath;

        #region "Properties"

        public int LevelPath
        {
            get { return levelPath; }
            set { levelPath = value; }
        }

        #endregion

        public ExiEventArgs()
        {

        }
    }
}
