/*--------------------------------------------------------------------------------------+
|   KeyinCommands.cs
|
|   Keyin command table class for Keyin command methods as defined in the command.xml.
|   Functions in Command.xml must be mapped to a method in KeyinCommands.cs.
|
+--------------------------------------------------------------------------------------*/

using GeoCode.UI;

namespace GeoCode
{
    /// <summary>
    /// Class used for running key-ins. The key-ins
    /// XML file commands.xml provides the class name and the method names.
    /// </summary>
    internal class KeyinCommands
    {
        /// <summary>
        /// Command to start SharedCellPickPlaceTool
        /// </summary>
        /// <param name="unparsed"></param>
        public static void CmdStartCellPickPlaceTool(string unparsed)
        {
            GeoCode.Instance().StartSharedCellPickPlaceTool();
            ElementSelector.ShowWindow();
        }
    }
}