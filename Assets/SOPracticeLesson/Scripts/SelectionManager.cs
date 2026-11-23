using UnityEngine;
using UnityEngine.EventSystems;

namespace SOPractice
{
    /// <summary>
    /// Блок «Альтернатива: централізований менеджер».
    /// Один скрипт обробляє кліки по всіх предметах через Raycast.
    /// Повторний клік по тому самому предмету або клік у порожнечу закриває панель.
    /// </summary>
    public class SelectionManager : MonoBehaviour
    {
        public ItemInfoPanel infoPanel;
        private SelectableItem lastSelected;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                HandleClick();
        }

        void HandleClick()
        {
            // В уроці цього немає: без перевірки клік по UI-кнопці «пробиває» UI,
            // Raycast влучає в порожнечу за кнопкою — і панель предмета закривається.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                SelectableItem item = hit.transform.GetComponent<SelectableItem>();

                if (item != null && item.itemData != null)
                {
                    // Повторний клік по тому самому — закрити панель
                    if (lastSelected == item)
                    {
                        infoPanel.HidePanel();
                        lastSelected = null;
                        return;
                    }

                    lastSelected = item;
                    infoPanel.ShowItem(item.itemData);
                }
                else
                {
                    // Клік по об'єкту без SO — закрити панель
                    infoPanel.HidePanel();
                    lastSelected = null;
                }
            }
            else
            {
                // Клік у порожньому місці — закрити панель
                infoPanel.HidePanel();
                lastSelected = null;
            }
        }

        void OnDisable() => lastSelected = null;
    }
}
