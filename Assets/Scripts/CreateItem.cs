using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public bool isGenerated;
    public int tNum;
    public int itemNumInViewPort;
    public DoublyLinkedList<RectTransform> _itemCircleList = new DoublyLinkedList<RectTransform>();

    private void OnEnable()
    {
    }
    public void GenerateItem()
    {
        isGenerated = true;

       _totalLength = (int)(_quantity * _itemPf.GetComponent<RectTransform>().rect.width); //원래 전체 오브젝트 생성시 길이
       
       var rectTrans = gameObject.GetComponent<RectTransform>();
       var pos = new Vector2(0, .5f);
       
       UIController.instance._scrollRect.content.pivot = pos;
       UIController.instance._scrollRect.content.sizeDelta = new Vector2(_totalLength, rectTrans.rect.height);
       // how many item should be present in view port?
       //1 total view port width / item width + spacing 
       // 1280 / 300 + 60 360
       //
       var viewport = UIController.instance._scrollRect.viewport;
       Vector3[] vCorners = new Vector3[4];
       viewport.GetWorldCorners(vCorners);
       Vector3[] iCorners = new Vector3[4];
       _itemPf.GetComponent<RectTransform>().GetWorldCorners(iCorners);
       itemNumInViewPort = (int)(viewport.rect.width /
                           (_itemPf.GetComponent<RectTransform>().rect.width + UIController.instance.spacing*2));

        // itemNumInViewPort += 2;

        for (int i = 0; i < _totalItemCnt * 2; i++)
        {
            var itemObj =Instantiate(_itemPf, _parent);
            //here need to be changed
            float xPos = (itemObj.GetComponent<RectTransform>().rect.width + UIController.instance.spacing) * i - (itemObj.GetComponent<RectTransform>().rect.width + UIController.instance.spacing) *2;
            itemObj.transform.localPosition = new Vector2(xPos, itemObj.transform.localPosition.y);
            itemObj.SetActive(true);
            itemObj.gameObject.name = $"{i}";
            _itemList.Add(itemObj.GetComponent<RectTransform>());
            _itemCircleList.Add(itemObj.GetComponent<RectTransform>());
            itemObj.GetComponentInChildren<TextMeshProUGUI>().text = (i-2).ToString();
            
        }
        //checking for properly  linked list done
        //int circleCount = _itemCircleList.Count();
        //for (int i = 0; i < circleCount; i++)
        //{
        //    var item = _itemCircleList.Next();
        //    Debug.Log( item.name, item);
        //}
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
