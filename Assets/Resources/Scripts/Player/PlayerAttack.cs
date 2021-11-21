using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //공격 애니메이션 실행
    private PlayerInput m_PlayerInput;
    private PlayerMovement m_PlayerMovement;
    private Animator m_PlayerAnimator;

    public float Power { get; private set; } 

    public bool IsAttack { get; private set; }
    private bool m_bPlayAttack01 = true;

    private void Start()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_PlayerAnimator = GetComponent<Animator>();

        Power = PlayerState.GetInstance.Power;
        IsAttack = false;
    }

    private void Update()
    {
        //공격 입력을 받았고 공격 중이 아닐 때 
        if (m_PlayerInput.Attack && !IsAttack)
        {
            //공격 상태 on
            IsAttack = true;
            //공격 중엔 이동할 수 없다.
            m_PlayerMovement.CanMove = false;

            //공격 애니메이션 재생 
            PlayAttackAnim();

            //공격 시작
            StartCoroutine(WeaponManager.GetInstance.Attack(MonsterManager.GetInstance.Enemis, this));
        }
    }

    public void MakeMove()
    {
        //다음 공격을 위한 입력을 받을 수 있도록 함
        //(attack 애니메이션 끝날 때 실행하는 event로 삽입)
        IsAttack = false;
        m_PlayerMovement.CanMove = true;
    }

    public void PlayAttackAnim()
    {
        //공격 애니메이션 2가지를 번갈아 재생할 것이기 때문에 1번 공격 모션 재생 플래그 on
        m_PlayerAnimator.SetBool("PlayAttack01", m_bPlayAttack01);

        //공격 애니메이션 재생 
        m_PlayerAnimator.SetTrigger("Attack");

        //공격 모션이 번갈아 재생되도록 애니메이터에서 컨디션을 반대로 설정해준다.
        //m_bPlayAttack01=true -> 1번 재생
        //m_bPlayAttack01=false -> 2번 재생 
        m_bPlayAttack01 = !m_bPlayAttack01;
    }
}
