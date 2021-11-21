using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager m_Instance = null;
    public static MonsterManager GetInstance
    {
        get
        {
            if (null == m_Instance)
            {
                m_Instance = new GameObject("MonsterManager").AddComponent<MonsterManager>();
            }
            return m_Instance;
        }
    }

    //몬스터 레벨 디자인을 위한 능력치 테이블 
    private List<MonsterStatus> m_MonsterTable = new List<MonsterStatus>();
    private int m_MonTableIndex = 0;

    private Color m_BossColor = Color.red;
    public bool BossIsSpawn { get; private set; } = false;

    public GameObject[] WanderPoints { get; private set; }
    [SerializeField] private Transform[] m_SpawnPoints;
    [SerializeField] private Transform m_BossPoint;

    private Enemy[] m_MonsterPrefabs;
    public List<Enemy> Enemis { get; private set; } = new List<Enemy>();
    private Enemy m_CurBoss;

    private float m_fSpawnRate = 5f; //스폰 간격은 정해져있고 스테이지 레벨이 높아질수록 몬스터 체력과 공격력/방어력 증가 
    private int m_iCurMonCount = 0;

    private void Awake()
    {
        if (null == m_Instance)
        {
            m_Instance = this;

            LoadMonsterTable();

            //hierarchy에서 wander points 찾는다. 
            //몬스터에게 현재 타겟이 없을 경우 배열에 저장된 wander points를 순회하며 이동 
            WanderPoints = GameObject.FindGameObjectsWithTag("WanderPoint");

            //Resources 폴더에서 몬스터 프리팹 로드 
            m_MonsterPrefabs = Resources.LoadAll<Enemy>("Prefabs/Monsters");

            MakeBossMonsters();

            return;
        }

        Destroy(gameObject);
    }

    private void LoadMonsterTable()
    {
        //전체 텍스트 파일 불러오기 
        TextAsset textAsset = Resources.Load<TextAsset>("DefaultMonster");
        //불러온 텍스트 파일에서 줄 단위로 잘라서 배열에 저장 
        string[] lines = textAsset.text.Split('\n');

        foreach (var line in lines)
        {
            //줄 단위로 저장된 텍스트에서 띄어쓰기 단위로 잘라서 저장
            //인덱스 0: 공격력
            //1: 현재체력
            //2: 처치했을 때 얻을 수 있는 경험치
            //3: 처치했을 때 얻을 수 있는 골드 
            string[] stats = line.Split(' ');

            MonsterStatus monster = new MonsterStatus(int.Parse(stats[0]), int.Parse(stats[1]),
                int.Parse(stats[2]), int.Parse(stats[3]));

            m_MonsterTable.Add(monster);
        }
    }

    public void Init()
    {
        m_iCurMonCount = 0;
        BossIsSpawn = false;
        m_CurBoss = null;

        if (0 < Enemis.Count)
        {
            foreach (var mon in Enemis)
            {
                mon.gameObject.transform.position = Vector3.zero;
                mon.gameObject.SetActive(false);
            }
        }

        StartCoroutine(SpawnMonsters());
    }

    public void SetTableIndex(int index)
    {
        m_MonTableIndex = index;
    }

    public void SetMonsterLv(int stage)
    {
        if (0 == stage % 3) m_MonTableIndex++;
    }

    private IEnumerator SpawnMonsters()
    {
        while (GameManager.GetInstance.isRunning)
        {
            yield return new WaitForSeconds(m_fSpawnRate);

            if (Constants.MonCountMax > m_iCurMonCount)
                SpawnMonster();
        }
    }

    private Enemy MakeMonster()
    {
        //배열에 저장된 몬스터 중 랜덤으로 선별 후 객체 생성 
        int randomIndex = Random.Range(0, m_MonsterPrefabs.Length - 1);

        if (0 < m_MonsterPrefabs.Length)
        {
            var enemy = Instantiate(m_MonsterPrefabs[randomIndex]);
            //해당 스테이지의 웨이브를 모두 통과하면 보스 몬스터가 나옴
            //보스 몬스터를 처리해야 다음 스테이지로 갈 수 있음 
            if (enemy)
            {
                enemy.OnDeath += () => { m_iCurMonCount--; };
                Enemis.Add(enemy);
            }

            return enemy;
        }

        return null;
    }

    private void SpawnMonster()
    {
        if (0 >= m_SpawnPoints.Length)
            return;

        //리스트에서 비활성화 상태인 몬스터를 찾는다.
        var mon = Enemis.Find(obj => !obj.gameObject.activeSelf);

        //비활성화 상태인 몬스터가 없거나 있는게 보스몬스터면 새로 만든다. 
        if (!mon || 8 == mon.gameObject.layer)
            mon = MakeMonster();

        if (mon)
        {
            //monster spawn
            var pos_index = Random.Range(0, m_SpawnPoints.Length);
            var pos = m_SpawnPoints[pos_index].position + Vector3.up * 1.5f;

            //몬스터 능력치 설정 -> 스테이지 레벨에 따라 점점 높게 설정 
            mon.Setup(false, m_MonsterTable[m_MonTableIndex]);

            mon.Init(pos);
            m_iCurMonCount++;
        }
    }

    private void MakeBossMonsters()
    {
        if (0 < m_MonsterPrefabs.Length)
        {
            foreach (var prefab in m_MonsterPrefabs)
            {
                var boss = Instantiate(prefab);

                if (boss)
                {
                    //보스 몬스터는 2배 크게 만듦 
                    boss.transform.localScale *= 2f;
                    boss.OnDeath += () => { BossIsDead(); };
                    boss.OnDeath += () => { GameManager.GetInstance.SetClear(); };

                    //boss cam에서 culling mask 적용을 위한 layer 설정 
                    boss.gameObject.layer = 8; //boss layer 8번
                    Transform[] objs = boss.GetComponentsInChildren<Transform>();
                    foreach (var obj in objs)
                    {
                        //자식 오브젝트들의 레이어도 바꿔준다. 
                        obj.gameObject.layer = 8;
                    }

                    Enemis.Add(boss);
                }
            }
        }
    }

    public void SpawnBoss()
    {
        //Init함수에서 보스몬스터를 먼저 생성한 후 일반몬스터를 생성하기 때문에 0번~3번(현재 몬스터 종류 4가지)에는 항상 보스가 들어있다.
        int index = Random.Range(0, Constants.CountOfBoss);
        var pos = m_BossPoint.position + Vector3.up * 1.5f;

        //보스 몬스터 생성시엔 색깔 다르게 설정해 줌 
        Enemis[index].Setup(true, m_MonsterTable[m_MonTableIndex], m_BossColor);

        Enemis[index].Init(pos);
        m_CurBoss = Enemis[index];

        BossIsSpawn = true;
    }

    public void SetRunning(bool isRunning)
    {
        if (isRunning)
            StartCoroutine(SpawnMonsters());

        foreach (var mon in Enemis)
            mon.RunBehaviour(isRunning);

        if (m_CurBoss)
            m_CurBoss.RunBehaviour(isRunning);
    }

    public void KillMonsters()
    {
        foreach (var mon in Enemis)
            mon.Kill();
    }

    public void BossIsDead()
    {
        BossIsSpawn = false;
        m_CurBoss = null;
    }
}
