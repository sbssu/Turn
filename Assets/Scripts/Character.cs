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
    public Transform lookAt;        // �ٶ� ���.
    public GameObject turnObject;

    protected float hp;             // ü��.
    protected bool isMyTurn;        // �� ���ΰ�?
    protected bool isLocked;        // ������ ����.

    float actionPoint;              // �ൿ��.
    Vector3 originPosition;         // ���� ��ġ.
    Action onEndTurn;               // �� ����� ������ �Լ�.


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
        turnObject.SetActive(false);

        ResetActionPoint();
    }        

    // ���� ����.
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

    // �ൿ.
    public void OnHitTargetEvent()
    {

    }
    protected void GoToPosition(Character target)
    {
        transform.position = (target == null) ? originPosition : target.originPosition + target.transform.forward * 1.5f;
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
