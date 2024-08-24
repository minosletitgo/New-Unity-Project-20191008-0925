//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//public class FindAtlasUseUtil : EditorWindow
//{
//    UIAtlas atlas;
//    Transform[] selectTrans;
//    List<UISprite> spriteList;
//    Vector2 scrollPosition;
//    bool selectedAtlas = false;
//    List<UIAtlas> atlasList;
//    string[] atlasPath = new string[] 
//    {
//        "Assets/Resource/UI/Atlas"
//        ,
//        "Assets/NGUI/Examples/Atlases/Fantasy"
//    };    
//    // TODO 指定项目里图集的父节点 需修改

//    static FindAtlasUseUtil window;

//    [MenuItem("Tools/查找NGUI图集引用")]
//    static void CreateWindow()
//    {
//        window = (FindAtlasUseUtil)EditorWindow.GetWindow(typeof(FindAtlasUseUtil), false, "查找图集引用");
//        window.Show();
//    }

//    private void OnEnable()
//    {
//        spriteList = new List<UISprite>();
//    }

//    private void OnGUI()
//    {
//        EditorGUILayout.Space();
//        EditorGUILayout.Space();
//        atlas = EditorGUILayout.ObjectField("需要查询的图集：", atlas, typeof(UIAtlas)) as UIAtlas;

//        if (GUILayout.Button("选择图集"))
//        {
//            ChooseAtlas();
//        }

//        EditorGUILayout.Space();
//        EditorGUILayout.Space();

//        GUILayout.Label("可多选“Hierarchy”、“Project”面板的预设体");
//        if (GUILayout.Button("查找引用"))
//        {
//            FindUse();
//        }

//        EditorGUILayout.Space();
//        EditorGUILayout.Space();

//        if (selectedAtlas)
//        {
//            ShowChooseAtlas();
//        }

//        if (spriteList.Count > 0 && atlas != null && !selectedAtlas)
//        {
//            ShowFindUse();
//        }
//    }

//    void ChooseAtlas()
//    {
//        selectedAtlas = true;
//        if (atlasList == null)
//        {
//            atlasList = new List<UIAtlas>();
//            // 第二个参数为图集的目录位置数组，可以指定项目里存放图集的父节点。切记不要在Assets节点下查找，这样会遍历所有物体，会很卡的~~
//            string[] guids = AssetDatabase.FindAssets("t:GameObject", atlasPath);
//            List<string> paths = new List<string>();
//            guids.ToList().ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
//            paths.ForEach(p => atlasList.Add(AssetDatabase.LoadAssetAtPath(p, typeof(UIAtlas)) as UIAtlas));
//            // 移除Null值
//            for (int i = 0; i < atlasList.Count; i++)
//            {
//                if (i < atlasList.Count && atlasList[i] == null)
//                {
//                    atlasList.Remove(atlasList[i]);
//                    i--;
//                }
//            }
//        }
//    }

//    void FindUse()
//    {
//        selectTrans = Selection.GetTransforms(SelectionMode.TopLevel);
//        spriteList = new List<UISprite>();
//        for (int i = 0; i < selectTrans.Length; i++)
//        {
//            UISprite[] sprites = selectTrans[i].GetComponentsInChildren<UISprite>(true);
//            for (int j = 0; j < sprites.Length; j++)
//            {
//                if (sprites[j] != null && sprites[j].atlas == atlas)
//                {
//                    spriteList.Add(sprites[j]);
//                }
//            }
//        }
//    }

//    void ShowChooseAtlas()
//    {
//        scrollPosition = GUI.BeginScrollView(new Rect(0, 150, Screen.width, 500), scrollPosition, new Rect(0, 150, Screen.width, atlasList.Count * 20));
//        for (int i = 0; i < atlasList.Count; i++)
//        {
//            atlasList[i] = EditorGUI.ObjectField(new Rect(50, 150 + i * 20, Screen.width, 20), name, atlasList[i], typeof(UIAtlas)) as UIAtlas;
//            if (GUI.Button(new Rect(5, 150 + i * 20, 40, 20), "选择"))
//            {
//                atlas = atlasList[i];
//                selectedAtlas = false;
//            }
//        }
//        GUI.EndScrollView();
//    }

//    void ShowFindUse()
//    {
//        scrollPosition = GUI.BeginScrollView(new Rect(0, 150, Screen.width, 500), scrollPosition, new Rect(0, 150, Screen.width, spriteList.Count * 20));
//        for (int i = 0; i < spriteList.Count; i++)
//        {
//            string name = spriteList[i].atlas != null ? spriteList[i].atlas.name : "空图集";
//            name += " " + spriteList[i].spriteName;
//            spriteList[i] = EditorGUI.ObjectField(new Rect(0, 150 + i * 20, Screen.width, 20), name, spriteList[i], typeof(UISprite)) as UISprite;

//        }
//        GUI.EndScrollView();
//    }
//}