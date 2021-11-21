using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private string m_MoveVerticalName = "Vertical";
    private string m_MoveHorizontalName = "Horizontal";
    private string m_AttackButtonName = "Attack";

    private float m_fMoveVertical = 0; //세로방향 이동 입력받을 변수 
    private float m_fMoveHorizontal = 0; //가로방향 이동 입력받을 변수 

    //이동 방향 normalize까지 한 값을 리턴해서 playerMovement에서 사용 
    public Vector3 Dir { get { return new Vector3(m_fMoveHorizontal, 0, m_fMoveVertical).normalized; } }

    public bool Attack { get; private set; } = false;

    void Update()
    {
        if (GameManager.GetInstance.isRunning)
        {
            m_fMoveVertical = Input.GetAxis(m_MoveVerticalName);
            m_fMoveHorizontal = Input.GetAxis(m_MoveHorizontalName);
            Attack = Input.GetButtonDown(m_AttackButtonName);
        }
        else
        {
            m_fMoveHorizontal = default;
            m_fMoveVertical = default;
            Attack = false;
        }
    }
}
