using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOfLife : MonoBehaviour
{
    private Image Image;
    private Texture2D Texture;
    private Color32[] Colors;
    private bool Paused;

    private void Awake()
    {
        Image = GetComponent<Image>();
        Paused = true;
        StartCoroutine(UpdateCor());
    }

    private IEnumerator UpdateCor()
    {
        while (true)
        {
            if (!Paused)
            {
                UpdateState();
                UpdateColors();
            }
            yield return null;
        }
    }

    public void Restart(Vector2Int resolution)
    {
        Paused = true;
        Colors = new Color32[resolution.x * resolution.y];
        Texture = new Texture2D(resolution.x, resolution.y);
        FillInitValues(resolution);
        UpdateColors();
    }

    public void Pause()
    {
        Paused = true;
    }

    public void Resume()
    {
        Paused = false;
    }

    private void FillInitValues(Vector2Int resolution)
    {
        for (int y = 0; y < resolution.y; y++)
        {
            for (int x = 0; x < resolution.x; x++)
            {
                Colors[y * resolution.x + x] = new Color32(0, 0, 0, 255);
            }
        }
    }

    private void UpdateColors()
    {
        Texture.SetPixels32(Colors);
        Texture.Apply();
    }

    private void UpdateState()
    {

    }
}
