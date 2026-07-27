using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CardPanel : MonoBehaviour
{
  public RawImage CatImage;
  public TextMeshProUGUI NumberText;
  public TextMeshProUGUI NameText;
  public TextMeshProUGUI DateText;
  public TextMeshProUGUI LocationText;
  public Image GenderIcon;
  public TMP_InputField MemoInputField;

  public void Show(CatData cat, int index)
  {
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
    gameObject.SetActive(false);
  }
}

