using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.WSA;

[System.Serializable]
public class TileData
{
    public Color color;
    public int value;
}
public class Tile : MonoBehaviour
{
    [Header("色と数字データ")]
    public TileData[] _tileData;
    [Header("タイルの色")]
    public Image _imageColor;
    [Header("タイル状の数字")]
    public int _tileValue;
    [Header("数字のテキスト")]
    public TMP_Text _valueText;

    // タイルの値を設定
    public void SetValue(int newValue)
    {
        TileData data = GetTileData(newValue);

        if (data != null)
        {
            _imageColor.color = data.color;
            _tileValue = data.value;
            _valueText.text = _tileValue.ToString();
        }
    }

    // タイルのデータ取得
    public TileData GetTileData(int targetValue_)
    {
        for (int i = 0; i < _tileData.Length; i++)
        {
            if (_tileData[i].value == targetValue_)
            {
                return _tileData[i];
            }
        }
        return null;
    }
}
