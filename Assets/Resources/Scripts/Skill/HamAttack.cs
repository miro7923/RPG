using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamAttack : Skill
{
    [SerializeField] Vector3 Range = new Vector3(1, 2, 1);
    [SerializeField] Vector3 Center = new Vector3(0, -2, 0);
    private BoxCollider m_Collider; //공격범위

    private void Awake()
    {
        m_Collider = gameObject.AddComponent<BoxCollider>();
        m_Collider.center = Center;
        m_Collider.size = Range;
        m_Collider.isTrigger = true;

        NeededMp = 5f;
        m_fDamage = 3f;
        CoolTime = 3f;

        gameObject.SetActive(false);
    }
}
