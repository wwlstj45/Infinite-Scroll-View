using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptsWIthHugeList))]
public class ScriptsWithHugeListEditor : Editor
{
    public Vector2 scrolPositon = Vector2.zero;
    private List<int> bigList = Enumerable.Range(0, 10000).ToList();
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.BeginHorizontal(GUILayout.Height(254f));

        Rect rectPos = EditorGUILayout.GetControlRect();
        Rect rectBox = new Rect(rectPos.x, rectPos.y, rectPos.width, 250f);
        
        EditorGUI.DrawRect(rectBox,Color.black);
        
        GUI.Box(rectBox,GUIContent.none);
        Rect viewRect = new Rect(rectBox.x, rectBox.y, rectBox.width, bigList.Count * 18);
        scrolPositon=GUI.BeginScrollView(rectBox, scrolPositon, viewRect,false,true,GUIStyle.none, GUI.skin.verticalScrollbar);
        
        int viewCount = 15; // 250/18 ==> total viewport height divided by individual slot height
        int fistIndex = (int)scrolPositon.y /18; //current scroll view pos / content width 
        
        Rect contentPos = new Rect(rectBox.x,  fistIndex * 18f,rectBox.width, 18f);

        for (int i = fistIndex; i < Mathf.Min(bigList.Count, fistIndex + viewCount); i++)
        {
            GUI.Label(contentPos, bigList[i].ToString());
            contentPos.y += 18;
        }
        
        
        
        EditorGUI.LabelField(contentPos, "Hello world");
   
        
        
        GUI.EndScrollView();
        EditorGUILayout.EndHorizontal();
        
        
        serializedObject.ApplyModifiedProperties();
        // base.OnInspectorGUI();
    }
}
