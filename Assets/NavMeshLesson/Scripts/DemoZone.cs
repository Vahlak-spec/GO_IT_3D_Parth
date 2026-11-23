using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshLesson
{
    /// <summary>
    /// Обв'язка однієї демки: запам'ятовує стартові позиції агентів, при вході в демку і по [R]
    /// повертає їх на місце й викликає Restart() у всіх IRestartable всередині.
    /// Статус для HUD збирається з усіх IStatusSource.
    /// </summary>
    public class DemoZone : MonoBehaviour, IDemo
    {
        public string title;
        [TextArea(2, 5)] public string hint;

        private readonly List<NavMeshAgent> agents = new List<NavMeshAgent>();
        private readonly List<Vector3> starts = new List<Vector3>();
        private readonly List<Quaternion> startRotations = new List<Quaternion>();

        public string Title => title;
        public string Hint => hint;

        public string Status
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var s in GetComponentsInChildren<IStatusSource>())
                {
                    if (sb.Length > 0) sb.Append('\n');
                    sb.Append(s.Status);
                }
                return sb.Length > 0 ? sb.ToString() : "—";
            }
        }

        void Awake()
        {
            foreach (var a in GetComponentsInChildren<NavMeshAgent>(true))
            {
                agents.Add(a);
                starts.Add(a.transform.position);
                startRotations.Add(a.transform.rotation);
            }
        }

        void OnEnable() => StartCoroutine(RestartNextFrame());

        void OnDisable()
        {
            // Агенти вже вимкнені разом з об'єктом — можна просто повернути transform
            for (int i = 0; i < agents.Count; i++)
                agents[i].transform.SetPositionAndRotation(starts[i], startRotations[i]);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) RestartZone();
        }

        public void RestartZone()
        {
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].isOnNavMesh) agents[i].Warp(starts[i]);
                agents[i].transform.rotation = startRotations[i];
            }
            foreach (var r in GetComponentsInChildren<IRestartable>())
                r.Restart();
        }

        IEnumerator RestartNextFrame()
        {
            yield return null; // даємо агентам стати на NavMesh після активації
            foreach (var r in GetComponentsInChildren<IRestartable>())
                r.Restart();
        }
    }
}
