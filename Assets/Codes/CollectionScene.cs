using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CollectionScene : MonoBehaviour
{
  public GameObject CatListItemPrefab;
  public Transform Content;
  public Slider BagGauge;
  public TextMeshProUGUI BagGaugeText;
  public CardPanel CardPanel;
  public EditPanel EditPanel;
  public GameObject SelectionBar;
  public Button EditButton;
  List<CatListItem> selectedItems = new List<CatListItem>();

  void Start()
  {
    int count = GameManager.Instance.CaughtCats.Count;
    BagGauge.value = count;
    BagGaugeText.text = count + "/30";

    foreach (CatData cat in GameManager.Instance.CaughtCats)
    {
      GameObject item = Instantiate(CatListItemPrefab, Content);
      int index = GameManager.Instance.CaughtCats.IndexOf(cat);
      item.GetComponent<CatListItem>().Setup(cat, index, CardPanel, this);
    }
  }

  public bool IsSelectionMode() => selectedItems.Count > 0;

  public void OnClickBackButton()
  {
    GameManager.Instance.GoToMainScene();
  }
  public void OnClickDeleteButton()
  {
    foreach (CatListItem item in selectedItems)
    {
      GameManager.Instance.CaughtCats.Remove(item.GetCatData());
    }
    GameManager.Instance.SaveCats();
    SceneManager.LoadScene("CollectionScene");
  }

  public void OnClickEditButton()
  {
    CatData cat = selectedItems[0].GetCatData();
    int index = GameManager.Instance.CaughtCats.IndexOf(cat);
    EditPanel.Show(cat, index);
  }

  public void OnItemSelected(CatListItem item, bool selected)
  {
    if (selected)
      selectedItems.Add(item);
    else
      selectedItems.Remove(item);

    SelectionBar.SetActive(selectedItems.Count > 0);
    EditButton.interactable = selectedItems.Count == 1;
  }
}
