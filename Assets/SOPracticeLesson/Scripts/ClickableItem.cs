using UnityEngine;
using UnityEngine.EventSystems;

namespace SOPractice
{
    /// <summary>
    /// Блок «OnPointerClick через EventSystem».
    /// Працює на 3D-об'єктах, лише якщо на камері є Physics Raycaster, а на сцені — EventSystem.
    /// </summary>
    // Цей скрипт чіпляємо замість або разом з SelectableItem
    public class ClickableItem : MonoBehaviour, IPointerClickHandler
    {
        public ItemData itemData;

        // Посилання на панель — перетягни в Inspector
        public ItemInfoPanel displayPanel;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemData != null)
            {
                displayPanel.ShowItem(itemData);
                Debug.Log("Вибрано: " + itemData.itemName);
            }
        }
    }
}
