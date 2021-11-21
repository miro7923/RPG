using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance = null;
    public static GameManager GetInstance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<GameManager>();
                if (!m_Instance) m_Instance = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    private float m_StageRoot = 0;
    public int Stage { get; private set; } = 1; //한 스테이지는 3웨이브로 구성되어 있음
    //한 웨이브당 몬스터 5마리 죽이면 웨이브 증가 -> 3번째 웨이브가 되면 컷신 재생 후 보스몬스터 출현
    public int Wave { get; private set; } = 1; 
    private bool isClear = false;
    private int m_iKillingCount = 0;
    private Camera m_BossCam;

    private SCENE m_eCurScene = SCENE.Play;

    private PlayerHealth m_PlayerHealth;
    private Shop m_Shop;
    private Watermelon m_Watermelon;

    public bool isRunning { get; private set; } = false;

    //메인 캔버스: 게임 진행에 필요한 ui 출력(boss cam 활성화되면 canvas 컴포넌트 비활성화)
    //서브 캔버스: 페이드 인/아웃 효과 출력 
    private SubCanvas m_SubCanvas;

    private void Awake()
    {
        if (!m_Instance)
            SceneManager.sceneLoaded += OnSceneLoaded;

        if (this != GetInstance)
            Destroy(gameObject);
    }

    private void Update()
    {
        switch (m_eCurScene)
        {
            case SCENE.Main:
                break;
            case SCENE.Prologue:
                break;
            case SCENE.Play:
                switch (isClear)
                {
                    case true:
                        if (!m_PlayerHealth.Dead && !m_SubCanvas.gameObject.activeSelf)
                        {
                            MonsterManager.GetInstance.SetRunning(false);
                            MonsterManager.GetInstance.KillMonsters();
                            isRunning = false;

                            ToggleCanvas(false);

                            StartCoroutine(m_SubCanvas.ShowClearUI(m_StageRoot));
                            PlayerState.GetInstance.GetGold(m_StageRoot);
                            Stage++;
                            MonsterManager.GetInstance.SetMonsterLv(Stage);
                        }
                        break;
                    case false:
                        if ((m_PlayerHealth && m_PlayerHealth.Dead) || (m_Watermelon && m_Watermelon.Dead))
                        {
                            //실행중인 모든 코루틴 종료 
                            StopAllCoroutines();

                            //main canvas 끄고 sub canvas 켜기 
                            ToggleCanvas(false);
                            //game over 텍스트 출력 
                            m_SubCanvas.ToggleGameOverTxt(true);
                            isRunning = false;

                            //키 입력 받으면 메인화면 로드
                            if (Input.anyKeyDown)
                                m_SubCanvas.StartMovingAnim(true);
                        }

                        if (Constants.MaxKill <= m_iKillingCount && Constants.MaxWave > Wave)
                        {
                            Wave++;
                            MainCanvas.GetInstance.UpdateWave(Wave);
                            m_iKillingCount -= Constants.MaxKill;
                        }

                        if (Constants.MaxWave == Wave && !MonsterManager.GetInstance.BossIsSpawn)
                            StartCoroutine(ShowBoss());
                        break;
                }
                break;
            case SCENE.Shop:
                break;
        }
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene("rpgpp_lt_scene_1.0");
    }

    public void LoadPrologue()
    {
        SceneManager.LoadScene("Prologue");
    }

    public void LoadShop()
    {
        //shop button 누르면 호출
        SceneManager.LoadScene("Blacksmith's Forge");
    }

    public void LoadMainMenu()
    {
        //Exit button 누르면 호출
        SceneManager.LoadScene("Main Menu");
    }

    private void SetBossCam()
    {
        if (!m_BossCam)
        {
            GameObject camObj = GameObject.FindGameObjectWithTag("BossCam");
            m_BossCam = camObj.GetComponent<Camera>();
            m_BossCam.gameObject.SetActive(false);
        }
    }

    public void SetPlayerHealth(PlayerHealth playerHealth)
    {
        m_PlayerHealth = playerHealth;
    }

    public void SetShop(Shop shop)
    {
        m_Shop = shop;
    }

    public void SetSubCanvas(SubCanvas subCanvas)
    {
        m_SubCanvas = subCanvas;
    }

    public void SetWatermelon(Watermelon watermelon)
    {
        m_Watermelon = watermelon;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Scene curScene = SceneManager.GetActiveScene();
        switch (curScene.name)
        {
            case "Main Menu":
                m_eCurScene = SCENE.Main;
                break;
            case "Prologue":
                m_eCurScene = SCENE.Prologue;
                break;
            case "rpgpp_lt_scene_1.0":
                m_eCurScene = SCENE.Play;
                SetBossCam();
                InitPlayObjects();
                PlayWaitingAnim();
                break;
            case "Blacksmith's Forge":
                m_eCurScene = SCENE.Shop;
                if (m_Shop)
                    m_Shop.Init();
                break;
        }

        AudioManager.GetInstance.PlayBgm(m_eCurScene);
    }

    public void InitPlayObjects()
    {
        if (m_PlayerHealth)
        {
            m_PlayerHealth.Init();
            PlayerState.GetInstance.Init();
        }

        WeaponManager.GetInstance.Init();

        if (m_Watermelon)
            m_Watermelon.Init();
    }

    public void PlayWaitingAnim()
    {
        ToggleCanvas(false);
        m_SubCanvas.UpdateStageTxt(Stage);
        m_SubCanvas.ToggleStateTxt(true);
    }

    public void PlaySceneInit()
    {
        ToggleCanvas(true);

        Wave = 1;
        m_iKillingCount = 0;
        m_StageRoot = 0;

        isClear = false;
        isRunning = true;

        MainCanvas.GetInstance.UpdateWave(Wave);

        int count = PlayerState.GetInstance.PlayerItems.Find((obj) => obj.Key.Sort.Equals(ITEM_SORT.Hp)).Value;
        MainCanvas.GetInstance.ShowPotionCount(ITEM_SORT.Hp, count);
        count = PlayerState.GetInstance.PlayerItems.Find((obj) => obj.Key.Sort.Equals(ITEM_SORT.Mp)).Value;
        MainCanvas.GetInstance.ShowPotionCount(ITEM_SORT.Mp, count);

        MonsterManager.GetInstance.Init();
    }

    public void AddKillingCount()
    {
        m_iKillingCount++;
    }

    private IEnumerator ShowBoss()
    {
        isRunning = false;
        MonsterManager.GetInstance.SpawnBoss();
        ToggleCanvas(false);

        yield return m_SubCanvas.BlackIn();

        m_BossCam.gameObject.SetActive(true); //Boss cam on

        yield return m_SubCanvas.BlackOut();

        yield return MainCanvas.GetInstance.ShowAppearBossTxt();

        yield return new WaitForSeconds(2f); //Show boss 

        yield return m_SubCanvas.BlackIn();

        m_BossCam.gameObject.SetActive(false); //Boss cam off
        ToggleCanvas(true);

        yield return m_SubCanvas.BlackOut();

        isRunning = true;
        MonsterManager.GetInstance.SetRunning(true);
    }

    public void SetClear()
    {
        isClear = true;
    }

    public void AddGold(float gold)
    {
        m_StageRoot += gold;
    }

    private void ToggleCanvas(bool on)
    {
        MainCanvas.GetInstance.Canvas.enabled = on;
        m_SubCanvas.gameObject.SetActive(!on);
    }
}
