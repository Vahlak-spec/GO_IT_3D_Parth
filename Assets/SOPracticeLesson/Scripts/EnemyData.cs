using UnityEngine;

namespace SOPractice
{
    /// <summary>
    /// Блок «Конфігурація ворогів через SO»: усі характеристики ворога — в .asset файлі.
    /// Меню змінене на "GameData (Lesson 64)/...", бо в проєкті вже є свій глобальний EnemyData.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "GameData (Lesson 64)/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("Основне")]
        public string enemyName;
        public int health;
        public float speed;
        public int damage;

        [Header("Візуал")]
        public Color enemyColor = Color.white;
        public Sprite enemySprite;
    }
}
