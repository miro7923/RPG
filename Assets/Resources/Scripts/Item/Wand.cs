using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : Weapon
{
    private LineRenderer m_LineRenderer;

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.positionCount = 2;
        m_LineRenderer.enabled = false;
    }

    public override IEnumerator AttackEffect(Vector3 hitPos)
    {
        yield return new WaitForSeconds(0.03f);

        //선의 시작점은 무기 위치 
        m_LineRenderer.SetPosition(0, transform.position);
        //선의 끝점은 target 위치 
        m_LineRenderer.SetPosition(1, hitPos);
        //라인 렌더러 활성화해서 궤적 그리기 
        m_LineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f);

        //처리 시간이 지나면 비활성화해서 궤적을 지운다. 
        m_LineRenderer.enabled = false;
    }
}
