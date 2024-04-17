using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnUIManager : MonoBehaviour
{
    public static TurnUIManager Instance { get; private set; }

    [SerializeField] TurnUI uiPrefab;
    [SerializeField] RectTransform uiParent;
    [SerializeField] float spacing;

    // �⺻������ �� �гε��� �ڽ��� ��ġ�� �̵��Ϸ��� �Ѵ�.
    // �̶� Locked�� �ɷ������� �ڸ��� �����.

    List<TurnUI> turnList;          // �г� ����Ʈ.
    bool isLocked;                  // �г��� ������ ����
    float oneHeight;                // �г� �ϳ��� ����

    private void Awake()
    {
        Instance = this;
    }
    public void Setup(Character[] characters)
    {
        Debug.Log("Setup");
        Debug.Log(characters.Length);

        // ���� UI ����.
        TurnUI[] prevUI = GetComponentsInChildren<TurnUI>();
        foreach(var ui in prevUI)
            Destroy(ui.gameObject);

        // ���ο� UI ����.
        oneHeight = uiPrefab.GetComponent<RectTransform>().sizeDelta.y;
        turnList = new List<TurnUI>();
        foreach (var character in characters)
            Add(character, -1, false);
    }

    public void Add(Character character, int index, bool isAnimate)
    {
        TurnUI ui = Instantiate(uiPrefab, uiParent);
        ui.Setup(character);

        float height = oneHeight * turnList.Count + spacing * turnList.Count;
        ui.rect.anchoredPosition = new Vector2(0f, -height);
        
        if(isAnimate)
            ui.Show();

        if(index == -1)
            turnList.Add(ui);
        else
            turnList.Insert(index, ui);
    }
    public void Remove(Character character, bool isAnimate)
    {
        TurnUI target = turnList.Find(x => x.character == character);
        if(isAnimate)
            target.Close();

        turnList.Remove(target);
    }
    public void Dead(Character character)
    {
        TurnUI target = turnList.Find(x => x.character == character);
        target.Dead();
        turnList.Remove(target);
    }
    public void Targeting(Character character, bool isOn)
    {

    }
    public void SwitchLock(bool isLocked)
    {
        this.isLocked = isLocked;
    }


    private void LateUpdate()
    {
        // �̵� ����.
        if (isLocked)
            return;

        // �׻� �ڽ��� �־���� ��ġ�� �̵��Ѵ�.
        for (int i = 0; i < turnList.Count; i++)
        {
            float height = oneHeight * i + spacing * i;
            Vector3 destination = new Vector3(0f, -height, 0f);
            turnList[i].rect.localPosition = Vector3.MoveTowards(turnList[i].rect.localPosition, destination, 600f * Time.deltaTime);
        }
        uiParent.sizeDelta = new Vector2(uiParent.sizeDelta.x, 0f);
    }
}

