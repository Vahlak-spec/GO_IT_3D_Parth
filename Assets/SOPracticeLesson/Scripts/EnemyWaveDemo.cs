using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SOPractice
{
    /// <summary>
    /// Практика 3.1 «Один скрипт — різні вороги».
    /// Спавнить трьох ворогів з одного префаба — кожному свій EnemyData.
    /// DamageAllEnemies() — метод з підказки уроку для UI-кнопки «Завдати шкоду».
    /// Respawn() — доповнення демо, бо Die() робить Destroy.
    /// </summary>
    public class EnemyWaveDemo : MonoBehaviour, IStatusSource
    {
        public Enemy enemyPrefab;
        public EnemyData[] enemyTypes;
        public Transform[] spawnPoints;
        public int damagePerClick = 25;

        private readonly List<Enemy> spawned = new List<Enemy>();

        void OnEnable() => Respawn();
        void OnDisable() => Clear();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) Respawn();
        }

        // Підказка з уроку для кнопки
        public void DamageAllEnemies()
        {
            // В уроці — FindObjectsOfType<Enemy>(). У Unity 6 він застарілий (obsolete),
            // сучасна заміна — FindObjectsByType.
            Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (Enemy enemy in allEnemies)
                enemy.TakeDamage(damagePerClick);
        }

        public void Respawn()
        {
            Clear();
            for (int i = 0; i < enemyTypes.Length && i < spawnPoints.Length; i++)
            {
                Enemy e = Instantiate(enemyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                e.data = enemyTypes[i]; // той самий префаб, але інший SO
                spawned.Add(e);
            }
        }

        void Clear()
        {
            foreach (var e in spawned)
                if (e != null) Destroy(e.gameObject);
            spawned.Clear();
        }

        public string Status
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var e in spawned)
                {
                    if (sb.Length > 0) sb.Append('\n');
                    if (e == null) { sb.Append("— знищений"); continue; }
                    if (e.data == null) continue;
                    sb.Append($"{e.data.enemyName}: HP {e.CurrentHealth}/{e.data.health}, speed {e.data.speed}, damage {e.data.damage}");
                }
                return sb.ToString();
            }
        }
    }
}
