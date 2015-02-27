// Copyright (c) 2011-2014 Rotorz Limited. All rights reserved.

#pragma warning disable 618

using UnityEngine;
using UnityEditor;

using System.Reflection;

using Rotorz.Tile;

using Type = System.Type;

/// <summary>
/// Utility class that upgrades a v1.x tile system into a v2.x tile system.
/// </summary>
/// <remarks>
/// <para>Brushes must have been upgraded before tile systems can be upgraded!</para>
/// </remarks>
public static class RtsTileSystemUpgraderUtility {

	#region Reflection

	private static Type _tyRotorzTileSystem;

	// Tile Instance Component
	private static Type _tyTileInstance;

	private static FieldInfo _fiTileInstance_row;
	private static FieldInfo _fiTileInstance_column;
	private static PropertyInfo _piTileInstance_brush;
	private static FieldInfo _fiTileInstance_orientationName;
	private static FieldInfo _fiTileInstance_variationIndex;

	// Tile Data
	private static Type _tyTileData;

	private static FieldInfo _fiTileData_brush;
	private static FieldInfo _fiTileData_gameObject;
	private static FieldInfo _fiTileData_orientationMask;
	private static FieldInfo _fiTileData_variationIndex;

	// Tile Systems
	private static MethodInfo _miTileSystem_GetTile;

	private static FieldInfo _fiTileSystem_version;

	private static FieldInfo _fiTileSystem_tileSize;
	private static FieldInfo _fiTileSystem_rows;
	private static FieldInfo _fiTileSystem_columns;
	private static FieldInfo _fiTileSystem_activeColumn;
	private static FieldInfo _fiTileSystem_activeRow;

	private static FieldInfo _fiTileSystem_chunkWidth;
	private static FieldInfo _fiTileSystem_chunkHeight;
	private static PropertyInfo _piTileSystem_chunks;

	private static PropertyInfo _piTileSystem_strippingPreset;
	private static PropertyInfo _piTileSystem_strippingOptions;
	private static FieldInfo _fiTileSystem_applyRuntimeStripping;

	private static FieldInfo _fiTileSystem_combineChunkWidth;
	private static FieldInfo _fiTileSystem_combineChunkHeight;
	private static FieldInfo _fiTileSystem_combineMethod;

	private static FieldInfo _fiTileSystem_generateSecondUVs;
	private static FieldInfo _fiTileSystem_hintEraseEmptyChunks;
	private static FieldInfo _fiTileSystem_staticVertexSnapping;

	/// <summary>
	/// Use reflection to access properties that may or may not be defined.
	/// </summary>
	private static void PrepareReflection() {
		BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		_tyRotorzTileSystem = Type.GetType("Rotorz.TileSystem.RotorzTileSystem,RotorzTileSystem");
		
		// Tile Instance Component
		_tyTileInstance = Type.GetType("Rotorz.TileSystem.TileInstance,RotorzTileSystem");
		if (_tyTileInstance != null) {
			_fiTileInstance_row = _tyTileInstance.GetField("row", instanceBindingFlags);
			_fiTileInstance_column = _tyTileInstance.GetField("column", instanceBindingFlags);
			_piTileInstance_brush = _tyTileInstance.GetProperty("brush", instanceBindingFlags);
			_fiTileInstance_orientationName = _tyTileInstance.GetField("orientationName", instanceBindingFlags);
			_fiTileInstance_variationIndex = _tyTileInstance.GetField("variationIndex", instanceBindingFlags);
		}

		// Tile Data
		_tyTileData = Type.GetType("Rotorz.TileSystem.TileData,RotorzTileSystem");
		if (_tyTileData != null) {
			_fiTileData_brush = _tyTileData.GetField("brush", instanceBindingFlags);
			_fiTileData_gameObject = _tyTileData.GetField("gameObject", instanceBindingFlags);
			_fiTileData_orientationMask = _tyTileData.GetField("orientationMask", instanceBindingFlags);
			_fiTileData_variationIndex = _tyTileData.GetField("variationIndex", instanceBindingFlags);
		}
		
		// Tile Systems
		_miTileSystem_GetTile = _tyRotorzTileSystem.GetMethod("GetTile", instanceBindingFlags, null, new Type[] { typeof(int), typeof(int) }, null);

		_fiTileSystem_version = _tyRotorzTileSystem.GetField("version", instanceBindingFlags);

		_fiTileSystem_tileSize = _tyRotorzTileSystem.GetField("tileSize", instanceBindingFlags);
		_fiTileSystem_rows = _tyRotorzTileSystem.GetField("rows", instanceBindingFlags);
		_fiTileSystem_columns = _tyRotorzTileSystem.GetField("columns", instanceBindingFlags);
		_fiTileSystem_activeColumn = _tyRotorzTileSystem.GetField("activeColumn", instanceBindingFlags);
		_fiTileSystem_activeRow = _tyRotorzTileSystem.GetField("activeRow", instanceBindingFlags);

		_fiTileSystem_chunkWidth = _tyRotorzTileSystem.GetField("chunkWidth", instanceBindingFlags);
		if (_fiTileSystem_chunkWidth == null)
			_fiTileSystem_chunkWidth = _tyRotorzTileSystem.GetField("_chunkWidth", instanceBindingFlags);

		_fiTileSystem_chunkHeight = _tyRotorzTileSystem.GetField("chunkHeight", instanceBindingFlags);
		if (_fiTileSystem_chunkHeight == null)
			_fiTileSystem_chunkHeight = _tyRotorzTileSystem.GetField("_chunkHeight", instanceBindingFlags);

		_piTileSystem_chunks = _tyRotorzTileSystem.GetProperty("chunks", instanceBindingFlags);

		_piTileSystem_strippingPreset = _tyRotorzTileSystem.GetProperty("strippingPreset", instanceBindingFlags);
		_piTileSystem_strippingOptions = _tyRotorzTileSystem.GetProperty("strippingOptions", instanceBindingFlags);
		_fiTileSystem_applyRuntimeStripping = _tyRotorzTileSystem.GetField("applyRuntimeStripping", instanceBindingFlags);

		_fiTileSystem_combineChunkWidth = _tyRotorzTileSystem.GetField("combineChunkWidth", instanceBindingFlags);
		_fiTileSystem_combineChunkHeight = _tyRotorzTileSystem.GetField("combineChunkHeight", instanceBindingFlags);
		_fiTileSystem_combineMethod = _tyRotorzTileSystem.GetField("combineMethod", instanceBindingFlags);

		_fiTileSystem_generateSecondUVs = _tyRotorzTileSystem.GetField("generateSecondUVs", instanceBindingFlags);
		_fiTileSystem_hintEraseEmptyChunks = _tyRotorzTileSystem.GetField("hintEraseEmptyChunks", instanceBindingFlags);
		_fiTileSystem_staticVertexSnapping = _tyRotorzTileSystem.GetField("staticVertexSnapping", instanceBindingFlags);
	}

	#endregion

	/// <summary>
	/// Generate v2.x tile system and remove former v1.x tile system.
	/// </summary>
	/// <remarks>
	/// <para>Where possible previously painted game objects will be retained, however
	/// all chunk game objects will be replaced. Previously painted atlas tiles will
	/// be repainted using their tileset brush counterpart.</para>
	/// </remarks>
	/// <param name="v1">Old tile system.</param>
	public static void UpgradeTileSystem(MonoBehaviour v1) {
		PrepareReflection();

		if (RequiresIntermediateUpgrade(v1))
			UpgradeTileSystemA(v1);
		else
			UpgradeTileSystemB(v1);
	}

	/// <summary>
	/// Determines whether tile system needs to be upgraded to v1.0.9.
	/// </summary>
	/// <param name="system">Old tile system.</param>
	/// <returns>
	/// A value of <c>true</c> when tile system must be upgraded; otherwise <c>false</c>.
	/// </returns>
	private static bool RequiresIntermediateUpgrade(MonoBehaviour v1) {
		string systemVersionString = (string)_fiTileSystem_version.GetValue(v1);
		if (string.IsNullOrEmpty(systemVersionString))
			return true;

		System.Version systemVersion = new System.Version(systemVersionString);
		System.Version breakingVersion = new System.Version("1.0.9");

		return (systemVersion < breakingVersion);
	}

	#region Tile System v1.0.0-v1.0.8 to v2.0.0

	/// <summary>
	/// Upgrade tile system from v1.0.0-v1.0.8 to v2.0.0.
	/// </summary>
	/// <remarks>
	/// <para>Replicates upgrade process that was included in v1.0.9+ but converts straight
	/// to v2.0.0 instead of v1.0.9.</para>
	/// </remarks>
	/// <param name="v1">Old tile system.</param>
	public static void UpgradeTileSystemA(MonoBehaviour v1) {
		RtsUpgradedBrushMap map = RtsBrushUpgradeUtility.BrushMappings;

		EditorUtility.DisplayProgressBar("Upgrade Tile System", "Initializing new data structure...", 0.0f);
		try {
			PropertyInfo piTileData_hasGameObject = typeof(TileData).GetProperty("HasGameObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			Vector3 tileSize = (Vector3)_fiTileSystem_tileSize.GetValue(v1);
			int rows = (int)_fiTileSystem_rows.GetValue(v1);
			int columns = (int)_fiTileSystem_columns.GetValue(v1);

			// Create v2.x tile system.
			TileSystem v2 = v1.gameObject.AddComponent<TileSystem>();
			v2.CreateSystem(tileSize.x, tileSize.y, tileSize.z, rows, columns, 30, 30);
			CopyProperties(v1, v2);

			// Assume value that was consistent with original default settings
			v2.applyRuntimeStripping = true;
			v2.StrippingPreset = StrippingPreset.NoStripping;
			v2.BeginBulkEdit();
		
			Component[] instanceComponents = v1.GetComponentsInChildren(_tyTileInstance, true);

			float task = 0.0f;
			float taskCount = instanceComponents.Length;
			float taskRatio = 1.0f / taskCount;

			TileData tile = new TileData();

			// Retrieve all tile instance components
			foreach (MonoBehaviour instance in instanceComponents) {
				EditorUtility.DisplayProgressBar("Upgrade Tile System", "Processing tile data...", (task++) * taskRatio);

				int row = (int)_fiTileInstance_row.GetValue(instance);
				int column = (int)_fiTileInstance_column.GetValue(instance);

				tile.Clear();
				
				// Create and assign tile data
				tile.brush = map.Lookup((Object)_piTileInstance_brush.GetValue(instance, null));
				tile.orientationMask = (byte)OrientationUtility.MaskFromName((string)_fiTileInstance_orientationName.GetValue(instance));
				tile.variationIndex = (byte)(int)_fiTileInstance_variationIndex.GetValue(instance);
				tile.Empty = false;
				tile.gameObject = instance.gameObject;
				piTileData_hasGameObject.SetValue(tile, true, null);

				v2.SetTileFrom(row, column, tile);

				Chunk chunk = v2.GetChunkFromTileIndex(row, column);
				ForceRepaintForAtlasTiles(v2.GetTile(row, column), chunk);
				if (instance == null)
					continue;

				// Cleanup original tile instance?
				if (!StrippingUtility.StripEmptyGameObject(instance.transform)) {
					// Reparent game object to its shiny new chunk!
					instance.gameObject.transform.parent = chunk.transform;
				}

				// Destroy unwanted tile instance component
				Object.DestroyImmediate(instance);
			}

			int count = v2.EndBulkEdit();
			RemoveTileSystem(v1);

			if (count > 0)
				Debug.Log(string.Format("Upgrade of tile system '{0}' completed and {1} tile(s) were force refreshed.", v2.name, count));
			else
				Debug.Log(string.Format("Upgrade of tile system '{0}' completed.", v2.name));
		}
		finally {
			EditorUtility.ClearProgressBar();
		}
	}
	
	#endregion

	#region Tile System v1.0.9+ to v2.0.0

	/// <summary>
	/// Generate v2.0.0 tile system and remove former v1.0.9+ tile system.
	/// </summary>
	/// <remarks>
	/// <para>Where possible previously painted game objects will be retained, however
	/// all chunk game objects will be replaced. Previously painted atlas tiles will
	/// be repainted using their tileset brush counterpart.</para>
	/// </remarks>
	/// <param name="v1">Old tile system.</param>
	public static void UpgradeTileSystemB(MonoBehaviour v1) {
		GameObject go = v1.gameObject;

		int chunkWidth = 30, chunkHeight = 30;
		if (_fiTileSystem_chunkWidth != null) {
			chunkWidth = (int)_fiTileSystem_chunkWidth.GetValue(v1);
			chunkHeight = (int)_fiTileSystem_chunkHeight.GetValue(v1);
		}

		Vector3 tileSize = (Vector3)_fiTileSystem_tileSize.GetValue(v1);
		int rows = (int)_fiTileSystem_rows.GetValue(v1);
		int columns = (int)_fiTileSystem_columns.GetValue(v1);

		// Create v2.x tile system.
		TileSystem v2 = go.AddComponent<TileSystem>();
		v2.CreateSystem(tileSize.x, tileSize.y, tileSize.z, rows, columns, chunkWidth, chunkHeight);
		
		CopyProperties(v1, v2);
		CopyTileData(v1, v2);
		RemoveTileSystem(v1);
	}
	
	/// <summary>
	/// Copy properties from v1.x tile system component into v2.x tile system component.
	/// </summary>
	/// <param name="v1">Old tile system.</param>
	/// <param name="v2">New tile system.</param>
	private static void CopyProperties(MonoBehaviour v1, TileSystem v2) {
		v2.activeColumn = (int)_fiTileSystem_activeColumn.GetValue(v1);
		v2.activeRow = (int)_fiTileSystem_activeRow.GetValue(v1);

		if (_fiTileSystem_applyRuntimeStripping != null)
			v2.applyRuntimeStripping = (bool)_fiTileSystem_applyRuntimeStripping.GetValue(v1);

		if (_fiTileSystem_combineChunkWidth != null) {
			v2.combineChunkWidth = (int)_fiTileSystem_combineChunkWidth.GetValue(v1);
			v2.combineChunkHeight = (int)_fiTileSystem_combineChunkHeight.GetValue(v1);
		}

		if (_fiTileSystem_combineMethod != null)
			v2.combineMethod = (BuildCombineMethod)_fiTileSystem_combineMethod.GetValue(v1);
		if (_fiTileSystem_generateSecondUVs != null)
			v2.generateSecondUVs = (bool)_fiTileSystem_generateSecondUVs.GetValue(v1);
		if (_fiTileSystem_hintEraseEmptyChunks != null)
			v2.hintEraseEmptyChunks = (bool)_fiTileSystem_hintEraseEmptyChunks.GetValue(v1);
		if (_fiTileSystem_staticVertexSnapping != null)
			v2.staticVertexSnapping = (bool)_fiTileSystem_staticVertexSnapping.GetValue(v1);

		v2.pregenerateProcedural = false;

		if (_piTileSystem_strippingPreset != null) {
			v2.StrippingPreset = (StrippingPreset)_piTileSystem_strippingPreset.GetValue(v1, null);
			if (v2.StrippingPreset == StrippingPreset.Custom)
				v2.StrippingOptions = (int)_piTileSystem_strippingOptions.GetValue(v1, null);
		}
	}
	
	/// <summary>
	/// Copy tile data from v1.x tile system to v2.x tile system.
	/// </summary>
	/// <param name="v1">Old tile system.</param>
	/// <param name="v2">New tile system.</param>
	private static void CopyTileData(MonoBehaviour v1, TileSystem v2) {
		RtsUpgradedBrushMap map = RtsBrushUpgradeUtility.BrushMappings;

		FieldInfo fiFlagsV1 = (_tyTileData != null)
			? _tyTileData.GetField("_flags", BindingFlags.NonPublic | BindingFlags.Instance)
			: null;
		FieldInfo fiFlagsV2 = typeof(TileData).GetField("_flags", BindingFlags.NonPublic | BindingFlags.Instance);
		
		TileData newTile = new TileData();

		int rows = (int)_fiTileSystem_rows.GetValue(v1);
		int columns = (int)_fiTileSystem_columns.GetValue(v1);

		for (int row = 0; row < rows; ++row) {
			for (int column = 0; column < columns; ++column) {
				object oldTile = _miTileSystem_GetTile.Invoke(v1, new object[] { row, column });
				if (oldTile == null)
					continue;
				
				if (fiFlagsV1 != null)
					fiFlagsV2.SetValue(newTile, fiFlagsV1.GetValue(oldTile));
				
				// Prepare new tile from old tile.
				newTile.brush = map.Lookup((Object)_fiTileData_brush.GetValue(oldTile));
				newTile.gameObject = (GameObject)_fiTileData_gameObject.GetValue(oldTile);
				newTile.orientationMask = (byte)_fiTileData_orientationMask.GetValue(oldTile);
				newTile.variationIndex = (byte)(int)_fiTileData_variationIndex.GetValue(oldTile);
				newTile.Empty = false;
				
				v2.SetTileFrom(row, column, newTile);
				Chunk chunk = v2.GetChunkFromTileIndex(row, column);

				ForceRepaintForAtlasTiles(newTile, chunk);

				if (newTile.gameObject != null) {
					// Transfer ownership of attached game object.
					newTile.gameObject.transform.parent = chunk.transform;
				}
			}
		}
		
		// Some tiles might need to be refreshed!
		v2.RefreshAllTiles(
			  RefreshFlags.PreservePaintedFlags
			| RefreshFlags.PreserveTransform
			| RefreshFlags.UpdateProcedural
			);
	}

	#endregion

	private static void ForceRepaintForAtlasTiles(TileData tile, Chunk chunk) {
		bool repaint = (tile.brush is TilesetBrush || (tile.AliasBrush != null && tile.AliasBrush.target is TilesetBrush));

		// Tiles that contain generated mesh assets must be repainted!
		if (!repaint && tile.gameObject != null) {
			MeshFilter filter = tile.gameObject.GetComponent<MeshFilter>();
			if (filter != null)
				repaint = AssetDatabase.GetAssetPath(filter.sharedMesh).StartsWith("Assets/Rotorz Tile System/Generated/");
		}
		
		// Tileset tiles must be repainted!
		if (repaint) {
			// Destroy the attached game object so that it can be repainted.
			Object.DestroyImmediate(tile.gameObject);
			tile.gameObject = null;
			tile.Dirty = true;
			chunk.Dirty = true;
		}
	}
	
	/// <summary>
	/// Remove v1.x tile system component and any chunk game objects.
	/// </summary>
	/// <param name="v1">Old tile system.</param>
	private static void RemoveTileSystem(MonoBehaviour v1) {
		// Remove chunk objects.
		if (_piTileSystem_chunks != null) {
			MonoBehaviour[] chunks = (MonoBehaviour[])_piTileSystem_chunks.GetValue(v1, null);
			foreach (MonoBehaviour chunk in chunks)
				if (chunk != null)
					Object.DestroyImmediate(chunk.gameObject);
		}

		// Remove tile system component.
		Object.DestroyImmediate(v1);
	}

}
