using UnityEditor;
using UnityEngine;
using System.IO;

public class FolderStructureGenerator
{
    [MenuItem("Tools/Create Default Folder Structure")]
    public static void CreateFolderStructure()
    {
        string[] folders = new string[]
        {
            "Art/A_KYR",
            "Art/A_KDY",
            "Audio/BGM",
            "Audio/SFX",
            "Materials",
            "Prefabs/Characters",
            "Prefabs/Enemies",
            "Prefabs/UI",
            "0_Scenes/P_Euni",
            "0_Scenes/P_CTH",
            "0_Scenes/P_PJH",
            "0_Scenes/P_JYS",
            "1_Scripts/P_Euni",
            "1_Scripts/P_CTH",
            "1_Scripts/P_PJH",
            "1_Scripts/P_JYS",
            "Animations",
            "Fonts",
            "Resources"
        };

        foreach (string folder in folders)
        {
            string folderPath = Path.Combine("Assets", folder);
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log("Created folder: " + folderPath);
            }
        }

        AssetDatabase.Refresh();
    }
}
