using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateItem : MonoBehaviour
{
    [SerializeField]public GameObject _itemPf;
    [SerializeField]private Transform _parent;

    public int _totalItemCnt;
    public int _quantity;
    public List<RectTransform> _itemList = new List<RectTransform>();
    public int _totalLength;

    private void OnEnable()
    {
    }
    public void GenerateItem()
    {
        Debug.Log($"<color=red>{_quantity}</color>");
       //maybe I need to change ScrollRect 's width hmmm...

       _totalLength = (int)(_quantity * _itemPf.GetComponent<RectTransform>().rect.width); //원래 전체 오브젝트 생성시 길이
       //first index....
       var rectTrans = gameObject.GetComponent<RectTransform>();
       var pos = new Vector2(0, .5f);
       // UIController.instance._scrollRect.content.anchorMax = pos;
       // UIController.instance._scrollRect.content.anchorMin = pos;
       UIController.instance._scrollRect.content.pivot = pos;
       UIController.instance._scrollRect.content.sizeDelta = new Vector2(_totalLength, rectTrans.rect.height);
        int firstIndex = (int)(UIController.instance.currentScrollRectHorizontalPosition / _itemPf.GetComponent<RectTransform>().rect.width);
        for (int i = 0; i < _totalItemCnt * 2; i++)
        {
            var itemObj =Instantiate(_itemPf, _parent);
            itemObj.transform.localPosition = new Vector2((itemObj.GetComponent<RectTransform>().rect.width + 30f) * i, itemObj.transform.localPosition.y);
            itemObj.SetActive(true);
            _itemList.Add(itemObj.GetComponent<RectTransform>());
        }
        UIController.instance.leftMostIndex = 0;
        UIController.instance.rightMostIndex = _itemList.Count -1;

        _itemPf.SetActive(false);
    }

    void OnUpDateData(float currentHorizontalRect)
    {
        int firstIndex = (int)(currentHorizontalRect / _itemPf.GetComponent<RectTransform>().rect.width);
        for (int i = firstIndex; i < _quantity; i++)
        {
            
        }
    }
    

}
