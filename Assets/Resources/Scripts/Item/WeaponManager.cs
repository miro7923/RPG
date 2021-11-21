using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //무기 교체 및 여러 무기들의 동작 관리하는 클래스 

    private static WeaponManager m_Instance = null;
    public static WeaponManager GetInstance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<WeaponManager>();
                if (!m_Instance) m_Instance = new GameObject("WeaponManager").AddComponent<WeaponManager>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    private WeaponObjects m_WeaponObjs;

    private SkillController m_SkillController;
    public Weapon CurWeapon { get; private set; }

    [SerializeField] float m_DotRange;
    private Enemy m_Target = null;

    private void Awake()
    {
        if (this != GetInstance) Destroy(gameObject);
    }

    public void Init()
    {
        SetWeapon(); 
        m_SkillController.GetCurWeapon(CurWeapon.Sort);
    }

    public void SetSkillController(SkillController skillController)
    {
        m_SkillController = skillController;
    }

    public void SetWeaponObjs(WeaponObjects weaponObjects)
    {
        m_WeaponObjs = weaponObjects;
    }

    public void SetTarget(List<Enemy> enemies, PlayerAttack player)
    {
        //현재 타겟이 없거나 죽었으면 몬스터 배열에서 한 마리를 타겟으로 설정 
        foreach (var enemy in enemies)
        {
            Vector3 vecMonsterPos = enemy.transform.position;
            float fDistance = Vector3.Distance(player.transform.position, vecMonsterPos);

            //평행할 때: 1/ 나는 앞을 보고 있고 몬스터는 옆에서 올 때(90도): 0
            float fDotProduct = Vector3.Dot(player.transform.forward, (vecMonsterPos - player.transform.position).normalized);

            if (CurWeapon.Range >= fDistance && m_DotRange <= fDotProduct)
            {
                m_Target = enemy;
                break;
            }
        }
    }

    private bool CheckDistanceToTarget(PlayerAttack player)
    {
        //플레이어와 타겟 몬스터까지의 거리 확인 
        Vector3 vecMonsterPos = m_Target.transform.position;
        float fDistance = Vector3.Distance(player.transform.position, vecMonsterPos);

        //평행할 때: 1/ 나는 앞을 보고 있고 몬스터는 옆에서 올 때(90도): 0
        float fDotProduct = Vector3.Dot(player.transform.forward, (vecMonsterPos - player.transform.position).normalized);

        if (CurWeapon.Range >= fDistance && m_DotRange <= fDotProduct)
            return true;

        return false;
    }

    public IEnumerator Attack(List<Enemy> enemies, PlayerAttack player)
    {
        //현재 타겟이 있고 공격거리 내에 있으며 죽지 않았으면 죽기 전 까지 타겟만 공격 
        if (m_Target && CheckDistanceToTarget(player) && !m_Target.Dead)
        {
            AudioManager.GetInstance.PlaySfx(SCENE.Play, PLAYER_ATCION.Attack);

            StartCoroutine(CurWeapon.AttackEffect(m_Target.transform.position));
            //무기 파워 정보 가져와서 플레이어 파워와 합친 데미지를 리턴
            float FinalDmg = CurWeapon.Power + player.Power;
            m_Target.OnDamage(FinalDmg);

            //무기별 공격 쿨타임 대기 
            yield return new WaitForSeconds(CurWeapon.Delay);
        }
        else //아니라면 새 타겟 탐색 
            SetTarget(enemies, player);
    }

    //상점기능 추가 후 기능 수정
    //!인벤토리에서 무기를 선택하면 체인지 버튼이 활성화 됨
    //-> 체인지 버튼 누르면 확인 팝업 출력 후 무기 교체
    //-> 무기 교체하면 원래 착용중이었던 무기의 위치가 스왑되어야 함 
    public void SetWeapon()
    {
        if (null != CurWeapon)
            CurWeapon.gameObject.SetActive(false);

        string weaponName = PlayerState.GetInstance.PlayerItems[0].Key.EngName;
        CurWeapon = m_WeaponObjs.Weapons.Find((obj) => obj.name.Equals(weaponName));

        CurWeapon.gameObject.SetActive(true);
    }
}
