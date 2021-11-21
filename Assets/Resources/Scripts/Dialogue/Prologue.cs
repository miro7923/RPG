using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Prologue : MonoBehaviour
{
    [SerializeField] private Dialogue m_Dialogue; //재생할 전체 다이얼로그 정보를 가진 인스턴스 

    [SerializeField] private Text m_Text; //텍스트 오브젝트 
    [SerializeField] private Image m_CharacterImg; //대사별 출력할 캐릭터 이미지 

    private List<string> m_Sentences = new List<string>(); //전체 대사 배열 
    private List<Sprite> m_Sprites = new List<Sprite>(); //전체 캐릭터 이미지 배열 

    private int m_Count = 0; //현재 출력할 대사 위치 

    private bool isTalking = false; //
    private bool DialogueIsEnd = false;

    [SerializeField] private Image m_BlackInPanel;

    private void Update()
    {
        if (isTalking && Input.anyKeyDown)
        {
            AudioManager.GetInstance.PlaySfx(SCENE.Prologue);

            if (DialogueIsEnd)
            {
                //입력이 들어왔는데 텍스트 출력이 끝났다면 다음 텍스트를 출력
                m_Count++;
                m_Text.text = "";

                if (m_Count == m_Sentences.Count)
                {
                    //모든 다이얼로그가 출력되었다면 
                    StopAllCoroutines();
                    ExitDialogue();
                    //게임씬 호출
                    StartCoroutine(LoadPlayScene());
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(DialogueCoroutine());
                }
            }
            else
            {
                //입력이 들어왔는데 텍스트 출력이 끝나지 않았으면 모든 텍스트 출력 후 멈춤 
                StopAllCoroutines();
                ShowDialogueToTheEnd();
            }
        }
    }

    public void ShowDialogue()
    {
        isTalking = true;

        for (int i = 0; m_Dialogue.Sentences.Length > i; i++)
        {
            m_Sentences.Add(m_Dialogue.Sentences[i]);
            m_Sprites.Add(m_Dialogue.Sprites[i]);
        }

        StartCoroutine(DialogueCoroutine());
    }

    public void ExitDialogue()
    {
        m_Count = 0;
        m_Text.text = "";

        m_Sentences.Clear();
        m_Sprites.Clear();

        isTalking = false;
    }

    private IEnumerator DialogueCoroutine()
    {
        isTalking = true;
        DialogueIsEnd = false;
        m_Text.text = "";

        if (0 < m_Count)
        {
            if (m_Sprites[m_Count] != m_Sprites[m_Count - 1])
            {
                m_CharacterImg.sprite = m_Sprites[m_Count];

                yield return new WaitForSeconds(0.1f);
            }
            else
                yield return new WaitForSeconds(0.05f);
        }
        else
            m_CharacterImg.sprite = m_Sprites[m_Count];

        StringBuilder sb = new StringBuilder();
        for (int i = 0; m_Sentences[m_Count].Length > i; i++)
        {
            sb.Append(m_Sentences[m_Count][i]);
            m_Text.text = sb.ToString();

            yield return new WaitForSeconds(0.01f);
        }

        DialogueIsEnd = true;
    }

    private void ShowDialogueToTheEnd()
    {
        m_Text.text = m_Sentences[m_Count];
        DialogueIsEnd = true;
    }

    private IEnumerator LoadPlayScene()
    {
        float alpha = m_BlackInPanel.color.a;
        while (1 > alpha)
        {
            alpha += Time.deltaTime * 2f;
            yield return null;
            m_BlackInPanel.color = new Color(0, 0, 0, alpha);
        }

        yield return new WaitForSeconds(0.5f);

        GameManager.GetInstance.LoadPlayScene();
    }
}
