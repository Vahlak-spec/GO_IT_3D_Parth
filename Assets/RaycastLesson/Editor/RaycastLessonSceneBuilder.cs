using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaycastLesson.EditorTools
{
    /// <summary>
    /// Меню Tools → Raycast Lesson → Build Demo Scene.
    /// Збирає демо-сцену до уроку «Вступ до Raycast»:
    ///  • додає шар "Clickable" (шар "Obstacle" у проєкті вже є, якщо ні — теж створить);
    ///  • створює матеріали в Assets/RaycastLesson/Materials;
    ///  • будує сцену з об'єктами, камерою і всіма демо-скриптами;
    ///  • зберігає її в Assets/RaycastLesson/Scenes/RaycastLesson.unity.
    /// Запускати можна повторно — сцена і матеріали перезапишуться.
    /// </summary>
    public static class RaycastLessonSceneBuilder
    {
        private const string Root = "Assets/RaycastLesson";
        private const string MaterialsDir = Root + "/Materials";
        private const string ScenesDir = Root + "/Scenes";
        private const string ScenePath = ScenesDir + "/RaycastLesson.unity";

        [MenuItem("Tools/Raycast Lesson/Build Demo Scene")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            if (File.Exists(ScenePath) &&
                !EditorUtility.DisplayDialog("Raycast Lesson",
                    "Сцена вже існує:\n" + ScenePath + "\n\nПерезібрати її з нуля?", "Перезібрати", "Скасувати"))
            {
                return;
            }

            int clickableLayer = EnsureLayer("Clickable");
            int obstacleLayer = EnsureLayer("Obstacle");
            if (clickableLayer < 0 || obstacleLayer < 0)
            {
                EditorUtility.DisplayDialog("Raycast Lesson",
                    "Не вдалося створити шари Clickable / Obstacle — немає вільних слотів у Tags and Layers.", "OK");
                return;
            }

            EnsureFolder(Root);
            EnsureFolder(MaterialsDir);
            EnsureFolder(ScenesDir);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // ---------- Камера ----------
            Camera cam = Camera.main;
            cam.transform.SetPositionAndRotation(new Vector3(0f, 6.5f, -10f), Quaternion.Euler(32f, 0f, 0f));
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);

            // ---------- Підлога ----------
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(2.5f, 1f, 2f);
            SetMaterial(ground, "M_Ground", new Color(0.35f, 0.37f, 0.4f));

            var renderers = new List<Renderer> { ground.GetComponent<Renderer>() };
            var clickables = new List<ClickableObject>();

            // ---------- Об'єкти ----------
            // Два кубики на шарі Clickable (практика 3.1)
            var cubeA = Spawn(PrimitiveType.Cube, "Cube_Clickable_A", new Vector3(-4f, 0.5f, 0f), "M_Blue", new Color(0.25f, 0.55f, 1f), clickableLayer);
            var cubeB = Spawn(PrimitiveType.Cube, "Cube_Clickable_B", new Vector3(1.5f, 0.5f, 2.5f), "M_Green", new Color(0.3f, 0.8f, 0.4f), clickableLayer);

            // Кубик на шарі Obstacle — стоїть перед Cube_Clickable_B і частково його закриває.
            // У демках 1–2 промінь упирається в нього, у демці 3 — пролітає крізь нього.
            var obstacle = Spawn(PrimitiveType.Cube, "Cube_Obstacle", new Vector3(1f, 0.75f, 0.2f), "M_Obstacle", new Color(0.55f, 0.55f, 0.55f), obstacleLayer);
            obstacle.transform.localScale = new Vector3(1.4f, 1.5f, 0.4f);

            // Звичайні об'єкти на шарі Default
            var sphere = Spawn(PrimitiveType.Sphere, "Sphere_Default", new Vector3(-1.5f, 0.5f, 1f), "M_Orange", new Color(1f, 0.6f, 0.2f), 0);
            var capsule = Spawn(PrimitiveType.Capsule, "Capsule_Default", new Vector3(4.2f, 1f, 0.5f), "M_Purple", new Color(0.7f, 0.4f, 0.95f), 0);

            // Сфера БЕЗ колайдера — Raycast її не бачить (практика 1.1, останній пункт)
            var ghost = Spawn(PrimitiveType.Sphere, "Sphere_NoCollider", new Vector3(-4f, 0.6f, 3.5f), "M_Ghost", new Color(0.95f, 0.95f, 1f), 0);
            Object.DestroyImmediate(ghost.GetComponent<Collider>());

            foreach (var go in new[] { cubeA, cubeB, obstacle, sphere, capsule, ghost })
            {
                renderers.Add(go.GetComponent<Renderer>());
                if (go.GetComponent<Collider>() != null)
                {
                    var c = go.AddComponent<ClickableObject>();
                    c.enabled = false; // вмикає MouseEventsDemo
                    clickables.Add(c);
                }
            }

            // ---------- Демо-скрипти на камері ----------
            var basic = cam.gameObject.AddComponent<RaycastBasic>();
            var colorChanger = cam.gameObject.AddComponent<RaycastColorChanger>();
            var layerFilter = cam.gameObject.AddComponent<RaycastLayerFilter>();
            layerFilter.mask = 1 << clickableLayer;
            var mouseEvents = cam.gameObject.AddComponent<MouseEventsDemo>();
            mouseEvents.targets = clickables.ToArray();

            colorChanger.enabled = false;
            layerFilter.enabled = false;
            mouseEvents.enabled = false;

            var switcher = new GameObject("RaycastDemoSwitcher").AddComponent<RaycastDemoSwitcher>();
            switcher.demos = new MonoBehaviour[] { basic, colorChanger, layerFilter, mouseEvents };
            switcher.resettable = renderers.ToArray();

            // ---------- Збереження ----------
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Selection.activeGameObject = switcher.gameObject;
            Debug.Log($"[Raycast Lesson] Сцену зібрано: {ScenePath}. Запускай Play, перемикай демки клавішами 1–4.");
        }

        // ================== helpers ==================

        private static GameObject Spawn(PrimitiveType type, string name, Vector3 pos, string matName, Color color, int layer)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.position = pos;
            go.layer = layer;
            SetMaterial(go, matName, color);
            return go;
        }

        private static void SetMaterial(GameObject go, string matName, Color color)
        {
            go.GetComponent<Renderer>().sharedMaterial = GetOrCreateMaterial(matName, color);
        }

        private static Material GetOrCreateMaterial(string name, Color color)
        {
            string path = $"{MaterialsDir}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }

            // URP Lit позначає _BaseColor як [MainColor], тож material.color працює і тут, і в рантаймі
            mat.color = color;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string leaf = Path.GetFileName(path);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }

        /// <summary>Повертає індекс шару; якщо шару немає — створює його в першому вільному слоті (8–31).</summary>
        private static int EnsureLayer(string layerName)
        {
            int existing = LayerMask.NameToLayer(layerName);
            if (existing != -1) return existing;

            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (assets == null || assets.Length == 0) return -1;

            var tagManager = new SerializedObject(assets[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty slot = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(slot.stringValue))
                {
                    slot.stringValue = layerName;
                    tagManager.ApplyModifiedPropertiesWithoutUndo();
                    Debug.Log($"[Raycast Lesson] Додано шар \"{layerName}\" (#{i}).");
                    return i;
                }
            }
            return -1;
        }
    }
}
