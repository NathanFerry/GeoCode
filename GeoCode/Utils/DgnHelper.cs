using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET;
using System.Collections.Generic;
using System.Linq;
using SharedCellDefinitionElement = Bentley.DgnPlatformNET.Elements.SharedCellDefinitionElement;

namespace GeoCode.Utils
{
    public class DgnHelper
    {
       

        private static List<DgnFile> otherFiles = new List<DgnFile>();
        private static List<LevelHandle> levelHandles = new List<LevelHandle>();
        private static List<SharedCellDefinitionElement> listCells = new List<SharedCellDefinitionElement>();

        public static bool LoadOtherDgnFile(string path)
        {
            DgnDocument dgnDocumentObj = DgnDocument.CreateForLocalFile(path);

            

            DgnFileOwner DgnFileOwnerObj = DgnFile.Create(dgnDocumentObj, DgnFileOpenMode.PreferableReadWrite);

            var OtherDGNFile = DgnFileOwnerObj.DgnFile;

            OtherDGNFile.LoadDgnFile(out var status);

            if (status == StatusInt.Success)
            {
                otherFiles.Add(OtherDGNFile);

                // Ajoute les niveaux du fichier à charger
                levelHandles.AddRange(OtherDGNFile.GetLevelCache().GetHandles());
                foreach (var level in OtherDGNFile.GetLevelCache().GetHandles())
                {
                    try
                    {
                        Session.Instance.GetActiveDgnFile().GetLevelCache().CopyLevel(level);
                    }
                    catch
                    {
                        Log.Write("Impossible de copier le level " + level.Name);
                    }
                }

                // Ajoute les cellules du fichier à charger
                listCells.AddRange(OtherDGNFile.GetNamedSharedCellDefinitions());
                foreach (var cell in OtherDGNFile.GetNamedSharedCellDefinitions())
                {
                    cell.AddChildComplete();
                }

                return true;
            } else
            {
                return false;
            }
        }
        public static IEnumerable<LevelHandle> GetAllLevelsFromLibrary()
        {

            return levelHandles.Concat(
                    Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles()
                    ).Distinct();
        }
        public static IEnumerable<SharedCellDefinitionElement> GetAllSharedCellsFromLibrary()
        {
            Session.Instance.GetActiveDgnModel().ReadAndLoadDgnAttachments(new DgnAttachmentLoadOptions(true, true, true));
            
            return listCells.Concat(Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions());
           

        }
    }
}
