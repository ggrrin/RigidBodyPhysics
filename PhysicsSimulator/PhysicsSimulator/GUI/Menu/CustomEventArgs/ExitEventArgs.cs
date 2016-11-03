using PhysicsSimulator.Engine.Game;

namespace PhysicsSimulator.GUI.Menu.CustomEventArgs
{
    /// <summary>
    /// Třída EventArgs poksytujci datové položky pro uložení některých základních typů, které se zrovna hodily.
    /// </summary>
    class ExitEventArgs
    {
        public bool IsContinue
        { get; set; }

        public int Number { get; set; }

        public string Output { get; set; }

        public EventBoxEnum BoxType { get; set; }

        public ExitEventArgs(int number)
        {
            this.Number = number;
        }

        public float NumberFL { get; set; }

        public ExitEventArgs(float fl)
        {
            this.NumberFL = fl;
        }

        public ExitEventArgs()
        {
            this.Output = null;
        }

        public ExitEventArgs(string output)
        {
            this.Output = output;
        }

        public ExitEventArgs(EventBoxEnum boxType)
        {
            this.BoxType = boxType;
        }
    }
}
