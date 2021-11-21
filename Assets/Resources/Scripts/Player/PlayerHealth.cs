using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public float CurMp { get; private set; } //스킬 사용할 때 쓸 MP
    private float m_fDeffense;

    private Animator m_PlayerAnimator;

    private PlayerMovement m_PlayerMovement;
    [SerializeField] private Transform m_StartPoint;

    public bool GetHit { get; private set; }

    private void Awake()
    {
        m_PlayerAnimator = GetComponent<Animator>();
        m_PlayerMovement = GetComponent<PlayerMovement>();
        PlayerState.GetInstance.SetPlayerHealth(this);
        GameManager.GetInstance.SetPlayerHealth(this);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        m_fStartingHealth = PlayerState.GetInstance.Hp;
        Health = m_fStartingHealth;
        CurMp = PlayerState.GetInstance.Mp;
        m_fDeffense = PlayerState.GetInstance.Deffense;
        MainCanvas.GetInstance.InitHealthSlider(Health, CurMp);

        transform.position = m_StartPoint.position;
        transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    public void InitHealth(float hp, float mp)
    {
        if (!Dead)
        {
            Health = hp;
            CurMp = mp;

            MainCanvas.GetInstance.InitHealthSlider(Health, CurMp);
        }
    }

    protected override void OnEnable()
    {
        //LivingEntity - OnEnable() 실행해서 상태 초기화 
        base.OnEnable();

        CurMp = PlayerState.GetInstance.Mp;
        //체력/마력 슬라이더 초기화 
        MainCanvas.GetInstance.InitHealthSlider(m_fStartingHealth, PlayerState.GetInstance.Mp);

        m_PlayerMovement.enabled = true;
    }

    public override void RestoreHealth(float fNewHealth)
    {
        //LivingEntity의 RestoreHealth 실행으로 체력 증가 
        base.RestoreHealth(fNewHealth);

        //증가된 체력으로 체력 슬라이더 갱신
        MainCanvas.GetInstance.UpdateHpSlider(Health);
    }

    public override void OnDamage(float fDamage)
    {
        if (!Dead)
        {
            //추가) 맞는 효과음 재생

            //맞는 애니메이션 재생 
            m_PlayerAnimator.SetTrigger("GetHit");
            //맞는 동안에는 이동 불가 
            m_PlayerMovement.CanMove = false;
        }

        //LivingEntity의 OnDamage 실행해서 입력된 데미지에서 방어력만큼 뺀 체력 감소 
        base.OnDamage(fDamage - m_fDeffense);

        //감소된 체력으로 슬라이더 갱신 
        MainCanvas.GetInstance.UpdateHpSlider(Health);
    }

    public override void Die()
    {
        //LivingEntity의 Die 실행해서 사망 이벤트 처리 
        base.Die();

        //사망 애니메이션 재생
        m_PlayerAnimator.SetBool("isDead", true);
        m_PlayerAnimator.SetTrigger("Die");

        m_PlayerMovement.enabled = false;
    }

    public void Skill(float usage)
    {
        //스킬의 mp 소모량만큼 감소 
        CurMp -= usage;

        MainCanvas.GetInstance.UpdateMpSlider(CurMp);
    }

    public void RestoreMp(float newMp)
    {
        CurMp += PlayerState.GetInstance.Mp * newMp;
        if (PlayerState.GetInstance.Mp < CurMp)
            CurMp = PlayerState.GetInstance.Mp;

        MainCanvas.GetInstance.UpdateMpSlider(CurMp);
    }
}
