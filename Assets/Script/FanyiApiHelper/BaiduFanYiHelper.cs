using System;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
//using LitJson;

using UnityEngine;


public class BaiduFanYiHelper
{
    private static BaiduFanYiHelper _instance;
    private BaiduFanYiHelper() { }
    public static BaiduFanYiHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BaiduFanYiHelper();
            }
            return _instance;
        }
    }


    //http://api.fanyi.baidu.com/api/trans/product/apidoc

    const string mAppId = "20190123000258727";
    const string mSecretKey = "MGLj_5WvgasYSnVhILP3";
    string mRandomString = null;
    
   

    public string DoTranslationFromBaiduFanyi(string strTransBeforeText, EM_TranslationLanguage emFrom, EM_TranslationLanguage emTo, out bool bIsSucc)
    {
        //WebClient类  提供用于将数据发送到由URI标识的资源，以及从由URI标识的资源接受数据的常用方法 
        WebClient client = new WebClient();
        
        string strSign = GetMd5Sign(strTransBeforeText);
        string strURL = string.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}",            
            strTransBeforeText,//HttpUtility.UrlEncode(strTransBeforeText),
            emFrom.ToString(),
            emTo.ToString(),
            mAppId,
            mRandomString,
            strSign
            );
        //从指定url加载资源
        byte[] aryBuffer = client.DownloadData(strURL);
        //将指定字节数组中的所有字节解码为一个字符串
        string strResult = Encoding.UTF8.GetString(aryBuffer);

        //返回最终结果字符串
        /*
        成功
        "{\"from\":\"en\",\"to\":\"zh\",\"trans_result\":[{\"src\":\"Please Enter Words\",\"dst\":\"\\u8bf7\\u8f93\\u5165\\u5355\\u8bcd\"}]}"
        失败
        "{\"error_code\":\"54001\",\"error_msg\":\"Invalid Sign\"}"
         */

        string strRet = null;
        LitJson.JsonData jsonRet = LitJson.JsonMapper.ToObject(strResult);
        if (((IDictionary)jsonRet).Contains("error_code"))
        {
            strRet = string.Format("error_code:{0}", jsonRet["error_code"].ToString());
            bIsSucc = false;
        }
        else
        {
            LitJson.JsonData jsonAry = jsonRet["trans_result"][0];
            strRet = jsonAry["dst"].ToString();
            bIsSucc = true;
        }

        return strRet;
    }

    private string GetMd5Sign(string strTransBeforeText)
    {
        string strSignBefore = null;
        mRandomString = GetRandomString();
        strSignBefore = mAppId + strTransBeforeText + mRandomString + mSecretKey;
        Debug.Log("strSignBefore: "+ strSignBefore);

        //将指定的字符串中的所有字符编码为一个UTF-8字节序列        
        //ASCIIEncoding encoding = new ASCIIEncoding();
        //byte[] aryBuffer = encoding.GetBytes(strSignBefore);
        byte[] aryBuffer = UTF8Encoding.UTF8.GetBytes(strSignBefore);
        
        //使用Create() 创建默认实例
        MD5 md5 = MD5.Create();
        //开始加密
        byte[] aryHash = md5.ComputeHash(aryBuffer);
        StringBuilder sbRet = new StringBuilder();
        for (int i = 0; i < aryHash.Length; i++)
        {
            sbRet.AppendFormat("{0:x2}", aryHash[i]);
        }
        //输出MD5加密后的字符串
        string strSignAfter = sbRet.ToString();
        Debug.Log("strSignAfter: " + strSignAfter);
        return strSignAfter;
    }

    private string GetRandomString()
    {
        StringBuilder sbRet = new StringBuilder();
       System.Random random = new System.Random();
        for (int i = 0; i < 10; i++)
        {
            sbRet.Append(random.Next(0, 9));
        }
        return sbRet.ToString();
    }
};