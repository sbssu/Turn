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
    public Transform targetPivot;   // 타겟 피벗.

    protected float hp;             // 체력.
    protected bool isMyTurn;        // 내 턴인가?
    protected bool isLocked;        // 움직임 제한.

    protected float actionPoint;              // 행동력.
    protected Vector3 originPosition;         // 원래 위치.
    protected Action onEndTurn;               // 턴 종료시 실행할 함수.
    protected Animator anim;                  // 애니메이터.   


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
        anim = GetComponent<Animator>();

        ResetActionPoint();
    }        

    // 차례 조정.
    public virtual void StartTurn(Action onEndTurn)
    {
        this.onEndTurn = onEndTurn;

        isMyTurn = true;
        isLocked = false;
    }
    protected virtual void EndTurn()
    {
        onEndTurn?.Invoke();

        isMyTurn = false;
    }

    // 행동.
    public void OnHitTargetEvent()
    {

    }

    Coroutine moveCoroutine;
    protected void GoToPosition(Character target)
    {  
        if(moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        Vector3 destination = originPosition;
        if (target != null)
            destination = target.transform.position + target.transform.forward * 2.5f;
        moveCoroutine = StartCoroutine(IEGoToPosition(destination));
    }
    private IEnumerator IEGoToPosition(Vector3 destination)
    {
        while(transform.position != destination)
        {
            transform.position = Vector3.Lerp(transform.position, destination, 20f * Time.deltaTime);
            yield return null;
        }
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
        anim.SetTrigger("onAttack");
    }
    protected virtual void SkillE()
    {
        if (isLocked)
            return;

        isLocked = true;
    }
    protected virtual void SkillR()
    {
        if (isLocked)
            return;

        isLocked = true;
    }

    protected virtual void Hit()
    {
        target.Damage();

        FxManager.Instance.PlayFx("Hit", target.transform.position + Vector3.up * 1.2f);
        LookAtCamera.Instance.ShakeCamera();
    }
    protected virtual async void EndHit()
    {
        GoToPosition(null);
        await Task.Delay(300);
        isLocked = false;
        EndTurn();
    }
    protected virtual void Damage()
    {
        anim.SetTrigger("onDamage");
    }
}
