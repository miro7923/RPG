using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxController : MonoBehaviour
{
    private AudioSource m_AudioPlayer;

    [SerializeField] private AudioClip m_PrologueSfx;
    [SerializeField] private AudioClip m_AttackSfx;
    [SerializeField] private AudioClip m_GetHitSfx;
    [SerializeField] private AudioClip m_SkillSfx;

    private void Awake()
    {
        m_AudioPlayer = GetComponent<AudioSource>();
    }

    public void PlaySfx(SCENE eScene, PLAYER_ATCION eAct = PLAYER_ATCION.None)
    {
        switch (eScene)
        {
            case SCENE.Prologue:
                m_AudioPlayer.PlayOneShot(m_PrologueSfx);
                break;
            case SCENE.Play:
                PlayPlayerSfx(eAct);
                break;
        }
    }

    private void PlayPlayerSfx(PLAYER_ATCION eAct)
    {
        switch (eAct)
        {
            case PLAYER_ATCION.Attack:
                //!플레이어가 공격할 때 효과음
                m_AudioPlayer.PlayOneShot(m_AttackSfx);
                break;
            case PLAYER_ATCION.GetHit:
                //!맞았을 때 효과음
                m_AudioPlayer.PlayOneShot(m_GetHitSfx);
                break;
            case PLAYER_ATCION.Skill:
                //!스킬 효과음
                m_AudioPlayer.PlayOneShot(m_SkillSfx);
                break;
        }
    }
}
