using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionScene : MonoBehaviour
{
  public GameObject CatListItemPrefab;
  public Transform Content;
  public Slider BagGauge;
  public TextMeshProUGUI BagGaugeText;
  public CardPanel cardPanel;

  void Start()
  {
    int count = GameManager.Instance.CaughtCats.Count;
    BagGauge.value = count;
    BagGaugeText.text = count + "/30";

    foreach (CatData cat in GameManager.Instance.CaughtCats)
    {
      GameObject item = Instantiate(CatListItemPrefab, Content);
      int index = GameManager.Instance.CaughtCats.IndexOf(cat);
      item.GetComponent<CatListItem>().Setup(cat, index, cardPanel);
    }
  }

  public void OnClickBackButton()
  {
    GameManager.Instance.GoToMainScene();
  }
}
