using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateText : MonoBehaviour
{
    public void CallPlaySceneInit()
    {
        gameObject.SetActive(false);
        GameManager.GetInstance.PlaySceneInit();
    }
}
