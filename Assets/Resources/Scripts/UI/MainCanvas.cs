using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MainCanvas : MonoBehaviour
{
    private static MainCanvas m_Instance = null;
    public static MainCanvas GetInstance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<MainCanvas>();
                if (!m_Instance) m_Instance = new GameObject("Main Canvas").AddComponent<MainCanvas>();
            }
            return m_Instance;
        }
    }

    public Canvas Canvas { get; set; }

    [SerializeField] private Slider m_ExpSlider;
    [SerializeField] private Text m_LvText;

    [SerializeField] private Slider m_HpSlider;
    [SerializeField] private Slider m_MpSlider;

    [SerializeField] private Button[] m_SkillIcons; //스킬 버튼 아이콘들을 저장할 배열. 0:Sword, 1:Hammer, 2:Wand
    [SerializeField] private Image[] m_SkillIconMasks; //스킬 버튼의 쿨타임 마스크들을 저장할 배열 

    [SerializeField] private Text[] m_PotionCountTxts; //물약 갯수 보여줄 텍스트 배열. 인덱스 0: hp/ 1: mp
    [SerializeField] private Image[] m_PotionMasks;
    [SerializeField] private Button[] m_PotionButtons;

    [SerializeField] private Text m_WaveTxt;

    [SerializeField] private Text m_AppearBossTxt;

    [SerializeField] private Slider m_WatermelonHpBar;

    private void Awake()
    {
        if (this != GetInstance) Destroy(gameObject);

        Canvas = GetComponent<Canvas>();

        //로드시 스킬 아이콘들은 일괄 비활성화시켜둔 후 현재 사용할 스킬이 결정되면 해당 아이콘만 활성화시킴 
        for (int i = 0; m_SkillIcons.Length > i; i++)
        {
            m_SkillIcons[i].gameObject.SetActive(false);
            m_SkillIconMasks[i].gameObject.SetActive(false);
        }

        foreach (var mask in m_PotionMasks)
            mask.gameObject.SetActive(false);

        m_AppearBossTxt.gameObject.SetActive(false);

        //시작할 땐 메인 캔버스는 꺼 놓고 서브 캔버스를 켜 놓음 (대기 애니메이션 출력용)
        Canvas.enabled = false;
    }

    public void UpdateLvText(int lv)
    {
        m_LvText.text = string.Format("Lv. {0}", lv);
    }

    public void InitExpSlider(float maxVal, float minVal)
    {
        m_ExpSlider.maxValue = maxVal;
        m_ExpSlider.minValue = minVal;
        m_ExpSlider.value = minVal;
    }

    public void UpdateExpValue(float exp)
    {
        m_ExpSlider.value = exp;
    }

    public void InitHealthSlider(float hp, float mp)
    {
        m_HpSlider.maxValue = hp;
        m_HpSlider.value = hp;

        m_MpSlider.maxValue = mp;
        m_MpSlider.value = mp;
    }

    public void UpdateHpSlider(float hp)
    {
        m_HpSlider.value = hp;
    }

    public void UpdateMpSlider(float mp)
    {
        m_MpSlider.value = mp;
    }

    public void SetSkillIcon(ITEM_SORT eSort)
    {
        int index = 0;
        switch (eSort)
        {
            case ITEM_SORT.Sword:
                index = 0;
                break;
            case ITEM_SORT.Hammer:
                index = 1;
                break;
            case ITEM_SORT.Wand:
                index = 2;
                break;
        }

        m_SkillIcons[index].gameObject.SetActive(true);
    }

    public IEnumerator ActivateCoolTimeIcon(ITEM_SORT eSort, float coolTime)
    {
        int index = 0;
        Image[] masks = null;
        Button[] buttons = null;
        switch (eSort)
        {
            case ITEM_SORT.Sword:
            case ITEM_SORT.Hammer:
            case ITEM_SORT.Wand:
                masks = m_SkillIconMasks;
                buttons = m_SkillIcons;
                index = ((int)eSort) - 1;
                break;
            case ITEM_SORT.Hp:
            case ITEM_SORT.Mp:
                masks = m_PotionMasks;
                buttons = m_PotionButtons;
                index = ((int)eSort) - 4;
                break;
        }

        //쿨타임이 시작되면 아이콘 마스크 활성화 & fillAmount 1로 바꿔줌 (1 -> 0으로 줄어들 것)
        masks[index].gameObject.SetActive(true);
        masks[index].fillAmount = 1;
        //스킬 아이콘 클릭 못 하게 비활성화 
        buttons[index].interactable = false;

        //fillAmount가 0이 될 때까지 감소시킨 후 
        while (0 < masks[index].fillAmount)
        {
            masks[index].fillAmount
                = Mathf.Clamp01(masks[index].fillAmount - Time.deltaTime * (1 / coolTime));

            yield return null;
        }

        //완료되면 아이콘 마스크 비활성화 
        masks[index].gameObject.SetActive(false);
        //스킬 아이콘 활성화시켜서 클릭 가능하게 바꿈 
        buttons[index].interactable = true;
    }

    public void ShowPotionCount(ITEM_SORT eSort, int count)
    {
        switch (eSort)
        {
            case ITEM_SORT.Hp:
                m_PotionCountTxts[0].text = count.ToString();
                break;
            case ITEM_SORT.Mp:
                m_PotionCountTxts[1].text = count.ToString();
                break;
        }
    }

    public void UpdateWave(int num)
    {
        m_WaveTxt.text = string.Format("Wave {0}", num);
    }

    public IEnumerator ShowAppearBossTxt()
    {
        yield return new WaitForSeconds(0.5f);

        m_AppearBossTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        m_AppearBossTxt.gameObject.SetActive(false);
    }

    public void InitWatermelonHpBar(float maxValue)
    {
        m_WatermelonHpBar.maxValue = maxValue;
        m_WatermelonHpBar.value = maxValue;
    }

    public void UpdateWatermelonHpBar(float value)
    {
        m_WatermelonHpBar.value = value;
    }
}
