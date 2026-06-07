using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_ShaderChange : MonoBehaviour
{
    public  GameObject  go;

    public string   name_shader = "";
    public Material mat;

    static  public  void    ChangeShader( GameObject go , string tar_shader )
    {
        if( go == null )
        {
            Debug.LogError( "Error!! ChangeShader : go == null " );
            return;
        }

        Renderer[] rds = go.GetComponentsInChildren<Renderer>();
        Shader shd = Shader.Find(tar_shader);

        if( shd == null )
        {
            Debug.LogError( "Error!! ChangeShader : shd == null : " + tar_shader );
            return;
        }

        foreach( Renderer r in rds )
        {
            foreach( Material m in r.materials )
            {
                m.shader = shd;
            }
        }
    }

    public  void    _ChangeShader()
    {
        ChangeShader( go , name_shader );
    }

    static  public  void    ChangeMaterial( GameObject go , Material tar_mat )
    {
        if( go == null )
        {
            Debug.LogError( "Error!! ChangeShader : go == null " );
            return;
        }

        if( tar_mat == null )
        {
            Debug.LogError( "Error!! ChangeShader : go == tar_mat " );
            return;
        }

        Renderer[] rds = go.GetComponentsInChildren<Renderer>();
        Material[] mats = new Material[]{ tar_mat };
        foreach( Renderer r in rds )
        {
            r.materials = mats;
        }
    }

    public  void    _ChangeMaterial()
    {
        ChangeMaterial( go , mat );
    }



}
