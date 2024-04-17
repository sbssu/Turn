using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Playable : Character
{
    public override bool IsPlayable => true;

    public override void StartTurn(Action onEndTurn)
    {
        base.StartTurn(onEndTurn);

        target = FindObjectsOfType<Enemy>().OrderBy(x => x.ActionPoint).First();
        ControlPanel.Instance.SetTarget(target.transform);

        ControlPanel.Instance.onSkillQ = SkillQ;
        ControlPanel.Instance.onSkillE = SkillE;
        ControlPanel.Instance.onSkillR = SkillR;
    }
    protected override void EndTurn()
    {
        base.EndTurn();

        ControlPanel.Instance.SetTarget(null);

        ControlPanel.Instance.onSkillQ = null;
        ControlPanel.Instance.onSkillE = null;
        ControlPanel.Instance.onSkillR = null;
    }


    private void Update()
    {
        if (!isMyTurn || isLocked)
            return;

        // 내 차례일 때 타겟을 선택할 수 있다.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << LayerMask.NameToLayer("Enemy")))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    target = enemy;
                    ControlPanel.Instance.SetTarget(target.transform);
                }
            }
        }
    }
}
