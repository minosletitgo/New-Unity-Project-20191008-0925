using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FindAtlasSpriteReference : EditorWindow
{
    static FindAtlasSpriteReference m_Window;
    UIAtlas m_AtlasOnlyOne;
    string m_strSpriteNameOnlyOne;
    List<UISprite> m_lstSprResult = new List<UISprite>();
    List<string> m_lstPrefabAllPath = new List<string>();
    Vector2 m_v2ScrollPosition;

    Transform[] m_arySelectingTrans;
    List<Transform> m_lstResultSelectingTrans = new List<Transform>();



    public static void CreateWindow(UIAtlas at, string strSpriteName)
    {
        m_Window = (FindAtlasSpriteReference)EditorWindow.GetWindow(typeof(FindAtlasSpriteReference), false, "查找NGUI图集的引用");
        m_Window.Show(at, strSpriteName);
    }

    void Show(UIAtlas at, string strSpriteName)
    {
        Clear(true);
        m_AtlasOnlyOne = at;
        m_strSpriteNameOnlyOne = strSpriteName;
        m_Window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        m_AtlasOnlyOne = EditorGUILayout.ObjectField(
            "指向查询的源图集：",
            m_AtlasOnlyOne,
            typeof(UIAtlas),
            true
            ) as UIAtlas;

        EditorGUILayout.PrefixLabel("指向查询的源图集的目标纹理:");
        EditorGUI.TextField(
            new Rect(153, 32, 100, 15),
            m_strSpriteNameOnlyOne
            );

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("请从下面两种方式选择其中一种!", MessageType.Warning, true);

        if (GUILayout.Button("方式一:在鼠标焦点选中(可多选)的Prefab中，查找引用"))
        {
            Clear(false);
            DoFindInPrefabs();
        }

        if (GUILayout.Button("方式二:在手动指向目标文件夹目录中，查找引用"))
        {
            Clear(false);
            DoFindInFolder();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (m_lstSprResult.Count > 0 && m_AtlasOnlyOne != null)
        {
            ShowFindResult();
        }
    }


    void DoFindInPrefabs()
    {
        if (m_AtlasOnlyOne == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("友情提示", "请先填充[源图集]!!", "确定");
            return;
        }

        if (m_arySelectingTrans != null)
        {
            Array.Clear(m_arySelectingTrans, 0, m_arySelectingTrans.Length);
        }
        m_arySelectingTrans = Selection.GetTransforms(SelectionMode.TopLevel);

        DoCollectSprite();
    }

    void DoFindInFolder()
    {
        if (m_AtlasOnlyOne == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("友情提示", "请先填充[源图集]!!", "确定");
            return;
        }

        string strFolderPath = EditorUtility.OpenFolderPanel("选择在哪个路径下查找Prefab", "Assets/Resources/UI", "");
        if (Directory.Exists(strFolderPath))
        {
            List<Transform> lstSelectingTrans = new List<Transform>();
            DirectoryInfo dir = new DirectoryInfo(strFolderPath);
            FileInfo[] aryFiles = dir.GetFiles("*.prefab", SearchOption.AllDirectories);
            int nIndexFile = 0;
            foreach (FileInfo _info in aryFiles)
            {
                //Debug.Log("" + _info.Name);
                int _indexAssets = _info.DirectoryName.IndexOf("Assets\\");
                string _strAssetPath = _info.DirectoryName.Substring(_indexAssets);
                //Debug.Log("-> " + _strAssetPath);
                string _strAssetFile = _strAssetPath + "\\" + _info.Name;
                //Debug.Log("-> " + _strAssetFile);

                EditorUtility.DisplayProgressBar(
                    "Finding~~~",
                    _strAssetFile,
                    (float)(nIndexFile + 1) / (float)aryFiles.Length
                    );

                UnityEngine.Object go = AssetDatabase.LoadAssetAtPath(_strAssetFile, typeof(GameObject));
                if (go != null)
                {
                    GameObject go02 = (GameObject)go;
                    lstSelectingTrans.Add(go02.transform);
                }

                nIndexFile++;
            }

            EditorUtility.ClearProgressBar();

            if (m_arySelectingTrans != null)
            {
                Array.Clear(m_arySelectingTrans, 0, m_arySelectingTrans.Length);
            }
            m_arySelectingTrans = lstSelectingTrans.ToArray();

            DoCollectSprite();
        }
    }

    void DoCollectSprite()
    {
        m_lstResultSelectingTrans.Clear();
        for (int iTrans = 0; iTrans < m_arySelectingTrans.Length; iTrans++)
        {
            Transform trSelecing = m_arySelectingTrans[iTrans];
            UISprite[] arySprRet = trSelecing.GetComponentsInChildren<UISprite>(true);

            bool bIsSucc = false;
            foreach (UISprite _spr in arySprRet)
            {
                if (_spr != null)
                {
                    EditorUtility.DisplayProgressBar(
                        "Finding~~~",
                        _spr.name,
                        (float)(iTrans + 1) / (float)m_arySelectingTrans.Length
                        );
                }

                if (_spr != null && 
                    _spr.atlas == m_AtlasOnlyOne &&
                    _spr.spriteName == m_strSpriteNameOnlyOne
                    )
                {
                    m_lstSprResult.Add(_spr);
                    m_lstPrefabAllPath.Add(AssetDatabase.GetAssetPath(trSelecing));

                    bIsSucc = true;
                }
            }

            if (bIsSucc)
            {
                m_lstResultSelectingTrans.Add(trSelecing);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    void ShowFindResult()
    {
        GUIStyle style = new GUIStyle(EditorStyles.textField);
        style.fontStyle = FontStyle.Bold;

        float fPosY = 180f;

        EditorGUI.LabelField(
            new Rect(0, fPosY, Screen.width, 20),
            "查找结果:"
            );
        fPosY += 20;

        EditorGUI.LabelField(
            new Rect(0, fPosY, Screen.width, 20),
            "Prefab个数: "
            );
        EditorGUI.TextField(
            new Rect(80, fPosY, 100, 20),
            m_lstResultSelectingTrans.Count.ToString(),
            style
            );
        fPosY += 20;

        EditorGUI.LabelField(
            new Rect(0, fPosY, Screen.width, 20),
            "纹理个数: "
            );
        EditorGUI.TextField(
            new Rect(80, fPosY, 100, 20),
            m_lstSprResult.Count.ToString(),
            style
            );
        fPosY += 35;



        m_v2ScrollPosition = GUI.BeginScrollView(
            new Rect(0, fPosY, Screen.width, 500),
            m_v2ScrollPosition,
            new Rect(0, fPosY, Screen.width, m_lstSprResult.Count * (20 * 4 + 35)),
            false,
            true
            );

        for (int i = 0; i < m_lstSprResult.Count; i++)
        {
            EditorGUI.LabelField(
                new Rect(0, fPosY, Screen.width, 20),
                "查找序号:"
                );
            EditorGUI.TextField(
                new Rect(60, fPosY, 500, 20),
                string.Format("{0}", (i + 1)),
                style
                );
            fPosY += 20;

            EditorGUI.LabelField(
                new Rect(0, fPosY, Screen.width, 20),
                "纹理名字:"
                );
            EditorGUI.TextField(
                new Rect(60, fPosY, 500, 20),
                string.Format("{0}", m_lstSprResult[i].spriteName),
                style
                );
            fPosY += 20;

            EditorGUI.LabelField(
                new Rect(0, fPosY, Screen.width, 20),
                "预置:"
                );
            EditorGUI.TextField(
                new Rect(60, fPosY, 500, 20),
                string.Format("{0}", m_lstPrefabAllPath[i]),
                style
                );
            fPosY += 20;

            EditorGUI.LabelField(
                new Rect(0, fPosY, Screen.width, 20),
                "GO名字:"
                );
            EditorGUI.TextField(
                new Rect(60, fPosY, 500, 20),
                string.Format("{0}", m_lstSprResult[i].name),
                style
                );
            fPosY += 20;

            m_lstSprResult[i] = EditorGUI.ObjectField(
                new Rect(0, fPosY, 578, 20),
                "",
                m_lstSprResult[i],
                typeof(UISprite),
                true
                ) as UISprite;

            fPosY += 35;
        }
        GUI.EndScrollView();
    }

    void Clear(bool bIsClearAtlas)
    {
        if (bIsClearAtlas)
        {
            m_AtlasOnlyOne = null;
        }

        m_lstSprResult.Clear();
        m_lstPrefabAllPath.Clear();

        if (m_arySelectingTrans != null)
        {
            Array.Clear(m_arySelectingTrans, 0, m_arySelectingTrans.Length);
        }
        m_lstResultSelectingTrans.Clear();
    }
}