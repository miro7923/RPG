using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField] private SwordSlash m_SwordSlash;
    [SerializeField] private HamAttack m_HamAttack;
    [SerializeField] private Meteor m_Meteor;
    private Skill m_Skill;
    private ITEM_SORT m_eSkillSort = ITEM_SORT.None;

    private PlayerHealth m_PlayerHealth;
    private PlayerAttack m_PlayerAttack;

    private void Awake()
    {
        m_PlayerHealth = GetComponentInParent<PlayerHealth>();
        m_PlayerAttack = GetComponentInParent<PlayerAttack>();
        WeaponManager.GetInstance.SetSkillController(this);
    }

    public void GetCurWeapon(ITEM_SORT eWeapon)
    {
        switch (eWeapon)
        {
            case ITEM_SORT.Sword:
                m_Skill = m_SwordSlash;
                break;
            case ITEM_SORT.Hammer:
                m_Skill = m_HamAttack;
                break;
            case ITEM_SORT.Wand:
                m_Skill = m_Meteor;
                break;
        }

        m_eSkillSort = eWeapon;
        MainCanvas.GetInstance.SetSkillIcon(m_eSkillSort);
    }

    public void Skill()
    {
        //쿨타임이 끝났고 플레이어의 현재 mp가 충분하면 스킬 사용 가능
        //해당 함수는 Skill 버튼이 클릭될 때 호출됨 
        if (!m_PlayerHealth.Dead && m_Skill && m_Skill.NeededMp <= m_PlayerHealth.CurMp)
        {
            m_PlayerAttack.PlayAttackAnim();
            //m_PlayerHealth.Skill(m_Skill.NeededMp);
            m_Skill.On();
            StartCoroutine(MainCanvas.GetInstance.ActivateCoolTimeIcon(m_eSkillSort, m_Skill.CoolTime));
        }
    }
}
