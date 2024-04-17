using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected string characterName;    // 캐릭터 이름
    [SerializeField] protected Sprite sprite;           // 캐릭터 이미지
    [SerializeField] protected Status status;           // 기본 스테이터스.

    [Header("Properties")]
    public Transform camPivot;      // 카메라 피벗.
    public Transform lookAt;        // 바라볼 대상.
    public GameObject turnObject;

    protected float hp;             // 체력.
    protected bool isMyTurn;        // 내 턴인가?
    protected bool isLocked;        // 움직임 제한.

    float actionPoint;              // 행동력.
    Vector3 originPosition;         // 원래 위치.
    Action onEndTurn;               // 턴 종료시 실행할 함수.


    // 기본 스테이터스.
    public float Hp => hp;
    public float Speed => status.speed;
    public float ActionPoint => actionPoint;
    public Sprite CharacterSprite => sprite;
    public abstract bool IsPlayable { get; }


    protected Character target;               // 타겟.

    private void Start()
    {
        hp = status.hp;
        originPosition = transform.position;
        turnObject.SetActive(false);

        ResetActionPoint();
    }        

    // 차례 조정.
    public virtual void StartTurn(Action onEndTurn)
    {
        turnObject.SetActive(true);
        this.onEndTurn = onEndTurn;

        isMyTurn = true;
        isLocked = false;
    }
    protected virtual void EndTurn()
    {
        turnObject.SetActive(false);
        onEndTurn?.Invoke();

        isMyTurn = false;
    }

    // 행동.
    public void OnHitTargetEvent()
    {

    }
    protected void GoToPosition(Character target)
    {
        transform.position = (target == null) ? originPosition : target.originPosition + target.transform.forward * 1.5f;
    }

    // 액션 포인트.
    public void DoActionPoint(Character actor)
    {
        actionPoint -= actor.actionPoint;
    }
    public void ResetActionPoint()
    {
        actionPoint = 10000f / Speed;
    }

    protected virtual async void SkillQ()
    {
        if (isLocked)
            return;

        isLocked = true;

        GoToPosition(target);
        await Task.Delay(200);
        await Task.Delay(400);
        GoToPosition(null);
        await Task.Delay(400);

        EndTurn();
    }
    protected virtual async void SkillE()
    {
        if (isLocked)
            return;

        isLocked = true;

        await Task.Yield();        
    }
    protected virtual async void SkillR()
    {
        if (isLocked)
            return;

        isLocked = true;

        await Task.Yield();        
    }
}
