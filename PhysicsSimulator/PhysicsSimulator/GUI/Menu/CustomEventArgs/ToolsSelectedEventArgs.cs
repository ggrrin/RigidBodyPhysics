using PhysicsSimulator.Editor.Menu;
using System;

namespace PhysicsSimulator.GUI.Menu.CustomEventArgs
{
    /// <summary>
    /// Třída Eventargs poskytujcí informace o vybraném toolu v editoru.
    /// </summary>
    class ToolsSelectedEventArgs : EventArgs
    {
        public Tools tool;

        public ToolsSelectedEventArgs(Tools sTool)
        {
            this.tool = sTool;
        }
    }
}
