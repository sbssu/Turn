using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnUIManager : MonoBehaviour
{
    [SerializeField] TurnUI uiPrefab;
    [SerializeField] RectTransform uiParent;
    [SerializeField] float spacing;

    List<TurnUI> turnList;
    float dealyTime;
    float oneHeight;

    public void Setup(Character[] characters)
    {
        // 이전 UI 삭제.
        TurnUI[] prevUI = GetComponentsInChildren<TurnUI>();
        foreach(var ui in prevUI)
            Destroy(ui.gameObject);

        // 새로운 UI 생성.
        oneHeight = uiPrefab.GetComponent<RectTransform>().sizeDelta.y;
        turnList = new List<TurnUI>();
        foreach (var character in characters)
            AddTurnUI(character);

    }
    public void AddTurnUI(Character character)
    {
        var ui = Instantiate(uiPrefab, uiParent);        
        ui.rect = ui.GetComponent<RectTransform>();
        ui.rect.pivot = new Vector2(0.5f, 1f);
        ui.Setup(character.CharacterSprite, character.IsPlayable);

        float height = oneHeight * turnList.Count + spacing * turnList.Count;
        ui.rect.anchoredPosition = new Vector2(0f, -height);
        ui.Switch(true);

        turnList.Add(ui);
    }
    public void CloseTurnUI(int index)
    {
        dealyTime = 0.2f;
        turnList[index].Switch(false);
        turnList.RemoveAt(index);        
    }
    public void DeadTurnUI(int index)
    {
        dealyTime = 0.8f;
        turnList[index].Dead();
        turnList.RemoveAt(index);
    }


    private void LateUpdate()
    {
        // 이동 딜레이 타임 적용.
        if(dealyTime > 0f)
        {
            dealyTime -= Time.deltaTime;
            return;
        }

        // 항상 자신이 있어야할 위치로 이동한다.
        for (int i = 0; i < turnList.Count; i++)
        {
            float height = oneHeight * i + spacing * i;
            Vector3 destination = new Vector3(0f, -height, 0f);
            turnList[i].rect.anchoredPosition = Vector3.MoveTowards(turnList[i].rect.anchoredPosition, destination, 600f * Time.deltaTime);
        }
        uiParent.sizeDelta = new Vector2(uiParent.sizeDelta.x, 0f);
    }
}

