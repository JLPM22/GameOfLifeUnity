using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class GameOfLife : MonoBehaviour
{
    public Camera Camera;
    public Vector2Int Resolution;

    private MeshRenderer Quad;
    private Texture2D Texture;
    private Color32[] Colors;
    public bool Paused { get; private set; }

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

    public void Restart()
    {
        Quad = GetComponent<MeshRenderer>();
        Paused = true;
        Colors = new Color32[Resolution.x * Resolution.y];
        Texture = new Texture2D(Resolution.x, Resolution.y);
        Quad.sharedMaterial.SetTexture("_MainTex", Texture);
        Quad.transform.localScale = new Vector3(Resolution.x, Resolution.y, 1.0f);

        PositionCamera();

        FillInitValues();
        UpdateColors();

        StopAllCoroutines();
        StartCoroutine(UpdateCor());
    }

    public void Pause()
    {
        Paused = true;
    }

    public void Resume()
    {
        Paused = false;
    }

    private void FillInitValues()
    {
        for (int y = 0; y < Resolution.y; y++)
        {
            for (int x = 0; x < Resolution.x; x++)
            {
                Colors[y * Resolution.x + x] = new Color32(0, 0, 0, 255);
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

    private void PositionCamera()
    {
        Camera.aspect = (float)Resolution.x / Resolution.y;
        float distanceFromQuad = (Resolution.y * 0.5f) / Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        Camera.transform.position = Quad.transform.position + Vector3.back * distanceFromQuad;
    }
}
