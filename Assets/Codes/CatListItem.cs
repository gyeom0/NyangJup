using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CatListItem : MonoBehaviour
{
  RawImage CatThumbnail;
  TextMeshProUGUI CatNameText;

  void Awake()
  {
    CatThumbnail = GetComponentInChildren<RawImage>();
    CatNameText = GetComponentInChildren<TextMeshProUGUI>();
  }

  public void Setup(CatData cat, int index, CardPanel cardPanel)
  {
    CatNameText.text = cat.name;

    var tex = ImageLoader.LoadFromPath(cat.photoPath);
    if (tex != null) CatThumbnail.texture = tex;

    GetComponent<Button>().onClick.AddListener(() =>
      cardPanel.Show(cat, index)
    );
  }
}
