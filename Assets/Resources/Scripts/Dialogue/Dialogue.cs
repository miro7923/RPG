using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(1, 2)] //두 줄 입력 가능하게 텍스트박스 늘림 
    public string[] Sentences; //대사 
    public Sprite[] Sprites; //캐릭터 이미지 
    public Sprite[] DialogueWindow; //대화창 배경 이미지 
}
