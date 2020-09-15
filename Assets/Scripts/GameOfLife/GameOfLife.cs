using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class GameOfLife : MonoBehaviour
{
    [Header("References")]
    public Camera Camera;
    public ComputeShader ComputeShader;
    [Header("Properties")]
    public Vector2Int Resolution;
    public float TickTime = 1.0f;

    private MeshRenderer Quad;
    public RenderTexture TextureA, TextureB;
    private bool IsTextureA;
    public bool Paused { get; private set; }

    // Compute Shader
    private int KernelCS;
    private int InputTexCS, ResultTexCS;
    private int ResolutionCS, LiveCellCS, DeadCellCS;

    // Init States
    public enum Init
    {
        Random,
        ScriptableObject
    }
    public Init InitMode = Init.Random;
    [HideInInspector] public Texture2D InitialState;

    private IEnumerator UpdateCor()
    {
        WaitForSecondsRealtime waitForSeconds = new WaitForSecondsRealtime(TickTime);
        while (true)
        {
            if (!Paused)
            {
                UpdateState();
            }
            yield return waitForSeconds;
        }
    }

    public void Restart()
    {
        if (!Application.isPlaying) Debug.LogWarning("Please, press Play to see the simulation");

        Quad = GetComponent<MeshRenderer>();
        Paused = true;
        TextureA = new RenderTexture(Resolution.x, Resolution.y, 24, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point
        };
        TextureA.Create();
        TextureB = new RenderTexture(Resolution.x, Resolution.y, 24, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point
        };
        TextureB.Create();
        Quad.transform.localScale = new Vector3(Resolution.x, Resolution.y, 1.0f);

        PositionCamera();

        InitValues();

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

    public Texture2D GetCurrentState()
    {
        Texture2D saveTexture = new Texture2D(Resolution.x, Resolution.y, TextureFormat.RGBA32, false, false)
        {
            filterMode = FilterMode.Point
        };
        RenderTexture.active = IsTextureA ? TextureA : TextureB;
        saveTexture.ReadPixels(new Rect(0, 0, Resolution.x, Resolution.y), 0, 0, false);
        saveTexture.Apply();
        return saveTexture;
    }

    private void InitValues()
    {
        Texture2D initTexture;
        if (InitMode == Init.Random)
        {
            // Init Colors
            Color32[] colors = new Color32[Resolution.x * Resolution.y];
            for (int y = 0; y < Resolution.y - 2; y++)
            {
                for (int x = 0; x < Resolution.x - 2; x++)
                {
                    byte c = ((x % 3 < 2) && (y % 3 < 2)) ? (byte)255 : (byte)0;
                    colors[y * Resolution.x + x] = new Color32(c, c, c, c);
                }
            }
            Vector2Int middlePoint = new Vector2Int(Resolution.x / 2, (Resolution.y / 2));
            // colors[middlePoint.y * Resolution.x + middlePoint.x] = new Color32(0, 0, 0, 0);
            colors[(middlePoint.y + 1) * Resolution.x + middlePoint.x] = new Color32(0, 0, 0, 0);
            // colors[(middlePoint.y - 1) * Resolution.x + middlePoint.x] = new Color32(255, 255, 255, 255);
            // colors[middlePoint.y * Resolution.x + middlePoint.x + 1] = new Color32(255, 255, 255, 255);
            // colors[middlePoint.y * Resolution.x + middlePoint.x - 1] = new Color32(255, 255, 255, 255);

            // Initial Texture
            initTexture = new Texture2D(Resolution.x, Resolution.y, TextureFormat.RGBA32, false, false)
            {
                filterMode = FilterMode.Point
            };
            initTexture.SetPixels32(colors);
            initTexture.Apply();
        }
        else if (InitMode == Init.ScriptableObject)
        {
            initTexture = InitialState;
            Resolution.x = InitialState.width;
            Resolution.y = InitialState.height;
        }
        else
        {
            initTexture = null;
        }


        // Texture2D to RenderTexture
        IsTextureA = true;
        Graphics.Blit(initTexture, TextureA);
        Quad.sharedMaterial.SetTexture("_MainTex", TextureA);

        // Compute Shaders
        KernelCS = ComputeShader.FindKernel("GameOfLife");
        InputTexCS = Shader.PropertyToID("Input");
        ResultTexCS = Shader.PropertyToID("Result");
        ResolutionCS = Shader.PropertyToID("Resolution");
        LiveCellCS = Shader.PropertyToID("LiveCellColor");
        DeadCellCS = Shader.PropertyToID("DeadCellColor");
    }

    private void UpdateState()
    {
        RenderTexture readTexture = IsTextureA ? TextureA : TextureB;
        RenderTexture rwTexture = IsTextureA ? TextureB : TextureA;

        ComputeShader.SetTexture(KernelCS, InputTexCS, readTexture);
        ComputeShader.SetTexture(KernelCS, ResultTexCS, rwTexture);

        ComputeShader.SetInts(ResolutionCS, Resolution.x, Resolution.y);
        ComputeShader.SetFloats(LiveCellCS, 1.0f, 1.0f, 1.0f);
        ComputeShader.SetFloats(DeadCellCS, 0.0f, 0.0f, 0.0f);

        ComputeShader.Dispatch(KernelCS, Resolution.x / 8, Resolution.y / 8, 1);

        Quad.sharedMaterial.SetTexture("_MainTex", rwTexture);

        IsTextureA = !IsTextureA;
    }

    private void PositionCamera()
    {
        Camera.aspect = (float)Resolution.x / Resolution.y;
        float distanceFromQuad = (Resolution.y * 0.5f) / Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        Camera.transform.position = Quad.transform.position + Vector3.back * distanceFromQuad;
    }
}
