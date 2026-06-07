using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJ_AniState : MonoBehaviour
{
	[System.Serializable]
	public	class _SJ_ANI_SETTING
	{
		public	AnimationClip	clip;
		public	int				layer;
		public	float			speed = 1;
	}
	public List<_SJ_ANI_SETTING>	list_SJ_ANI_SETTING = new List<_SJ_ANI_SETTING>();
	public	Animation	ani;
	
	public	bool	use_Awake;
	public	bool	use_Enable;

	private void Awake()
	{
		if(use_Awake) AniState();
	}

	private void OnEnable()
	{
		if(use_Enable) AniState();
	}

	public	void	AniState()
	{
		foreach( _SJ_ANI_SETTING s in list_SJ_ANI_SETTING )
		{
			AnimationState at =	ani[ s.clip.name ];
            if( at == null )
            {
                Debug.Log("!!! SJTrgPlayer_Mono : at == null : clip.name : " + s.clip.name + " : " + gameObject.name + " : 레거시 타입 체크요망~" );
                continue;
            }
			at.layer = s.layer;
			at.speed = s.speed;
		}
	}
}
