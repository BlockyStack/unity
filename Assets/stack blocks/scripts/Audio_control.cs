using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Audio_control : MonoBehaviour
{

    [Header("game_start")]
    public AudioClip audio_clip_game_start;

    [Header("game_over")]
    public AudioClip audio_clip_game_over;

    [Header("cut")]
    public AudioClip audio_clip_cut;

    [Header("score")]
    public AudioClip audio_clip_score;



    //单列模式 Singleton mode
    public static Audio_control instance;

    //音源 audio source
    private AudioSource audio_source;

    void Awake()
    {
        //单列模式 Singleton mode
        Audio_control.instance = this;
    }

    // Use this for initialization
    void Start()
    {
        this.audio_source = this.GetComponent<AudioSource>();
    }

    public void play_sound_game_start()
    {
        if (this.audio_source != null)
        {
            this.audio_source.PlayOneShot(this.audio_clip_game_start);
        }
    }

    public void play_sound_game_over()
    {
        if (this.audio_source != null)
        {
            this.audio_source.PlayOneShot(this.audio_clip_game_over);
        }
    }

    public void play_sound_cut()
    {
        if (this.audio_source != null)
        {
            this.audio_source.PlayOneShot(this.audio_clip_cut);
        }
    }

    public void play_sound_score()
    {
        if (this.audio_source != null)
        {
            this.audio_source.PlayOneShot(this.audio_clip_score);
        }
    }


}

