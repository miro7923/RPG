using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager m_Instance = null;
    public static AudioManager GetInstance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<AudioManager>();
                if (!m_Instance) m_Instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    [SerializeField] private BgmController m_Bgm;
    [SerializeField] private SfxController m_Sfx;

    private void Awake()
    {
        if (this != GetInstance) Destroy(gameObject);
    }

    public void PlayBgm(SCENE eScene)
    {
        m_Bgm.PlayBgm(eScene);
    }

    public void PlaySfx(SCENE eScene, PLAYER_ATCION eAct = PLAYER_ATCION.None)
    {
        m_Sfx.PlaySfx(eScene, eAct);
    }
}
