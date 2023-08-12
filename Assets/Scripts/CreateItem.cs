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
       //UIController.instance._scrollRect.content.sizeDelta = new Vector2(_totalLength, rectTrans.rect.height)
        // how many item should be present in view port?
        //1 total view port width / item width + spacing 
        // 1280 / 300 + 60 360
        //

        var viewport = UIController.instance._scrollRect.viewport;
        #region Why Is these are needed?
        Vector3[] vCorners = new Vector3[4];
        viewport.GetWorldCorners(vCorners);
        Vector3[] iCorners = new Vector3[4];
        _itemPf.GetComponent<RectTransform>().GetWorldCorners(iCorners);
        #endregion
        switch (UIController.instance._direction)
        {
            case UIController.Direction.Horizontal:
                HorizontalGenerator(viewport);
                break;
            case UIController.Direction.Vertical:
                VerticalGenerator(viewport);
                break;
        }
        //if Vertical
        _itemPf.SetActive(false);
    }
    void HorizontalGenerator(RectTransform viewport)
    {
        itemNumInViewPort = (int)(viewport.rect.width /
                      (_itemPf.GetComponent<RectTransform>().rect.width + UIController.instance.spacing * 2));

        for (int i = 0; i < _totalItemCnt * 2; i++)
        {
            var itemObj = Instantiate(_itemPf, _parent);
            //here need to be changed
            float xPos = (itemObj.GetComponent<RectTransform>().rect.width + UIController.instance.spacing) * i - (itemObj.GetComponent<RectTransform>().rect.width + UIController.instance.spacing) * 2;
            itemObj.transform.localPosition = new Vector2(xPos, itemObj.transform.localPosition.y);
            itemObj.SetActive(true);
            itemObj.gameObject.name = $"{i}";
            _itemList.Add(itemObj.GetComponent<RectTransform>());
            _itemCircleList.Add(itemObj.GetComponent<RectTransform>());
            itemObj.GetComponentInChildren<TextMeshProUGUI>().text = (i - 2).ToString();

        }
        UIController.instance.leftMostIndex = 0;
        UIController.instance.rightMostIndex = _itemList.Count - 1;
    }
    void VerticalGenerator(RectTransform viewport)
    {
        //Debug.Log($"<color=red>Is Even Comin here?</color>");
        itemNumInViewPort = (int)(viewport.rect.height /
                      (_itemPf.GetComponent<RectTransform>().rect.height + UIController.instance.spacing * 2));

        Vector3[] viewPortCorners = new Vector3[4];
        var content = viewport.GetChild(0).GetComponent<RectTransform>();
        viewport.GetWorldCorners(viewPortCorners);
        var startYPos = viewport.localPosition.y;
        for (int i = 0; i < _totalItemCnt * 2; i++)
        {
            var itemObj = Instantiate(_itemPf, _parent);
            itemObj.GetComponent<RectTransform>().pivot = new Vector2(.5f, 1);

            float yPos = startYPos - (itemObj.GetComponent<RectTransform>().rect.height + UIController.instance.spacing) * i;
            itemObj.transform.localPosition = new Vector2(itemObj.transform.localPosition.x, yPos);
            itemObj.SetActive(true);
            itemObj.gameObject.name = $"{i}";
            _itemList.Add(itemObj.GetComponent<RectTransform>());
            _itemCircleList.Add(itemObj.GetComponent<RectTransform>());
            itemObj.GetComponentInChildren<TextMeshProUGUI>().text = (i - 2).ToString();
        }
    }
}
