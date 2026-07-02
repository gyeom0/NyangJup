using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainSceneUI : MonoBehaviour
{
  public TextMeshProUGUI baitCountText;

  void Start()
  {
    UpdateBaitUI();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      GameManager.Instance.AddBait(1);
      UpdateBaitUI();
    }
  }

  public void OnClickCameraButton()
  {
    GameManager.Instance.GoToCameraScene();
  }

  public void OnClickCollectionButton()
  {
    GameManager.Instance.GoToCollectionScene();
  }

  void UpdateBaitUI()
  {
    baitCountText.text = $"미끼: {GameManager.Instance.BaitCount}개";
  }
}