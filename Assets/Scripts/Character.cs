using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected string characterName;    // ĳ���� �̸�
    [SerializeField] protected Sprite sprite;           // ĳ���� �̹���
    [SerializeField] protected Status status;           // �⺻ �������ͽ�.

    [Header("Properties")]
    public Transform camPivot;      // ī�޶� �ǹ�.
    public Transform targetPivot;   // Ÿ�� �ǹ�.

    protected float hp;             // ü��.
    protected bool isMyTurn;        // �� ���ΰ�?
    protected bool isLocked;        // ������ ����.

    protected float actionPoint;              // �ൿ��.
    protected Vector3 originPosition;         // ���� ��ġ.
    protected Action onEndTurn;               // �� ����� ������ �Լ�.
    protected Animator anim;                  // �ִϸ�����.   


    // �⺻ �������ͽ�.
    public float Hp => hp;
    public float Speed => status.speed;
    public float ActionPoint => actionPoint;
    public Sprite CharacterSprite => sprite;
    public abstract bool IsPlayable { get; }


    protected Character target;               // Ÿ��.

    private void Start()
    {
        hp = status.hp;
        originPosition = transform.position;
        anim = GetComponent<Animator>();

        ResetActionPoint();
    }        

    // ���� ����.
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

    // �ൿ.
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

    // �׼� ����Ʈ.
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
