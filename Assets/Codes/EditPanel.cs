using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditPanel : MonoBehaviour
{
  public TextMeshProUGUI NumberText;
  public RawImage CatImage;
  public Image GenderIcon;
  public TMP_InputField NameInput;
  public TextMeshProUGUI LocationText;
  public TextMeshProUGUI DateText;
  public TMP_InputField MemoInput;
  public GameObject Overlay;
  CatData catData;

  public void Show(CatData cat, int index)
  {
    Overlay.SetActive(true);
    catData = cat;
    NumberText.text = $"No. {index + 1}";
    //GenderIcon.sprite = cat.gender;
    NameInput.text = cat.name;
    LocationText.text = cat.location;
    DateText.text = cat.date;
    MemoInput.text = cat.memo;

    var tex = ImageLoader.LoadFromPath(cat.photoPath);
    if (tex != null) CatImage.texture = tex;

    gameObject.SetActive(true);
  }

  public void OnClickSaveButton()
  {
    catData.name = NameInput.text;
    catData.memo = MemoInput.text;
    GameManager.Instance.SaveCats();
    gameObject.SetActive(false);
    UnityEngine.SceneManagement.SceneManager.LoadScene("CollectionScene");
  }

  public void OnClickCancelButton()
  {
    Overlay.SetActive(false);
    gameObject.SetActive(false);
  }
}
