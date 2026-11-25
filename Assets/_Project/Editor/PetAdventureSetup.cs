using System;
using System.IO;
using PetAdventure.Core;
using PetAdventure.Gameplay;
using PetAdventure.Pets;
using PetAdventure.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PetAdventure.EditorTools
{
    /// <summary>Công cụ một lần: sinh asset linh thú, sprite placeholder và scene chính.</summary>
    public static class PetAdventureSetup
    {
        private const string ProjectRoot = "Assets/_Project";
        private const string ArtRoot = "Assets/_Project/Resources/Art";
        private const string PetRoot = "Assets/_Project/Resources/Pets";
        private const string ScenePath = "Assets/_Project/Scenes/PetAdventure.unity";

        private sealed class PetSeed
        {
            public string Key;
            public string Name;
            public PetElement Element;
            public string Description;
        }

        private static readonly PetSeed[] Seeds =
        {
            new PetSeed { Key = "firefox", Name = "FireFox", Element = PetElement.Fire, Description = "Tinh linh lửa nhỏ bé, luôn mang ý chí chiến đấu." },
            new PetSeed { Key = "aqua", Name = "Aqua", Element = PetElement.Water, Description = "Tinh linh nước, mang lại sự bình yên." },
            new PetSeed { Key = "leafy", Name = "Leafy", Element = PetElement.Nature, Description = "Tinh linh thiên nhiên, bảo vệ khu rừng xanh." },
            new PetSeed { Key = "lumi", Name = "Lumi", Element = PetElement.Lightning, Description = "Tinh linh sấm sét, nhanh nhẹn và tinh nghịch." },
            new PetSeed { Key = "terran", Name = "Terran", Element = PetElement.Earth, Description = "Tinh linh đất, vững chãi và mạnh mẽ." }
        };

        [MenuItem("Tools/Pet Adventure/Build Project")]
        public static void BuildProject()
        {
            EnsureFolders();
            CreatePetAssets();
            CreateMainScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Pet Adventure] Build xong: asset linh thú + scene " + ScenePath);
        }

        [MenuItem("Tools/Pet Adventure/Build Scene Only")]
        public static void BuildSceneOnly()
        {
            EnsureFolders();
            CreateMainScene();
            AssetDatabase.SaveAssets();
            Debug.Log("[Pet Adventure] Đã dựng lại scene " + ScenePath);
        }

        [MenuItem("Tools/Pet Adventure/Self Check")]
        public static void SelfCheck()
        {
            bool ok = false;
            GameBootstrap bootstrap = null;

            try
            {
                EnsureFolders();
                GameObject root = new GameObject("SelfCheckRoot");
                bootstrap = root.AddComponent<GameBootstrap>();
                bootstrap.Initialize();

                ScreenManager manager = bootstrap.Ui.Manager;
                if (manager == null) throw new Exception("ScreenManager null");
                if (manager.ScreenCount != 9) throw new Exception("Số màn hình = " + manager.ScreenCount + ", cần 9");
                if (!manager.Has(ScreenId.PetCollection)) throw new Exception("Thiếu màn PetCollection");

                foreach (ScreenId id in Enum.GetValues(typeof(ScreenId)))
                {
                    if (!manager.Has(id)) throw new Exception("Thiếu màn " + id);
                }

                int buttons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
                int labels = UnityEngine.Object.FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
                int pets = manager.State.Pets.Count;
                int npcs = UnityEngine.Object.FindObjectsByType<NpcInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
                int collectibles = UnityEngine.Object.FindObjectsByType<Collectible>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;

                if (buttons < 20) throw new Exception("Quá ít button: " + buttons);
                if (labels < 40) throw new Exception("Quá ít label: " + labels);
                if (pets != PetElementInfo.All.Length) throw new Exception("Số linh thú demo = " + pets);
                if (bootstrap.Player == null) throw new Exception("Thiếu Player");
                if (bootstrap.Companion == null) throw new Exception("Thiếu PetCompanion");
                if (npcs != 3) throw new Exception("Số NPC = " + npcs + ", cần 3");
                if (collectibles != 5) throw new Exception("Số vật phẩm = " + collectibles + ", cần 5");

                Debug.Log($"[Pet Adventure] SELFCHECK PASS screens={manager.ScreenCount} buttons={buttons} labels={labels} pets={pets} npcs={npcs} items={collectibles}");
                ok = true;
            }
            catch (Exception exception)
            {
                Debug.LogError("[Pet Adventure] SELFCHECK FAIL: " + exception);
            }
            finally
            {
                if (bootstrap != null)
                {
                    UnityEngine.Object.DestroyImmediate(bootstrap.gameObject);
                }
            }

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(ok ? 0 : 1);
            }
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                ProjectRoot,
                ProjectRoot + "/Scripts",
                ProjectRoot + "/Scenes",
                ProjectRoot + "/Prefabs",
                ProjectRoot + "/ScriptableObjects",
                ProjectRoot + "/Art",
                ProjectRoot + "/Audio",
                "Assets/_Project/Resources",
                ArtRoot,
                ArtRoot + "/Pets",
                ArtRoot + "/PetIcons",
                PetRoot
            };

            foreach (string folder in folders)
            {
                if (AssetDatabase.IsValidFolder(folder)) continue;

                string parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
                string name = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private static void CreatePetAssets()
        {
            foreach (PetSeed seed in Seeds)
            {
                string iconPath = ArtRoot + "/PetIcons/" + seed.Key + ".png";
                string worldPath = ArtRoot + "/Pets/" + seed.Key + "_world.png";

                Sprite icon = WriteSprite(iconPath, SpriteFactory.PetIcon(seed.Element), 96f, FilterMode.Bilinear);
                Sprite world = WriteSprite(worldPath, SpriteFactory.PetWorld(seed.Element), 32f, FilterMode.Point);

                string assetPath = PetRoot + "/Pet_" + seed.Name + ".asset";
                PetDefinition definition = AssetDatabase.LoadAssetAtPath<PetDefinition>(assetPath);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<PetDefinition>();
                    AssetDatabase.CreateAsset(definition, assetPath);
                }

                definition.Configure(seed.Key, seed.Name, seed.Element, seed.Description);
                definition.icon = icon;
                definition.overworldSprite = world;
                EditorUtility.SetDirty(definition);
            }

            AssetDatabase.SaveAssets();
        }

        private static Sprite WriteSprite(string path, Sprite source, float pixelsPerUnit, FilterMode filter)
        {
            Texture2D texture = source.texture;
            byte[] png = texture.EncodeToPNG();
            string absolute = Path.Combine(Directory.GetCurrentDirectory(), path);
            Directory.CreateDirectory(Path.GetDirectoryName(absolute) ?? ".");
            File.WriteAllBytes(absolute, png);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = pixelsPerUnit;
                importer.filterMode = filter;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void CreateMainScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject game = new GameObject("Game");
            game.AddComponent<GameBootstrap>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }
    }
}
