using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private ItemInfoPanel _infoPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        _infoPanel.ShowItem(_itemData);

    }
}
