using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateEffect : MonoBehaviour
{
    private float m_fTimer = 0;
    [SerializeField] private float m_fLimit;


    void Update()
    {
        //일정시간이 지나면 이펙트 비활성화 
        m_fTimer += Time.deltaTime;
        if (m_fTimer > m_fLimit)
        {
            gameObject.SetActive(false);
            m_fTimer = 0;
        }
    }
}
