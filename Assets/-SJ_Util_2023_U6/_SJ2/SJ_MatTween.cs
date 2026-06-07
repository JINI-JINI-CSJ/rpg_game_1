using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(SJ_DymMaterial))]
public class SJ_MatTween : SJ_UITween_Color
{
    public SJ_DymMaterial dymMaterial;
    public string shaderPropertyColor = "_BaseColor";
    public string shaderPropName_FLOAT = "";
    public float shaderProp_FLOAT_from;
    public float shaderProp_FLOAT_to;

    [HideInInspector]
    public List<Material> mats_user_mat;

    Dictionary<Material, Color> matColorDict = new Dictionary<Material, Color>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Init()
    {
        if( dymMaterial != null )dymMaterial.Init();
        SaveDefaultColor();
    }


    public void SaveDefaultColor()
    {
        List<Material> mats = null;
        if( dymMaterial != null )
            mats = dymMaterial.GetMaterials();
        else if( mats_user_mat != null )
        {
            mats = mats_user_mat;
        }
        if( mats == null || mats.Count < 1 ) return;

        foreach( Material mat in mats )
        {
            if( mat == null ) continue;
            if( string.IsNullOrEmpty( shaderPropertyColor ) == false )
            {
                if( !matColorDict.ContainsKey( mat ) )
                {
                    matColorDict[mat] = mat.GetColor( shaderPropertyColor );
                }    
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        FrameMove( Time.deltaTime );
    }

    override public void OnFrameMove()
    {
        List<Material> mats = null;
        if( dymMaterial != null )
            mats = dymMaterial.GetMaterials();
        else if( mats_user_mat != null )
        {
            mats = mats_user_mat;
        }
        if( mats == null || mats.Count < 1 ) return;


        Color col = Color.Lerp( color_from , color_to , ratio_cur );
        float shaderProp_FLOAT_cur = Mathf.Lerp( shaderProp_FLOAT_from , shaderProp_FLOAT_to , ratio_cur );
        foreach( Material mat in mats )
        {
            if( mat == null ) continue;

            if( string.IsNullOrEmpty( shaderPropertyColor ) == false )
            {
                if( !matColorDict.ContainsKey( mat ) )
                {
                    matColorDict[mat] = mat.GetColor( shaderPropertyColor );
                }    
                mat.SetColor( shaderPropertyColor, col );                
            }

            if( string.IsNullOrEmpty( shaderPropName_FLOAT ) == false )
            {
                mat.SetFloat( shaderPropName_FLOAT , shaderProp_FLOAT_cur );
            }   
        }
    }

    override public void Stop()
    {
        base.Stop();
        if( dymMaterial == null )
        {
            dymMaterial = GetComponent<SJ_DymMaterial>();
            if( dymMaterial == null )
            {
                Debug.LogError( "SJ_MatTween : dymMaterial is null! " + gameObject.name );
                return;
            }
        }

        // List<Material> mats = dymMaterial.GetMaterials();
        // if( mats.Count < 1 ) return;

        List<Material> mats = null;
        if( dymMaterial != null )
            mats = dymMaterial.GetMaterials();
        else if( mats_user_mat != null )
        {
            mats = mats_user_mat;
        }
        if( mats == null || mats.Count < 1 ) return;

        foreach( Material mat in mats )
        {
            if( mat == null ) continue;
            Color col_default = Color.white;
            if( matColorDict.ContainsKey( mat ) )
            {
                col_default = matColorDict[mat];
            }
            mat.SetColor( shaderPropertyColor, col_default );

            //Debug.Log( "SJ_MatTween : 스톱~~~~ " + gameObject.name + " Stop col_default : " + col_default );
        }
    }
}
