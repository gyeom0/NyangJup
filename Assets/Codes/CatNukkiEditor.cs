using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// CatImage RawImage와 같은 오브젝트에 붙이기
public class CatNukkiEditor : MonoBehaviour, IDragHandler, IBeginDragHandler
{
  [Header("연결")]
  public RawImage TargetImage;

  [Header("지우개 설정")]
  public int ScreenEraseRadius = 30; // 화면 기준 반경(px)

  Texture2D editTex;

  // CameraScene에서 누끼 완성 후 호출
  public void SetTexture(Texture2D source)
  {
    editTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
    editTex.SetPixels32(source.GetPixels32());
    editTex.Apply();
    TargetImage.texture = editTex;
  }

  // CreateCardButton 누를 때 최종 텍스처 가져오기
  public Texture2D GetResult() => editTex;

  public void OnBeginDrag(PointerEventData eventData) => Erase(eventData);
  public void OnDrag(PointerEventData eventData) => Erase(eventData);

  void Erase(PointerEventData eventData)
  {
    if (editTex == null) return;

    RectTransform rt = TargetImage.rectTransform;

    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
        rt, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
      return;

    Rect rect = rt.rect;
    float u = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
    float v = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

    int cx = Mathf.RoundToInt(u * editTex.width);
    int cy = Mathf.RoundToInt(v * editTex.height);

    // 화면 반경 → 텍스처 반경 변환
    float scale = (float)editTex.width / rect.width;
    int r = Mathf.RoundToInt(ScreenEraseRadius * scale);

    for (int dy = -r; dy <= r; dy++)
    {
      for (int dx = -r; dx <= r; dx++)
      {
        if (dx * dx + dy * dy > r * r) continue;
        int px = cx + dx, py = cy + dy;
        if (px < 0 || px >= editTex.width || py < 0 || py >= editTex.height) continue;
        editTex.SetPixel(px, py, Color.clear);
      }
    }

    editTex.Apply();
  }
}
