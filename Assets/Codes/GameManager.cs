using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  public enum GameState
  {
    Main,
    Camera,
    Collection,
    CatProfile
  }

  public GameState CurrentGameState { get; private set; } = GameState.Main;

  public Texture2D CapturedCatTexture;

  public string PlayerName { get; private set; } = "집사";
  public int BaitCount { get; private set; } = 10;
  public int TotalCats { get; private set; } = 0;

  const string KEY_PLAYER_NAME = "PlayerName";
  const string KEY_BAIT_COUNT = "BaitCount";
  const string KEY_TOTAL_CATS = "TotalCats";

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

  public void ResetData()
  {
    BaitCount = 10;
    TotalCats = 0;
    SaveData();
  }

  public void SaveData()
  {
    PlayerPrefs.SetString(KEY_PLAYER_NAME, PlayerName);
    PlayerPrefs.SetInt(KEY_BAIT_COUNT, BaitCount);
    PlayerPrefs.SetInt(KEY_TOTAL_CATS, TotalCats);
    PlayerPrefs.Save();

    Debug.Log("[GameManager] 저장 완료");
  }

  public void LoadData()
  {
    PlayerName = PlayerPrefs.GetString(KEY_PLAYER_NAME, "집사");
    BaitCount = PlayerPrefs.GetInt(KEY_BAIT_COUNT, 10);
    TotalCats = PlayerPrefs.GetInt(KEY_TOTAL_CATS, 0);

    Debug.Log($"[GameManager] 불러오기 완료 - {PlayerName}, 미끼: {BaitCount}개");
  }

  public bool UseBait(int amount = 1)
  {
    if (BaitCount < amount)
    {
      Debug.Log("[GameManager] 미끼 부족");
      return false;
    }
    else
    {
      BaitCount -= amount;
      SaveData();
      Debug.Log($"[GameManager] 미끼 사용 - {amount}개, 남은 미끼: {BaitCount}개");
      return true;
    }
  }

  public void AddBait(int amount)
  {
    BaitCount += amount;
    SaveData();
    Debug.Log($"[GameManager] 미끼 획득 - {amount}개, 총 미끼: {BaitCount}개");
  }

  public void OnCatCaught()
  {
    TotalCats++;
    SaveData();
    Debug.Log($"[GameManager] 고양이 획득! 도감 고양이 수: {TotalCats}");
  }

  public void GoToMainScene()
  {
    CurrentGameState = GameState.Main;
    SceneManager.LoadScene("MainScene");
  }
  public void GoToCameraScene()
  {

    CurrentGameState = GameState.Camera;
    SceneManager.LoadScene("CameraScene");
  }
  public void GoToCollectionScene()
  {
    CurrentGameState = GameState.Collection;
    SceneManager.LoadScene("CollectionScene");
  }

  public void GoToCatProfileScene()
  {
    CurrentGameState = GameState.CatProfile;
    SceneManager.LoadScene("CatProfileScene");
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
