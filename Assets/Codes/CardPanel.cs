using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CardPanel : MonoBehaviour
{
  public TextMeshProUGUI NumberText;
  public RawImage CatImage;
  public Image GenderIcon;
  public TextMeshProUGUI NameText;
  public TextMeshProUGUI LocationText;
  public TextMeshProUGUI DateText;
  public TMP_InputField MemoInputField;
  public GameObject Overlay;

  public void Show(CatData cat, int index)
  {
    Overlay.SetActive(true);
    NumberText.text = "No." + (index + 1);
    NameText.text = cat.name;
    DateText.text = cat.date;
    LocationText.text = cat.location;
    MemoInputField.text = cat.memo;
    MemoInputField.onValueChanged.AddListener((value) =>
    {
      cat.memo = value;
      GameManager.Instance.SaveCats();
    });

    var tex = ImageLoader.LoadFromPath(cat.photoPath);
    if (tex != null) CatImage.texture = tex;

    gameObject.SetActive(true);
  }

  public void OnClickCloseButton()
  {
    Overlay.SetActive(false);
    gameObject.SetActive(false);
  }
}

