/*--------------------------------------------------------------------------------------+
|   ElementSelector.cs
|
+--------------------------------------------------------------------------------------*/


using System.Collections.Generic;
using System.Windows;
using GeoCode.Saving;
using GeoCode.ViewModel;

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
    public partial class ElementSelector : UserControl
    {
        #region Bentley DockableWindow

        public static DockableWindow ElementSelectorDockableWindow { get; set; }
        public static readonly List<DockableWindow> OwnedWindows = new();

        internal static void ShowWindow(string unparsed = "")
        {
            if (null != ElementSelectorDockableWindow)
            {
                ElementSelectorDockableWindow.Focus();
                return;
            }

            ElementSelectorDockableWindow = new DockableWindow();
            ElementSelectorDockableWindow.Content = new ElementSelector();
            ElementSelectorDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(ElementSelectorDockableWindow.MinWidth),
                        Convert.ToInt32(ElementSelectorDockableWindow.MinHeight)));
            ElementSelectorDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        { 
            OwnedWindows.ForEach(it => it.Close());
            SaveManager.Save();
            e.CloseAction = ContentCloseAction.Dispose;
            if (null == ElementSelectorDockableWindow) return;
            ElementSelectorDockableWindow.Detach();
            ElementSelectorDockableWindow.Dispose();
            ElementSelectorDockableWindow = null;
        }
        #endregion
    }
}