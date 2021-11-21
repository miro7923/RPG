using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmController : MonoBehaviour
{
    private AudioSource m_AudioPlayer;

    private SCENE m_eCurScene = SCENE.None;

    [SerializeField] private AudioClip m_TitleBgm;
    [SerializeField] private AudioClip m_PlayBgm;
    [SerializeField] private AudioClip m_ShopBgm;

    private void Awake()
    {
        m_AudioPlayer = GetComponent<AudioSource>();
        m_AudioPlayer.loop = true;
    }

    public void PlayBgm(SCENE eType)
    {
        if (m_eCurScene.Equals(eType)) return;

        switch (eType)
        {
            case SCENE.Main:
                m_eCurScene = SCENE.Main;
                m_AudioPlayer.clip = m_TitleBgm;
                break;
            case SCENE.Prologue:
                //배경음 없이 대화 넘어가는 효과음만 출력
                m_eCurScene = SCENE.Prologue;
                m_AudioPlayer.clip = null;
                break;
            case SCENE.Play:
                m_eCurScene = SCENE.Play;
                m_AudioPlayer.clip = m_PlayBgm;
                break;
            case SCENE.Shop:
                m_eCurScene = SCENE.Shop;
                m_AudioPlayer.clip = m_ShopBgm;
                break;
        }

        if (m_AudioPlayer.clip)
            m_AudioPlayer.Play();
    }
}
