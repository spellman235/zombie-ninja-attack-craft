// Copyright (c) 2011-2014 Rotorz Limited. All rights reserved.

using UnityEditor;

using Rotorz.Tile.Editor;
using Rotorz.Tile.Editor.Internal;

/// <summary>
/// Integrates Rotorz Tile System into the Unity user interface.
/// </summary>
static class RtsUnityIntegration {

	#region RTS: Tools - Menu Items

	[MenuItem("CONTEXT/_RTS_TOOLS_/Create Tile System...", false, 0)]
	[MenuItem("GameObject/Create Other/Rotorz Tile System...")]
	static void Menu_RTS_CreateTileSystem() {
		CreateTileSystemWindow.ShowWindow();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Create Brush or Tileset...", false, 0)]
	[MenuItem("Assets/Create/Rotorz Brush or Tileset...")]
	static void Menu_RTS_Brushes_CreateBrushOrTileset() {
		CreateBrushWindow.ShowWindow();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Use as Prefab Offset", false, 100)]
	static void Menu_RTS_UseAsPrefabOffset() {
		TransformUtility.UseAsPrefabOffset();
	}
	[MenuItem("CONTEXT/_RTS_TOOLS_/Use as Prefab Offset", true)]
	static bool Menu_RTS_UseAsPrefabOffset_Validate() {
		return TransformUtility.UseAsPrefabOffset_Validate();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Replace by Brush...", false, 100)]
	static void Menu_RTS_Tools_ReplaceByBrush() {
		TileSystemCommands.Command_ReplaceByBrush();
	}
	[MenuItem("CONTEXT/_RTS_TOOLS_/Build Scene...", false, 100)]
	static void Menu_RTS_Tools_BuildScene() {
		BuildUtility.BuildScene();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Rescan Brushes", false, 200)]
	static void Menu_RTS_Brushes_RescanBrushes() {
		// Refresh asset database.
		AssetDatabase.Refresh();

		// Automatically detect new brushes.
		BrushDatabase.Instance.Rescan(RefreshPreviews.ClearCache);
		// Check for updates.
		BrushDatabase.Instance.CheckForLegacySuffixes();

		// Repaint windows that may have been affected.
		ToolUtility.RepaintPaletteWindows();
		DesignerWindow.RepaintWindow();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Editor Windows/Designer", false, 5000)]
	static void Menu_RTS_BrushDesigner() {
		DesignerWindow.ShowWindow().Focus();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Editor Windows/Scene", false, 5000)]
	static void Menu_RTS_EditorWindows_Scene() {
		ToolUtility.ShowScenePalette();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Editor Windows/Brushes", false, 5000)]
	static void Menu_RTS_EditorWindows_Brushes() {
		ToolUtility.ShowBrushPalette();
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Online Resources/Home Page", false, 5000)]
	static void Menu_RTS_OnlineResources_HomePage() {
		Help.BrowseURL("http://rotorz.com/tilesystem");
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Online Resources/User Guide", false, 5100)]
	static void Menu_RTS_OnlineResources_UserGuide() {
		Help.BrowseURL("http://rotorz.com/tilesystem/guide");
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Online Resources/API Reference", false, 5100)]
	static void Menu_RTS_OnlineResources_API() {
		Help.BrowseURL("http://rotorz.com/tilesystem/api");
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Online Resources/Release Notes", false, 5100)]
	static void Menu_RTS_OnlineResources_ReleaseNotes() {
		Help.BrowseURL("http://rotorz.com/tilesystem/release-notes");
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Online Resources/YouTube Channel", false, 5200)]
	static void Menu_RTS_OnlineResources_YouTubeChannel() {
		Help.BrowseURL("http://www.youtube.com/user/RotorzLimited");
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Online Resources/Twitter Profile", false, 5200)]
	static void Menu_RTS_OnlineResources_TwitterProfile() {
		Help.BrowseURL("https://twitter.com/rotorzlimited");
	}

	[MenuItem("CONTEXT/_RTS_TOOLS_/Online Resources/Facebook Profile", false, 5200)]
	static void Menu_RTS_OnlineResources_FacebookProfile() {
		Help.BrowseURL("https://www.facebook.com/rotorzlimited");
	}

	#endregion

}
