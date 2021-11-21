using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : LivingEntity
{
    private float m_fExp;
    private float m_fGold;

    private bool m_isHit = false;
    [SerializeField] private LayerMask m_TargetLayer; //초기 타겟은 수박(9번)
    private LivingEntity m_TargetEntity;
    private NavMeshAgent m_PathFinder;

    private Animator m_EnemyAnimator;
    private Renderer m_EnemyRenderer;

    private float m_fDamage = 20f; //몬스터 공격력 
    private float m_fAttackDleay = 2f; //공격 간격

    private float m_fHpBarOffset;
    [SerializeField] private Slider m_HealthSlider;
    private RectTransform m_HpBarRectTr;

    private bool m_isAttack = false;

    private float m_fFollowSpeed = 2f;
    private float m_fWanderSpeed = 0.5f;

    private Vector3 m_vecEndPos = Vector3.zero;

    private int m_iWanderPoint = 0;

    private float m_fEffectOffset;

    private CapsuleCollider m_CapsuleCollider;
    private BoxCollider m_BoxCollider;

    private bool m_TargetChanged = false;

    private void Awake()
    {
        m_PathFinder = GetComponent<NavMeshAgent>();
        m_EnemyAnimator = GetComponent<Animator>();
        m_EnemyRenderer = GetComponentInChildren<Renderer>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_BoxCollider = GetComponent<BoxCollider>();

        //내비메시 에이전트의 이동 속도 설정 
        m_PathFinder.speed = m_fWanderSpeed;

        //몬스터 머리 위 체력바 위치 & 피격 효과 재생 위치 정해줌 
        //몬스터 디자인별로 키가 달라서 태그에 따라 각각 설정 
        if (gameObject.CompareTag("Slime") || gameObject.CompareTag("TurtleShell"))
        {
            m_fHpBarOffset = 1.5f;
            m_fEffectOffset = 0.5f;
        }
        else if (gameObject.CompareTag("Beholder"))
        {
            m_fHpBarOffset = 2.5f;
            m_fEffectOffset = 1.5f;
        }
        else if (gameObject.CompareTag("ChestMonster"))
        {
            m_fHpBarOffset = 2f;
            m_fEffectOffset = 1f;
        }

        //위치 초기화 
        transform.position = Vector3.zero;

        //체력바를 메인 캔버스의 자식으로 만들어 줌 
        m_HealthSlider.transform.SetParent(MainCanvas.GetInstance.Canvas.transform);
        //체력바 Rect transform 얻어오기 
        m_HpBarRectTr = m_HealthSlider.GetComponent<RectTransform>();
        SetHpSliderPos();
        m_HealthSlider.gameObject.SetActive(false);

        //죽으면 경험치를 얻을 수 있는 함수 event 등록 
        OnDeath += () => { PlayerState.GetInstance.GetExp(m_fExp); };
        //스테이지를 클리어하면 죽인 몬스터 수 만큼 골드를 지급할 수 있게 따로 모아 둠 
        OnDeath += () => { GameManager.GetInstance.AddGold(m_fGold); };
        //현재까지 죽인 몬스터 카운트 증가 (현재 진행상황 확인용)
        OnDeath += () => { GameManager.GetInstance.AddKillingCount(); };
        //시체 제거) 죽으면 2초 뒤에 비활성화 시켜뒀다가 재활용
        OnDeath += () => { StartCoroutine(Remove()); };

        gameObject.SetActive(false);
    }

    private void Update()
    {
        //플레이어한테 맞으면 타겟 레이어를 플레이어로 변경 및 타겟 재탐색 
        if (m_isHit && !m_TargetChanged)
        {
            m_TargetLayer = LayerMask.GetMask("Player");
            m_TargetEntity = null;
            m_TargetChanged = true;
        }

        m_EnemyAnimator.SetBool("HasTarget", HasTarget);
        m_EnemyAnimator.SetFloat("Speed", m_PathFinder.speed);

        //몬스터 움직임 따라서 머리 위에서 체력바 이동
        SetHpSliderPos();
    }

    public void Init(Vector3 pos)
    {
        //생성 위치 정해주기
        transform.position = pos;

        //활성화 
        gameObject.SetActive(true);
        m_HealthSlider.gameObject.SetActive(true);
        m_PathFinder.enabled = true;
        m_PathFinder.speed = 0;
        m_CapsuleCollider.enabled = true;
        m_BoxCollider.enabled = true;
        m_isHit = false;
        m_TargetLayer = LayerMask.GetMask("Watermelon");

        //현재 러닝 상태에 따라서 동작 시작/정지 
        RunBehaviour(GameManager.GetInstance.isRunning);
    }

    public void RunBehaviour(bool running)
    {
        switch (running)
        {
            case true:
                if (gameObject.activeSelf)
                    StartCoroutine(UpdatePath());
                break;
            case false:
                if (gameObject.activeSelf && m_PathFinder.enabled)
                    m_PathFinder.isStopped = true;
                break;
        }
    }

    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(2f);

        transform.position = Vector3.zero;
        m_HpBarRectTr.position = Vector3.zero;
        m_HealthSlider.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SetHpSliderPos()
    {
        Vector3 newPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, m_fHpBarOffset, 0));
        m_HpBarRectTr.position = newPos;
    }

    protected override void OnEnable()
    {
        //LivingEntity - OnEnable() 실행해서 상태 초기화 
        base.OnEnable();

        //체력 슬라이더 위치 지정 
        SetHpSliderPos();
        //체력 슬라이더 활성화 
        m_HealthSlider.gameObject.SetActive(true);
        //슬라이더 최대값 시작체력으로 변경 
        m_HealthSlider.maxValue = m_fStartingHealth;
        //슬라이더 값 현재 체력으로 변경 
        m_HealthSlider.value = Health;
    }

    private bool HasTarget
    {
        get
        {
            //추적할 대상이 존재하고 대상이 사망하지 않았다면 true 
            if (null != m_TargetEntity && !m_TargetEntity.Dead)
                return true;

            //아니면 false
            m_PathFinder.speed = 1;
            return false;
        }
    }

    //적 AI의 초기 스펙 셋업 
    public void Setup(bool boss, MonsterStatus status, Color SkinColor = default)
    {
        if (boss) //boss monster는 일반 몬스터와 다르게 능력치 설정 
        {
            //체력 설정
            m_fStartingHealth = status.Hp * 2;
            Health = status.Hp;
            //공격력 설정 
            m_fDamage = status.Power * 2;
            //처지시 획득할 수 있는 경험치 설정 
            m_fExp = status.Exp * 2;
            //처치시 획득할 수 있는 금액 설정 
            m_fGold = status.Gold * 2;
            //Renderer matarial 컬러 변경 -> 보스 몬스터용 
            m_EnemyRenderer.material.color = SkinColor;
            //처지시 획득할 수 있는 경험치 설정 
            m_fExp = status.Exp * 2;
            //처치시 획득할 수 있는 금액 설정 
            m_fGold = status.Gold * 2;

            return;
        }

        //체력 설정
        m_fStartingHealth = status.Hp;
        Health = status.Hp;
        //공격력 설정 
        m_fDamage = status.Power;
        //처지시 획득할 수 있는 경험치 설정 
        m_fExp = status.Exp;
        //처치시 획득할 수 있는 금액 설정 
        m_fGold = status.Gold;
    }

    private IEnumerator UpdatePath()
    {
        while (!Dead && GameManager.GetInstance.isRunning)
        {
            if (HasTarget)
            {
                //추적 대상이 존재하고 만나지 못했음 -> 경로 갱신 & AI 이동 진행 
                //만남 -> 멈춰서 공격
                //!타겟 레이어 바뀌면 타겟 자체도 바꿔주기 
                Vector3 vecTargetPos = m_TargetEntity.transform.position;
                float fDistance = Vector3.Distance(transform.position, vecTargetPos);
                m_PathFinder.speed = m_fFollowSpeed;

                //공격 범위 안이 아니면 목적지로 이동하기 
                if (2 < fDistance)
                {
                    m_PathFinder.isStopped = false;
                    m_EnemyAnimator.SetBool("isAttack", false);
                    m_PathFinder.SetDestination(m_TargetEntity.transform.position);
                }
                //공격 범위 안이면 공격 
                else
                {
                    //이동 중지 
                    m_PathFinder.isStopped = true;

                    if (!m_isAttack)
                    {
                        yield return Attack();
                    }
                }
            }
            else
            {
                //추적 대상 없음 -> 주변 배회하기 
                m_PathFinder.isStopped = false;
                m_EnemyAnimator.SetBool("isAttack", false);
                m_PathFinder.speed = m_fWanderSpeed;

                SetWanderPoint();
                m_PathFinder.SetDestination(m_vecEndPos);

                //20유닛의 반지름을 가진 가상의 구를 그렸을 때
                //구와 겹치는 모든 콜라이더를 가져옴
                //-> m_WhatIsTarget Layer를 가진 콜라이더만 가져오도록 필터링
                Collider[] Colliders = Physics.OverlapSphere(transform.position, 20f, m_TargetLayer);

                //모든 콜라이더를 순회하면서 살아 있는 LivingEntity 찾기
                for (int i = 0; Colliders.Length > i; i++)
                {
                    //콜라이더로부터 LivingEntity Component 가져오기 
                    LivingEntity livingEntity = Colliders[i].GetComponent<LivingEntity>();

                    //LivingEntity Component가 존재하며, 해당 LivingEntity가 살아 있다면 
                    if (null != livingEntity && !livingEntity.Dead)
                    {
                        //추적 대상을 해당 LivingEntity로 설정 
                        m_TargetEntity = livingEntity;
                        
                        //반복문 즉시 탈출 
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private void SetWanderPoint()
    {
        //타겟이 없을 때 혼자 돌아다닐 것인데 일정 범위 안에서 움직이도록 목적지 설정 
        m_vecEndPos = MonsterManager.GetInstance.WanderPoints[m_iWanderPoint].transform.position;

        if (transform.position.x == m_vecEndPos.x)
        {
            //목적지에 도착하면 인덱스 증가시켜서 새로운 목적지 설정 
            m_iWanderPoint++;
            if (MonsterManager.GetInstance.WanderPoints.Length <= m_iWanderPoint)
                m_iWanderPoint = 0;
        }
    }

    public override void OnDamage(float fDamage)
    {
        if (!Dead && GameManager.GetInstance.isRunning)
        {
            m_isHit = true;

            //맞는 동안에는 이동 및 공격 불가/ 타겟 탐색 코루틴 중지
            m_EnemyAnimator.SetTrigger("GetHit");

            //LivingEntity의 OnDamage() 실행해서 데미지 적용 
            base.OnDamage(fDamage);
            m_HealthSlider.value = Health;

            //effect manager에서 만들어놓은 것 호출
            EffectManager.GetInstance.PlayHitEffect(transform.position + new Vector3(0, m_fEffectOffset, 0));
        }
    }

    public override void Die()
    {
        //LivingEntity의 Die() 실행해서 기본 사망 처리 실행 
        base.Die();

        //다른 AI를 방해하지 않도록 자신의 모든 콜라이더 비활성화 
        Collider[] EnemyColliders = GetComponents<Collider>();
        for (int i = 0; EnemyColliders.Length > i; i++)
            EnemyColliders[i].enabled = false;

        //AI 추적 중지/ 내비메쉬 컴포넌트 비활성화 
        m_PathFinder.isStopped = true;
        m_PathFinder.enabled = false;

        //사망 애니메이션 재생
        m_EnemyAnimator.SetBool("isDead", true);
        m_EnemyAnimator.SetTrigger("Die");

        m_HealthSlider.gameObject.SetActive(false);
    }

    private IEnumerator Attack()
    {
        //공격 애니메이션 재생 & 데미지 주기 
        m_isAttack = true;
        m_EnemyAnimator.SetBool("isAttack", true);
        m_EnemyAnimator.SetTrigger("Attack");
        m_TargetEntity.OnDamage(m_fDamage);

        //공격 쿨타임 대기 
        yield return new WaitForSeconds(m_fAttackDleay);

        m_isAttack = false;
        m_EnemyAnimator.SetBool("isAttack", false);
    }

    public void Kill()
    {
        if (gameObject.activeSelf && !Dead)
            OnDamage(m_fStartingHealth);
    }
}
