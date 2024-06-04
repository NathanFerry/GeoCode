using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using GeoCode.Model;
using Newtonsoft.Json;

namespace GeoCode.Saving;

public static class SaveManager
{
    private static readonly DirectoryInfo DirectoryInfo =
        new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.CreateSubdirectory("CodifGeo");

    private static readonly string FilePath = DirectoryInfo.FullName + Path.DirectorySeparatorChar + "config.json";
    public static void Save()
    {
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(GeoCode.Application, Formatting.Indented));
    }

    public static Application Load()
    {
        if (!File.Exists(FilePath))
        {
            return new Application()
            {
                Categories = new ObservableCollection<Category>()
            };
        }
        var serializedData = new FileInfo(FilePath).OpenText().ReadToEnd();
        return JsonConvert.DeserializeObject<Application>(serializedData);
    }
}