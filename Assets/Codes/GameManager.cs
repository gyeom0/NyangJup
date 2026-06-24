using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  public enum GameState
  {
    Main,
    Camera,
    Catching,
    Collection
  }

  public GameState CurrentGameState { get; private set; } = GameState.Main;
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // 씬 이동해도 파괴금지
    }
    else
    {
      Destroy(gameObject); // 이미 존재하면 파괴
    }
  }
}
