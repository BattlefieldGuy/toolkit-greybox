using NL.XRLab.ToolkitGreybox.GameplayModules;
using UnityEditor;
using UnityEngine;

namespace NL.XRLab.ToolkitGreybox.Editor
{
	/// <summary>
	///    Custom editor for GameplayModuleData:
	///    - shows an editable SceneAsset picker and ConnectedModules list
	///    - shows ScenePath as read-only
	///    - updates ScenePath automatically when SceneAsset changes
	/// </summary>
	[CustomEditor(typeof(GameplayModuleData))]
	public class GameplayModuleDataEditor : UnityEditor.Editor
	{
		private SerializedProperty _connectedModulesProp;
		private SerializedProperty _sceneAssetProp;
		private SerializedProperty _scenePathProp;

		private void OnEnable()
		{
			// names must match the private field names in GameplayModuleData
			_sceneAssetProp = serializedObject.FindProperty("_sceneAsset");
			_scenePathProp = serializedObject.FindProperty("_scenePath");
			_connectedModulesProp = serializedObject.FindProperty("ConnectedModules");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("Gameplay Module", EditorStyles.boldLabel);
			EditorGUILayout.Space(4);

			// Scene asset picker (editable)
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_sceneAssetProp, new GUIContent("Scene Asset"));
			if (EditorGUI.EndChangeCheck())
			{
				// Read the selected scene asset and write its path to _scenePath
				var sceneAsset = _sceneAssetProp.objectReferenceValue as SceneAsset;
				if (sceneAsset == null)
				{
					_scenePathProp.stringValue = string.Empty;
				}
				else
				{
					string assetPath = AssetDatabase.GetAssetPath(sceneAsset);
					// store full path (e.g., "Assets/Scenes/MyScene.unity")
					_scenePathProp.stringValue = assetPath;
					// If you prefer the scene name only, use:
					// scenePathProp.stringValue = System.IO.Path.GetFileNameWithoutExtension(assetPath);
				}
			}

			EditorGUILayout.Space(6);

			// ScenePath shown as read-only
			GUI.enabled = false;
			EditorGUILayout.PropertyField(_scenePathProp, new GUIContent("Scene Path (Read Only)"));
			GUI.enabled = true;

			EditorGUILayout.Space(8);

			// Connected modules list (editable)
			EditorGUILayout.PropertyField(_connectedModulesProp, new GUIContent("Connected Modules"), true);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
