using UnityEditor;

namespace NL.XRLab.Toolkit.Greybox.Editor.GameplayDirector.GameplayModules.Graph
{
	/// <summary>
	/// Editor window that displays the graph view for gameplay modules. It listens to project changes and refreshes the graph accordingly.
	/// </summary>
	public class GameplayModuleGraphWindow : EditorWindow
	{
		/// <summary>
		/// The graph view that displays the gameplay modules and their connections. It is initialized when the window is enabled and removed when the window is disabled.
		/// </summary>
		private GameplayModuleGraphView _graphView;

		private void OnEnable()
		{
			_graphView = new GameplayModuleGraphView { name = "Gameplay Module Graph" };

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

		/// <summary>
		/// Shows the gameplay module graph window.
		/// </summary>
		[MenuItem("XR-Lab/Open Gameplay Module Graph")]
		public static void ShowWindow()
		{
			var window = GetWindow<GameplayModuleGraphWindow>("Gameplay Module Graph");
			window.Show();
		}
	}
}
