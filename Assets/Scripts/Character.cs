using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected string characterName;    // 캐릭터 이름
    [SerializeField] protected Sprite sprite;           // 캐릭터 이미지
    [SerializeField] protected Status status;           // 기본 스테이터스.

    [Header("Properties")]
    public Transform lookAt;     // 바라볼 대상.

    protected float hp;          // 체력.
    protected bool isMyTurn;     // 내 차례인가?

    // 기본 스테이터스.
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
