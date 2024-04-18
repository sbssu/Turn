using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public override bool IsPlayable => false;

    public override void StartTurn(System.Action onEndTurn)
    {
        base.StartTurn(onEndTurn);
        Character[] targets = FindObjectsOfType<Playable>();
        target = targets[Random.Range(0, targets.Length)];
        LookAtCamera.Instance.SetPivot(target.camPivot, transform);
        StartCoroutine(IEUpdate());
    }

    IEnumerator IEUpdate()
    {
        yield return new WaitForSeconds(1f);
        SkillQ();
    }
}
