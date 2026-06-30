using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainSceneUI : MonoBehaviour
{
  public TextMeshProUGUI tinCountText;

  void Start()
  {
    UpdateTinUI();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      GameManager.Instance.AddTin(1);
      UpdateTinUI();
    }
    if (Input.GetKeyDown(KeyCode.R))
    {
      GameManager.Instance.UseTin();
      UpdateTinUI();
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

  void UpdateTinUI()
  {
    tinCountText.text = $"통조림: {GameManager.Instance.TinCount}개";
  }




}