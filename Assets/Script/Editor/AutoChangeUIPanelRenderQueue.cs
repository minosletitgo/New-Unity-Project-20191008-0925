using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AutoChangeUIPanelRenderQueue : EditorWindow
{
    static AutoChangeUIPanelRenderQueue m_Window;
    const int m_nCstRenderQueueStartValue = 3000;
    int m_nRenderQueueStartValue = m_nCstRenderQueueStartValue;
    List<UIPanel> m_lstPanelResult = new List<UIPanel>();
    List<string> m_lstPrefabAllPath = new List<string>();
    Vector2 m_v2ScrollPosition;

    Transform[] m_arySelectingTrans;
    List<Transform> m_lstResultSelectingTrans = new List<Transform>();



    [MenuItem("Tool/NGUI/自动更改UIPanel渲染序列", false, 1003)]
    static void CreateWindow()
    {
        m_Window = (AutoChangeUIPanelRenderQueue)EditorWindow.GetWindow(typeof(AutoChangeUIPanelRenderQueue), false, "自动更改UIPanel渲染序列");
        m_Window.Show();
    }

    [MenuItem("Tool/NGUI/自动更改UIPanel渲染序列", true)]
    static bool ValidatedCreateWindow()
    {
        return false;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        m_nRenderQueueStartValue = EditorGUILayout.IntField("RenderQueueStartValue", m_nRenderQueueStartValue);

        string strTitle = string.Format("UIPanel.startingRenderQueue = {0} + UIPanel.depth * 100 ", m_nRenderQueueStartValue);
        EditorGUILayout.LabelField(strTitle);

        if (GUILayout.Button("在手动指向目标文件夹目录中，更改其下所有UI预置内的UIPanel"))
        {
            Clear();
            DoFindInFolder();
        }

        if (m_lstPanelResult.Count > 0)
        {
            if (GUILayout.Button("保存/写入"))
            {
                if (EditorUtility.DisplayDialog("温馨提示", "您确认把本次的修改写入对应的预置文件中吗?", "确定"))
                {
                    AssetDatabase.SaveAssets();
                }                    
            }
        }        

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (m_lstPanelResult.Count > 0)
        {
            ShowFindResult();
        }
    }

    void DoFindInFolder()
    {
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

            DoCollectPanel();
        }
    }

    void DoCollectPanel()
    {
        m_lstResultSelectingTrans.Clear();
        for (int iTrans = 0; iTrans < m_arySelectingTrans.Length; iTrans++)
        {
            Transform trSelecing = m_arySelectingTrans[iTrans];
            UIPanel[] aryPanelRet = trSelecing.GetComponentsInChildren<UIPanel>(true);

            bool bIsSucc = false;
            foreach (UIPanel _panel in aryPanelRet)
            {
                if (_panel != null)
                {
                    EditorUtility.DisplayProgressBar(
                        "Finding~~~",
                        _panel.name,
                        (float)(iTrans + 1) / (float)m_arySelectingTrans.Length
                        );
                }

                if (_panel != null)
                {
                    int nRenderQueue = m_nRenderQueueStartValue + _panel.depth * 100;
                    if (_panel.renderQueue != UIPanel.RenderQueue.StartAt ||
                        _panel.startingRenderQueue != nRenderQueue
                        )
                    {
                        m_lstPanelResult.Add(_panel);
                        m_lstPrefabAllPath.Add(AssetDatabase.GetAssetPath(trSelecing));

                        _panel.renderQueue = UIPanel.RenderQueue.StartAt;
                        _panel.startingRenderQueue = nRenderQueue;

                        EditorUtility.SetDirty(trSelecing);

                        bIsSucc = true;
                    }   
                }
            }

            if (bIsSucc)
            {
                m_lstResultSelectingTrans.Add(trSelecing);
            }
        }

        EditorUtility.ClearProgressBar();

        //if (bIsNeedSave)
        //{
        //    AssetDatabase.SaveAssets();
        //}
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
            "UIPanel个数: "
            );
        EditorGUI.TextField(
            new Rect(80, fPosY, 100, 20),
            m_lstPanelResult.Count.ToString(),
            style
            );
        fPosY += 35;



        m_v2ScrollPosition = GUI.BeginScrollView(
            new Rect(0, fPosY, Screen.width, 500),
            m_v2ScrollPosition,
            new Rect(0, fPosY, Screen.width, m_lstPanelResult.Count * (20 * 4 + 35)),
            false,
            true
            );

        for (int i = 0; i < m_lstPanelResult.Count; i++)
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
                "UIPanel名字:"
                );
            EditorGUI.TextField(
                new Rect(60, fPosY, 500, 20),
                string.Format("{0}", m_lstPanelResult[i].name),
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
                string.Format("{0}", m_lstPanelResult[i].name),
                style
                );
            fPosY += 20;

            m_lstPanelResult[i] = EditorGUI.ObjectField(
                new Rect(0, fPosY, 578, 20),
                "",
                m_lstPanelResult[i],
                typeof(UIPanel),
                true
                ) as UIPanel;

            fPosY += 35;
        }
        GUI.EndScrollView();
    }

    void Clear()
    {
        m_nRenderQueueStartValue = m_nCstRenderQueueStartValue;

        m_lstPanelResult.Clear();
        m_lstPrefabAllPath.Clear();

        if (m_arySelectingTrans != null)
        {
            Array.Clear(m_arySelectingTrans, 0, m_arySelectingTrans.Length);
        }
        m_lstResultSelectingTrans.Clear();
    }
}