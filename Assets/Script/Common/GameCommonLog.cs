using UnityEngine;
using System;
using System.Text;
using System.IO;


//-------------------------------------------------------------------------
public class GameCommonLog
{
#if UNITY_IPHONE || !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || DEBUG_IO)
    static public bool RunOnWindow = false;
    static public bool LogToFile = true;
    public static string mLogFileName = MakeGamePathFileName("GameRun.txt");
#else
    static public bool RunOnWindow = true;
    static public bool LogToFile = true;
    public static string mLogFileName = MakeGamePathFileName("GameRun.txt");
    
#endif

    static public GameCommonLog instance = null;
    StreamWriter mLogWriter = null;

    public static GameCommonLog Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameCommonLog();
            }
            return instance;
        }
    }

    public void CLog()
    {
        try
        {
            if (LogToFile)
            {
                if (File.Exists(mLogFileName))
                    File.Delete(mLogFileName);

                FileStream f = new FileStream(mLogFileName, FileMode.Create);

                if (LogToFile)
                    mLogWriter = new StreamWriter(f, Encoding.Unicode);

                Log(mLogFileName);
                Debug.LogWarning("LOG file>" + mLogFileName);

                if (RunOnWindow)
                {
                    if (Directory.Exists("TableLog"))
                        Directory.Delete("TableLog", true);
                    Directory.CreateDirectory("TableLog");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error :" + e.ToString());
        }
    }

    public void Log(string info)
    {
        if (LogToFile)
        {
            if (mLogWriter != null)
            {
                mLogWriter.WriteLine("[" + DateTime.Now.ToString("T") + ":" + DateTime.Now.Millisecond.ToString() + "] " + info);
                mLogWriter.Flush();
            }
            else
            {
                CLog();
                Log(info);
            }
        }
    }

    public static string MakeGamePathFileName(string strFileName)
    {
        if (RunOnWindow)
        {
            return Application.dataPath + "/" + strFileName;
        }
        else
        {
            return Application.persistentDataPath + "/" + strFileName;
        }
    }
}



