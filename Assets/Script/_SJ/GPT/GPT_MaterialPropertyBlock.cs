using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class GPT_MaterialPropertyBlock : MonoBehaviour
{
    private Renderer targetRenderer;
    private MaterialPropertyBlock propertyBlock;

    [Header("Base Color")]
    public bool useColor;

    [ColorUsage(true, true)]
    public Color color = Color.white;

    [Header("Main Texture")]
    public bool useTexture;

    public Texture texture;

    [Header("Smoothness")]
    public bool useSmoothness;

    [Range(0, 1)]
    public float smoothness = 0.5f;

    [Header("Emission")]
    public bool useEmission;

    [ColorUsage(true, true)]
    public Color emissionColor = Color.black;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        Apply();
    }

    private void OnValidate()
    {
        Initialize();
        Apply();
    }

    void Initialize()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
    }

    public void Apply()
    {
        if (targetRenderer == null)
            return;

        targetRenderer.GetPropertyBlock(propertyBlock);

        //---------------------------------

        if (useColor)
            propertyBlock.SetColor("_BaseColor", color);
        else
            propertyBlock.Clear();

        //---------------------------------

        if (useTexture)
            propertyBlock.SetTexture("_BaseMap", texture);

        //---------------------------------

        if (useSmoothness)
            propertyBlock.SetFloat("_Smoothness", smoothness);

        //---------------------------------

        if (useEmission)
        {
            propertyBlock.SetColor("_EmissionColor", emissionColor);
        }

        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    public void Clear()
    {
        if (targetRenderer == null)
            return;

        propertyBlock.Clear();
        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}