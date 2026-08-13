using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class CatListItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler
{
  RawImage CatThumbnail;
  TextMeshProUGUI CatNameText;
  float holdTime = 0.5f;
  Coroutine holdCoroutine;
  bool isSelected = false;
  bool isDragging = false;
  bool isLongPressed = false;
  CollectionScene collectionScene;
  CatData catData;

  void Awake()
  {
    CatThumbnail = GetComponentInChildren<RawImage>();
    CatNameText = GetComponentInChildren<TextMeshProUGUI>();
  }

  public void Setup(CatData cat, int index, CardPanel cardPanel, CollectionScene collectionScene)
  {
    this.catData = cat;
    this.collectionScene = collectionScene;
    CatNameText.text = cat.name;

    var tex = ImageLoader.LoadFromPath(cat.photoPath);
    if (tex != null) CatThumbnail.texture = tex;

    GetComponent<Button>().onClick.AddListener(() =>
    {
      if (isLongPressed) { isLongPressed = false; return; }
      if (collectionScene.IsSelectionMode())
        ToggleSelect();
      else
        cardPanel.Show(cat, index);
    }
    );
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    isDragging = false;
    holdCoroutine = StartCoroutine(HoldDetect());
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    if (holdCoroutine != null)
      StopCoroutine(holdCoroutine);
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    isDragging = true;
    if (holdCoroutine != null)
      StopCoroutine(holdCoroutine);
  }

  public void SetSelected(bool selected)
  {
    isSelected = selected;
    GetComponent<Image>().color = selected ? new Color(0.4f, 0.4f, 0.4f, 1f) : new Color(1f, 1f, 1f, 0.2f);
  }

  public CatData GetCatData() => catData;

  void ToggleSelect()
  {
    SetSelected(!isSelected);
    collectionScene.OnItemSelected(this, isSelected);
  }

  IEnumerator HoldDetect()
  {
    yield return new WaitForSeconds(holdTime);
    if (isDragging) yield break;
    isLongPressed = true;
    Handheld.Vibrate();
    SetSelected(!isSelected);
    collectionScene.OnItemSelected(this, isSelected);
  }
}
