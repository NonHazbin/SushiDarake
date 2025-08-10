using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField, Header("升目のオブジェクト")]
    private GameObject[] _squares;
    private GameObject[,] _squareArray = new GameObject[4, 4]; //升目を二次元配列として認識

    public GameObject _tilePrefab; //生成するタイル

    private Vector2 _touchStartPos; //スワイプ
    private float _swipeThershold = 80f; // スワイプを検出するための閾値

    private bool _canPlay = true;
    private bool _tapped = false;

    private void Start()
    {
        SetSquareArray();
        SpawnTile();
        SpawnTile();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _canPlay)
        {
            _touchStartPos = Input.mousePosition;
            _tapped = true;
        }
        else if (Input.GetMouseButton(0) && _canPlay && _tapped)
        {
            Vector2 touchEndPos = Input.mousePosition;

            float swipeX = touchEndPos.x - _touchStartPos.x;
            float swipeY = touchEndPos.y - _touchStartPos.y;

            if (Mathf.Abs(swipeX) > Mathf.Abs(swipeY))
            {
                if (swipeX > _swipeThershold)
                {
                    _tapped = false;
                    RightSwipe();
                }
            }
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
                _squareArray[i,j].GetComponent<Square>().tileValue = 0;
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

        newTile.GetComponent<Tile>().SetValue(newValue);

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

    private void RightSwipe()
    {
        bool canMove = false;

        for (int row = 0; row < 4; row++)
        {
            for (int column = 3; column >= 0; column--)
            {
                Debug.Log(_squareArray[row, column].GetComponent<Square>().tileValue);
                int myNumber;
                if (_squareArray[row, column].GetComponent<Square>().tileValue != 0)
                {
                    GameObject temp = _squareArray[row, column].GetComponent<Square>().tileOnMe;
                    myNumber = _squareArray[row, column].GetComponent<Square>().tileValue;

                    int x = column;
                    while (x < 3 && _squareArray[row, x + 1].GetComponent<Square>().tileValue == 0)
                    {
                        canMove = true;
                        x++;
                    }

                    if (!canMove)
                    {
                        continue;
                    }
                    GameObject targetSquare = _squareArray[row, x];
                    targetSquare.GetComponent<Square>().tileValue = myNumber;

                    EmptyMySquare(_squareArray[row, column]);

                    TileMoveAnimation(temp, targetSquare);

                    targetSquare.GetComponent<Square>().tileOnMe = temp;

                }
            }
        }
        if (canMove)
        {
            _canPlay = false;
            Invoke("SpawnTile", 0.2f);
        }
    }

    // 元いたマスを空っぽにする
    private void EmptyMySquare(GameObject mySquare_)
    {
        mySquare_.GetComponent<Square>().tileValue = 0;
        mySquare_.GetComponent<Square>().tileOnMe = null;
    }
    // タイルの移動
    private void TileMoveAnimation(GameObject temp_, GameObject targetSquare_)
    {
        RectTransform tempRect = temp_.GetComponent<RectTransform>();
        tempRect.SetParent(targetSquare_.transform);

        Vector2 endPos = targetSquare_.GetComponent<RectTransform>().position;
        tempRect.DOAnchorPos(endPos, 0.1f).SetEase(Ease.Linear).SetLink(temp_);
    }
}
