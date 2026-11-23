using UnityEngine;

namespace SOPractice
{
    /// <summary>
    /// Допоміжний скрипт демо: Enemy з уроку йде вперед безкінечно і зникає з кадру.
    /// Коли ворог проходить maxZ — повертаємо його на початок доріжки (minZ).
    /// </summary>
    public class EnemyLane : MonoBehaviour
    {
        public float minZ = 1f;
        public float maxZ = 11f;

        void LateUpdate()
        {
            if (transform.position.z > maxZ)
            {
                Vector3 p = transform.position;
                p.z = minZ;
                transform.position = p;
            }
        }
    }
}
