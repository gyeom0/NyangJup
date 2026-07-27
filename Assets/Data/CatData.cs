using System.Collections.Generic;

public enum Gender { 미등록, 암컷, 수컷, 중성 }

[System.Serializable]
public class CatData
{
  public string name;
  public string date;
  public string location;
  public string photoPath;
  public Gender gender = Gender.미등록;
  public string memo = "";
}
[System.Serializable]
public class CatDataList
{
  public List<CatData> cats = new List<CatData>();
}
