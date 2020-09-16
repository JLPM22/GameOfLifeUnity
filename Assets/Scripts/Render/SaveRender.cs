using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SaveRender : MonoBehaviour
{
    private bool Render;
    private Action<Texture2D> RenderCallback;
    private Vector2Int Resolution;
    private GameOfLife GameOfLife;

    private Camera Camera;
    private RenderTexture RenderTexture;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    public void SaveCurrentRender(Action<Texture2D> callback, Vector2Int resolution, GameOfLife gameOfLife)
    {
        RenderCallback = callback;
        Resolution = resolution;
        GameOfLife = gameOfLife;
        Render = true;
    }

    private void LateUpdate()
    {
        if (Render)
        {
            StartCoroutine(SaveCameraView());
            Render = false;
        }
    }

    public Texture2D texture;

    private IEnumerator SaveCameraView()
    {
        yield return new WaitForEndOfFrame();

        if (RenderTexture == null || RenderTexture.width != Resolution.x || RenderTexture.height != Resolution.y)
        {
            RenderTexture = new RenderTexture(Resolution.x, Resolution.y, 24, RenderTextureFormat.ARGB32);
            RenderTexture.Create();
        }

        RenderTexture.active = RenderTexture;
        Camera.targetTexture = RenderTexture;
        Camera.Render();

        texture = new Texture2D(Resolution.x, Resolution.y, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, Resolution.x, Resolution.y), 0, 0, false);
        texture.Apply();
        RenderTexture.active = null;
        Camera.targetTexture = null;

        RenderCallback(texture);
    }
}
