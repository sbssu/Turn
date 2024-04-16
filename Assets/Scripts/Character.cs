using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected string characterName;    // ĳ���� �̸�
    [SerializeField] protected Sprite sprite;           // ĳ���� �̹���
    [SerializeField] protected Status status;           // �⺻ �������ͽ�.

    [Header("Properties")]
    public Transform lookAt;     // �ٶ� ���.

    protected float hp;          // ü��.
    protected bool isMyTurn;     // �� �����ΰ�?

    // �⺻ �������ͽ�.
    public string Name => characterName;
    public float Hp => hp;
    public float Attack => status.attack;
    public float Defense => status.defense;
    public float Speed => status.speed;
    public Sprite CharacterSprite => sprite;

    private void Start()
    {
        hp = status.hp;
    }

    public void StartTurn()
    {
        isMyTurn = true;
    }
    public void EndTurn()
    {
        isMyTurn = false;
        GameManager.Instance.EndTurn();
    }

    public abstract bool IsPlayable { get; }
    public abstract bool SkillQ();
    public abstract bool SkillE();
    public abstract bool SkillR();
}
