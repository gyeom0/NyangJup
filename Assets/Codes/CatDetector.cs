using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Sentis;

public class CatDetector : MonoBehaviour
{
  public ModelAsset ModelAsset;
  Worker worker;

  void Start()
  {
    var model = ModelLoader.Load(ModelAsset);
    worker = new Worker(model, BackendType.CPU);
  }

  void OnDestroy()
  {
    worker.Dispose();
  }

  public bool DetectCat(Texture2D photo)
  {
    using Tensor<float> input = TextureConverter.ToTensor(photo, 640, 640, 3);
    worker.Schedule(input);

    using Tensor<float> output = (worker.PeekOutput("output0") as Tensor<float>).ReadbackAndClone();


    // YOLOv8 출력에서 고양이(클래스 15) 확인
    for (int i = 0; i < output.shape[2]; i++)
    {
      float maxScore = 0;
      int classId = 0;

      for (int c = 4; c < 84; c++)
      {
        float score = output[0, c, i];
        if (score > maxScore)
        {
          maxScore = score;
          classId = c - 4;
        }
      }

      if (classId == 15 && maxScore > 0.5f)
        return true;
    }
    return false;
  }

  public Texture2D RemoveBackground(Texture2D photo)
  {
    using Tensor<float> input = TextureConverter.ToTensor(photo, 640, 640, 3);
    worker.Schedule(input);

    // output0: (1, 116, 8400) — 감지 결과 + 마스크 계수
    // output1: (1, 32, 160, 160) — 32장의 프로토타입 마스크
    using Tensor<float> output0 = (worker.PeekOutput("output0") as Tensor<float>).ReadbackAndClone();
    using Tensor<float> output1 = (worker.PeekOutput("output1") as Tensor<float>).ReadbackAndClone();

    // 1. 고양이(클래스 15) 중 가장 점수 높은 감지 찾기
    int bestIdx = -1;
    float bestScore = 0.5f;
    for (int i = 0; i < output0.shape[2]; i++)
    {
      float score = output0[0, 4 + 15, i];
      if (score > bestScore)
      {
        bestScore = score;
        bestIdx = i;
      }
    }

    if (bestIdx < 0)
    {
      Debug.Log("[CatDetector] 마스크 생성 실패: 고양이 감지 없음");
      return photo;
    }

    // 2. 그 감지의 마스크 계수 32개 가져오기 (인덱스 84~115)
    float[] coeffs = new float[32];
    for (int k = 0; k < 32; k++)
      coeffs[k] = output0[0, 84 + k, bestIdx];

    // 3. 32개 프로토타입 마스크에 계수 곱해서 합산 → 160×160 고양이 마스크
    int mw = 160, mh = 160;
    bool[,] catMask = new bool[mh, mw];
    for (int y = 0; y < mh; y++)
    {
      for (int x = 0; x < mw; x++)
      {
        float sum = 0f;
        for (int k = 0; k < 32; k++)
          sum += coeffs[k] * output1[0, k, y, x];
        // sigmoid 후 0.5 기준으로 고양이/배경 구분
        catMask[y, x] = (1f / (1f + Mathf.Exp(-sum))) > 0.5f;
      }
    }

    // 4. 마스크를 바깥쪽으로 팽창(dilation)해서 테두리 영역 만들기
    int outlineSize = 3; // 160×160 기준 3픽셀
    bool[,] outlineMask = new bool[mh, mw];
    for (int y = 0; y < mh; y++)
    {
      for (int x = 0; x < mw; x++)
      {
        if (!catMask[y, x]) continue;
        for (int dy = -outlineSize; dy <= outlineSize; dy++)
        {
          for (int dx = -outlineSize; dx <= outlineSize; dx++)
          {
            int ny = y + dy, nx = x + dx;
            if (nx >= 0 && nx < mw && ny >= 0 && ny < mh)
              outlineMask[ny, nx] = true;
          }
        }
      }
    }

    // 5. 색상 적용
    Color neonGreen = new Color(0.08f, 1f, 0.28f, 1f); // 형광 초록 테두리

    Texture2D result = new Texture2D(photo.width, photo.height, TextureFormat.RGBA32, false);
    Color[] pixels = photo.GetPixels();
    for (int y = 0; y < photo.height; y++)
    {
      for (int x = 0; x < photo.width; x++)
      {
        int mx = Mathf.Clamp(Mathf.FloorToInt((float)x / photo.width * mw), 0, mw - 1);
        int my = Mathf.Clamp(Mathf.FloorToInt((float)(photo.height - 1 - y) / photo.height * mh), 0, mh - 1);

        int idx = y * photo.width + x;
        if (catMask[my, mx])
          pixels[idx].a = 1f;           // 고양이 → 원본 색상
        else if (outlineMask[my, mx])
          pixels[idx] = neonGreen;      // 고양이 바깥 테두리 → 형광 초록
        else
          pixels[idx] = Color.clear;  // 배경 → 투명
      }
    }

    result.SetPixels(pixels);
    result.Apply();
    return result;
  }
}
