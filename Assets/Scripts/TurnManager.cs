using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/*
 * 규칙
1.필살기 사용시 최우선적으로 턴 소모
2.일반공격시 공용 코스트 회복
3.특수 공격시 공용 코스트 사용
4.공격&피격시 필살기 게이지 회복
5.공격에는 타격or회복or버프
6.턴은 속도에 따라서 돌아간다.
 */

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    List<Character> turnQueue;     // 순서에 따른 큐.
    TurnUIManager turnUIManager;    // 턴 UI 매니저.

    GAME_PHASE gamePhase;           // 게임 상태.
    int skillCost;                  // 공용 스킬 코스트.

    Character current;      // 현재 턴인 캐릭터.
    bool isPlayable;        // 플레이어 차례인가?

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // 모든 캐릭터를 찾은 뒤 스피드 값으로 내림차순 정렬.
        turnQueue = FindObjectsOfType<Character>().OrderByDescending(x => x.Speed).ToList();

        turnUIManager = TurnUIManager.Instance;
        turnUIManager.Setup(turnQueue.ToArray());

        current = turnQueue[0];
        StartTurn();
    }
  
    public void StartTurn()
    {
        isPlayable = current.IsPlayable;        // 플레이어 차례인지 확인.
        current = turnQueue[0];                 // 현재 캐릭터를 설정.
        gamePhase = isPlayable ? GAME_PHASE.INPUT : GAME_PHASE.ENEMY;  

        // 턴을 진행한다.
        current.StartTurn(EndTurn);
    }
    private async void EndTurn()
    {
        if(await NextTurn())
            StartTurn();
    }
    private async Task<bool> NextTurn()
    {
        // 죽은 캐릭터는 큐에서 제거하며 UI상에서도 제거한다.
        var dead = turnQueue.Where(x => x.Hp <= 0).ToList();
        foreach (var target in dead)
            turnUIManager.Dead(target);

        // 남은 캐릭터가 없으면 게임오버 또는 클리어.
        turnQueue = turnQueue.Except(dead).ToList();
        int playableCount = turnQueue.Count(x => x.IsPlayable);  // 플레이어 캐릭터의 수.
        int enemyCount = turnQueue.Count(x => !x.IsPlayable);    // 적 캐릭터의 수.
        if (playableCount == 0 || enemyCount == 0)
            return false;

        // 차례가 끝난 캐릭터 패널 삭제.
        turnUIManager.SwitchLock(true);
        turnUIManager.Remove(current, true);

        // 다음 차례 계산.
        turnQueue.Remove(current);                          // 큐에서 제거.
        turnQueue.ForEach(x => x.DoActionPoint(current));   // 액션 포인트를 진행시킨다.                

        current.ResetActionPoint();                         // 액션 포인트를 초기화한다.
        turnQueue.Add(current);                             // 다시 큐에 넣는다.
        turnQueue.OrderBy(x => x.ActionPoint);              // 남은 포인트에 따라 정렬.

        await Task.Delay(500);                              // 0.5초 대기.
        turnUIManager.Add(current, -1, true);               // 다음 순서인 캐릭터 패널 추가.
        turnUIManager.SwitchLock(false);                    // 턴 패널의 잠금 해제.
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
        PAUSE,      // 정지.
        ENEMY,      // 적 차례.
        INPUT,      // 플레이어 입력.
        PROCESS,    // 입력에 따른 처리.
        ENDGAME,    // 끝.
    }
}
