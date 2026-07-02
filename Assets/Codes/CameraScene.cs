using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraScene : MonoBehaviour
{
  public RawImage CameraView;
  WebCamTexture webCamTexture;

  public GameObject FoucusButton;
  public GameObject MinigamePanel;
  public RectTransform SliderBar;
  public RectTransform Indicator;
  public RectTransform RedZone;
  public TextMeshProUGUI ResultText;
  public GameObject RedFlash;
  float direction = 1f;
  float speed = 500f;
  bool isMinigamePlaying = false;

  void Start()
  {
    webCamTexture = new WebCamTexture();
    CameraView.texture = webCamTexture;
    webCamTexture.Play();
  }

  void Update()
  {
    if (!isMinigamePlaying)
      return;

    float sliderBarHalf = SliderBar.rect.width / 2;
    // indicator를 좌우로 움직임
    float indicatorX = Indicator.anchoredPosition.x + speed * direction * Time.deltaTime;
    // 끝에 닿으면 방향 반전
    if (indicatorX > sliderBarHalf || indicatorX < -sliderBarHalf)
    {
      direction *= -1;
    }
    Indicator.anchoredPosition = new Vector2(indicatorX, 0);
  }

  public void OnClickBackButton()
  {
    GameManager.Instance.GoToMainScene();
    isMinigamePlaying = false;
  }

  public void OnClickFocusButton()
  {
    webCamTexture.Pause();
    MinigamePanel.SetActive(true);
    isMinigamePlaying = true;
    SetRedZone();
    FoucusButton.SetActive(false);
  }

  public void OnClickBaitButton()
  {
    if (!isMinigamePlaying) return;
    if (!GameManager.Instance.UseBait())
    {
      Debug.Log("[CameraScene] 미끼 부족!");
      StartCoroutine(ShowText("미끼가 부족해서\n고양이가 다가오지 않아요!"));
      return;
    }

    float indicatorX = Indicator.anchoredPosition.x;
    float redZoneHalf = RedZone.rect.width / 2;
    float LeftEdge = RedZone.anchoredPosition.x - redZoneHalf;
    float RightEdge = RedZone.anchoredPosition.x + redZoneHalf;

    if (indicatorX > LeftEdge && indicatorX < RightEdge)
    {
      float catchRate = 0.7f;
      if (Random.value < catchRate)
      {
        //포획성공
        Debug.Log("[CameraScene] 고양이 잡기 성공!");
        GameManager.Instance.OnCatCaught();
        isMinigamePlaying = false;


        // 카드 만들고 도감등록 하는 걸로 이어지는 로직
      }
      else
      {
        // 타이밍 맞았지만 도망
        isMinigamePlaying = false;
        StartCoroutine(ShowTextAndClose("고양이가 도망갔어요!"));
      }
    }
    else
    {
      SetRedZone();
      StartCoroutine(FlashRed());
      Handheld.Vibrate();
    }
  }
  void SetRedZone()
  {
    float randomWidth = Random.Range(60f, 180f);
    RedZone.sizeDelta = new Vector2(randomWidth, RedZone.sizeDelta.y);
    float sliderBarHalf = SliderBar.rect.width / 2;
    float redZoneHalf = RedZone.rect.width / 2;
    float randomX = Random.Range(-sliderBarHalf + redZoneHalf, sliderBarHalf - redZoneHalf);
    RedZone.anchoredPosition = new Vector2(randomX, 0);
    Indicator.anchoredPosition = new Vector2(-sliderBarHalf, 0);
  }

  IEnumerator ShowText(string message)
  {
    isMinigamePlaying = false;
    ResultText.text = message;
    ResultText.gameObject.SetActive(true);
    yield return new WaitForSeconds(1f);
    ResultText.gameObject.SetActive(false);
    isMinigamePlaying = true;
  }

  IEnumerator ShowTextAndClose(string message)
  {
    ResultText.text = message;
    ResultText.gameObject.SetActive(true);
    yield return new WaitForSeconds(1f);
    ResultText.gameObject.SetActive(false);

    MinigamePanel.SetActive(false);
    webCamTexture.Play();
    FoucusButton.SetActive(true);
  }

  IEnumerator FlashRed()
  {
    RedFlash.SetActive(true);
    yield return new WaitForSeconds(0.1f);
    RedFlash.SetActive(false);
  }

  void OnDestroy()
  {
    if (webCamTexture != null)
    {
      webCamTexture.Stop();
    }
  }
}