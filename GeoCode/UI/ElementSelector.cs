/*--------------------------------------------------------------------------------------+
|   ElementSelector.cs
|
+--------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using Bentley.MstnPlatformNET.WPF;
using Bentley.Windowing;
using GeoCode.Saving;
using GeoCode.Utils;

#region  Documentation

/*
|   This is the main menu of the GUI.
|   It's using WPF.
|   If you have any question, please refer to the documentation of WPF or the website https://wpf-tutorial.com/ .
*/

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
            ElementSelectorDockableWindow.Title = "Geocode sélection";
            ElementSelectorDockableWindow.Content = new ElementSelector();
            ElementSelectorDockableWindow.Attach(GeoCode.Addin, "control", new Size(Convert.ToInt32(ElementSelectorDockableWindow.MinWidth),
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