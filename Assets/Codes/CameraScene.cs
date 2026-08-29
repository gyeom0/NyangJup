using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraScene : MonoBehaviour
{
  public RawImage CameraView;
  WebCamTexture webCamTexture;
  Texture2D capturedTexture;

  public GameObject FoucusButton;
  public GameObject MinigamePanel;
  public RectTransform SliderBar;
  public RectTransform Indicator;
  public RectTransform RedZone;
  public TextMeshProUGUI ResultText;
  public GameObject RedFlash;
  public GameObject SuccessPanel;
  public RawImage NukkiImage;
  public CatDetector CatDetector;
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
    FoucusButton.SetActive(false);

    // 미니게임 전에 먼저 고양이인지 확인
    capturedTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
    capturedTexture.SetPixels(webCamTexture.GetPixels());
    capturedTexture.Apply();

    if (!CatDetector.DetectCat(capturedTexture))
    {
      // 고양이 아님 → 메시지 띄우고 카메라로 복귀
      StartCoroutine(ShowTextAndClose("고양이가 찍히지 않았어요"));
      return;
    }

    // 고양이 맞음 → 미니게임 시작
    MinigamePanel.SetActive(true);
    isMinigamePlaying = true;
    SetRedZone();
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
        // 포획 성공
        Debug.Log("[CameraScene] 고양이 잡기 성공!");
        isMinigamePlaying = false;
        MinigamePanel.SetActive(false);

        Texture2D nukki = CatDetector.RemoveBackground(capturedTexture);
        GameManager.Instance.CapturedCatTexture = nukki;
        NukkiImage.texture = nukki;
        SuccessPanel.SetActive(true);
        StartCoroutine(DropImage(NukkiImage.rectTransform));
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

  public void OnClickRetakeButton()
  {
    SuccessPanel.SetActive(false);
    webCamTexture.Play();
    FoucusButton.SetActive(true);
  }

  public void OnClickCreateCardButton()
  {
    SuccessPanel.SetActive(false);
    GameManager.Instance.GoToCatProfileScene();
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

  //미니게임 도중 메시지 띄우기
  IEnumerator ShowText(string message)
  {
    isMinigamePlaying = false;
    ResultText.text = message;
    ResultText.gameObject.SetActive(true);
    yield return new WaitForSeconds(1f);
    ResultText.gameObject.SetActive(false);
    isMinigamePlaying = true;
  }

  //미니게임 켜지지 않을 때 메시지 띄우기
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

  IEnumerator DropImage(RectTransform rt)
  {
    Vector2 targetPos = rt.anchoredPosition;
    Vector2 startPos = new Vector2(targetPos.x, targetPos.y + 1200f);
    rt.anchoredPosition = startPos;

    float duration = 0.8f;
    float elapsed = 0f;

    while (elapsed < duration)
    {
      elapsed += Time.deltaTime;
      float t = Mathf.Clamp01(elapsed / duration);

      Vector2 pos;
      if (t < 0.78f)
      {
        // 떨어지기: 중력처럼 가속
        float u = t / 0.78f;
        pos = Vector2.Lerp(startPos, targetPos, u * u * u);
      }
      else
      {
        // 끝에서 한 번만 위로 튕기고 안착
        float u = (t - 0.78f) / 0.22f;
        float bounce = Mathf.Sin(u * Mathf.PI) * 100f;
        pos = new Vector2(targetPos.x, targetPos.y + bounce);
      }

      rt.anchoredPosition = pos;
      yield return null;
    }

    rt.anchoredPosition = targetPos;
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