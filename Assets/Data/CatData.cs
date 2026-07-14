using System.Collections.Generic;

// CatData 클래스를 직렬화 가능하게 만들어서 JSON으로 저장할 수 있도록 함
[System.Serializable]
public class CatData
{
  public string name;
  public string date;
  public string location;
  public string photoPath;
}
[System.Serializable]
public class CatDataList
{
  public List<CatData> cats = new List<CatData>();
}
