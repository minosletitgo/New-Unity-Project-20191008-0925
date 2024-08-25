using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class CMovieTexture : MonoBehaviour
{
    // 声明用于获取视频纹理的MovieTexture字段Movie.
    public MovieTexture Movie = null;
    // 声明用于获取视频的音频剪辑的AudioSource.  
    private AudioSource audios;


    void Start()
    {
        // 获取PLane对象的MeshRenderer组件.
        MeshRenderer meshrenderer = GetComponent<MeshRenderer>();
        // 将MeshRenderer组件的纹理材质替换为MovieTexture类型的视频.
        meshrenderer.material.mainTexture = Movie;
        audios = GetComponent<AudioSource>();
        // 将MovieTexture的音频剪辑赋值给Audio Source组件的clip
        audios.clip = Movie.audioClip;
        //audio.spatialBlend = 0;
    }

    // 按下空格键进行播放控制.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Movie.isPlaying && !audios.isPlaying)
        {
            Movie.Play();
            audios.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && Movie.isPlaying && audios.isPlaying)
        {
            Movie.Pause();
            audios.Pause();
        }
    } 
}