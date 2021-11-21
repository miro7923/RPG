using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    protected float m_fStartingHealth; //시작체력
    public float Health { get; protected set; } //현재체력
    public bool Dead { get; protected set; } //사망상태 
    public event Action OnDeath; //사망시 발동할 이벤트

    //생명체가 활성화될 때 상태 리셋 
    protected virtual void OnEnable()
    {
        //죽지 않은 상태로 시작 
        Dead = false;
        //체력을 시작 체력으로 초기화 
        Health = m_fStartingHealth;
    }

    public virtual void OnDamage(float fDamage)
    {
        //맞은 데미지만큼 체력 감소 
        Health -= fDamage;

        if (0 >= Health && !Dead)
        {
            //체력이 0이하이고 아직 죽지 않았으면 사망 처리 실행 
            Die();
        }
    }

    public virtual void RestoreHealth(float fNewHealth)
    {
        if (Dead)
            return; //죽었으면 체력 회복 불가 

        //살아있으면 체력 회복
        Health += Health * fNewHealth;
        if (m_fStartingHealth < Health)
            Health = m_fStartingHealth;
    }

    public virtual void Die()
    {
        //OnDeath 이벤트에 등록된 메서드가 있으면 실행 
        if (null != OnDeath)
            OnDeath();

        //사망 상태 참으로 변경 
        Dead = true;
    }
}
