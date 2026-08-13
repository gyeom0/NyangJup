using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class CatProfileScene : MonoBehaviour
{
  public RawImage CatImage;
  public TMP_InputField NameInputField;
  public TextMeshProUGUI DateText;
  public TextMeshProUGUI LocationText;
  public TextMeshProUGUI WarningText;
  bool isShowingWarning = false;

  void Start()
  {
    CatImage.texture = GameManager.Instance.CapturedCatTexture;
    DateText.text = System.DateTime.Now.ToString("yyyy-MM-dd\nHH:mm");
    NameInputField.text = "야옹1";
    LocationText.text = "위치 불러오는 중...";
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
}
