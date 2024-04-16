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

    [SerializeField] CinemachineFreeLook freeLook; // ī�޶� ��Ʈ�ѷ�.
    [SerializeField] TurnUIManager turnUIManager;  // �� UI �Ŵ���.

    List<Turn> turnQueue;           // ������ ���� ť.
    Character currentCharacter;     // ���� ���� ĳ����.

    GAME_PHASE gamePhase;           // ���� ����.
    int skillCost;                  // ���� ��ų �ڽ�Ʈ.

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // ��� ĳ���͸� ã�� �� ���ǵ� ������ �������� ����.
        Character[] characters = FindObjectsOfType<Character>();
        characters = characters.OrderByDescending(x => x.Speed).ToArray();
        turnQueue = characters.Select(x => new Turn { character = x }).ToList();      // ��� ĳ���͸� Turn ť�� �߰�.
        turnQueue.ForEach(x => x.Reset());                                            // ���� �Ÿ��� ����.        
        turnUIManager.Setup(turnQueue.Select(x => x.character).ToArray());            // �� UI ����.

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
        currentCharacter = turnQueue[0].character;      // ���� ĳ���͸� ����.
        currentCharacter.StartTurn();                   // �� ����.

        freeLook.LookAt = currentCharacter.lookAt;      // ī�޶� Ÿ�� ����.
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
        // ���� ĳ���ʹ� ť���� �����Ѵ�.
        turnQueue = turnQueue.Where(x => x.character.Hp > 0).ToList();
        Debug.Log($"���� ĳ����:{turnQueue.Count()}");
        int playableCount = turnQueue.Count(x => x.character.IsPlayable);  // �÷��̾� ĳ������ ��.
        int enemyCount = turnQueue.Count(x => !x.character.IsPlayable);    // �� ĳ������ ��.
        
        if(playableCount == 0 || enemyCount == 0)
            return playableCount <= 0 ? GAME_PHASE.GAMEOVER : GAME_PHASE.CLEAR;

        Debug.Log("NEXT TURN");

        // ���� ���� ���.
        Turn next = turnQueue[0];                   // ���� �տ� �ִ� ĳ���� ����
        turnQueue.RemoveAt(0);                      // ť���� ����.
        turnQueue.ForEach(x => x.Action(next));     // ���� Turn�� �����Ų��.                

        next.Reset();                               // next�� turn�� �ʱ�ȭ�Ѵ�.
        turnQueue.Add(next);                        // �ٽ� ť�� �ִ´�.
        turnQueue.OrderBy(x => x.reamining);        // ���� �Ÿ��� ���� ����.

        // UI�󿡼� ���� ���� panel�� ��Ȱ��ȭ.
        turnUIManager.CloseTurnUI(0);              // ���� ������ ĳ���� �г� ����.
        await Task.Delay(500);                      // 0.5�� ���.

        turnUIManager.AddTurnUI(next.character);    // ���� ������ ĳ���� �г� �߰�.
        return GAME_PHASE.PLAY;
    }

}
