using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RaycastAdvanced.EditorTools
{
    /// <summary>
    /// Меню Tools → Raycast Lesson → Build Advanced Demo Scene (Lesson 66).
    /// Збирає демо-сцену до уроку «Raycast — розширений рівень»:
    ///  • шари "Ground" (створює, якщо немає) і "Obstacle" (у проєкті вже є);
    ///  • матеріали, префаби (кубик, дерево, House/Tower/Wall) і три BuildingData SO;
    ///  • три зони сцени: A — рельєф для нормалей/спавну/будівництва, B — тир, C — арена AI;
    ///  • DemoSwitcher із камерними станціями й HUD;
    ///  • зберігає в Assets/RaycastAdvancedLesson/Scenes/RaycastAdvanced.unity.
    /// Запускати можна повторно — все перезбирається.
    /// </summary>
    public static class RaycastAdvancedSceneBuilder
    {
        private const string Root = "Assets/RaycastAdvancedLesson";
        private const string MaterialsDir = Root + "/Materials";
        private const string PrefabsDir = Root + "/Prefabs";
        private const string DataDir = Root + "/Data";
        private const string ScenesDir = Root + "/Scenes";
        private const string ScenePath = ScenesDir + "/RaycastAdvanced.unity";

        [MenuItem("Tools/Raycast Lesson/Build Advanced Demo Scene (Lesson 66)")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            if (File.Exists(ScenePath) &&
                !EditorUtility.DisplayDialog("Raycast Lesson 66",
                    "Сцена вже існує:\n" + ScenePath + "\n\nПерезібрати її з нуля?", "Перезібрати", "Скасувати"))
            {
                return;
            }

            int groundLayer = EnsureLayer("Ground");
            int obstacleLayer = EnsureLayer("Obstacle");
            if (groundLayer < 0 || obstacleLayer < 0)
            {
                EditorUtility.DisplayDialog("Raycast Lesson 66",
                    "Не вдалося створити шари Ground / Obstacle — немає вільних слотів у Tags and Layers.", "OK");
                return;
            }
            int groundMask = 1 << groundLayer;
            int obstacleMask = 1 << obstacleLayer;

            EnsureFolder(MaterialsDir);
            EnsureFolder(PrefabsDir);
            EnsureFolder(DataDir);
            EnsureFolder(ScenesDir);

            // ---------- Матеріали ----------
            var mGround   = Mat("M_Ground",   new Color(0.42f, 0.52f, 0.38f));
            var mRamp     = Mat("M_Ramp",     new Color(0.62f, 0.52f, 0.36f));
            var mWall     = Mat("M_Wall",     new Color(0.55f, 0.56f, 0.62f));
            var mArrow    = Mat("M_Arrow",    new Color(0.2f, 0.45f, 1f));
            var mDisc     = Mat("M_Disc",     new Color(0.92f, 0.92f, 0.95f));
            var mCube     = Mat("M_SpawnCube", new Color(1f, 0.6f, 0.2f));
            var mTrunk    = Mat("M_Trunk",    new Color(0.45f, 0.3f, 0.18f));
            var mLeaves   = Mat("M_Leaves",   new Color(0.25f, 0.68f, 0.3f));
            var mPreview  = TransparentMat("M_Preview", new Color(0.3f, 1f, 0.4f, 0.4f));
            var mTarget   = Mat("M_Target",   new Color(0.3f, 0.85f, 0.4f));
            var mEnemy    = Mat("M_Enemy",    new Color(0.6f, 0.6f, 0.6f));
            var mVisor    = Mat("M_Visor",    new Color(0.08f, 0.08f, 0.1f));
            var mPlayer   = Mat("M_Player",   new Color(0.25f, 0.5f, 1f));
            var mMarker   = Mat("M_Marker",   new Color(0.2f, 0.95f, 1f));
            var mObstacle = Mat("M_Obstacle", new Color(0.38f, 0.38f, 0.44f));
            var mHouse    = Mat("M_House",    new Color(0.92f, 0.86f, 0.7f));
            var mRoof     = Mat("M_Roof",     new Color(0.72f, 0.25f, 0.2f));
            var mTower    = Mat("M_Tower",    new Color(0.6f, 0.62f, 0.68f));
            var mWallBld  = Mat("M_WallBuilding", new Color(0.76f, 0.64f, 0.48f));
            var mImpact   = Mat("M_Impact",   new Color(1f, 0.9f, 0.2f));
            var mLine     = LineMat("M_Line");

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            Camera cam = Camera.main;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);

            // ---------- Префаби (pivot унизу, без колайдерів) ----------
            var spawnCubePrefab = SavePrefab(MakeSpawnCube(mCube), "SpawnCube");
            var treePrefab      = SavePrefab(MakeTree("Tree", mTrunk, mLeaves), "Tree");
            var housePrefab     = SavePrefab(MakeHouse(mHouse, mRoof), "House");
            var towerPrefab     = SavePrefab(MakeTower(mTower, mRoof), "Tower");
            var wallPrefab      = SavePrefab(MakeWall(mWallBld), "Wall");

            // ---------- Scriptable Objects ----------
            var houseData = BuildingAsset("House", "House", 100, housePrefab);
            var towerData = BuildingAsset("Tower", "Tower", 250, towerPrefab);
            var wallData  = BuildingAsset("Wall",  "Wall",  50,  wallPrefab);

            // ================= Зона A: рельєф (шар Ground) =================
            var zoneA = new GameObject("Zone_A_Terrain").transform;
            Prim(PrimitiveType.Plane,  "Floor_A", zoneA, new Vector3(0f, 0f, 0f),     Vector3.zero,               new Vector3(3f, 1f, 2f),     mGround, groundLayer);
            Prim(PrimitiveType.Cube,   "Ramp_A",  zoneA, new Vector3(-7f, 1.2f, 2f),  new Vector3(0f, 0f, 22f),   new Vector3(7f, 0.4f, 5f),   mRamp,   groundLayer);
            Prim(PrimitiveType.Cube,   "Hill_B",  zoneA, new Vector3(6f, 0.7f, 3f),   new Vector3(-18f, 25f, 0f), new Vector3(5f, 0.4f, 5f),   mRamp,   groundLayer);
            Prim(PrimitiveType.Sphere, "Dome_C",  zoneA, new Vector3(0.5f, -1.8f, -1.5f), Vector3.zero,           Vector3.one * 6f,            mRamp,   groundLayer);
            Prim(PrimitiveType.Cube,   "Wall_D",  zoneA, new Vector3(0f, 2f, 7.5f),   Vector3.zero,               new Vector3(12f, 4f, 0.5f),  mWall,   groundLayer);

            // ================= Зона B: тир =================
            var zoneB = new GameObject("Zone_B_ShootingRange").transform;
            Prim(PrimitiveType.Plane, "Floor_B", zoneB, new Vector3(40f, 0f, 3f), Vector3.zero, new Vector3(1.5f, 1f, 2.5f), mGround, 0);
            float[] targetZ = { 0f, 2.5f, 5f, 7.5f, 10f };
            float[] jitterX = { -0.2f, 0.25f, -0.1f, 0.2f, -0.25f };
            var targets = new Health[targetZ.Length];
            for (int i = 0; i < targetZ.Length; i++)
            {
                var t = Prim(PrimitiveType.Cube, $"Target_{i + 1}", zoneB,
                    new Vector3(40f + jitterX[i], 1.2f, targetZ[i]), Vector3.zero, new Vector3(1.6f, 2.4f, 0.15f), mTarget, 0);
                targets[i] = t.AddComponent<Health>();
            }
            Prim(PrimitiveType.Cube, "Backstop", zoneB, new Vector3(40f, 2.5f, 14f), Vector3.zero, new Vector3(10f, 5f, 0.5f), mWall, 0);
            Prim(PrimitiveType.Cube, "Gun", zoneB, new Vector3(40.55f, 1.15f, -7.6f), Vector3.zero, new Vector3(0.12f, 0.12f, 0.7f), mVisor, 0, collider: false);
            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(zoneB, false);
            muzzle.position = new Vector3(40.55f, 1.15f, -7.25f);

            // ================= Зона C: арена AI =================
            var zoneC = new GameObject("Zone_C_VisionArena").transform;
            Prim(PrimitiveType.Plane, "Floor_C", zoneC, new Vector3(-40f, 0f, 0f), Vector3.zero, new Vector3(2f, 1f, 2f), mGround, groundLayer);
            Prim(PrimitiveType.Cube, "Wall_C1",  zoneC, new Vector3(-42f, 1f, -1f), Vector3.zero, new Vector3(4f, 2f, 0.5f),   mObstacle, obstacleLayer);
            Prim(PrimitiveType.Cube, "Wall_C2",  zoneC, new Vector3(-37f, 1f, -3f), Vector3.zero, new Vector3(0.5f, 2f, 4f),   mObstacle, obstacleLayer);
            Prim(PrimitiveType.Cube, "Wall_C3",  zoneC, new Vector3(-44f, 1f, 4f),  Vector3.zero, new Vector3(0.5f, 2f, 3f),   mObstacle, obstacleLayer);
            Prim(PrimitiveType.Cube, "Pillar_C", zoneC, new Vector3(-38f, 1f, 3f),  Vector3.zero, new Vector3(1.2f, 2f, 1.2f), mObstacle, obstacleLayer);

            var enemy = Prim(PrimitiveType.Capsule, "Enemy", zoneC, new Vector3(-45f, 1f, -6f), new Vector3(0f, 45f, 0f), Vector3.one, mEnemy, 0);
            Prim(PrimitiveType.Cube, "Visor", enemy.transform, new Vector3(0f, 0.45f, 0.42f), Vector3.zero, new Vector3(0.7f, 0.18f, 0.25f), mVisor, 0, collider: false);
            var sightLine = MakeRayLine("SightLine", zoneC, mLine, 0.06f);

            var player = Prim(PrimitiveType.Capsule, "Player", zoneC, new Vector3(-35f, 1f, 5f), Vector3.zero, Vector3.one, mPlayer, 0);
            var marker = Prim(PrimitiveType.Cylinder, "MoveTargetMarker", zoneC, new Vector3(-35f, 0.02f, 5f), Vector3.zero, new Vector3(0.7f, 0.02f, 0.7f), mMarker, 0, collider: false);
            marker.SetActive(false);

            var vision = enemy.AddComponent<EnemyVision>();
            vision.player = player.transform;
            vision.visionRange = 15f;
            vision.obstacleLayer = obstacleMask;
            vision.sightLine = sightLine;
            vision.body = enemy.GetComponent<Renderer>();
            vision.enabled = false;

            var mover = player.AddComponent<ClickToMove>();
            mover.moveSpeed = 5f;
            mover.groundLayer = groundMask;
            mover.targetMarker = marker.transform;
            mover.enabled = false;

            // ================= Камерні станції =================
            var stationsRoot = new GameObject("CameraStations").transform;
            var stationA = Station(stationsRoot, "Station_A_Terrain", new Vector3(0f, 11f, -15f), new Vector3(35f, 0f, 0f));
            var stationB = Station(stationsRoot, "Station_B_Range",   new Vector3(40f, 1.6f, -9f), Vector3.zero);
            var stationC = Station(stationsRoot, "Station_C_Arena",   new Vector3(-40f, 17f, -13f), new Vector3(55f, 0f, 0f));
            cam.transform.SetPositionAndRotation(stationA.position, stationA.rotation);

            // ================= UI для практики 3.1 =================
            var label = MakeBuildingLabel();

            // ================= Демки =================
            var demosRoot = new GameObject("Demos").transform;

            // F1 — нормалі
            var d1 = DemoObject(demosRoot, "F1_Normals");
            var arrow = new GameObject("NormalArrow").transform;
            arrow.SetParent(d1.transform, false);
            Prim(PrimitiveType.Cylinder, "Shaft", arrow, new Vector3(0f, 0.5f, 0f),   Vector3.zero, new Vector3(0.06f, 0.5f, 0.06f), mArrow, 0, collider: false);
            Prim(PrimitiveType.Sphere,   "Tip",   arrow, new Vector3(0f, 1f, 0f),     Vector3.zero, Vector3.one * 0.2f,               mArrow, 0, collider: false);
            Prim(PrimitiveType.Cylinder, "Disc",  arrow, new Vector3(0f, 0.005f, 0f), Vector3.zero, new Vector3(0.6f, 0.005f, 0.6f),  mDisc,  0, collider: false);
            arrow.gameObject.SetActive(false);
            d1.AddComponent<NormalVisualizer>().normalMarker = arrow;

            // F2 — базовий спавн
            var d2 = DemoObject(demosRoot, "F2_Spawner");
            var spawner = d2.AddComponent<ObjectSpawner>();
            spawner.prefabToSpawn = spawnCubePrefab;
            spawner.spawnableLayers = groundMask;

            // F3 — дерева
            var d3 = DemoObject(demosRoot, "F3_Trees");
            var preview = MakeTree("TreePreview", mPreview, mPreview);
            preview.transform.SetParent(d3.transform, false);
            foreach (var r in preview.GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = ShadowCastingMode.Off;
            preview.SetActive(false);
            var adv = d3.AddComponent<AdvancedSpawner>();
            adv.prefabToSpawn = treePrefab;
            adv.maxObjects = 15;
            adv.minDistanceBetween = 2f;
            adv.spawnableLayers = groundMask;
            adv.previewObject = preview;

            // F4 — пробивна куля
            var d4 = DemoObject(demosRoot, "F4_PenetratingShot");
            var shot = d4.AddComponent<PenetratingShot>();
            shot.shotPower = 3;
            shot.damage = 25;
            shot.muzzle = muzzle;
            shot.tracer = MakeRayLine("Tracer", d4.transform, mLine, 0.04f);
            shot.impactMaterial = mImpact;
            shot.targets = targets;

            // F5 — AI-видимість + рух по кліку
            var d5 = DemoObject(demosRoot, "F5_VisionArena");
            var arena = d5.AddComponent<VisionArenaDemo>();
            arena.enemyVision = vision;
            arena.clickToMove = mover;

            // F6 — будівництво з SO
            var d6 = DemoObject(demosRoot, "F6_Buildings");
            var placer = d6.AddComponent<BuildingPlacer>();
            placer.buildings = new[] { houseData, towerData, wallData };
            placer.currentBuilding = houseData;
            placer.buildableLayers = groundMask;
            placer.currentLabel = label;

            var demos = new[] { d1, d2, d3, d4, d5, d6 };
            for (int i = 1; i < demos.Length; i++) demos[i].SetActive(false);

            var switcher = new GameObject("DemoSwitcher").AddComponent<DemoSwitcher>();
            switcher.demos = demos;
            switcher.cameraStations = new[] { stationA, stationA, stationA, stationB, stationC, stationA };

            // ---------- Збереження ----------
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Selection.activeGameObject = switcher.gameObject;
            Debug.Log($"[Raycast Lesson 66] Сцену зібрано: {ScenePath}. Play → перемикай демки F1–F6 або [ / ].");
        }

        // ================== Префаби ==================

        private static GameObject MakeSpawnCube(Material m)
        {
            var root = new GameObject("SpawnCube");
            Prim(PrimitiveType.Cube, "Body", root.transform, new Vector3(0f, 0.4f, 0f), Vector3.zero, Vector3.one * 0.8f, m, 0, collider: false);
            return root;
        }

        private static GameObject MakeTree(string name, Material trunk, Material leaves)
        {
            var root = new GameObject(name);
            Prim(PrimitiveType.Cylinder, "Trunk", root.transform, new Vector3(0f, 0.5f, 0f), Vector3.zero, new Vector3(0.25f, 0.5f, 0.25f), trunk, 0, collider: false);
            Prim(PrimitiveType.Sphere,   "Crown", root.transform, new Vector3(0f, 1.4f, 0f), Vector3.zero, Vector3.one * 1.1f, leaves, 0, collider: false);
            return root;
        }

        private static GameObject MakeHouse(Material body, Material roof)
        {
            var root = new GameObject("House");
            Prim(PrimitiveType.Cube, "Body", root.transform, new Vector3(0f, 0.6f, 0f),  Vector3.zero, new Vector3(1.6f, 1.2f, 1.6f), body, 0, collider: false);
            Prim(PrimitiveType.Cube, "Roof", root.transform, new Vector3(0f, 1.35f, 0f), Vector3.zero, new Vector3(1.9f, 0.3f, 1.9f), roof, 0, collider: false);
            return root;
        }

        private static GameObject MakeTower(Material body, Material top)
        {
            var root = new GameObject("Tower");
            Prim(PrimitiveType.Cylinder, "Body", root.transform, new Vector3(0f, 1.5f, 0f),  Vector3.zero, new Vector3(0.9f, 1.5f, 0.9f), body, 0, collider: false);
            Prim(PrimitiveType.Cube,     "Top",  root.transform, new Vector3(0f, 3.15f, 0f), Vector3.zero, new Vector3(1.2f, 0.3f, 1.2f), top,  0, collider: false);
            return root;
        }

        private static GameObject MakeWall(Material m)
        {
            var root = new GameObject("Wall");
            Prim(PrimitiveType.Cube, "Body", root.transform, new Vector3(0f, 0.6f, 0f), Vector3.zero, new Vector3(3f, 1.2f, 0.35f), m, 0, collider: false);
            return root;
        }

        private static GameObject SavePrefab(GameObject go, string name)
        {
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabsDir}/{name}.prefab");
            Object.DestroyImmediate(go);
            return prefab;
        }

        private static BuildingData BuildingAsset(string file, string buildingName, int cost, GameObject prefab)
        {
            string path = $"{DataDir}/{file}.asset";
            var data = AssetDatabase.LoadAssetAtPath<BuildingData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<BuildingData>();
                AssetDatabase.CreateAsset(data, path);
            }
            data.buildingName = buildingName;
            data.cost = cost;
            data.prefab = prefab;
            EditorUtility.SetDirty(data);
            return data;
        }

        // ================== Сцена ==================

        private static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 euler,
            Vector3 scale, Material mat, int layer, bool collider = true)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = euler;
            go.transform.localScale = scale;
            go.layer = layer;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        private static GameObject DemoObject(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        private static Transform Station(Transform parent, string name, Vector3 pos, Vector3 euler)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.SetPositionAndRotation(pos, Quaternion.Euler(euler));
            return t;
        }

        private static RayLine MakeRayLine(string name, Transform parent, Material mat, float width)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.sharedMaterial = mat;
            lr.widthMultiplier = width;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.shadowCastingMode = ShadowCastingMode.Off;
            lr.receiveShadows = false;
            return go.AddComponent<RayLine>();
        }

        private static TMP_Text MakeBuildingLabel()
        {
            var canvasGo = new GameObject("UI_Canvas", typeof(Canvas), typeof(CanvasScaler));
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            var labelGo = new GameObject("CurrentBuildingLabel", typeof(RectTransform));
            labelGo.transform.SetParent(canvasGo.transform, false);
            var rt = (RectTransform)labelGo.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-30f, -30f);
            rt.sizeDelta = new Vector2(700f, 80f);

            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "Будівля: —";
            tmp.fontSize = 40;
            tmp.alignment = TextAlignmentOptions.TopRight;
            tmp.color = Color.white;

            labelGo.SetActive(false); // вмикає BuildingPlacer
            return tmp;
        }

        // ================== Матеріали / ассети ==================

        private static Material Mat(string name, Color color)
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
            // URP Lit позначає _BaseColor як [MainColor], тож material.color працює
            mat.color = color;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static Material TransparentMat(string name, Color color)
        {
            var mat = Mat(name, color);
            if (mat.shader != null && mat.shader.name.StartsWith("Universal Render Pipeline"))
            {
                mat.SetFloat("_Surface", 1f); // Transparent
                mat.SetFloat("_Blend", 0f);   // Alpha
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                mat.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
                mat.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
                mat.SetFloat("_ZWrite", 0f);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)RenderQueue.Transparent;
                mat.SetShaderPassEnabled("ShadowCaster", false);
            }
            mat.color = color;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static Material LineMat(string name)
        {
            string path = $"{MaterialsDir}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                // Sprites/Default враховує колір вершин — LineRenderer.startColor/endColor працюють
                Shader shader = Shader.Find("Sprites/Default");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            return mat;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
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
                    Debug.Log($"[Raycast Lesson 66] Додано шар \"{layerName}\" (#{i}).");
                    return i;
                }
            }
            return -1;
        }
    }
}
