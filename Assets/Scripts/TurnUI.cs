using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnUI : MonoBehaviour
{
    public enum STATE
    {
        READY,      // 대기 상태.
        TURN,       // 자신의 턴.
        TARGET,     // 타게팅 당하는 중
        SHOWING,    // 열리는 중.
        CLOSING,    // 닫히는 중(가리는 중)
        DEAD,       // 죽었다.
    }


    [SerializeField] RectTransform panel;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image characterImage;
    [SerializeField] Image blinkImage;
    [SerializeField] Image teamImage;

    [HideInInspector] public RectTransform rect;
    [HideInInspector] public Character character;

    STATE state;

    public void Setup(Sprite sprite, bool isPlayable)
    {
        characterImage.sprite = sprite;
        teamImage.color = isPlayable ? new Color(0.2f, 1.0f, 0.2f) : new Color(1.0f, 0.2f, 0.2f);
    }

    public void MyTurn(bool isMyTurn)
    {
        panel.localScale = Vector3.one * (isMyTurn ? 1.1f : 1f);
        characterImage.transform.localScale = Vector3.one * (isMyTurn ? 1.2f : 1f);
    }
    public void Targeting(bool isTarget)
    {
        state = isTarget ? STATE.TARGET : STATE.READY;
        SetAlpha(blinkImage, 0);
        blinkTime = 0;
    }
    public void Dead()
    {
        state = STATE.DEAD;
        canvasGroup.alpha = 1f;
    }
    public void Switch(bool isOn)
    {
        state = isOn ? STATE.SHOWING : STATE.CLOSING;
        panel.anchoredPosition = isOn ? new Vector3(30f, 0f, 0f) : Vector3.zero;
        canvasGroup.alpha = isOn ? 0f : 1f;
    }

    float blinkTime = 0;
    private void Update()
    {
        switch(state)
        {
            case STATE.TARGET:
                {
                    blinkTime += Time.deltaTime;
                    float alpha = Mathf.Sin(blinkTime % 1 * 180f);
                    SetAlpha(blinkImage, alpha);
                }
                break;

            case STATE.DEAD:
                {
                    canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha - Time.deltaTime * 2f, 0.0f, 1.0f);
                }
                break;

            case STATE.SHOWING:
                {
                    panel.anchoredPosition = Vector3.MoveTowards(panel.anchoredPosition, Vector3.zero, Time.deltaTime * 300f);
                    canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha + Time.deltaTime * 4f, 0.0f, 1.0f);
                }
                break;
            case STATE.CLOSING:
                {
                    panel.anchoredPosition = Vector3.MoveTowards(panel.anchoredPosition, new Vector3(-300f, 0f, 0f), Time.deltaTime * 300f);
                    canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha - Time.deltaTime * 4f, 0.0f, 1.0f);
                }
                break;
        }
    }

    private void SetAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
}
