using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : Skill
{
    [SerializeField] float Range = 2f;
    private SphereCollider m_Collider; //공격범위 

    private void Awake()
    {
        m_Collider = gameObject.AddComponent<SphereCollider>();
        m_Collider.radius = Range;
        m_Collider.isTrigger = true;

        NeededMp = 10f;
        m_fDamage = 1.3f;
        CoolTime = 5f;

        gameObject.SetActive(false);
    }
}
