using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }
  public GameState CurrentGameState { get; private set; } = GameState.Main;

  public enum GameState
  {
    Main,
    Camera,
    Collection,
    CatProfile
  }
  public List<CatData> CaughtCats = new List<CatData>();

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

    string path = Path.Combine(Application.persistentDataPath, "cats.json"); // 파일 경로
    if (File.Exists(path)) // 파일 있을 때만 불러오기
    {
      CatDataList list = JsonUtility.FromJson<CatDataList>(File.ReadAllText(path)); // JSON -> 객체
      CaughtCats = list.cats; // 불러온 리스트 게임에 적용
    }

    Debug.Log($"[GameManager] 불러오기 완료 - {PlayerName}, 미끼: {BaitCount}개");
  }

  public void SaveCats()
  {
    CatDataList list = new CatDataList(); // 빈 보관함 만들기
    list.cats = CaughtCats; // 고양이 리스트 담기
    File.WriteAllText(Path.Combine(Application.persistentDataPath, "cats.json"), JsonUtility.ToJson(list));
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

  public void AddCat(CatData catData)
  {
    CaughtCats.Add(catData);
    TotalCats++;
    SaveData();
    SaveCats();
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
