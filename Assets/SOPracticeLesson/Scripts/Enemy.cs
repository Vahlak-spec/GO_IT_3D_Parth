using UnityEngine;

namespace SOPractice
{
    /// <summary>
    /// Скрипт ворога, що читає SO. Один скрипт для всіх — різниця лише в EnemyData.asset.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Дані ворога")]
        public EnemyData data;

        private int currentHealth;
        private Renderer rend;

        /// <summary>Доповнення демо-сцени: поточне HP для HUD.</summary>
        public int CurrentHealth => currentHealth;

        void Start()
        {
            if (data == null)
            {
                Debug.LogError("EnemyData не підключений! Об'єкт: " + gameObject.name);
                return;
            }

            // Ініціалізація з SO
            currentHealth = data.health;
            gameObject.name = data.enemyName;

            // Застосовуємо колір
            rend = GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = data.enemyColor;

            Debug.Log($"Створено: {data.enemyName} | HP: {currentHealth} | Speed: {data.speed}");
        }

        void Update()
        {
            // В уроці цієї перевірки немає: Start() при data == null робить return,
            // але Update() все одно звертається до data.speed — NullReferenceException щокадру.
            if (data == null) return;

            // Рух уперед зі швидкістю з SO
            transform.Translate(Vector3.forward * data.speed * Time.deltaTime);
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            Debug.Log($"{data.enemyName} отримав {amount} шкоди. HP: {currentHealth}");

            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            Debug.Log(data.enemyName + " знищений!");
            Destroy(gameObject);
        }
    }
}
