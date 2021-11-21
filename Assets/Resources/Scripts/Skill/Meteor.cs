using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Skill
{
    [SerializeField] private float Range = 1f;
    private BoxCollider m_ScreenBox; //공격범위 
    private float m_fScreenX;
    private float m_fScreenY;

    private void Awake()
    {
        //화면 크기 구하기 
        m_fScreenY = Camera.main.orthographicSize * 2f;
        m_fScreenX = m_fScreenY * Camera.main.aspect * 2f;

        m_ScreenBox = gameObject.AddComponent<BoxCollider>();
        m_ScreenBox.size = new Vector3(1, m_fScreenY, m_fScreenX);
        m_ScreenBox.isTrigger = true;

        NeededMp = 20f;
        m_fDamage = 1.6f;
        CoolTime = 7f;

        gameObject.SetActive(false);
    }
}
