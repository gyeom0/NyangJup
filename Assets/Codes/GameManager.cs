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

  public string PlayerName { get; private set; } = "집사";
  public int TinCount { get; private set; } = 10;
  public int TotalCatsFound { get; private set; } = 0;

  const string KEY_PLAYER_NAME = "PlayerName";
  const string KEY_TIN_COUNT = "TinCount";
  const string KEY_TOTAL_CATS_FOUND = "TotalCatsFound";

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // 씬 이동해도 파괴금지
      LoadData();
    }
    else
    {
      Destroy(gameObject); // 이미 존재하면 파괴
    }
  }

  public void SaveData()
  {
    PlayerPrefs.SetString(KEY_PLAYER_NAME, PlayerName);
    PlayerPrefs.SetInt(KEY_TIN_COUNT, TinCount);
    PlayerPrefs.SetInt(KEY_TOTAL_CATS_FOUND, TotalCatsFound);
    PlayerPrefs.Save();

    Debug.Log("[GameManager] 저장 완료");
  }

  public void LoadData()
  {
    PlayerName = PlayerPrefs.GetString(KEY_PLAYER_NAME, "집사");
    TinCount = PlayerPrefs.GetInt(KEY_TIN_COUNT, 10);
    TotalCatsFound = PlayerPrefs.GetInt(KEY_TOTAL_CATS_FOUND, 0);

    Debug.Log($"[GameManager] 불러오기 완료 - {PlayerName}, 통조림: {TinCount}개");
  }

  void OnApplicationPause(bool pauseStatus) //백그라운드 시 자동저장
  {
    if (pauseStatus) SaveData();
  }

  void OnApplicationQuit() // 종료 시 자동저장
  {
    SaveData();
  }

}
