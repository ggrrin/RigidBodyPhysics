
namespace PhysicsSimulator
{
#if WINDOWS || XBOX
    /// <summary>
    /// Program
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
    {

            using (GameCore game = new GameCore())
            {
                game.Run();
            }

            /*return;

            using (TestPhysics UI = new TestPhysics())
                UI.Run();

            return; 
            
            using (UserInteracter UI = new UserInteracter())
                UI.Run();

            return;

            using (Game1 game = new Game1())
            {
                game.Run();
            }*/

        }
    }
#endif
}

