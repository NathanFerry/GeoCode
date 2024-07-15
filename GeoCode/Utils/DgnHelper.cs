using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.Interop.MicroStationDGN;
using Bentley.MstnPlatformNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharedCellDefinitionElement = Bentley.DgnPlatformNET.Elements.SharedCellDefinitionElement;

namespace GeoCode.Utils
{
    public class DgnHelper
    {
       

        private static DgnFile otherFile;

        public static bool LoadOtherDgnFile(string path)
        {
            DgnDocument dgnDocumentObj = DgnDocument.CreateForLocalFile(path);

            DgnFileOwner DgnFileOwnerObj = DgnFile.Create(dgnDocumentObj, DgnFileOpenMode.PreferableReadWrite);

            var OtherDGNFile = DgnFileOwnerObj.DgnFile;

            OtherDGNFile.LoadDgnFile(out var status);

            if (status == StatusInt.Success)
            {
                otherFile = OtherDGNFile;
                return true;
            } else
            {
                return false;
            }
        }
        public static IEnumerable<LevelHandle> GetAllLevelsFromLibrary()
        {
            

            if (otherFile != null)
            {
                return otherFile.GetLevelCache().GetHandles().Concat(
                    Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles()
                    ).Distinct();
            } else
            {
                return Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles();
            }
        }
        public static IEnumerable<SharedCellDefinitionElement> GetAllSharedCellsFromLibrary()
        {
           

            if (otherFile != null)
            {
                return otherFile.GetNamedSharedCellDefinitions().Concat(Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions());
            }
            else
            {
                return Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions();
            }
        }
    }
}
