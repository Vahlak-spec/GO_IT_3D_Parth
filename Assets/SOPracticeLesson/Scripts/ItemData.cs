using UnityEngine;

namespace SOPractice
{
    public enum ItemType { Weapon, Armor, Potion, Key }

    /// <summary>
    /// Блок «ItemData — SO для предметів». Дані предмета: назва, опис, іконка, тип, цінність.
    /// Меню змінене на "GameData (Lesson 64)/...", бо в проєкті вже є свій глобальний ItemData.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "GameData (Lesson 64)/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("Основна інформація")]
        public string itemName;
        [TextArea] public string description;
        public Sprite icon;
        public ItemType itemType;

        [Header("Характеристики")]
        public int value;           // ціна або потужність ефекту
        public bool isStackable;    // чи можна складати в стопку

        [Header("Звук")]
        public AudioClip pickupSound;
    }
}
