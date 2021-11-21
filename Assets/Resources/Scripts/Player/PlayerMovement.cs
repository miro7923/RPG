using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput m_PlayerInput;
    private Rigidbody m_PlayerRigidbody;
    private Animator m_PlayerAnimator;

    private float m_fMoveSpeed = 5f;
    private float m_fRatateSpeed = 180f;

    public bool CanMove { get; set; }

    void Start()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_PlayerRigidbody = GetComponent<Rigidbody>();
        m_PlayerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (CanMove)
        {
            Move();
            UpdateAnim();
            Rotate();
        }
    }

    private void UpdateAnim()
    {
        m_PlayerAnimator.SetFloat("Speed", m_PlayerInput.Dir.magnitude);
    }

    private void Move()
    {
        m_PlayerRigidbody.MovePosition(transform.position + m_PlayerInput.Dir * m_fMoveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        if (0 == m_PlayerInput.Dir.magnitude)
            return;

        Quaternion rotation = Quaternion.LookRotation(m_PlayerInput.Dir);

        m_PlayerRigidbody.rotation = Quaternion.Slerp(m_PlayerRigidbody.rotation,
            rotation, m_fRatateSpeed * Time.deltaTime);
    }

    public void MakeMove()
    {
        CanMove = true;
    }
}
