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

public class GameManager : MonoBehaviour
{
    private enum GAME_PHASE
    {
        READY,
        PLAY,
        CLEAR,
        GAMEOVER,
        PAUSE,
    }

    public static GameManager Instance;

    class Turn
    {
        public Character character;
        public float reamining;

        public void Action(Turn actor)
        {
            reamining -= actor.reamining;
        }
        public void Reset()
        {
            reamining = 10000 / character.Speed;
        }
    }

    [SerializeField] CinemachineFreeLook freeLook; // 카메라 컨트롤러.
    [SerializeField] TurnUIManager turnUIManager;  // 턴 UI 매니저.

    List<Turn> turnQueue;           // 순서에 따른 큐.
    Character currentCharacter;     // 현재 턴인 캐릭터.

    GAME_PHASE gamePhase;           // 게임 상태.
    int skillCost;                  // 공용 스킬 코스트.

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // 모든 캐릭터를 찾은 뒤 스피드 값으로 내림차순 정렬.
        Character[] characters = FindObjectsOfType<Character>();
        characters = characters.OrderByDescending(x => x.Speed).ToArray();
        turnQueue = characters.Select(x => new Turn { character = x }).ToList();      // 모든 캐릭터를 Turn 큐에 추가.
        turnQueue.ForEach(x => x.Reset());                                            // 남은 거리를 리셋.        
        turnUIManager.Setup(turnQueue.Select(x => x.character).ToArray());            // 턴 UI 설정.

        StartTurn();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            EndTurn();
        }
    }


    public void StartTurn()
    {
        currentCharacter = turnQueue[0].character;      // 현재 캐릭터를 설정.
        currentCharacter.StartTurn();                   // 턴 시작.

        freeLook.LookAt = currentCharacter.lookAt;      // 카메라 타겟 지정.
    }
    public async void EndTurn()
    {
        GAME_PHASE gamePhase = await NextTurn();
        switch(gamePhase)
        {
            case GAME_PHASE.PLAY:
                StartTurn();
                break;
            case GAME_PHASE.CLEAR:
            case GAME_PHASE.GAMEOVER:

                break;
        }               
    }
    
    private async Task<GAME_PHASE> NextTurn()
    {
        // 죽은 캐릭터는 큐에서 제거한다.
        turnQueue = turnQueue.Where(x => x.character.Hp > 0).ToList();
        Debug.Log($"남은 캐릭터:{turnQueue.Count()}");
        int playableCount = turnQueue.Count(x => x.character.IsPlayable);  // 플레이어 캐릭터의 수.
        int enemyCount = turnQueue.Count(x => !x.character.IsPlayable);    // 적 캐릭터의 수.
        
        if(playableCount == 0 || enemyCount == 0)
            return playableCount <= 0 ? GAME_PHASE.GAMEOVER : GAME_PHASE.CLEAR;

        Debug.Log("NEXT TURN");

        // 다음 차례 계산.
        Turn next = turnQueue[0];                   // 가장 앞에 있는 캐릭터 빼기
        turnQueue.RemoveAt(0);                      // 큐에서 제거.
        turnQueue.ForEach(x => x.Action(next));     // 남은 Turn을 진행시킨다.                

        next.Reset();                               // next의 turn을 초기화한다.
        turnQueue.Add(next);                        // 다시 큐에 넣는다.
        turnQueue.OrderBy(x => x.reamining);        // 남은 거리에 따라 정렬.

        // UI상에서 턴이 끝난 panel을 비활성화.
        turnUIManager.CloseTurnUI(0);              // 현재 순서인 캐릭터 패널 삭제.
        await Task.Delay(500);                      // 0.5초 대기.

        turnUIManager.AddTurnUI(next.character);    // 다음 순서인 캐릭터 패널 추가.
        return GAME_PHASE.PLAY;
    }

}
