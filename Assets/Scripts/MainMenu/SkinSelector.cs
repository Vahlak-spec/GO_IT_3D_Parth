using TMPro;
using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    [SerializeField] private int _skinArrayLenth;
    [SerializeField] private TextMeshProUGUI _indexText;

    private int _currentSkinIndex;

    private void Start()
    {
        _currentSkinIndex = PlayerPrefs.GetInt("SkinIndex");
        _indexText.text = (_currentSkinIndex + 1).ToString();
    }

    public void NextSkin()
    {
        _currentSkinIndex++;
        if(_currentSkinIndex >= _skinArrayLenth)
            _currentSkinIndex = 0;

        UpdateSkinData();
    }
    public void PrevSkin() 
    {
        _currentSkinIndex--;
        if(_currentSkinIndex < 0)
            _currentSkinIndex = _skinArrayLenth - 1;

        UpdateSkinData();
    }

    private void UpdateSkinData()
    {
        _indexText.text = (_currentSkinIndex + 1).ToString();
        PlayerPrefs.SetInt("SkinIndex", _currentSkinIndex);
        PlayerPrefs.Save();
    }
}
