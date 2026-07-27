using UnityEngine;
using System.IO;

public static class ImageLoader
{
  public static Texture2D LoadFromPath(string path)
  {
    if (!File.Exists(path)) return null;
    byte[] bytes = File.ReadAllBytes(path);
    Texture2D tex = new Texture2D(2, 2);
    tex.LoadImage(bytes);
    return tex;
  }
}