using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager m_Instance = null;
    public static EffectManager GetInstance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new GameObject("EffectManager").AddComponent<EffectManager>();
            }

            return m_Instance;
        }
    }

    [SerializeField] private GameObject m_HitEffectPrefab;
    private List<GameObject> m_HitEffects = new List<GameObject>();

    [SerializeField] private GameObject m_SkillEffectPrefab;
    private List<GameObject> m_SkillEffects = new List<GameObject>();

    private void Awake()
    {
        if (null == m_Instance)
        {
            m_Instance = this;

            Init();

            return;
        }

        Destroy(gameObject);
    }

    public void Init()
    {
        //피격당했을 때 재생할 이펙트 생성 후 플레이 중엔 활성화/비활성화하며 돌려쓰기
        for (int i = 0; 10 > i; i++)
        {
            CreateHitEffect();
            CreateSkillEffect();
        }
    }

    private GameObject CreateHitEffect()
    {
        if (m_HitEffectPrefab)
        {
            var effect = Instantiate(m_HitEffectPrefab, transform.position, transform.rotation);
            if (effect)
            {
                effect.gameObject.SetActive(false);
                m_HitEffects.Add(effect);
            }

            return effect;
        }

        return null;
    }

    private GameObject CreateSkillEffect()
    {
        if (m_SkillEffectPrefab)
        {
            var effect = Instantiate(m_SkillEffectPrefab, transform.position, transform.rotation);
            if (effect)
            {
                effect.gameObject.SetActive(false);
                m_SkillEffects.Add(effect);
            }

            return effect;
        }

        return null;
    }

    public void PlayHitEffect(Vector3 pos)
    {
        //비활성화 상태인 effect 찾기 
        var effect = m_HitEffects.Find(b => !b.gameObject.activeSelf);
        if (!effect) //없으면 새로 만든다 
            effect = CreateHitEffect();

        if (effect)
        {
            //위치 정해주고 재생(활성화)
            effect.transform.position = pos;
            effect.gameObject.SetActive(true);
        }
    }

    public void PlaySkillEffect(Vector3 pos)
    {
        var effect = m_SkillEffects.Find(b => !b.gameObject.activeSelf);
        if (!effect) effect = CreateSkillEffect();

        if (effect)
        {
            effect.transform.position = pos;
            effect.gameObject.SetActive(true);
        }
    }
}
