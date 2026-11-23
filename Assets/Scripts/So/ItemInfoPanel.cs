using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    [Header("UI Елементи")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemValueText;
    public Image itemIcon;

    void Start()
    {
        // На старті панель прихована
        gameObject.SetActive(false);
    }

    public void ShowItem(ItemData data)
    {

        gameObject.SetActive(true);

        itemNameText.text = data.ItemName;
        itemTypeText.text = "Тип: " + data.ItemType.ToString();
        itemDescriptionText.text = data.Description;
        itemValueText.text = "Цінність: " + data.Value;

        if (data.Icon != null)
            itemIcon.sprite = data.Icon;
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
