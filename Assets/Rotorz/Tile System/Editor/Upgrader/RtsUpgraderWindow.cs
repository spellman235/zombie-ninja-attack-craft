// Copyright (c) 2011-2014 Rotorz Limited. All rights reserved.

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Rotorz.Tile.Editor;

using Type = System.Type;

/// <summary>
/// This editor window provides the user interface to upgrade tile systems
/// that were created using v1.x into ones that can be used with v2.x.
/// </summary>
public sealed class RtsUpgraderWindow : EditorWindow {
	
	[MenuItem("CONTEXT/_RTS_TOOLS_/Editor Windows/Upgrader", false, 5900)]
	private static void Display() {
		GetWindow<RtsUpgraderWindow>();
	}
	
	/// <summary>
	/// A tile system that can be selected to be upgraded.
	/// </summary>
	private class TileSystemItem {
		public bool selected;
		public MonoBehaviour system;

		public TileSystemItem(MonoBehaviour system) {
			this.system = system;
		}
	}
	
	private List<TileSystemItem> _systems;
	
	/// <summary>
	/// Refresh list of v1.x tile systems in current scene.
	/// </summary>
	void RefreshTileSystems() {
		if (_systems == null)
			_systems = new List<TileSystemItem>();
		else
			_systems.Clear();

		// Is tile system class available?
		Type tyRotorzTileSystem = Type.GetType("Rotorz.TileSystem.RotorzTileSystem,RotorzTileSystem");
		if (tyRotorzTileSystem == null)
			return;

		foreach (MonoBehaviour system in Resources.FindObjectsOfTypeAll(tyRotorzTileSystem) as MonoBehaviour[]) {
			if (PrefabUtility.GetPrefabType(system) == PrefabType.Prefab)
				continue;

			_systems.Add(new TileSystemItem(system));
		}
	}
	
	void OnHierarchyChange() {
		RefreshTileSystems();
		Repaint();
	}
	
	private bool _hasWelcomed;
	private string _welcomeMessage;

	void OnEnable() {
		base.title = "RTS: Upgrader";
		base.minSize = new Vector2(327, 300);

		Type tyProductVersion = Type.GetType("Rotorz.TileSystem.ProductInfo,RotorzTileSystem");
		if (tyProductVersion != null) {
			object version = null;

			PropertyInfo piVersion = tyProductVersion.GetProperty("version", BindingFlags.Public | BindingFlags.Static);
			if (piVersion != null) {
				version = piVersion.GetValue(null, null);
			}
			else {
				FieldInfo fiVersion = tyProductVersion.GetField("version", BindingFlags.Public | BindingFlags.Static);
				version = fiVersion.GetValue(null);
			}

			if (version != null) {
				string originalVersion = (string)(version is System.Version
					? (version as System.Version).ToString(3)
					: version);

				_welcomeMessage = string.Format(
					"Thank you for upgrading from version {0} to {1} of Rotorz Tile System!",
					originalVersion,
					Rotorz.Tile.ProductInfo.version
				);
			}
		}
	}

	private bool _hasPreparedStyles;
	private GUIStyle _boldWrapStyle;

	void PrepareStyles() {
		if (_hasPreparedStyles)
			return;

		_boldWrapStyle = new GUIStyle(GUI.skin.label);
		_boldWrapStyle.fontStyle = FontStyle.Bold;
		_boldWrapStyle.wordWrap = true;

		_hasPreparedStyles = true;
	}

	void OnGUI() {
		PrepareStyles();

		GUILayout.Space(5);

		if (!RtsBrushDatabaseWrapper.Instance.IsExistingDatabaseAvailable) {
			OnGUI_UpgradeNone();
			return;
		}

		if (!File.Exists(Directory.GetCurrentDirectory() + "/Assets/TileBrushes/BrushMappingsV1toV2.asset")) {
			if (!_hasWelcomed)
				OnGUI_Welcome();
			else
				OnGUI_UpgradeBrushes();
			return;
		}
		
		if (_systems == null)
			RefreshTileSystems();
		
		OnGUI_UpgradeTileSystems();
	}
	
	void OnGUI_Welcome() {
		RotorzEditorGUI.Title("Step 1: Welcome");
		
		GUILayout.Label(_welcomeMessage, _boldWrapStyle);
		GUILayout.Space(5);
		GUILayout.Label("The latest update includes a number of new features including new and improved brushes.", EditorStyles.wordWrappedLabel);
		GUILayout.Space(5);
		GUILayout.Label("Existing brushes and tile systems can be upgraded for use with the newer version of this extension. Alternatively you are welcome to use both the newer and older versions of this extension side by side.", EditorStyles.wordWrappedLabel);
		
		GUILayout.Space(10);
		EditorGUILayout.HelpBox("Backup all of your files before proceeding. Upgraded brushes may differ from original brushes.", MessageType.Warning, true);
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		
		if (File.Exists(Directory.GetCurrentDirectory() + "/Assets/Rotorz/Tile System/Support/Migration Guide from 1x.pdf")) {
			if (GUILayout.Button("Migration Guide...", GUILayout.Height(24))) {
				EditorUtility.OpenWithDefaultApp("Assets/Rotorz/Tile System/Support/Migration Guide from 1x.pdf");
				GUIUtility.ExitGUI();
			}
			
			GUILayout.Space(5);
		}
		
		if (GUILayout.Button("Proceed to Next Step", GUILayout.Height(24))) {
			_hasWelcomed = true;
			GUIUtility.ExitGUI();
		}
		
		GUILayout.EndHorizontal();
	}
	
	private RtsBrushUpgradeUtility _brushUpgrader;
	
	void OnGUI_UpgradeBrushes() {
		RotorzEditorGUI.Title("Step 2: Upgrade Brushes");

		if (RtsBrushDatabaseWrapper.Instance.records.Count == 0) {
			OnGUI_UpgradeNoBrushes();
			return;
		}
		
		if (_brushUpgrader == null)
			_brushUpgrader = new RtsBrushUpgradeUtility();
		
		GUILayout.Label("Existing brushes will not be removed automatically.", _boldWrapStyle);
		GUILayout.Space(5);
		GUILayout.Label("An asset will be generated that maps existing brushes to their upgraded counterparts which is used when upgrading tile systems. If map asset is deleted this wizard restart.", EditorStyles.wordWrappedLabel);
		GUILayout.Space(5);
		EditorGUILayout.HelpBox("Do not remove older version of extension nor existing brushes folder 'Assets/TilePrefabs' until you have upgraded all of your tile systems.", MessageType.Warning, true);
	
		GUILayout.Space(5);
		GUILayout.BeginHorizontal();
			GUILayout.Space(15);
			GUILayout.BeginVertical();
				_brushUpgrader.useProceduralTilesets = GUILayout.Toggle(_brushUpgrader.useProceduralTilesets, "Use procedural tilesets / atlas brushes");
				_brushUpgrader.useNewAssets = GUILayout.Toggle(_brushUpgrader.useNewAssets, "Use newer smooth platform brushes.");
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5);
		GUILayout.Label("Please click 'Upgrade Brushes' to proceed.", EditorStyles.wordWrappedLabel);
		
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Go Back", GUILayout.Height(24))) {
			_hasWelcomed = false;
			GUIUtility.ExitGUI();
		}
		
		if (GUILayout.Button("Upgrade Brushes", GUILayout.Height(24))) {
			_brushUpgrader.UpgradeBrushes();
			
			ToolUtility.RepaintPaletteWindows();
			GUIUtility.ExitGUI();
		}
		
		GUILayout.EndHorizontal();
	}
	
	void OnGUI_UpgradeNone() {
		GUILayout.Label("Older version Rotorz Tile System v1.x not detected.", _boldWrapStyle);
	}

	void OnGUI_UpgradeNoBrushes() {
		GUILayout.Label("There are no existing brushes to upgrade.", _boldWrapStyle);
		
		GUILayout.Space(10);

		if (GUILayout.Button("Rescan Existing Brushes", GUILayout.Height(24))) {
			RtsBrushDatabaseWrapper.Instance.Rescan();
			GUIUtility.ExitGUI();
		}
	}
	
	private Vector2 _scrollPosition;
	
	void OnGUI_UpgradeTileSystems() {
		RotorzEditorGUI.Title("Step 3: Upgrade Tile Systems");
		GUILayout.Space(3);
		
		if (EditorApplication.isPlaying) {
			GUILayout.Label("Cannot upgrade tile systems during play mode.", EditorStyles.wordWrappedMiniLabel);
			return;
		}
		
		if (_systems == null || _systems.Count == 0) {
			OnGUI_NoOlderTileSystems();
			return;
		}
		
		RotorzEditorGUI.Splitter();
		
		GUILayout.Space(-7);
		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
		
		EditorGUILayout.HelpBox("Ensure that your project is backed up before proceeding.", MessageType.Warning, true);
		
		GUILayout.Space(7);
		foreach (TileSystemItem item in _systems) {
			GUILayout.Space(-4);
			DrawTileSystemItem(item);
			GUILayout.Space(2);
			RotorzEditorGUI.SplitterLight();
		}
		
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndScrollView();
		
		GUILayout.Space(-7);
		RotorzEditorGUI.Splitter();
		
		switch (GUILayout.Toolbar(-1, new string[] { "All", "None", "Invert" })) {
			case 0: // All
				for (int i = 0; i < _systems.Count; ++i)
					_systems[i].selected = true;
				break;
			case 1: // None
				for (int i = 0; i < _systems.Count; ++i)
					_systems[i].selected = false;
				break;
			case 2: // Invert
				for (int i = 0; i < _systems.Count; ++i)
					_systems[i].selected = !_systems[i].selected;
				break;
		}
		
		GUILayout.Space(5);
		
		GUI.enabled = _systems.FindIndex(item => item.selected) != -1;
		if (GUILayout.Button("Bulk Upgrade", GUILayout.Height(24))) {
			OnBulkUpgrade();
			GUIUtility.ExitGUI();
		}
		GUI.enabled = true;
		
		GUILayout.Space(5);
	}
	
	void OnGUI_NoOlderTileSystems() {
		GUILayout.Label("Scene does not contain any v1.x tile systems.", EditorStyles.wordWrappedMiniLabel);
		
		GUILayout.Space(5);
		
		if (GUILayout.Button("Save Scene", GUILayout.Height(24))) {
			EditorApplication.SaveScene();
			GUIUtility.ExitGUI();
		}
		
		GUILayout.FlexibleSpace();
		
		GUILayout.Label("If you have upgraded all of the tile systems in your project you can:", _boldWrapStyle);
		GUILayout.Space(3);
		GUILayout.Label(" - Remove unwanted tile brush prefabs.\n - Remove older version of extension.", EditorStyles.wordWrappedLabel);
		
		GUILayout.Space(5);
		EditorGUILayout.HelpBox("Be careful not to delete assets that are still in use.", MessageType.Warning, true);
		GUILayout.Space(5);
	}
	
	void DrawTileSystemItem(TileSystemItem item) {
		GUILayout.BeginHorizontal();
		
		Rect r = EditorGUILayout.BeginHorizontal();
			item.selected = GUI.Toggle(new Rect(r.x + 5, r.y + 3, r.width - 5, 22), item.selected, item.system.name);
			GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		if (GUILayout.Button("Upgrade")) {
			RtsTileSystemUpgraderUtility.UpgradeTileSystem(item.system);
			RefreshTileSystems();
			GUIUtility.ExitGUI();
		}
		
		if (GUILayout.Button("Locate")) {
			EditorGUIUtility.PingObject(item.system.gameObject);
			GUIUtility.ExitGUI();
		}
		
		GUILayout.EndHorizontal();
	}
	
	void OnBulkUpgrade() {
		foreach (TileSystemItem item in _systems)
			if (item.selected)
				RtsTileSystemUpgraderUtility.UpgradeTileSystem(item.system);
		RefreshTileSystems();
	}
	
}
