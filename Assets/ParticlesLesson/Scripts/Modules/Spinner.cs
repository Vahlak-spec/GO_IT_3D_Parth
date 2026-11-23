using UnityEngine;

namespace ParticlesLesson
{
    /// <summary>Просто обертає об'єкт (диск болгарки).</summary>
    public class Spinner : MonoBehaviour
    {
        public Vector3 axis = Vector3.up;
        [Tooltip("Градусів за секунду")]
        public float speed = 1200f;

        void Update()
        {
            transform.Rotate(axis, speed * Time.deltaTime, Space.Self);
        }
    }
}
