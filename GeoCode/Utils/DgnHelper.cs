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
            return listCells.Concat(Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions());
        }


        public static bool LocateAllCellsModels()
        {
            var opts = CellLibraryOptions.Include3d
                | CellLibraryOptions.IncludeAllLibraries
                | CellLibraryOptions.IncludeParametric
                | CellLibraryOptions.IncludeShared
                | CellLibraryOptions.IncludeNonParametric
                | CellLibraryOptions.DefaultAll
                | CellLibraryOptions.Default
                ;
            var libs = new CellLibraryCollection(opts);

            List<DgnModel> cellModels = new List<DgnModel>();

            foreach (var lib in libs)
            {
                StatusInt status;
                var model = lib.File.LoadRootModelById(out status, lib.File.FindModelIdByName(lib.Name), true, false, true);

                if (model == null) { continue; }
                var hdlr = DgnComponentDefinitionHandler.GetForModel(model);
                var s = hdlr.DefinitionModelHandler.CreateCellDefinition(Session.Instance.GetActiveDgnFile());

                if (s == ParameterStatus.Success)
                {
                    Log.Write(lib.Name + " Chargé !");
                    cellModels.Add(model);
                }

            }
            return cellModels.Count > 0;
        }

    }
}
