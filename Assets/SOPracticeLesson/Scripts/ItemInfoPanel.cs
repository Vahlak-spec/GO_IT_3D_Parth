using UnityEngine;
using UnityEngine.UI;

namespace SOPractice
{
    /// <summary>
    /// Блок «UI панель — ItemInfoPanel»: показує дані ItemData у UI.
    /// </summary>
    public class ItemInfoPanel : MonoBehaviour
    {
        [Header("UI Елементи")]
        public Text itemNameText;
        public Text itemTypeText;
        public Text itemDescriptionText;
        public Text itemValueText;
        public Image itemIcon;

        /// <summary>Доповнення демо-сцени: який предмет зараз показано (для HUD).</summary>
        public ItemData CurrentItem { get; private set; }

        void Start()
        {
            // На старті панель прихована.
            // Пастка: якщо вимкнути панель в ієрархії ще в редакторі, Start() не виконається до першого
            // ShowItem — а тоді виконається одразу після нього і сховає панель. Перший клік «не спрацює».
            gameObject.SetActive(false);
        }

        public void ShowItem(ItemData data)
        {
            if (data == null)
            {
                HidePanel();
                return;
            }

            gameObject.SetActive(true);
            CurrentItem = data;

            itemNameText.text        = data.itemName;
            itemTypeText.text        = "Тип: " + data.itemType.ToString();
            itemDescriptionText.text = data.description;
            itemValueText.text       = "Цінність: " + data.value;

            // В уроці тут лише if (data.icon != null) itemIcon.sprite = data.icon;
            // тоді предмет без іконки показує іконку попереднього. Ховаємо Image, якщо іконки немає.
            itemIcon.sprite = data.icon;
            itemIcon.enabled = data.icon != null;
        }

        public void HidePanel()
        {
            CurrentItem = null;
            gameObject.SetActive(false);
        }
    }
}
