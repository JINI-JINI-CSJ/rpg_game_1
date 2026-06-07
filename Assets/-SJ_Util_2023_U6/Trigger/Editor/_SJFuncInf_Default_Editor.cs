using UnityEditor;
using UnityEngine;
using System.Collections;

//[CustomPropertyDrawer(typeof(_SJFuncInf_Default))]
//class _SJFuncInf_Default_Editor : PropertyDrawer 
//{
//	[CustomEditor(typeof(_SJFuncInf_Default._CreateObj))]
//	class _SJFuncInf_Default_CreateObj : PropertyDrawer
//	{
//		_SJFuncInf_Default._CreateObj _sjFuncInf_Default_Attribute { get { return ((_SJFuncInf_Default._CreateObj)attribute); } }
//		//public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) { return 30.0f; }
//		public override void OnGUI(Rect position,
//								SerializedProperty prop,
//								GUIContent label)

//		{
//			EditorGUI.BeginProperty(position, label, prop);

//			EditorGUI.PropertyField( position , prop.FindPropertyRelative ("go"), GUIContent.none );

//			EditorGUI.EndProperty();
//		}
//	}
//	_SJFuncInf_Default_CreateObj arg_create_edit = new _SJFuncInf_Default_CreateObj();



//	// Provide easy access to the RegexAttribute for reading information from it.
//	//_SJFuncInf_Default _sjFuncInf_Default_Attribute { get { return ((_SJFuncInf_Default)attribute); } }

//	// Here you must define the height of your property drawer. Called by Unity.
//	//public override float GetPropertyHeight (SerializedProperty prop,
//	//                                         GUIContent label) {
//	//    return 50.0f;
//	//}

//	public override void OnGUI (Rect position,
//                                SerializedProperty prop,
//                                GUIContent label)
	
//	{
//		EditorGUI.BeginProperty(position, label, prop);


//		//EditorGUILayout.PropertyField( prop.FindPropertyRelative ("func_default"), GUIContent.none );

//		//EditorGUILayout.LabelField("CSJ~~~");

//		arg_create_edit.OnGUI( position , prop , label );

//		//arg_create_edit.DrawDefaultInspector();

//		//EditorGUILayout.EndVertical();

//		EditorGUI.EndProperty();
//    }
//}