using UnityEngine;


[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class GPT_DynamicSimpleLitMaterial : MonoBehaviour
{

    private Renderer targetRenderer;


    private Material runtimeMaterial;



    [Header("Base Color")]
    public bool useColor;

    [ColorUsage(true,true)]
    public Color color = Color.white;



    [Header("Main Texture")]
    public bool useTexture;

    public Texture texture;



    [Header("Smoothness")]
    public bool useSmoothness;

    [Range(0,1)]
    public float smoothness = 0.5f;



    [Header("Emission")]
    public bool useEmission;

    [ColorUsage(true,true)]
    public Color emissionColor;



    // 원본 Material 보관
    private Material originalMaterial;



    private void Awake()
    {
        Initialize();
    }



    private void OnEnable()
    {
        Initialize();
        ApplyMaterial();
    }



    private void OnValidate()
    {
        Initialize();

        ApplyMaterial();
    }



    private void Initialize()
    {

        if(targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();


        if(targetRenderer == null)
            return;



        // 이미 생성됨
        if(runtimeMaterial != null)
            return;



        // 원본 저장
        if(originalMaterial == null)
        {
            originalMaterial = 
                targetRenderer.sharedMaterial;
        }



        if(originalMaterial == null)
            return;



        runtimeMaterial = new Material(originalMaterial);


        runtimeMaterial.name =
            originalMaterial.name 
            + "_DynamicInstance";


        // 중요
        // Editor에서도 삭제되지 않도록 보호
        runtimeMaterial.hideFlags =
            HideFlags.HideAndDontSave;

        // sharedMaterial 사용 금지
        // renderer.material 사용
        targetRenderer.material = runtimeMaterial;

    }





    private void ApplyMaterial()
    {

        if(runtimeMaterial == null)
            return;



        if(useColor &&
           runtimeMaterial.HasProperty("_BaseColor"))
        {
            runtimeMaterial.SetColor(
                "_BaseColor",
                color);
        }




        if(useTexture &&
           runtimeMaterial.HasProperty("_BaseMap"))
        {
            runtimeMaterial.SetTexture(
                "_BaseMap",
                texture);
        }





        if(useSmoothness &&
           runtimeMaterial.HasProperty("_Smoothness"))
        {
            runtimeMaterial.SetFloat(
                "_Smoothness",
                smoothness);
        }





        if(runtimeMaterial.HasProperty("_EmissionColor"))
        {

            if(useEmission)
            {
                runtimeMaterial.EnableKeyword("_EMISSION");

                runtimeMaterial.SetColor(
                    "_EmissionColor",
                    emissionColor);
            }
            else
            {
                runtimeMaterial.DisableKeyword("_EMISSION");
            }
        }

    }





    public Material GetMaterial()
    {
        return runtimeMaterial;
    }





    private void OnDestroy()
    {

        // Play 종료 또는 Editor 삭제시에만 제거

        if(runtimeMaterial != null)
        {

            if(Application.isPlaying)
            {
                Destroy(runtimeMaterial);
            }
            else
            {
                DestroyImmediate(runtimeMaterial);
            }


            runtimeMaterial = null;
        }

    }

}