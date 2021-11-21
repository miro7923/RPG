using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermelon : LivingEntity
{
    //수박 크기별 오브젝트를 가지고 있는 배열 -> 남은 체력에 따라 크기가 작은 수박 활성화 
    [SerializeField] private List<GameObject> m_Pieces = new List<GameObject>();

    private void Awake()
    {
        m_fStartingHealth = 20f;

        GameManager.GetInstance.SetWatermelon(this);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        RestoreHealth(m_fStartingHealth);
        MainCanvas.GetInstance.InitWatermelonHpBar(m_fStartingHealth);

        m_Pieces[0].SetActive(true);
        for (int i = 1; m_Pieces.Count > i; i++)
            m_Pieces[i].SetActive(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        MainCanvas.GetInstance.UpdateWatermelonHpBar(Health);
    }

    public override void RestoreHealth(float fNewHealth)
    {
        base.RestoreHealth(fNewHealth);

        MainCanvas.GetInstance.UpdateWatermelonHpBar(Health);
    }

    public void SetHealth()
    {
        if (0 == GameManager.GetInstance.Stage)
            m_fStartingHealth += Constants.AddHealth;

        MainCanvas.GetInstance.UpdateWatermelonHpBar(Health);
    }

    public override void OnDamage(float fDamage)
    {
        base.OnDamage(1);

        //피격효과 재생
        EffectManager.GetInstance.PlayHitEffect(transform.position);

        //남은 체력에 따라 수박 크기 다르게 출력 
        int rate = (int)(Health * 0.25f);
        switch (rate)
        {
            case 3:
                foreach (var piece in m_Pieces)
                    piece.SetActive(false);
                m_Pieces[1].SetActive(true);
                break;
            case 2:
                foreach (var piece in m_Pieces)
                    piece.SetActive(false);
                m_Pieces[2].SetActive(true);
                break;
            case 1:
                foreach (var piece in m_Pieces)
                    piece.SetActive(false);
                m_Pieces[3].SetActive(true);
                break;
        }

        MainCanvas.GetInstance.UpdateWatermelonHpBar(Health);
    }

    public override void Die()
    {
        base.Die();
    }
}
