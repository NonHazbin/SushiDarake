using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField, Header("升目のオブジェクト")]
    private GameObject[] _squares;
    private GameObject[,] _squareArray = new GameObject[4, 4]; //升目を二次元配列として認識

    public GameObject _tilePrefab; //生成するタイル

    private Vector2 touchStartPos; //スワイプ
    private float swipeThershold = 80f; // スワイプを検出するための閾値

    private bool canPlay = true;
    private bool tapped = false;

    private void Start()
    {
        SetSquareArray();
        SpawnTile();
        SpawnTile();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canPlay)
        {
            touchStartPos = Input.mousePosition;
            tapped = true;
        }
        else if (Input.GetMouseButton(0) && canPlay && tapped)
        {
            
        }
    }

    // 4*4のマス生成
    private void SetSquareArray()
    {
        int count = 0;
        const int FOUR = 4;
        for (int i = 0; i < FOUR; i++)
        {
            for (int j = 0; j < FOUR; j++)
            {
                _squareArray[i, j] = _squares[count];
                //_squareArray[i,j].GetComponent<Square>().tileValue = 0;
                count++;
            }
        }
    }

    //タイル生成
    private void SpawnTile()
    {
        GameObject emptyCell = FindEmptySquare(); //空っぽのマスの座標

        GameObject newTile = Instantiate(_tilePrefab, emptyCell.transform); //Squareの位置に生成
        newTile.transform.SetParent(emptyCell.transform, false); //Squareの子オブジェクトにする
        RectTransform newTileRect = newTile.GetComponent<RectTransform>(); //サイズ、位置調整

        newTileRect.sizeDelta = _tilePrefab.GetComponent<RectTransform>().sizeDelta; //新しいタイルのサイズを設定
        newTileRect.position = emptyCell.GetComponent<RectTransform>().position;

        int newValue = Random.Range(1, 3) * 2;

        newTileRect.DOScale(Vector3.one, 0.1f).SetLink(newTile);

        emptyCell.GetComponent<Square>().tileValue = newTile.GetComponent<Tile>()._tileValue;
        emptyCell.GetComponent<Square>().tileOnMe = newTile;
    }

    private GameObject FindEmptySquare()
    {
        List<GameObject> emptySquares = new List<GameObject>();
        foreach (var item in _squareArray)
        {
            if (item.GetComponent<Square>().tileValue == 0)
            {
                emptySquares.Add(item);
            }
        }
        if (emptySquares.Count > 0)
        {
            int randomIndex = Random.Range(0, emptySquares.Count);
            GameObject selectedSquare = emptySquares[randomIndex];

            return selectedSquare;
        }
        return null;
    }
}
