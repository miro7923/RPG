using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public float CoolTime { get; protected set; }
    protected float m_fDamage; //damage 배율 
    public float NeededMp { get; protected set; } //스킬 한 번 쓰는데 필요한 마나

    public void On()
    {
        gameObject.SetActive(true);
        StartCoroutine(Off());
    }

    protected IEnumerator Off()
    {
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (Equals(other.gameObject.layer, LayerMask.NameToLayer("Monster")) ||
            Equals(other.gameObject.layer, LayerMask.NameToLayer("Boss")))
        {
            //스킬 사용가능하면 데미지 입히는 처리
            LivingEntity livingEntity = other.GetComponent<LivingEntity>();
            if (livingEntity && !livingEntity.Dead)
            {
                AudioManager.GetInstance.PlaySfx(SCENE.Play, PLAYER_ATCION.Skill);
                EffectManager.GetInstance.PlaySkillEffect(livingEntity.transform.position);

                //데미지 입히기
                //총 데미지x배율
                float playerPow = PlayerState.GetInstance.Power;
                float weaopnPow = WeaponManager.GetInstance.CurWeapon.Power;
                livingEntity.OnDamage((playerPow + weaopnPow) * m_fDamage);
            }
        }
    }
}
