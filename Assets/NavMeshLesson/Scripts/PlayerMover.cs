using UnityEngine;

namespace NavMeshLesson
{
    /// <summary>
    /// Керування гравцем для «Втечі з лабіринту»: WASD / стрілки + пробіл для стрибка.
    /// CharacterController не дає проходити крізь стіни і дозволяє застрибнути на «Зону безпеки».
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour
    {
        public float speed = 6f;
        public float jumpHeight = 2.2f;
        public float gravity = -20f;

        private CharacterController controller;
        private float verticalVelocity;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            Vector3 move = Vector3.ClampMagnitude(input, 1f) * speed;

            if (controller.isGrounded)
            {
                verticalVelocity = -1f;
                if (Input.GetKeyDown(KeyCode.Space))
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            verticalVelocity += gravity * Time.deltaTime;
            move.y = verticalVelocity;

            controller.Move(move * Time.deltaTime);

            if (input.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.z));
        }

        /// <summary>Телепорт: CharacterController треба вимкнути, інакше він перезапише позицію.</summary>
        public void Teleport(Vector3 position)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
            verticalVelocity = 0f;
        }
    }
}
