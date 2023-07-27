using System;
using System.Collections;
using System.Collections.Generic;
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
      _QuantityInputField.onValueChanged.AddListener(delegate(string arg0) { SetQuantity(); });

      Debug.Log(_createItem._itemPf.GetComponent<RectTransform>().rect.width);
      var _itemRectWith =_createItem._itemPf.GetComponent<RectTransform>().rect.width + 10f;
     
     _createItem._totalItemCnt =  (int)(( _scrollRect.viewport.rect.width)/_itemRectWith);  
    // _scrollRect.onValueChanged.AddListener(delegate(Vector2 arg0) { ChangeDataAndScrollBarPosition();});
    _scrollRect.onValueChanged.AddListener(ChangeCellPosition);
   }



   void ChangeDataAndScrollBarPosition()
   {
      // currentScrollRectHorizontalPosition =
      //    _scrollRect.horizontalNormalizedPosition * _scrollRect.content.rect.width;
      // int firstIndex = (int)(currentScrollRectHorizontalPosition /
      //                  _createItem._itemPf.GetComponent<RectTransform>().rect.width);
      
      // Debug.Log(_scrollRect.horizontalScrollbar.value);
      var dir = _scrollRect.horizontalScrollbar.direction;
      if (dir == Scrollbar.Direction.LeftToRight)
         scrollPos += _scrollRect.horizontalScrollbar.value;
      else
      {
         scrollPos -= _scrollRect.horizontalScrollbar.value;
      }
      if (dir == Scrollbar.Direction.LeftToRight &&Mathf.Abs(_scrollRect.horizontalScrollbar.value) >= .9f)
      {
         scrollPos += _scrollRect.horizontalScrollbar.value;
         var firstIndex = (int)Mathf.Abs((scrollPos /_createItem._itemList.Count));
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
      var movedDistance = prevPos.x-Mathf.Abs(_scrollRect.content.anchoredPosition.x);
      
      if (Mathf.Abs(movedDistance) >= constantMoveDistance)
      {
         prevPos = new Vector2(Mathf.Abs(_scrollRect.content.anchoredPosition.x), 0f);

         var rightPos = _createItem._itemList[rightMostIndex].anchoredPosition.x +
                        _createItem._itemList[rightMostIndex].rect.width +30f;
         
        _createItem._itemList[leftMostIndex].anchoredPosition =
           new Vector2(rightPos, _createItem._itemList[leftMostIndex].anchoredPosition.y);
        
        rightMostIndex = leftMostIndex;
        leftMostIndex = leftMostIndex + 1 < _createItem._itemList.Count ? leftMostIndex + 1 : 0;
      
      }
   }

   void ChangeIndecies()
   {
      rightMostIndex = leftMostIndex;
      
   }
   void GoToRight()
   {
      Debug.Log("GoToRight");
   }
   private void Update()
   {
     
   }

   private void OnDisable()
   {
      Application.targetFrameRate = 60;
   }

   void SetQuantity()
   {
      var isValidInput = CheckValidInput();
      if(!isValidInput) return;
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
