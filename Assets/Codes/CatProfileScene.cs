using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class CatProfileScene : MonoBehaviour
{
  public RawImage CatImage;
  public TMP_InputField NameInputField;
  public TextMeshProUGUI DateText;
  public TextMeshProUGUI LocationText;
  public TextMeshProUGUI WarningText;
  bool isShowingWarning = false;

  IEnumerator Start()
  {
    CatImage.texture = GameManager.Instance.CapturedCatTexture;
    DateText.text = System.DateTime.Now.ToString("yyyy-MM-dd\nHH:mm");
    NameInputField.text = "야옹1";
    LocationText.text = "위치 불러오는 중...";

    yield return StartCoroutine(GetLocation());
  }

  IEnumerator ShowWarning()
  {
    isShowingWarning = true;
    WarningText.gameObject.SetActive(true);
    yield return new WaitForSeconds(2f);
    WarningText.gameObject.SetActive(false);
    isShowingWarning = false;
  }

  public void OnClickSaveButton()
  {
    if (isShowingWarning) return;

    bool isDuplicate = GameManager.Instance.CaughtCats.Exists(c => c.name == NameInputField.text);
    if (isDuplicate)
    {
      StartCoroutine(ShowWarning());
      return;
    }

    CatData newCata = new CatData();
    newCata.name = NameInputField.text;
    newCata.date = DateText.text;
    newCata.location = LocationText.text;

    string fileName = "cat_" + System.DateTime.Now.Ticks + ".png";
    string filePath = System.IO.Path.Combine(Application.persistentDataPath, fileName);
    System.IO.File.WriteAllBytes(filePath, GameManager.Instance.CapturedCatTexture.EncodeToPNG());
    newCata.photoPath = filePath;
    GameManager.Instance.AddCat(newCata);

    GameManager.Instance.GoToCollectionScene();
  }

  IEnumerator GetLocation()
  {
    if (!Input.location.isEnabledByUser)
    {
      LocationText.text = "위치 권한이 없어요";
      yield break;
    }

    Input.location.Start();

    int timeout = 10;
    while (Input.location.status == LocationServiceStatus.Initializing && timeout > 0)
    {
      yield return new WaitForSeconds(1);
      timeout--;
    }

    if (Input.location.status == LocationServiceStatus.Failed)
    {
      LocationText.text = "위치 정보를 가져올 수 없어요";
      yield break;
    }

    float lat = Input.location.lastData.latitude;
    float lon = Input.location.lastData.longitude;
    yield return StartCoroutine(GetAddress(lat, lon));

    Input.location.Stop();
  }

  IEnumerator GetAddress(float lat, float lon)
  {
    string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lon}&accept-language=ko";
    UnityWebRequest request = UnityWebRequest.Get(url);
    request.SetRequestHeader("User-Agent", "NyangJup/1.0 ");
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
      var json = JsonUtility.FromJson<NominatimResponse>(request.downloadHandler.text);
      LocationText.text = json.display_name;
    }
    else
    {
      LocationText.text = "주소를 가져올 수 없어요";
    }
  }
}
[System.Serializable]
public class NominatimResponse
{
  public string display_name;
}

