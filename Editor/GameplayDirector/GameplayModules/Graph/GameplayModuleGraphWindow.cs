using UnityEditor;

namespace NL.XRLab.Toolkit.Greybox.Editor.GameplayDirector.GameplayModules.Graph
{
	public class GameplayModuleGraphWindow : EditorWindow
	{
		private GameplayModuleGraphView _graphView;

		private void OnEnable()
		{
			_graphView = new GameplayModuleGraphView
			{
				name = "Gameplay Module Graph"
			};

			_graphView.style.flexGrow = 1;
			rootVisualElement.Add(_graphView);
			_graphView.ForceRefreshGraph();

			EditorApplication.projectChanged += _graphView.RefreshGraphIfNeeded;
		}

		private void OnDisable()
		{
			EditorApplication.projectChanged -= _graphView.RefreshGraphIfNeeded;
			rootVisualElement.Remove(_graphView);
		}

		[MenuItem("XR-Lab/Gameplay Director/Gameplay Module Graph")]
		public static void ShowWindow()
		{
			var window = GetWindow<GameplayModuleGraphWindow>("Gameplay Module Graph");
			window.Show();
		}
	}
}
