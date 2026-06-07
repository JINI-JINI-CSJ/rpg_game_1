using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_UVTexture_SkyBox : SJ_UVTexture
{
    private void Awake() {
        if( list_UVMat.Count < 1 )
        {
            Debug.Log( "SJ_UVTexture_SkyBox : 1개 리스트가 있어야 한다~~~~~~~~~~~~~~~~~~~~~~~~"  );
            return;
        }
        list_UVMat[0].mat = RenderSettings.skybox;
    }
}
