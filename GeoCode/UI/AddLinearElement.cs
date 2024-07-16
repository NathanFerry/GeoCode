/*--------------------------------------------------------------------------------------+
|   AddLinearElement.cs
|
+--------------------------------------------------------------------------------------*/

#region System Namespaces
using System;
using System.Windows.Controls;
#endregion

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET.WPF;
using Bentley.Windowing;

#endregion

namespace GeoCode.UI
{
    public partial class AddLinearElement : UserControl
    {
        #region Bentley DockableWindow
        private static DockableWindow AddLinearElementDockableWindow { get; set; }

        internal static void ShowWindow(string unparsed = "")
        {
            if (null != AddLinearElementDockableWindow)
            {
                AddLinearElementDockableWindow.Focus();
                return;
            }

            AddLinearElementDockableWindow = new DockableWindow
            {
                Content = new AddLinearElement()
            };
            AddLinearElementDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(AddLinearElementDockableWindow.MinWidth),
                        Convert.ToInt32(AddLinearElementDockableWindow.MinHeight)));
            AddLinearElementDockableWindow.WindowContent.CanDockVertically = false;
            AddLinearElementDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
        }

        internal static void CloseWindow(string unparsed = "")
        {
            if (null == AddLinearElementDockableWindow)
            {
                return;
            }

            AddLinearElementDockableWindow.Close();
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        {
            e.CloseAction = ContentCloseAction.Dispose;
            if (null != AddLinearElementDockableWindow)
            {
                AddLinearElementDockableWindow.Detach();
                AddLinearElementDockableWindow.Dispose();
                AddLinearElementDockableWindow = null;
            }
        }
        #endregion
    }
}