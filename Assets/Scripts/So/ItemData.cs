using UnityEngine;

public enum ItemType { Weapon, Armor, Potion, Key }

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemName => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public ItemType ItemType => itemType;

    public int Value => value;
    public bool IsStackable => isStackable;
    public AudioClip PickupSound => pickupSound;

    [Header("Основна інформація")]
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType;

    [Header("Характеристики")]
    [SerializeField] private int value;           // ціна або потужність ефекту
    [SerializeField] private bool isStackable;    // чи можна складати в стопку

    [Header("Звук")]
    [SerializeField] private AudioClip pickupSound;
}
