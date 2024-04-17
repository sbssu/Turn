using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/*
 * ��Ģ
1.�ʻ�� ���� �ֿ켱������ �� �Ҹ�
2.�Ϲݰ��ݽ� ���� �ڽ�Ʈ ȸ��
3.Ư�� ���ݽ� ���� �ڽ�Ʈ ���
4.����&�ǰݽ� �ʻ�� ������ ȸ��
5.���ݿ��� Ÿ��orȸ��or����
6.���� �ӵ��� ���� ���ư���.
 */

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    List<Character> turnQueue;     // ������ ���� ť.
    TurnUIManager turnUIManager;    // �� UI �Ŵ���.

    GAME_PHASE gamePhase;           // ���� ����.
    int skillCost;                  // ���� ��ų �ڽ�Ʈ.

    Character current;      // ���� ���� ĳ����.
    bool isPlayable;        // �÷��̾� �����ΰ�?

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // ��� ĳ���͸� ã�� �� ���ǵ� ������ �������� ����.
        turnQueue = FindObjectsOfType<Character>().OrderByDescending(x => x.Speed).ToList();

        turnUIManager = TurnUIManager.Instance;
        turnUIManager.Setup(turnQueue.ToArray());

        current = turnQueue[0];
        StartTurn();
    }
  
    public void StartTurn()
    {
        isPlayable = current.IsPlayable;        // �÷��̾� �������� Ȯ��.
        current = turnQueue[0];                 // ���� ĳ���͸� ����.
        gamePhase = isPlayable ? GAME_PHASE.INPUT : GAME_PHASE.ENEMY;  

        // ���� �����Ѵ�.
        current.StartTurn(EndTurn);
    }
    private async void EndTurn()
    {
        if(await NextTurn())
            StartTurn();
    }
    private async Task<bool> NextTurn()
    {
        // ���� ĳ���ʹ� ť���� �����ϸ� UI�󿡼��� �����Ѵ�.
        var dead = turnQueue.Where(x => x.Hp <= 0).ToList();
        foreach (var target in dead)
            turnUIManager.Dead(target);

        // ���� ĳ���Ͱ� ������ ���ӿ��� �Ǵ� Ŭ����.
        turnQueue = turnQueue.Except(dead).ToList();
        int playableCount = turnQueue.Count(x => x.IsPlayable);  // �÷��̾� ĳ������ ��.
        int enemyCount = turnQueue.Count(x => !x.IsPlayable);    // �� ĳ������ ��.
        if (playableCount == 0 || enemyCount == 0)
            return false;

        // ���ʰ� ���� ĳ���� �г� ����.
        turnUIManager.SwitchLock(true);
        turnUIManager.Remove(current, true);

        // ���� ���� ���.
        turnQueue.Remove(current);                          // ť���� ����.
        turnQueue.ForEach(x => x.DoActionPoint(current));   // �׼� ����Ʈ�� �����Ų��.                

        current.ResetActionPoint();                         // �׼� ����Ʈ�� �ʱ�ȭ�Ѵ�.
        turnQueue.Add(current);                             // �ٽ� ť�� �ִ´�.
        turnQueue.OrderBy(x => x.ActionPoint);              // ���� ����Ʈ�� ���� ����.

        await Task.Delay(500);                              // 0.5�� ���.
        turnUIManager.Add(current, -1, true);               // ���� ������ ĳ���� �г� �߰�.
        turnUIManager.SwitchLock(false);                    // �� �г��� ��� ����.
        return true;
    }


    private void OnGUI()
    {
        GUIStyle layout = new GUIStyle() { fontSize = 50 };
        GUI.Label(new Rect(200, 0, 1000, 50), $"Phase : {gamePhase}", layout);
        GUI.Label(new Rect(200, 50, 1000, 50), $"Current : {current.name}", layout);
    }

    private enum GAME_PHASE
    {
        PAUSE,      // ����.
        ENEMY,      // �� ����.
        INPUT,      // �÷��̾� �Է�.
        PROCESS,    // �Է¿� ���� ó��.
        ENDGAME,    // ��.
    }
}
