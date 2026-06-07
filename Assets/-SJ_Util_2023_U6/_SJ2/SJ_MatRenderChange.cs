using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 랜더러의 매터리얼 체인지
public class SJ_MatRenderChange : MonoBehaviour
{

    [System.Serializable]
    public class _CHANGE_MAT
    {
        public string Name;

        [System.Serializable]
        public class _MAT_SRC_TAR
        {
            public Material mat_src;
            public Material mat_tar;
            [HideInInspector]
            public Material mat_inst_cur;

            public Material MatInst()
            {
                if( mat_inst_cur == null )
                    mat_inst_cur = new Material( mat_tar );
                return mat_inst_cur;
            }
        }
        public List<_MAT_SRC_TAR> lt_mat_src_tar;
        public Material mat_tar_default;
        Material mat_tar_default_inst;

        public Material Find_Match(Material mat_src)
        {
            foreach( var s in lt_mat_src_tar )
            {
                if( s.mat_src == mat_src )
                {
                    return s.MatInst();
                }
            }

            return MatInst();
        }

        Material MatInst()
        {
            if( mat_tar_default_inst == null )
                mat_tar_default_inst = new Material( mat_tar_default );
            return mat_tar_default_inst;
        }

        List<Material> change_mats;
        public List<Material> ChangeRandererMat( Renderer rd  )
        {
            if( change_mats == null )
            {
                change_mats = new List<Material>();
                foreach( var s in rd.materials )
                {
                    Material mat_change = MatInst();
                    change_mats.Add(mat_change);
                }                
            }

            return change_mats;
        }

        public List<Material> GetCurChage()
        {
            return change_mats;
        }

    }
    public List<_CHANGE_MAT> lt_CHANGE_MAT;

    [HideInInspector]
    public _CHANGE_MAT cur_chnage_mat;


    // 랜더러 별로 기억 했다가
    public class _BACKUP_RD
    {
        public Renderer rd;
        public Material[] mats_src;
        //List<Material> mats_change;

        public void Backup(Renderer _rd)
        {
            rd = _rd;
            mats_src = rd.materials;
        }

        public void Change( _CHANGE_MAT cm )
        {
            List<Material> lt_new_ch = cm.ChangeRandererMat( rd );
            rd.materials = lt_new_ch.ToArray();
        }

        public void Restore()
        {
            rd.materials = mats_src;
        }
        
    }
    [HideInInspector]
    public List<_BACKUP_RD> lt_BACKUP_RD;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("ChangeDefault")]
    public void ChangeDefault()
    {
        if( lt_CHANGE_MAT.Count < 1 ) return;
        Change( lt_CHANGE_MAT[0] );
    }

    public void Change( string mat_chnage_name )
    {
        _CHANGE_MAT change_mat = null;
        foreach( var s in lt_CHANGE_MAT )
        {
            if( s.Name == mat_chnage_name )
            {
                change_mat = s;
                break;   
            }
        }

        if( change_mat == null )
        {
            Debug.LogError( "에러!! 바꿀 재질 없다 : " + mat_chnage_name );
            return;
        }
        Change( change_mat );
    }
    
    public void Change( _CHANGE_MAT change_mat )
    {
        if( lt_BACKUP_RD == null )
        {
            lt_BACKUP_RD = new List<_BACKUP_RD>();
            Renderer[] rds = GetComponentsInChildren<Renderer>(true);
            foreach( var s in rds )
            {
                _BACKUP_RD br = new _BACKUP_RD();
                br.Backup( s );
                lt_BACKUP_RD.Add( br );
            }
        }
        foreach( var s in lt_BACKUP_RD )
        {
            s.Change(change_mat);
        }
        cur_chnage_mat = change_mat;
    }

    [ContextMenu("Restore")]
    public void Restore()
    {
        if( lt_BACKUP_RD == null ) return;
        foreach( var s in lt_BACKUP_RD )
        {
            s.Restore();
        }
        cur_chnage_mat = null;
    }
}
