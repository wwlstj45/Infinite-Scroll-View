using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    [SerializeField]
    private Button _generateBtn;

    [SerializeField] private CreateItem _createItem;

    public TMP_InputField _QuantityInputField;
    public ScrollRect _scrollRect;

    [FormerlySerializedAs("contentCount")] [FormerlySerializedAs("viewContent")] public int itemCnt;
    public float currentScrollRectHorizontalPosition;
    private float scrollPos;

    public int rightMostIndex, leftMostIndex;
    private int nextIndex;
    private int callCount;
    private Vector2 prevPos = Vector2.zero;
    private Vector3[] fourConers = new Vector3[4];

    private Vector2 viewPortMin;
    private Vector2 viewPortMax;
    public Vector3 rightPos;
    public float spacing= 30f;
    int testInt=0;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        Application.targetFrameRate = 35;
        _generateBtn.onClick.AddListener(Onbutton_GenerateClicked);
        _QuantityInputField.onValueChanged.AddListener(delegate (string arg0) { SetQuantity(); });

        
        var _itemRectWith = _createItem._itemPf.GetComponent<RectTransform>().rect.width + 10f;
        _createItem._totalItemCnt = Mathf.CeilToInt(_scrollRect.viewport.rect.width / _itemRectWith);

        //move Scrollbar version
        // _scrollRect.onValueChanged.AddListener(delegate(Vector2 arg0) { ChangeDataAndScrollBarPosition();});
        
        //Move OBJ version
        _scrollRect.onValueChanged.AddListener(ChangeCellPosition);
        SetRecycleBounds();

    }

    private void SetRecycleBounds()
    {
        _scrollRect.viewport.GetWorldCorners(fourConers);
        var threshold = 2f * (fourConers[0].x + fourConers[2].x);
        viewPortMax = new Vector2(fourConers[2].x + threshold, fourConers[2].y);
        viewPortMin = new Vector2(fourConers[0].x - threshold, fourConers[0].y);
    }


    void ChangeDataAndScrollBarPosition()
    {
        var dir = _scrollRect.horizontalScrollbar.direction;
        if (dir == Scrollbar.Direction.LeftToRight)
            scrollPos += _scrollRect.horizontalScrollbar.value;
        else
        {
            scrollPos -= _scrollRect.horizontalScrollbar.value;
        }
        if (dir == Scrollbar.Direction.LeftToRight && Mathf.Abs(_scrollRect.horizontalScrollbar.value) >= .9f)
        {
            scrollPos += _scrollRect.horizontalScrollbar.value;
            var firstIndex = (int)Mathf.Abs((scrollPos / _createItem._itemList.Count));
            Debug.Log("This is the end of the scroll can get ");
            // go back to the start position
            _scrollRect.horizontalScrollbar.value = 0.2f;
            //calculate firstIndex's number
            Debug.Log($"<color=red>{firstIndex}</color>");
            Debug.Log(scrollPos);

            for (int i = 0; i < _createItem._itemList.Count; i++)
            {
                Debug.Log("HO");
                _createItem._itemList[i].GetComponentInChildren<TextMeshProUGUI>().text = (firstIndex + i).ToString();
            }
            scrollPos += _createItem._itemList.Count;
        }
        else if (dir == Scrollbar.Direction.RightToLeft && _scrollRect.horizontalScrollbar.value <= .1f)
        {
            scrollPos -= _scrollRect.horizontalScrollbar.value;
            var firstIndex = (int)Mathf.Abs(scrollPos / _createItem._itemList.Count);
            _scrollRect.horizontalScrollbar.value = 0.85f;
            for (int i = firstIndex + _createItem._itemList.Count; i <= firstIndex; i--)
            {
                _createItem._itemList[i].GetComponentInChildren<TextMeshProUGUI>().text = (firstIndex + i).ToString();
            }

            scrollPos -= _createItem._itemList.Count;
        }
        else
        {
            Debug.Log(Mathf.Abs(_scrollRect.horizontalScrollbar.value));
        }
    }

    void ChangeCellPosition(Vector2 normalizedPos)
    {

        var constantMoveDistance = _createItem._itemList[leftMostIndex].rect.width;
        var movedDistance = prevPos.x - Mathf.Abs(_scrollRect.content.anchoredPosition.x);

        var viewPortCorners = new Vector3[4];
        _scrollRect.viewport.GetWorldCorners(viewPortCorners);

        Vector3[] minCorners = new Vector3[4];

        var flagNum = leftMostIndex + _createItem.itemNumInViewPort;
        if (flagNum >= _createItem._itemList.Count)
        {
            flagNum = 0 + (flagNum - _createItem._itemList.Count);
        }
        _createItem._itemList[flagNum].GetWorldCorners(minCorners);

        //if(leftMostIndex +2 > viewportMin) change Dir 
        Vector3[] maxCorners = new Vector3[4];
        var leftFlagIdx = leftMostIndex + 2;
        if(leftFlagIdx >= _createItem._itemList.Count)
        {
            leftFlagIdx = 0 + (leftFlagIdx - _createItem._itemList.Count);
        }
        _createItem._itemList[leftFlagIdx].GetWorldCorners(maxCorners);


        //certain amount of item should be on the screen. 
        // right
        // total length - viewportitems == when to change position
        var flagIndex = (_createItem._itemList.Count - 1) - _createItem.itemNumInViewPort;
        Vector3[] test = new Vector3[4];
        _createItem._itemList[flagIndex].GetWorldCorners(test);


        //left
        //tlqkf 
        //0 1 2  | 3 4 5 6 7 8 9
        //3 4 5  | 6 7 8 9 0 1 2
        //6 7 8  | 9 0 1 2 3 4 5
        //9 0 1  | 2 3 4 5 6 7 8
        //2 3 4  | 5 6 7 8 9 0 1
        //5 6 7  | 8 9 0 1 2 3 4
        //8 9 0  | 1 2 3 4 5 6 7
        //1 2 3  | 4 5 6 7 8 9 0
        //4 5 6  | 7 8 9 0 1 2 3
        //7 8 9  | 0 1 2 3 4 5 6
        //0 1 2  | 3 4 5 6 7 8 9

        // 왜 위치가 이상해 지지?
        //if (minCorners[0].x < viewPortMin.x)
        //{
        //    int tNum = 0;
        //    int addedNum = 0;
        //    var currnetMaxNum = int.Parse(_createItem._itemList[rightMostIndex].GetComponentInChildren<TextMeshProUGUI>().text);
        //    for (int i = leftMostIndex; i < leftMostIndex + _createItem.itemNumInViewPort; i++)
        //    {
        //        ++tNum;
        //        ++addedNum;
        //        var targetXPos = (_createItem._itemList[rightMostIndex].anchoredPosition.x
        //                         + (_createItem._itemList[rightMostIndex].sizeDelta.x + 30f) * tNum);

        //        Vector3 targetPos = new Vector3(targetXPos, _createItem._itemList[rightMostIndex].anchoredPosition.y);

        //        if (i >= _createItem._itemList.Count)
        //        {
        //            var num = 0 + (i - _createItem._itemList.Count);
        //            _createItem._itemList[num].anchoredPosition = targetPos;
        //            _createItem._itemList[num].GetComponentInChildren<TextMeshProUGUI>().text = (currnetMaxNum + addedNum).ToString();

        //        }
        //        else
        //        {
        //            _createItem._itemList[i].anchoredPosition = targetPos;
        //            _createItem._itemList[i].GetComponentInChildren<TextMeshProUGUI>().text = (currnetMaxNum + addedNum).ToString();
        //        }


        //    }
        //    // update numbers

        //    SetMinAndMax();

        //}

        if (maxCorners[2].x >viewPortMin.x)
        {
            ++testInt;
            Debug.Log(testInt);
            Debug.Log(maxCorners[2].x);
            //current Error happens due to currenly generate position.

            //Set target Pos

            //get right most index obj and it's nearby three objs
            for (int i = 0; i <_createItem.itemNumInViewPort; i++)
            {
                var changeObjIdx = rightMostIndex - i;
                if (changeObjIdx < 0)
                    changeObjIdx = _createItem.itemNumInViewPort - changeObjIdx;
                float targetXPos = _createItem._itemList[leftMostIndex].anchoredPosition.x - (_createItem._itemList[leftMostIndex].rect.width + spacing) * (i+1);
                Vector2 targetPos = new Vector2(targetXPos, _createItem._itemList[leftMostIndex].anchoredPosition.y);

                _createItem._itemList[changeObjIdx].anchoredPosition = targetPos;
            }
            SetMinAndMax();
        }
    }
    void SetMinAndMax()
    {
        var min = _createItem._itemList.OrderBy(x => x.anchoredPosition.x).First();
        var max = _createItem._itemList.OrderByDescending(x => x.anchoredPosition.x).First();
        for (int i = 0; i < _createItem._itemList.Count; i++)
        {
            if (_createItem._itemList[i] == min)
            {
                leftMostIndex = i;
            }
            else if (_createItem._itemList[i] == max)
            {
                rightMostIndex = i;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (!_createItem.isGenerated) return;
        DrawViewPortBoundary();
        DrawLeftMostObj();
        // DrawNextTeleportPos();
        DrawRightMostPos();


    }

    void DrawRightMostPos()
    {
        Vector3[] wordPos = new Vector3[4];
        _createItem._itemList[rightMostIndex].GetWorldCorners(wordPos);
        var dir = wordPos[1] - wordPos[0];
        Gizmos.color = Color.green;
        Gizmos.DrawLine(wordPos[0], dir * 100f);
    }

    void DrawNextTeleportPos()
    {
        if (rightPos != null)
        {
            Gizmos.color = Color.blue;
            Vector3[] wordPos = new Vector3[4];
            _createItem._itemList[leftMostIndex].GetWorldCorners(wordPos);
            var drawFrom = new Vector3(wordPos[0].x + _createItem._itemList[rightMostIndex].sizeDelta.x, wordPos[0].y,
               wordPos[0].z);
            var drawTo = new Vector3(wordPos[0].x + Mathf.Abs(_createItem._itemList[rightMostIndex].sizeDelta.x * 100f),
               wordPos[0].y,
               wordPos[0].z);
            var dir = drawTo - drawFrom;
            Gizmos.DrawLine(drawFrom, dir);
            // var targetdir = new Vector3(rightPos.x, Mathf.Abs(rightPos.y * 100f), rightPos.z) - rightPos;
            // Gizmos.DrawLine(rightPos, targetdir);
        }
    }
    void DrawLeftMostObj()
    {
        Vector3[] wordPos = new Vector3[4];
        _createItem._itemList[leftMostIndex].GetWorldCorners(wordPos);
        var dir = wordPos[1] - wordPos[0];
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wordPos[0], dir * 100f);
    }

    void DrawViewPortBoundary()
    {
        Vector3[] viewPortCorner = new Vector3[4];
        _scrollRect.viewport.GetWorldCorners(viewPortCorner);
        var upperdir = viewPortCorner[1] - viewPortCorner[0];

        Gizmos.DrawRay(viewPortCorner[0], upperdir * 100f);
    }

    void ChangeIndecies()
    {
        rightMostIndex = leftMostIndex;

    }


    private void OnDisable()
    {
        Application.targetFrameRate = 60;
    }

    void SetQuantity()
    {
        var isValidInput = CheckValidInput();
        if (!isValidInput) return;
        var quant = int.Parse(_QuantityInputField.text);
        _createItem._quantity = quant;
    }

    bool CheckValidInput()
    {
        var isValidInput = _QuantityInputField.text == "" | _QuantityInputField.text == null;
        var texts = _QuantityInputField.text.ToCharArray();
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] < 48 | texts[i] > 57)
            {
                isValidInput = false;
            }
            else
            {
                isValidInput = true;
            }
        }

        return isValidInput;
    }
    void Onbutton_GenerateClicked()
    {
        SetQuantity();
        _createItem.GenerateItem();
    }
}
