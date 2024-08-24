using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.IO;
//using Logic;


//-------------------------------------------------------------------------
//public class LOG
//{
//    static public void log(string info)
//    {
//#if UNITY_EDITOR
//        Logic.EventCenter.Log(LOG_LEVEL.GENERAL, info);
//        Debug.Log(info);
//#else
//#   if UNITY_IPHONE
//        Debugger.Log(info);
//#   endif
//        if (CommonParam.LogToFile)        
//            Logic.EventCenter.Log(LOG_LEVEL.GENERAL, info);        
//#endif
//    }

//    static public void logError(string info)
//    {
//#if UNITY_EDITOR
//        Logic.EventCenter.Log(LOG_LEVEL.ERROR, info);
//        //Debug.LogError(info);
//#else
//#   if UNITY_IPHONE
//        Debugger.LogError(info);
//#   endif
//        if (CommonParam.LogToFile)        
//            Logic.EventCenter.Log(LOG_LEVEL.ERROR, info);      
//#endif
//    }

//    static public void logWarn(string info)
//    {
//#if UNITY_EDITOR
//        Logic.EventCenter.Log(LOG_LEVEL.WARN, info);
//        Debug.LogWarning(info);
//#else
//#   if UNITY_IPHONE
//        Debugger.LogWarning(info);
//#   endif
//        if (CommonParam.LogToFile)        
//            Logic.EventCenter.Log(LOG_LEVEL.WARN, info); 
//#endif
//    }

//    static public void LogFormat(string info, params object[] param)
//    {
//        log(string.Format(info, param));
//    }

//    internal static void log(object p)
//    {
//        throw new NotImplementedException();
//    }
//}


public class EditorLOG
{
    static private string errorColor = "red";
    static private string warningColor = "yellow";
    static private string logColor = "white";

    static public void log(string info)
    {
#if UNITY_EDITOR
        //Logic.EventCenter.Log(LOG_LEVEL.GENERAL, info);
        Debug.Log(string.Concat("<color=", logColor, ">", info, "</color>"));
#endif
    }

    static public void logError(string info)
    {
#if UNITY_EDITOR
        //Logic.EventCenter.Log(LOG_LEVEL.ERROR, info);
        Debug.LogError(string.Concat("<color=", errorColor, ">", info, "</color>"));
#endif
    }

    static public void logWarn(string info)
    {
#if UNITY_EDITOR
        //Logic.EventCenter.Log(LOG_LEVEL.WARN, info);
        Debug.LogWarning(string.Concat("<color=", warningColor, ">", info, "</color>"));
#endif
    }
}

//public class UDPLog : UDPNet
//{
//    DataBuffer mBuffer = new DataBuffer(1500);

//    public UDPLog()
//    {
//        InitNet("192.168.2.70", 4980, false);
//    }

//    public void SendLog(string info)
//    {
//        byte[] str = StaticDefine.GetBytes(info);
//        SendData(str, 0, str.Length<1200?str.Length:1200);
//    }
//}



//public class CLog : tLog
//{
//	//static UITextList debugTextUi;
//	static public GUIText mLogObject;
//    static public bool mbLogFile = false;

//    StreamWriter mLogWriter = null;

//    [ThreadStatic]
//    static UDPLog msLogUDP;

//    static void sendLog(string info)
//    {
//        if (msLogUDP == null)
//        {
//            msLogUDP = new UDPLog();
//        }

//        msLogUDP.SendLog(info);
//    }
	
//	public CLog()
//	{
//#if RELEASE_WEB
		
//#else
//        try
//        {
//            if (CommonParam.LogToFile)
//            {
//                if (File.Exists(mLogFileName))
//                    File.Delete(mLogFileName);

//                FileStream f = new FileStream(mLogFileName, FileMode.Create);

//                if (CommonParam.LogToFile)
//                    mLogWriter = new StreamWriter(f, Encoding.Unicode);

//                Log(mLogFileName);
//                Debug.LogWarning("LOG file>" + mLogFileName);

//                if (CommonParam.RunOnWindow)
//                {
//                    if (Directory.Exists("TableLog"))
//                        Directory.Delete("TableLog", true);
//                    Directory.CreateDirectory("TableLog");
//                }
//            }
//        }
//        catch (Exception e)
//        {
//            sendLog("Error :" + e.ToString());
//        }
//        catch
//        {

//        }
//#endif
//	}
	
//	//static public void Init()
//	//{
//	//    //UiControl.self.ShowDebugInfoPanel(true);
//	//    //UiControl.self.ShowFramePanel(true);
//	//    GameObject obj = GameObject.Find("InfoTextList") as GameObject;
//	//    if (obj!=null)
//	//    {
//	//        obj.SetActiveRecursively(true);
//	//        debugTextUi = obj.GetComponentInChildren<UITextList>();
//	//    }
	
//	//}
	
//	#if UNITY_IPHONE || UNITY_ANDROID || DEBUG_IO
//	public static string mLogFileName = GameCommon.MakeGamePathFileName("GameRun.txt");
//	#else
//	//public static string mLogFileName = GameCommon.MakeGamePathFileName("GameRun.txt");
//	public static string mLogFileName = "GameRun.txt";
//	#endif
//	public override void Log(string info)
//	{
//		//GameObject g = GameObject.Find("DebugInfo2");
//		//if (null != mLogObject)
//		//{
//		//    GUIText gt = mLogObject.GetComponent<GUIText>();
//		//    if (gt!=null)
//		//        gt.text += info + "\n\t";
//		//}
		
//		//if (mLogObject != null)
//		//	mLogObject.text += info + "\n\t";
		
		
//		//Debug.Log( info );
//		#if RELEASE_WEB
//		//if (debugTextUi == null)
//		//    Init();
//		//if (debugTextUi != null)
//		//    debugTextUi.Add(info);
		
//		#else
//        if (CommonParam.LogToFile)
//        {
//            //GameMain.Show(info);
//            //??? sendLog(info);
////#if UNITY_EDITOR
////            bool b = File.Exists(mLogFileName);
////            StreamWriter writer = new StreamWriter(mLogFileName, b, Encoding.Unicode);
////            writer.WriteLine(DateTime.Now.ToString("T") + ">" + info);            
////            writer.Write("\r\n");
////            writer.Close();
////#else
//            if (mLogWriter!=null)
//            {
//                mLogWriter.WriteLine("[" + DateTime.Now.ToString("T") + ":" + DateTime.Now.Millisecond.ToString() + "] " + info);
//                mLogWriter.Flush();
//            }
////#endif
//        }
//		#endif
//	}
//	public GameObject log;
	
	
//}



