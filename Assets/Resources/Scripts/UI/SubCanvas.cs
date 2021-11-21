using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubCanvas : MonoBehaviour
{
    [SerializeField] private Image m_FadeInOutPanel; //페이드 인 아웃 때 사용할 패널 이미지 

    [SerializeField] private GameObject m_StateTxts;
    [SerializeField] private Text m_StageTxt; //게임 시작 전에 보여주는 스테이지 정보 

    [SerializeField] private GameObject m_Popup_ContinueOrNot; //다음 스테이지로 진행할 것인지 아닌지 확인하는 팝업 

    [SerializeField] private Text m_ClearTxt; //스테이지 클리어 텍스트 

    [SerializeField] private Text m_GameOverTxt; //게임오버 텍스트 

    private void Awake()
    {
        GameManager.GetInstance.SetSubCanvas(this);
        ToggleGameOverTxt(false);
    }

    public IEnumerator BlackOut()
    {
        float alpha = m_FadeInOutPanel.color.a;
        while (0 < alpha)
        {
            alpha -= Time.deltaTime * 2f;
            yield return null;
            m_FadeInOutPanel.color = new Color(0, 0, 0, alpha);
        }

        m_FadeInOutPanel.gameObject.SetActive(false);
    }

    public IEnumerator BlackIn()
    {
        m_FadeInOutPanel.gameObject.SetActive(true);

        float alpha = m_FadeInOutPanel.color.a;
        while (1 > alpha)
        {
            alpha += Time.deltaTime * 2f;
            yield return null;
            m_FadeInOutPanel.color = new Color(0, 0, 0, alpha);
        }
    }

    public void ToggleStateTxt(bool on)
    {
        m_StateTxts.SetActive(on);
    }

    public void UpdateStageTxt(int stage)
    {
        m_StageTxt.text = string.Format("Stage {0}", stage);
    }

    public IEnumerator ShowClearUI(float gold)
    {
        m_ClearTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        m_ClearTxt.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        m_ClearTxt.text = string.Format("클리어 보상으로 {0}G가 지급 되었어요.", gold);
        m_ClearTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        m_ClearTxt.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        m_Popup_ContinueOrNot.SetActive(true);
        m_ClearTxt.text = "Stage Clear!!";
    }

    private IEnumerator BlackInOut()
    {
        yield return BlackIn();

        GameManager.GetInstance.InitPlayObjects();

        yield return BlackOut();

        GameManager.GetInstance.PlayWaitingAnim();
    }

    public void PlayStartAnim()
    {
        StartCoroutine(BlackInOut());
    }

    public void ToggleGameOverTxt(bool on)
    {
        switch (on)
        {
            case true:
                if (!m_GameOverTxt.gameObject.activeSelf)
                    m_GameOverTxt.gameObject.SetActive(true);
                break;
            case false:
                m_GameOverTxt.gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator MovingAnim(bool isMain)
    {
        //black in 후 isMain값에 따라 메인화면으로 이동하거나 상점으로 이동
        yield return BlackIn();

        switch (isMain)
        {
            case true:
                GameManager.GetInstance.LoadMainMenu();
                break;
            case false:
                GameManager.GetInstance.LoadShop();
                break;
        }
    }

    public void StartMovingAnim(bool isMain)
    {
        //팝업창에서 yes/no 버튼 누르면 호출 
        StartCoroutine(MovingAnim(isMain));
    }
}
