using UnityEngine;

namespace RaycastAdvanced
{
    /// <summary>
    /// Простий Health для сценарію «пробивна куля». В уроці клас Health використовується, але не показаний.
    /// Колір мішені плавно переходить із зеленого в червоний, на 0 HP мішень зникає.
    /// </summary>
    public class Health : MonoBehaviour
    {
        public int maxHealth = 100;
        public Color fullColor = new Color(0.3f, 0.85f, 0.4f);
        public Color emptyColor = new Color(0.9f, 0.2f, 0.2f);

        public int Current { get; private set; }

        private Renderer rend;

        void Awake()
        {
            rend = GetComponent<Renderer>();
            Current = maxHealth;
            UpdateColor();
        }

        public void TakeDamage(int amount)
        {
            Current = Mathf.Max(0, Current - amount);
            UpdateColor();

            if (Current == 0)
            {
                Debug.Log(name + " знищено");
                gameObject.SetActive(false);
            }
        }

        public void ResetHealth()
        {
            gameObject.SetActive(true);
            if (rend == null) rend = GetComponent<Renderer>();
            Current = maxHealth;
            UpdateColor();
        }

        void UpdateColor()
        {
            if (rend != null)
                rend.material.color = Color.Lerp(emptyColor, fullColor, (float)Current / maxHealth);
        }
    }
}
