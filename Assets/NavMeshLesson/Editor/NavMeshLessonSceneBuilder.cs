using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace NavMeshLesson.EditorTools
{
    /// <summary>
    /// Меню Tools → NavMesh Lesson → Build Demo Scene (Lesson 67).
    /// Збирає демо-сцену до уроку «Навігація AI: основи NavMesh та створення розумного NPC»:
    ///  • об'єкт Environment з компонентом NavMesh Surface (Collect Objects = Children) —
    ///    уся геометрія рівня лежить усередині нього, агенти й гравець — ні;
    ///  • три зони: A — перешкоди (F1, F3–F5), B — острови/міст/сходинки (F2), C — лабіринт (F6);
    ///  • агенти з NavMeshAgent і скриптами уроку, DemoSwitcher з камерними станціями і HUD;
    ///  • запікає NavMesh (як кнопка Bake) і зберігає його поруч зі сценою.
    /// Після зміни геометрії вручну достатньо натиснути Bake на Environment → NavMesh Surface.
    /// Потрібен пакет AI Navigation (com.unity.ai.navigation) — у проєкті він уже є.
    /// </summary>
    public static class NavMeshLessonSceneBuilder
    {
        private const string Root = "Assets/NavMeshLesson";
        private const string MaterialsDir = Root + "/Materials";
        private const string ScenesDir = Root + "/Scenes";
        private const string ScenePath = ScenesDir + "/NavMeshLesson.unity";
        private const string NavMeshPath = ScenesDir + "/NavMeshLesson_NavMesh.asset";

        [MenuItem("Tools/NavMesh Lesson/Build Demo Scene (Lesson 67)")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            if (File.Exists(ScenePath) &&
                !EditorUtility.DisplayDialog("NavMesh Lesson 67",
                    "Сцена вже існує:\n" + ScenePath + "\n\nПерезібрати її з нуля?", "Перезібрати", "Скасувати"))
            {
                return;
            }

            EnsureFolder(MaterialsDir);
            EnsureFolder(ScenesDir);

            // ---------- Матеріали ----------
            var mFloor    = Mat("M_Floor",    new Color(0.42f, 0.5f, 0.42f));
            var mObstacle = Mat("M_Obstacle", new Color(0.55f, 0.56f, 0.62f));
            var mWall     = Mat("M_Wall",     new Color(0.38f, 0.38f, 0.45f));
            var mIsland   = Mat("M_Island",   new Color(0.55f, 0.47f, 0.33f));
            var mBridge   = Mat("M_Bridge",   new Color(0.7f, 0.55f, 0.35f));
            var mStairs   = Mat("M_Stairs",   new Color(0.75f, 0.72f, 0.65f));
            var mSafe     = Mat("M_SafeZone", new Color(1f, 0.85f, 0.2f));
            var mNpc      = Mat("M_NPC",      new Color(1f, 0.6f, 0.2f));
            var mTruck    = Mat("M_NPC_Truck", new Color(0.65f, 0.4f, 0.95f));
            var mPlayer   = Mat("M_Player",   new Color(0.25f, 0.5f, 1f));
            var mEnemy    = Mat("M_Enemy",    new Color(0.9f, 0.22f, 0.2f));
            var mVisor    = Mat("M_Visor",    new Color(0.08f, 0.08f, 0.1f));
            var mTarget   = Mat("M_Target",   new Color(0.2f, 0.95f, 1f));
            var mTarget2  = Mat("M_Target2",  new Color(1f, 0.35f, 0.85f));
            var mLine     = LineMat("M_PathLine");

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            Camera cam = Camera.main;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);

            // ================= Environment + NavMesh Surface =================
            var env = new GameObject("Environment");
            var surface = env.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children; // лише геометрія всередині Environment
            Transform envT = env.transform;

            // ---------- Зона A: перешкоди ----------
            var zoneA = Group(envT, "Zone_A_Obstacles");
            Prim(PrimitiveType.Plane,    "Floor_A",           zoneA, new Vector3(0f, 0f, 0f),   Vector3.zero, new Vector3(3f, 1f, 2f),     mFloor);
            Prim(PrimitiveType.Cube,     "Obstacle_Cube",     zoneA, new Vector3(-3f, 1f, 0f),  Vector3.zero, new Vector3(3f, 2f, 3f),     mObstacle);
            Prim(PrimitiveType.Cylinder, "Obstacle_Cylinder", zoneA, new Vector3(2f, 1.5f, 2.5f), Vector3.zero, new Vector3(2f, 1.5f, 2f), mObstacle);
            Prim(PrimitiveType.Capsule,  "Obstacle_Capsule",  zoneA, new Vector3(5f, 2f, -3f),  Vector3.zero, new Vector3(2f, 2f, 2f),     mObstacle);
            Prim(PrimitiveType.Cube,     "Wall_A1",           zoneA, new Vector3(0f, 1f, 5.5f), Vector3.zero, new Vector3(10f, 2f, 0.5f),  mWall);
            Prim(PrimitiveType.Cube,     "Wall_A2",           zoneA, new Vector3(-6f, 1f, -3f), Vector3.zero, new Vector3(0.5f, 2f, 7f),   mWall);

            // ---------- Зона B: острови, міст, сходинки ----------
            var zoneB = Group(envT, "Zone_B_Islands");
            Prim(PrimitiveType.Cube, "Island_1",           zoneB, new Vector3(32f, 0.5f, 0f),   Vector3.zero, new Vector3(10f, 1f, 10f),  mIsland);
            Prim(PrimitiveType.Cube, "Bridge",             zoneB, new Vector3(39.5f, 0.85f, 0f), Vector3.zero, new Vector3(5f, 0.3f, 1.6f), mBridge);
            Prim(PrimitiveType.Cube, "Island_2",           zoneB, new Vector3(47f, 0.5f, 0f),   Vector3.zero, new Vector3(10f, 1f, 10f),  mIsland);
            Prim(PrimitiveType.Cube, "Island_3_NoBridge",  zoneB, new Vector3(47f, 0.5f, 13f),  Vector3.zero, new Vector3(6f, 1f, 6f),    mIsland);
            for (int i = 0; i < 5; i++)
            {
                float h = 0.35f * (i + 1); // висота сходинки 0.35 < Step Height 0.75
                Prim(PrimitiveType.Cube, $"Step_{i + 1}", zoneB,
                    new Vector3(47f, 1f + h / 2f, -4.5f + 0.8f * i), Vector3.zero, new Vector3(3f, h, 0.8f), mStairs);
            }
            Prim(PrimitiveType.Cube, "Upper_Deck", zoneB, new Vector3(47f, 1.875f, 1.1f), Vector3.zero, new Vector3(4f, 1.75f, 4f), mStairs);

            // ---------- Зона C: лабіринт ----------
            var zoneC = Group(envT, "Zone_C_Maze");
            zoneC.position = new Vector3(-40f, 0f, 0f);
            Prim(PrimitiveType.Plane, "Floor_C", zoneC, Vector3.zero, Vector3.zero, new Vector3(2.4f, 1f, 2.4f), mFloor);
            Vector4[] walls =
            {
                // (x, z, sizeX, sizeZ) у локальних координатах лабіринту
                new Vector4(0f, 12f, 24.5f, 0.5f), new Vector4(0f, -12f, 24.5f, 0.5f),
                new Vector4(12f, 0f, 0.5f, 24.5f), new Vector4(-12f, 0f, 0.5f, 24.5f),
                new Vector4(-4f, 8f, 16f, 0.5f),   new Vector4(4f, 4f, 16f, 0.5f),
                new Vector4(-4f, 0f, 16f, 0.5f),   new Vector4(4f, -4f, 16f, 0.5f),
                new Vector4(-4f, -8f, 16f, 0.5f),
            };
            for (int i = 0; i < walls.Length; i++)
            {
                Vector4 w = walls[i];
                Prim(PrimitiveType.Cube, $"MazeWall_{i + 1}", zoneC, new Vector3(w.x, 1f, w.y), Vector3.zero, new Vector3(w.z, 2f, w.w), mWall);
            }
            // Зона безпеки: висока платформа без сходів (1.5 м > Step Height 0.75)
            Prim(PrimitiveType.Cube, "SafeZone", zoneC, new Vector3(-9.5f, 0.75f, 10f), Vector3.zero, new Vector3(3f, 1.5f, 3f), mSafe);

            // ================= Камерні станції =================
            var stations = new GameObject("CameraStations").transform;
            var stA = Station(stations, "Station_A_Obstacles", new Vector3(0f, 18f, -17f),  new Vector3(48f, 0f, 0f));
            var stB = Station(stations, "Station_B_Islands",   new Vector3(40f, 20f, -14f), new Vector3(50f, 0f, 0f));
            var stC = Station(stations, "Station_C_Maze",      new Vector3(-40f, 30f, -6f), new Vector3(78f, 0f, 0f));
            cam.transform.SetPositionAndRotation(stA.position, stA.rotation);

            // ================= Демки =================
            var demosRoot = new GameObject("Demos").transform;

            // F1 — перший NPC (скрипт з уроку)
            var d1 = Demo(demosRoot, "F1_FirstNPC", "Перший NPC: NPCMover",
                "NPC отримує ціль один раз у Start() і сам обходить куб, циліндр, капсулу та стіни. " +
                "Лінія — шлях агента (agent.path). Блакитний NavMesh видно у Scene View. [R] — перезапуск.");
            var t1 = Target(d1.transform, "Target", new Vector3(11f, 0f, 6f), mTarget);
            var npc1 = Agent(d1.transform, "NPC", new Vector3(-11f, 1f, -6f), mNpc, mVisor, mLine, new Color(1f, 0.6f, 0.2f));
            npc1.gameObject.AddComponent<NPCMover>().target = t1;

            // F2 — ландшафт: сходинки, міст, острови
            var d2 = Demo(demosRoot, "F2_Terrain", "Майстер ландшафтів: сходинки, міст, острови",
                "ЛКМ по будь-якій поверхні — агент іде туди. Сходинки (0.35 м) нижчі за Step Height (0.75 м), тому агент по них піднімається. " +
                "Між першими островами — лише вузький міст. На дальній острів шляху немає: статус покаже PathPartial.");
            var clickMarker = Marker(d2.transform, "ClickMarker", mTarget);
            clickMarker.gameObject.SetActive(false);
            var npc2 = Agent(d2.transform, "NPC", new Vector3(32f, 2f, 0f), mNpc, mVisor, mLine, new Color(1f, 0.6f, 0.2f));
            npc2.gameObject.AddComponent<ClickToMoveAgent>().clickMarker = clickMarker;

            // F3 — налаштування агента
            var d3 = Demo(demosRoot, "F3_AgentSettings", "Налаштування NavMeshAgent",
                "Помаранчевий — звичайний агент: Speed 3.5, Acceleration 8, Stopping Distance 0. " +
                "Фіолетовий — «вантажівка»: Speed 2.5, Acceleration 0.6, Stopping Distance 3 — розганяється повільно і зупиняється за 3 м до цілі. [R] — ще раз.");
            var t3 = Target(d3.transform, "Target", new Vector3(11f, 0f, 6f), mTarget);
            var npcDefault = Agent(d3.transform, "NPC_Default", new Vector3(-11f, 1f, -7.5f), mNpc, mVisor, mLine, new Color(1f, 0.6f, 0.2f));
            npcDefault.gameObject.AddComponent<NPCMover>().target = t3;
            var npcTruck = Agent(d3.transform, "NPC_Truck", new Vector3(-11f, 1f, -4.5f), mTruck, mVisor, mLine, new Color(0.7f, 0.45f, 1f));
            npcTruck.speed = 2.5f;
            npcTruck.acceleration = 0.6f;
            npcTruck.stoppingDistance = 3f;
            npcTruck.gameObject.AddComponent<NPCMover>().target = t3;

            // F4 — старт по пробілу
            var d4 = Demo(demosRoot, "F4_SpaceStart", "Старт по пробілу (рівень 2)",
                "NPC стоїть, доки не натиснеш пробіл: SetDestination викликається в Update() за умовою Input.GetKeyDown(KeyCode.Space). [R] — повернути на старт.");
            var t4 = Target(d4.transform, "Target", new Vector3(11f, 0f, 6f), mTarget);
            var npc4 = Agent(d4.transform, "NPC", new Vector3(-11f, 1f, -6f), mNpc, mVisor, mLine, new Color(1f, 0.6f, 0.2f));
            npc4.gameObject.AddComponent<NPCMoverSpace>().target = t4;

            // F5 — дві цілі
            var d5 = Demo(demosRoot, "F5_Patrol", "Дві цілі: патруль (рівень 3)",
                "NPC іде до Target 1 (блакитна), а дійшовши (перевірка дистанції), розвертається до Target 2 (рожева) — і так по колу. [R] — перезапуск.");
            var t5a = Target(d5.transform, "Target 1", new Vector3(11f, 0f, -7.5f), mTarget);
            var t5b = Target(d5.transform, "Target 2", new Vector3(-11f, 0f, 7.5f), mTarget2);
            var npc5 = Agent(d5.transform, "NPC", new Vector3(-11f, 1f, 6f), mNpc, mVisor, mLine, new Color(1f, 0.6f, 0.2f));
            var patrol = npc5.gameObject.AddComponent<NPCPatrol>();
            patrol.target1 = t5a;
            patrol.target2 = t5b;

            // F6 — втеча з лабіринту
            var d6 = Demo(demosRoot, "F6_MazeEscape", "Головне завдання: втеча з лабіринту",
                "WASD/стрілки — рух, пробіл — стрибок. Червоний ворог переслідує тебе лабіринтом. " +
                "Жовта платформа — зона безпеки без сходів: застрибни на неї, і ворог не зможе дістатися. [M] — рівень 1/2, [R] — новий раунд.");
            var player = Prim(PrimitiveType.Capsule, "Player", d6.transform, new Vector3(-45f, 1.1f, 10f), Vector3.zero, Vector3.one, mPlayer, collider: false);
            Prim(PrimitiveType.Cube, "Visor", player.transform, new Vector3(0f, 0.45f, 0.42f), Vector3.zero, new Vector3(0.7f, 0.18f, 0.25f), mVisor, collider: false);
            var cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = Vector3.zero;
            var playerMover = player.AddComponent<PlayerMover>();

            var enemy = Agent(d6.transform, "Enemy", new Vector3(-30f, 1f, -10f), mEnemy, mVisor, mLine, new Color(1f, 0.3f, 0.25f));
            enemy.speed = 4f;
            var chaser = enemy.gameObject.AddComponent<EnemyChaser>();
            chaser.target = player.transform;
            var game = d6.AddComponent<MazeGame>();
            game.player = playerMover;
            game.enemy = chaser;

            var demos = new[] { d1, d2, d3, d4, d5, d6 };
            for (int i = 1; i < demos.Length; i++) demos[i].SetActive(false);

            var switcher = new GameObject("DemoSwitcher").AddComponent<DemoSwitcher>();
            switcher.demos = demos;
            switcher.cameraStations = new[] { stA, stB, stA, stA, stA, stC };

            // ================= Bake =================
            surface.BuildNavMesh(); // те саме, що кнопка Bake в інспекторі
            if (AssetDatabase.LoadAssetAtPath<NavMeshData>(NavMeshPath) != null)
                AssetDatabase.DeleteAsset(NavMeshPath);
            AssetDatabase.CreateAsset(surface.navMeshData, NavMeshPath);
            EditorUtility.SetDirty(surface);

            // ---------- Збереження ----------
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Selection.activeGameObject = env;
            Debug.Log($"[NavMesh Lesson 67] Сцену зібрано і NavMesh запечено: {ScenePath}. Play → F1–F6 або [ / ].");
        }

        // ================== Об'єкти ==================

        private static GameObject Demo(Transform parent, string name, string title, string hint)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var zone = go.AddComponent<DemoZone>();
            zone.title = title;
            zone.hint = hint;
            return go;
        }

        /// <summary>Капсула з NavMeshAgent (без колайдера — агенту він не потрібен) і лінією шляху.</summary>
        private static NavMeshAgent Agent(Transform parent, string name, Vector3 pos, Material body, Material visor, Material lineMat, Color lineColor)
        {
            var go = Prim(PrimitiveType.Capsule, name, parent, pos, Vector3.zero, Vector3.one, body, collider: false);
            Prim(PrimitiveType.Cube, "Visor", go.transform, new Vector3(0f, 0.45f, 0.42f), Vector3.zero, new Vector3(0.7f, 0.18f, 0.25f), visor, collider: false);

            var agent = go.AddComponent<NavMeshAgent>();
            agent.radius = 0.5f;
            agent.height = 2f;
            agent.baseOffset = 1f; // pivot капсули — у центрі, тому піднімаємо на половину висоти
            agent.speed = 3.5f;
            agent.acceleration = 8f;
            agent.angularSpeed = 360f;
            agent.stoppingDistance = 0f;

            var lineGo = new GameObject("PathLine");
            lineGo.transform.SetParent(go.transform, false);
            var lr = lineGo.AddComponent<LineRenderer>();
            lr.sharedMaterial = lineMat;
            lr.widthMultiplier = 0.08f;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.useWorldSpace = true;
            lr.positionCount = 0;
            lr.shadowCastingMode = ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lineGo.AddComponent<PathLine>().agent = agent;

            return agent;
        }

        private static Transform Target(Transform parent, string name, Vector3 pos, Material mat)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.position = pos;
            Prim(PrimitiveType.Cylinder, "Disc", t, new Vector3(0f, 0.02f, 0f), Vector3.zero, new Vector3(0.9f, 0.02f, 0.9f), mat, collider: false);
            Prim(PrimitiveType.Cylinder, "Pole", t, new Vector3(0f, 0.6f, 0f),  Vector3.zero, new Vector3(0.08f, 0.6f, 0.08f), mat, collider: false);
            return t;
        }

        private static Transform Marker(Transform parent, string name, Material mat)
        {
            var go = Prim(PrimitiveType.Cylinder, name, parent, Vector3.zero, Vector3.zero, new Vector3(0.7f, 0.02f, 0.7f), mat, collider: false);
            return go.transform;
        }

        private static Transform Group(Transform parent, string name)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            return t;
        }

        private static Transform Station(Transform parent, string name, Vector3 pos, Vector3 euler)
        {
            var t = new GameObject(name).transform;
            t.SetParent(parent, false);
            t.SetPositionAndRotation(pos, Quaternion.Euler(euler));
            return t;
        }

        private static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 euler,
            Vector3 scale, Material mat, bool collider = true)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = euler;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        // ================== Ассети ==================

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
            mat.color = color; // URP Lit: _BaseColor позначений [MainColor]
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
    }
}
