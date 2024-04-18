using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    public static ControlPanel Instance { get; private set; }

    [SerializeField] RectTransform targetRect;

    public Action onSkillQ;
    public Action onSkillE;
    public Action onSkillR;

    private void Awake()
    {
        Instance = this;
    }

    Transform target;
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    private void Update()
    {
        targetRect.gameObject.SetActive(target);
        if (target != null)
            targetRect.position = Camera.main.WorldToScreenPoint(target.position);
    }

    public void OnSKillQ()
    {
        onSkillQ?.Invoke();
    }
    public void OnSKillE()
    {
        onSkillE?.Invoke();
    }
    public void OnSKillR()
    {
        onSkillR?.Invoke();
    }
}
