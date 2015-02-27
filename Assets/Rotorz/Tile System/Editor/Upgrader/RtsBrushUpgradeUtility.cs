// Copyright (c) 2011-2014 Rotorz Limited. All rights reserved.

using UnityEngine;
using UnityEditor;

using System.Reflection;
using System.IO;
using System.Xml.Xsl;
using System.Xml.XPath;

using Type = System.Type;

using Rotorz.Tile;
using Rotorz.Tile.Editor;

/// <summary>
/// Utility class that upgrades a v1.x brushes into v2.x brushes.
/// </summary>
public sealed class RtsBrushUpgradeUtility {
	
	/// <summary>
	/// Gets an object that maps older brushes to their upgraded counterparts.
	/// </summary>
	/// <remarks>
	/// <para>Asset is automatically created if it does not already exist.</para>
	/// </remarks>
	public static RtsUpgradedBrushMap BrushMappings {
		get {
			RtsUpgradedBrushMap map = RtsUpgradedBrushMap.BrushMappings;

			if (map == null) {
				BrushUtility.GetBrushAssetPath();

				map = ScriptableObject.CreateInstance<RtsUpgradedBrushMap>();
				AssetDatabase.CreateAsset(map, "Assets/TileBrushes/BrushMappingsV1toV2.asset");
				AssetDatabase.ImportAsset("Assets/TileBrushes/BrushMappingsV1toV2.asset");

				RtsUpgradedBrushMap._brushMap = map;
				map.Cleanup();
			}

			return map;
		}
	}

	#region Reflection

	// Brush types
	private Type _tyBasicTileBrush;
	private Type _tyOrientedTileBrush;
	private Type _tyAliasTileBrush;
	private Type _tyAtlasTileBrush;
	private Type _tyEmptyTileBrush;

	private Type _tyTileBrush;

	// Tile Brush
	private FieldInfo _fiTileBrush_hideBrush;
	private FieldInfo _fiTileBrush_tileGroup;
	private FieldInfo _fiTileBrush_userFlags;
	private FieldInfo _fiTileBrush_overrideTag;
	private FieldInfo _fiTileBrush_overrideLayer;
	private FieldInfo _fiTileBrush_category;
	private FieldInfo _fiTileBrush_applyPrefabTransform;
	private FieldInfo _fiTileBrush_scaleMode;
	private FieldInfo _fiTileBrush_smooth;
	private FieldInfo _fiTileBrush_materialMapFrom;
	private FieldInfo _fiTileBrush_materialMapTo;

	private FieldInfo _fiTileBrush_coalesce;
	private FieldInfo _fiTileBrush_coalesceTileGroup;

	// Oriented Tile Brush
	private FieldInfo _fiOrientedTileBrush_defaultOrientation;
	private FieldInfo _fiOrientedTileBrush_fallbackMode;

	private FieldInfo _fiOrientedTileBrush_forceOverrideFlags;

	private Type _tyTileBrushOrientation;
	private FieldInfo _fiTileBrushOrientation_variations;

	// Alias Tile Brush
	private FieldInfo _fiAliasTileBrush_overrideFlags;
	private FieldInfo _fiAliasTileBrush_overrideTransforms;
	private FieldInfo _fiAliasTileBrush_aliasOf;

	// Atlas Tile Brush
	private FieldInfo _fiAtlasTileBrush_atlasTexture;
	private FieldInfo _fiAtlasTileBrush_atlasTileWidth;
	private FieldInfo _fiAtlasTileBrush_atlasTileHeight;
	private FieldInfo _fiAtlasTileBrush_atlasRow;
	private FieldInfo _fiAtlasTileBrush_atlasColumn;

	// New Base Brush
	private FieldInfo _fiBrush_userFlags;

	/// <summary>
	/// Use reflection to access properties that may or may not be defined.
	/// </summary>
	private void PrepareReflection() {
		BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		// Brush types
		_tyBasicTileBrush = Type.GetType("Rotorz.TileSystem.BasicTileBrush,RotorzTileSystem");
		_tyOrientedTileBrush = Type.GetType("Rotorz.TileSystem.OrientedTileBrush,RotorzTileSystem");
		_tyAliasTileBrush = Type.GetType("Rotorz.TileSystem.AliasTileBrush,RotorzTileSystem");
		_tyAtlasTileBrush = Type.GetType("Rotorz.TileSystem.AtlasTileBrush,RotorzTileSystem");
		_tyEmptyTileBrush = Type.GetType("Rotorz.TileSystem.EmptyTileBrush,RotorzTileSystem");

		// Note: `BasicTileBrush` used to be the base class, then it was later changed
		//		 to `TileBrush`, so this may vary now!
		_tyTileBrush = Type.GetType("Rotorz.TileSystem.TileBrush,RotorzTileSystem");
		if (_tyTileBrush == null)
			_tyTileBrush = _tyBasicTileBrush;

		// Tile Brush
		if (_tyTileBrush != null) {
			_fiTileBrush_hideBrush = _tyTileBrush.GetField("hideBrush", instanceBindingFlags);
			_fiTileBrush_tileGroup = _tyTileBrush.GetField("tileGroup", instanceBindingFlags);
			_fiTileBrush_userFlags = _tyTileBrush.GetField("_userFlags", instanceBindingFlags);
			_fiTileBrush_overrideTag = _tyTileBrush.GetField("overrideTag", instanceBindingFlags);
			_fiTileBrush_overrideLayer = _tyTileBrush.GetField("overrideLayer", instanceBindingFlags);
			_fiTileBrush_category = _tyTileBrush.GetField("category", instanceBindingFlags);
			_fiTileBrush_applyPrefabTransform = _tyTileBrush.GetField("applyPrefabTransform", instanceBindingFlags);
			_fiTileBrush_scaleMode = _tyTileBrush.GetField("scaleMode", instanceBindingFlags);
			_fiTileBrush_smooth = _tyTileBrush.GetField("smooth", instanceBindingFlags);
			_fiTileBrush_materialMapFrom = _tyTileBrush.GetField("materialMapFrom", instanceBindingFlags);
			_fiTileBrush_materialMapTo = _tyTileBrush.GetField("materialMapTo", instanceBindingFlags);
			
			_fiTileBrush_coalesce = _tyTileBrush.GetField("coalesce", instanceBindingFlags);
			_fiTileBrush_coalesceTileGroup = _tyTileBrush.GetField("coalesceTileGroup", instanceBindingFlags);
		}

		// Oriented Tile Brush
		if (_tyOrientedTileBrush != null) {
			_fiOrientedTileBrush_defaultOrientation = _tyOrientedTileBrush.GetField("defaultOrientation", instanceBindingFlags);
			_fiOrientedTileBrush_fallbackMode = _tyOrientedTileBrush.GetField("fallbackMode", instanceBindingFlags);

			_fiOrientedTileBrush_forceOverrideFlags = _tyOrientedTileBrush.GetField("forceOverrideFlags", instanceBindingFlags);
		}

		_tyTileBrushOrientation = Type.GetType("Rotorz.TileSystem.TileBrushOrientation,RotorzTileSystem");
		if (_tyTileBrushOrientation != null) {
			_fiTileBrushOrientation_variations = _tyTileBrushOrientation.GetField("variations", instanceBindingFlags);
		}

		// Alias Tile Brush
		if (_tyAliasTileBrush != null) {
			_fiAliasTileBrush_overrideFlags = _tyAliasTileBrush.GetField("overrideFlags", instanceBindingFlags);
			_fiAliasTileBrush_overrideTransforms = _tyAliasTileBrush.GetField("overrideTransforms", instanceBindingFlags);
			_fiAliasTileBrush_aliasOf = _tyAliasTileBrush.GetField("aliasOf", instanceBindingFlags);
		}

		// Atlas Tile Brush
		if (_tyAtlasTileBrush != null) {
			_fiAtlasTileBrush_atlasTexture = _tyAtlasTileBrush.GetField("atlasTexture", instanceBindingFlags);
			_fiAtlasTileBrush_atlasTileWidth = _tyAtlasTileBrush.GetField("atlasTileWidth", instanceBindingFlags);
			_fiAtlasTileBrush_atlasTileHeight = _tyAtlasTileBrush.GetField("atlasTileHeight", instanceBindingFlags);
			_fiAtlasTileBrush_atlasRow = _tyAtlasTileBrush.GetField("atlasRow", instanceBindingFlags);
			_fiAtlasTileBrush_atlasColumn = _tyAtlasTileBrush.GetField("atlasColumn", instanceBindingFlags);
		}

		// New Base Brush
		_fiBrush_userFlags = typeof(Brush).GetField("_userFlags", instanceBindingFlags);
	}
	
	#endregion

	#region Utility Methods

	/// <summary>
	/// Gets unique asset path for a migrated asset.
	/// </summary>
	/// <remarks>
	/// <para>Creates the path "Assets/TileBrushes/Migrated" if it does not already
	/// exist. The resulting asset path can be used to save assets that are generated
	/// during the upgrade process.</para>
	/// <para>For example, previously additional components could be added to basic
	/// brush prefabs which would later be added to tiles as they are painted. In v2.x
	/// this is now achieved by attaching a prefab to the brush. So, attachment prefabs
	/// must be created and attached to the upgraded brushes.</para>
	/// </remarks>
	/// <param name="assetName">Name for asset.</param>
	/// <returns>
	/// The unique asset path.
	/// </returns>
	private static string GetUniqueMigratedPath(string assetName) {
		BrushUtility.GetBrushAssetPath();
		
		// First make sure that the "Migrated" folder exists!
		if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Assets/TileBrushes/Migrated"))
			AssetDatabase.CreateFolder("Assets/TileBrushes", "Migrated");
		
		return AssetDatabase.GenerateUniqueAssetPath("Assets/TileBrushes/Migrated/" + assetName);
	}
	
	#endregion
	
	#region Upgrade Process
	
	private int _taskCount;
	private int _tasksCompleted;
	private float _taskRatio;
	
	/// <summary>
	/// Indicates if new assets should be used where possible.
	/// </summary>
	/// <remarks>
	/// <para>For example, both v1.x and v2.x include a "Default" material,
	/// though in many cases this can be automatically reassigned when upgrading
	/// brush assets.</para>
	/// </remarks>
	public bool useNewAssets = true;
	
	/// <summary>
	/// Upgrade brushes created using v1.x brush components to v2.x brush assets.
	/// </summary>
	public void UpgradeBrushes() {
		PrepareReflection();

		// Rescan old brush database just in case user is experimenting with
		// deleting upgraded brushes, creating new ones, and then performing
		// upgrade again.
		RtsBrushDatabaseWrapper.Instance.Rescan();

		BeginBulkEdit();
		
		try {
			CopyBrushCategoryData();
			CopyPresetFile();
			
			// Ensure that brush mappings asset is available.
			RtsUpgradedBrushMap map = BrushMappings;
			
			// Count of tasks that will be indicated to user using progress bar.
			_taskCount = RtsBrushDatabaseWrapper.Instance.records.Count * 5;
			_tasksCompleted = 0;
			_taskRatio = 1.0f / (float)_taskCount;
			
			GenerateTilesetsForUpgradedBrushes();
			
			// Do not upgrade oriented or alias brushes yet because those types
			// of brush may require that other brushes are upgraded beforehand.
			foreach (RtsBrushAssetRecordWrapper record in RtsBrushDatabaseWrapper.Instance.records) {
				++_tasksCompleted;
			
				// Skip if invalid or brush has already been upgraded.
				if (record.brush == null || map.Lookup(record.brush) != null)
					continue;
				
				Type brushType = record.brush.GetType();
				if (brushType == _tyOrientedTileBrush)
					continue; // Save oriented brushes until last!
				else if (brushType == _tyAliasTileBrush)
					continue; // Do these afterwards!
				else if (brushType == _tyAtlasTileBrush)
					UpgradeAtlasBrush(record);
				else if (brushType == _tyBasicTileBrush)
					UpgradeBasicBrush(record);
				else if (brushType == _tyEmptyTileBrush)
					UpgradeEmptyBrush(record);
				else
					Debug.LogError(string.Format("Cannot upgrade unknown brush type '{0}'.", record.brush.GetType().FullName));
				
				DisplayProgress("Upgrading primative brushes.");
			}
			
			// Upgrade alias brushes that do not target oriented brushes.
			// Reason: Alias brushes can be nested inside oriented brushes.
			foreach (RtsBrushAssetRecordWrapper record in RtsBrushDatabaseWrapper.Instance.records) {
				++_tasksCompleted;
			
				// Skip if invalid or brush has already been upgraded.
				if (record.brush == null || map.Lookup(record.brush) != null)
					continue;
				
				if (record.brush.GetType() == _tyAliasTileBrush) {
					object aliasOf = _fiAliasTileBrush_aliasOf.GetValue(record.brush);
					if (aliasOf != null && aliasOf.GetType() != _tyOrientedTileBrush)
						UpgradeAliasBrush(record);
				}
				
				DisplayProgress("Upgrading alias brushes.");
			}
			
			// Upgrade oriented brushes!
			foreach (RtsBrushAssetRecordWrapper record in RtsBrushDatabaseWrapper.Instance.records) {
				++_tasksCompleted;
			
				// Skip if invalid or brush has already been upgraded.
				if (record.brush == null || map.Lookup(record.brush) != null)
					continue;

				if (record.brush.GetType() == _tyOrientedTileBrush)
					UpgradeOrientedBrush(record);
				
				DisplayProgress("Upgrading oriented brushes.");
			}
			
			// Finally upgrade alias brushes that target oriented brushes.
			foreach (RtsBrushAssetRecordWrapper record in RtsBrushDatabaseWrapper.Instance.records) {
				++_tasksCompleted;
			
				// Skip if invalid or brush has already been upgraded.
				if (record.brush == null || map.Lookup(record.brush) != null)
					continue;

				if (record.brush.GetType() == _tyAliasTileBrush) {
					object aliasOf = _fiAliasTileBrush_aliasOf.GetValue(record.brush);
					if (aliasOf != null && aliasOf.GetType() == _tyOrientedTileBrush)
						UpgradeAliasBrush(record);
				}
				
				DisplayProgress("Upgrading oriented brushes.");
			}
			
			// The brush map likely needs to be saved now.
			EditorUtility.SetDirty(map);
		}
		finally {
			// Make sure that the progress bar is hidden before leaving method!
			EditorUtility.ClearProgressBar();
			// Finish bulk editing regardless of whether an exception has occurred.
			EndBulkEdit();
		}
	}

	/// <summary>
	/// Display and update progress bar.
	/// </summary>
	/// <param name="message">Message to display in progress bar window.</param>
	private void DisplayProgress(string message) {
		EditorUtility.DisplayProgressBar("Upgrade Brushes", message, (float)_tasksCompleted * _taskRatio);
	}
	
	/// <summary>
	/// Begins bulk editing of brush assets.
	/// </summary>
	/// <remarks>
	/// <para>Helps to improve performance of bulk creating brush assets by
	/// avoiding repetitive saves and rescanning the brush database.</para>
	/// <para>Uses reflection to access bulk editing functionality provided by
	/// Rotorz Tile System v2.0.0.</para>
	/// </remarks>
	private void BeginBulkEdit() {
		MethodInfo mi = typeof(BrushUtility).GetMethod("BeginBulkEdit", BindingFlags.NonPublic | BindingFlags.Static);
		mi.Invoke(null, null);
	}
	/// <summary>
	/// Ends bulk editing of brush assets.
	/// </summary>
	/// <remarks>
	/// <para>Uses reflection to access bulk editing functionality provided by
	/// Rotorz Tile System v2.0.0.</para>
	/// </remarks>
	private void EndBulkEdit() {
		MethodInfo mi = typeof(BrushUtility).GetMethod("EndBulkEdit", BindingFlags.NonPublic | BindingFlags.Static);
		mi.Invoke(null, null);
	}
	
	#endregion
	
	#region User Configuration Files
	
	/// <summary>
	/// Copies the data asset that stores the list of user defined brush categories.
	/// </summary>
	private void CopyBrushCategoryData() {
		if (File.Exists(Directory.GetCurrentDirectory() + "/Assets/TilePrefabs/brush.cats")) {
			string brushAssetPath = BrushUtility.GetBrushAssetPath();
			
			// Do not copy categories data file if one already exists.
			// i.e. If categories have been defined using the v2.x user interface.
			if (!File.Exists(Directory.GetCurrentDirectory() + "/" + brushAssetPath + "Assets/TilePrefabs/categories.data"))
				AssetDatabase.CopyAsset("Assets/TilePrefabs/brush.cats", brushAssetPath + "categories.data");
		}
	}
	
	/// <summary>
	/// Copies the create tile system preset file.
	/// </summary>
	private void CopyPresetFile() {
		string sourcePresetsPath = Directory.GetCurrentDirectory() + "/Assets/Rotorz Tile System/prefs.xml";
		if (File.Exists(sourcePresetsPath)) {
			// Do not copy preset file if one already exists.
			// i.e. If presets have been defined using the v2.x user interface.
			string outputPresetsPath = Directory.GetCurrentDirectory() + "/Assets/Rotorz/Tile System/preset.xml";
			if (File.Exists(outputPresetsPath))
				return;

			// Load transform stylesheet
			string transformPath = Directory.GetCurrentDirectory() + "/Assets/Rotorz/Tile System/Editor/Upgrader/UpgradePresets.xsl";
			if (!File.Exists(transformPath))
				return;
			XslCompiledTransform xslt = new XslCompiledTransform();
			xslt.Load(transformPath);

			using (FileStream fs = new FileStream(outputPresetsPath, FileMode.Create)) {
				xslt.Transform(new XPathDocument(sourcePresetsPath), null, fs);
			}
		}
	}
	
	#endregion
	
	#region Tilesets
	
	/// <summary>
	/// Indicates if procedural tilesets should be used.
	/// </summary>
	/// <remarks>
	/// <para>Defaults to non-procedural because that was the nature of v1.x
	/// for which people have become accustomed to. Though the default for
	/// new tilesets is now procedural.</para>
	/// </remarks>
	public bool useProceduralTilesets = false;
	
	/// <summary>
	/// Generates tileset assets by looking at previously defined atlas brushes.
	/// </summary>
	private void GenerateTilesetsForUpgradedBrushes() {
		RtsUpgradedBrushMap map = RtsUpgradedBrushMap.BrushMappings;

		foreach (RtsBrushAssetRecordWrapper record in RtsBrushDatabaseWrapper.Instance.records) {
			++_tasksCompleted;
			
			// Skip if invalid or brush has already been upgraded.
			if (record.brush == null || map.Lookup(record.brush) != null)
				continue;
			
			DisplayProgress("Generating tilesets from atlas brushes.");
			
			if (record.brush.GetType() == _tyAtlasTileBrush)
				GenerateTilesetFromAtlasBrush(record.brush);
		}
		
		// Need to rescan brush database so that tilesets can be accessed when
		// upgrading atlas brushes to tileset brushes.
		BrushDatabase.Instance.Rescan();
	}
	
	/// <summary>
	/// Generate tileset using an atlas brush.
	/// </summary>
	/// <remarks>
	/// <para>One tileset asset is generated for each unique atlas material that
	/// exists amongst all defined atlas brushes.</para>
	/// </remarks>
	/// <param name="brush">Existing atlas brush.</param>
	/// <returns>
	/// The tileset.
	/// </returns>
	private Tileset GenerateTilesetFromAtlasBrush(MonoBehaviour brush) {
		MeshRenderer renderer = brush.GetComponent<MeshRenderer>();
		if (renderer == null)
			return null;
		
		Material mat = renderer.sharedMaterial;
		if (mat == null)
			return null;

		Texture2D atlasTexture = _fiAtlasTileBrush_atlasTexture.GetValue(brush) as Texture2D;
		if (atlasTexture == null)
			return null;

		RtsUpgradedBrushMap map = RtsUpgradedBrushMap.BrushMappings;

		Tileset tileset = map.Lookup(mat);
		if (tileset != null)
			return tileset;
		
		// Create folder for atlas assets
		string atlasFolder = AssetDatabase.GenerateUniqueAssetPath(BrushUtility.GetBrushAssetPath() + atlasTexture.name);
		Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/" + atlasFolder);

		// Create material for tileset
		Material material = Object.Instantiate(mat) as Material;
		material.mainTexture = atlasTexture;
		
		AssetDatabase.CreateAsset(material, atlasFolder + "/atlas.mat");
		AssetDatabase.ImportAsset(atlasFolder + "/atlas.mat");

		int atlasTileWidth = (int)_fiAtlasTileBrush_atlasTileWidth.GetValue(brush);
		int atlasTileHeight = (int)_fiAtlasTileBrush_atlasTileHeight.GetValue(brush);
		
		// Calculate metrics for tileset
		TilesetMetrics metrics = new TilesetMetrics(atlasTexture, atlasTileWidth, atlasTileHeight, 0, 0.5f);

		// Create new tileset asset.
		tileset = ScriptableObject.CreateInstance<Tileset>();
		tileset.Initialize(material, atlasTexture, metrics);
		tileset.procedural = useProceduralTilesets;
		
		// Save tileset asset.
		string assetPath = atlasFolder + "/" + atlasTexture.name + ".set.asset";
		AssetDatabase.CreateAsset(tileset, assetPath);
		AssetDatabase.ImportAsset(assetPath);

		map.SetMapping(mat, tileset);
		return tileset;
	}
	
	#endregion
	
	#region Brushes
	
	/// <summary>
	/// Upgrades basic brush (v1.x) into an oriented brush (v2.x).
	/// </summary>
	/// <remarks>
	/// <para>The concept of basic brushes was removed in v2.x of Rotorz Tile System.
	/// Instead the default orientation of an oriented brush can be used.</para>
	/// </remarks>
	/// <param name="record">Record for existing v1.x brush.</param>
	private void UpgradeBasicBrush(RtsBrushAssetRecordWrapper record) {
		// We need to take a copy of the original brush because that is the
		// tile prefab, but we'll remove the original brush component.
		GameObject tilePrefabCopyGO = PrefabUtility.InstantiatePrefab(record.brush.gameObject) as GameObject;
		GameObject newTilePrefab = null;
		
		try {
			Object.DestroyImmediate(tilePrefabCopyGO.GetComponent(_tyTileBrush));
			
			// Create a new prefab for this prefab.
			newTilePrefab = PrefabUtility.CreatePrefab(GetUniqueMigratedPath(record.displayName + ".prefab"), tilePrefabCopyGO);
		}
		finally {
			// Destroy the temporary copy.
			Object.DestroyImmediate(tilePrefabCopyGO);
		}
		
		if (newTilePrefab == null) {
			Debug.LogError(string.Format("An error occurred whilst upgrading brush '{0}' of type '{1}'.", record.displayName, record.brush.GetType().FullName));
			return;
		}
		
		// Basic brushes will now become oriented brushes!
		OrientedBrush newBrush = BrushUtility.CreateOrientedBrush(record.displayName);
		newBrush.DefaultOrientation.AddVariation(newTilePrefab);
		
		if (_fiTileBrush_coalesce != null)
			newBrush.Coalesce = (Coalesce)_fiTileBrush_coalesce.GetValue(record.brush);
		if (_fiTileBrush_coalesceTileGroup != null)
			newBrush.CoalesceWithBrushGroups.Add((int)_fiTileBrush_coalesceTileGroup.GetValue(record.brush));
		
		FinalizeStandaloneBrush(record, newBrush);
		
		// Do not override tag and layer by default.
		newBrush.overrideTag = false;
		newBrush.overrideLayer = false;
	}
	
	/// <summary>
	/// Upgrades oriented brush (v1.x) into an oriented brush (v2.x).
	/// </summary>
	/// <param name="record">Record for existing v1.x brush.</param>
	private void UpgradeOrientedBrush(RtsBrushAssetRecordWrapper record) {
		if (useNewAssets) {
			// Is this the original "Smooth Platform" master brushes?
			if (record.assetPath == "Assets/Rotorz Tile System/Brushes/Smooth Platform.prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/Brushes/Smooth Platform (Join).prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/Brushes/Smooth Platform (Small).prefab")
			{
				// This is a special case that can be switched to new version
				// if it was imported.
				OrientedBrush newMasterBrush = AssetDatabase.LoadAssetAtPath("Assets/Rotorz/Tile System/TileBrushes/Master/" + record.displayName + ".brush.asset", typeof(OrientedBrush)) as OrientedBrush;
				if (newMasterBrush != null) {
					RtsUpgradedBrushMap.BrushMappings.SetMapping(record.brush, newMasterBrush);
					return;
				}
				
				// If new version was not found then just proceed to upgrade
				// existing version.
			}
			// Is this one of the original demo brushes?
			else if (record.assetPath == "Assets/Rotorz Tile System/_Demos/TilePrefabs/Diamond.prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/_Demos/TilePrefabs/Hat Guy.prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/_Demos/TilePrefabs/Steel Brick.prefab")
			{
				// This is a special case that can be switched to new version
				// if it was imported.
				OrientedBrush newDemoBrush = AssetDatabase.LoadAssetAtPath("Assets/Rotorz/Tile System/Demo/Hat Guy/TileBrushes/" + record.displayName + ".brush.asset", typeof(OrientedBrush)) as OrientedBrush;
				if (newDemoBrush != null) {
					RtsUpgradedBrushMap.BrushMappings.SetMapping(record.brush, newDemoBrush);
					return;
				}
				
				// If new version was not found then just proceed to upgrade
				// existing version.
			}
		}
		
		Transform oldBrushTransform = record.brush.transform;
		
		OrientedBrush newBrush = BrushUtility.CreateOrientedBrush(record.displayName);
		newBrush.RemoveOrientation(newBrush.DefaultOrientationMask);

		RtsUpgradedBrushMap map = RtsUpgradedBrushMap.BrushMappings;

		// Copy orientations from old brush.
		for (int ti = 0; ti < oldBrushTransform.childCount; ++ti) {
			MonoBehaviour oldOrientation = oldBrushTransform.GetChild(ti).GetComponent(_tyTileBrushOrientation) as MonoBehaviour;
			if (oldOrientation == null)
				continue;

			Object[] oldVariations = (Object[])_fiTileBrushOrientation_variations.GetValue(oldOrientation);

			BrushOrientation newOrientation = newBrush.AddOrientation(OrientationUtility.MaskFromName(oldOrientation.name));
			
			for (int i = 0; i < oldVariations.Length; ++i) {
				if (oldVariations[i] == null)
					continue;

				Type variationType = oldVariations[i].GetType();
				GameObject variationGO = oldVariations[i] as GameObject;
				Object variationBrush = null;

				// If game object is nested, check if it is a tile brush prefab!
				if (variationGO != null) {
					variationBrush = variationGO.GetComponent(_tyTileBrush);
				}
				// If variation is a tile brush then...
				else if (_tyTileBrush.IsAssignableFrom(variationType)) {
					variationBrush = oldVariations[i];
				}

				// Need to isolate nested brushes!
				if (variationBrush != null) {
					// Note: This only works because oriented brushes are processed last,
					//		 and it is not possible to nest oriented brushes.
					
					// Use new version of brush!
					Brush replacementBrush = map.Lookup(variationBrush);
					if (replacementBrush != null)
						newOrientation.AddVariation(replacementBrush);
				}
				else if (variationGO != null) {
					newOrientation.AddVariation(variationGO);
				}
			}
		}

		newBrush.DefaultOrientationMask = OrientationUtility.MaskFromName((string)_fiOrientedTileBrush_defaultOrientation.GetValue(record.brush));
		newBrush.FallbackMode = (FallbackMode)_fiOrientedTileBrush_fallbackMode.GetValue(record.brush);

		if (_fiTileBrush_coalesce != null)
			newBrush.Coalesce = (Coalesce)_fiTileBrush_coalesce.GetValue(record.brush);
		if (_fiTileBrush_coalesceTileGroup != null)
			newBrush.CoalesceWithBrushGroups.Add((int)_fiTileBrush_coalesceTileGroup.GetValue(record.brush));
		
		if (_fiOrientedTileBrush_forceOverrideFlags != null)
			newBrush.forceOverrideFlags = (bool)_fiOrientedTileBrush_forceOverrideFlags.GetValue(record.brush);
		
		FinalizeStandaloneBrush(record, newBrush);
	}
	
	/// <summary>
	/// Upgrades alias brush (v1.x) into an alias brush (v2.x).
	/// </summary>
	/// <param name="record">Record for existing v1.x brush.</param>
	private void UpgradeAliasBrush(RtsBrushAssetRecordWrapper record) {
		AliasBrush newBrush;
		
		if (useNewAssets) {
			// Is this the original "Grass Block" or "Cave Block" example brushes?
			if (record.assetPath == "Assets/Rotorz Tile System/TilePrefabs/Cave Block.prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/TilePrefabs/Cave Block (Join).prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/TilePrefabs/Cave Block (Small).prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/TilePrefabs/Grass Block.prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/TilePrefabs/Grass Block (Join).prefab"
				|| record.assetPath == "Assets/Rotorz Tile System/TilePrefabs/Grass Block (Small).prefab")
			{
				// This is a special case that can be switched to new version
				// if it was imported.
				newBrush = AssetDatabase.LoadAssetAtPath("Assets/Rotorz/Tile System/Demo/TileBrushes/" + record.displayName.Replace("Block", "Platform") + ".brush.asset", typeof(AliasBrush)) as AliasBrush;
				if (newBrush != null) {
					RtsUpgradedBrushMap.BrushMappings.SetMapping(record.brush, newBrush);
					return;
				}
				
				// If new version was not found then just proceed to upgrade
				// existing version.
			}
		}

		Brush targetBrush = RtsUpgradedBrushMap.BrushMappings.Lookup((Object)_fiAliasTileBrush_aliasOf.GetValue(record.brush));
		newBrush = BrushUtility.CreateAliasBrush(record.displayName, targetBrush);

		if (_fiTileBrush_coalesce != null)
			newBrush.Coalesce = (Coalesce)_fiTileBrush_coalesce.GetValue(record.brush);
		if (_fiTileBrush_coalesceTileGroup != null)
			newBrush.CoalesceWithBrushGroups.Add((int)_fiTileBrush_coalesceTileGroup.GetValue(record.brush));
		
		if (_fiAliasTileBrush_overrideFlags != null)
			newBrush.overrideFlags = (bool)_fiAliasTileBrush_overrideFlags.GetValue(record.brush);
		if (_fiAliasTileBrush_overrideTransforms != null)
			newBrush.overrideTransforms = (bool)_fiAliasTileBrush_overrideTransforms.GetValue(record.brush);
		
		FinalizeStandaloneBrush(record, newBrush);
	}
	
	/// <summary>
	/// Upgrades atlas brush (v1.x) into a tileset brush (v2.x).
	/// </summary>
	/// <remarks>
	/// <para>These are now called tileset brushes because they utilise a central
	/// tileset asset which makes it easier to manage such brushes.</para>
	/// </remarks>
	/// <param name="record">Record for existing v1.x brush.</param>
	private void UpgradeAtlasBrush(RtsBrushAssetRecordWrapper record) {
		MonoBehaviour oldBrush = record.brush;
		Transform oldTransform = oldBrush.transform;
		
		// Cannot upgrade atlas tile brush if tileset was not generated
		Tileset tileset = GenerateTilesetFromAtlasBrush(oldBrush);
		if (tileset == null) {
			Debug.LogError(string.Format("Could not generate tileset for atlas brush '{0}'.", oldBrush.name));
			return;
		}

		Texture2D atlasTexture = (Texture2D)_fiAtlasTileBrush_atlasTexture.GetValue(oldBrush);
		int atlasTileWidth = (int)_fiAtlasTileBrush_atlasTileWidth.GetValue(oldBrush);
		int atlasRow = (int)_fiAtlasTileBrush_atlasRow.GetValue(oldBrush);
		int atlasColumn = (int)_fiAtlasTileBrush_atlasColumn.GetValue(oldBrush);
		
		// Create the new tileset brush.
		int atlasColumns = atlasTexture.width / atlasTileWidth;
		int tileIndex = atlasRow * atlasColumns + atlasColumn;
		TilesetBrush newBrush = BrushUtility.CreateTilesetBrush(record.displayName, tileset, tileIndex, InheritYesNo.Inherit);
		
		// Was unit collider added to original atlas brush?
		BoxCollider automaticCollider = oldBrush.collider as BoxCollider;
		if (automaticCollider != null && automaticCollider.size == Vector3.one && automaticCollider.center == Vector3.zero)
			newBrush.addCollider = true;
		
		int componentCount = oldTransform.GetComponents<Component>().Length;
		if (newBrush.addCollider)
			--componentCount;
		
		// Should prefab be generated and attached?
		//   - Attach prefab if it contains child game objects.
		//   - Attach prefab if collider is non-standard.
		//   - Contains extra components (1=transform, 2=brush, 3=filter, 4=renderer).
		bool attachPrefab = ( oldTransform.childCount > 0 )
			|| ( !newBrush.addCollider && oldBrush.collider != null )
			|| ( componentCount != 4)
			;
		if (attachPrefab) {
			GameObject attachment = PrefabUtility.InstantiatePrefab(oldBrush.gameObject) as GameObject;
			
			// Destroy the previous brush component.
			Object.DestroyImmediate(attachment.GetComponent(_tyTileBrush));
			// Destroy collider as it's not needed.
			if (newBrush.addCollider)
				Object.DestroyImmediate(attachment.collider);
			
			// Remove mesh filter and renderer components.
			Object.DestroyImmediate(attachment.renderer);
			Object.DestroyImmediate(attachment.GetComponent<MeshFilter>());
			
			string assetPath = GetUniqueMigratedPath(oldBrush.name + ".prefab");
			newBrush.attachPrefab = PrefabUtility.CreatePrefab(assetPath, attachment);
			
			Object.DestroyImmediate(attachment);
		}
		
		CopyCommonBrushProperties(newBrush, record.brush);
		RtsUpgradedBrushMap.BrushMappings.SetMapping(record.brush, newBrush);
		
		if (newBrush.visibility == BrushVisibility.Shown)
			newBrush.visibility = BrushVisibility.Favorite;
		else
			newBrush.visibility = BrushVisibility.Shown;
	}
	
	/// <summary>
	/// Upgrades empty brush (v1.x) into an empty brush (v2.x).
	/// </summary>
	/// <param name="record">Record for existing v1.x brush.</param>
	private void UpgradeEmptyBrush(RtsBrushAssetRecordWrapper record) {
		if (useNewAssets) {
			// Is this the original "Empty Variation" master brush?
			if (record.assetPath == "Assets/Rotorz Tile System/Brushes/Empty Variation.prefab") {
				// This is a special case that can be switched to new version
				// if it was imported.
				EmptyBrush newMasterBrush = AssetDatabase.LoadAssetAtPath("Assets/Rotorz/Tile System/TileBrushes/Master/Empty Variation.brush.asset", typeof(EmptyBrush)) as EmptyBrush;
				if (newMasterBrush != null) {
					RtsUpgradedBrushMap.BrushMappings.SetMapping(record.brush, newMasterBrush);
					return;
				}
				
				// If new version was not found then just proceed to upgrade
				// existing version.
			}
		}
		
		// Create the new empty brush.
		EmptyBrush newBrush = BrushUtility.CreateEmptyBrush(record.displayName);
		
		FinalizeStandaloneBrush(record, newBrush);
		
		// If we have upgraded the original empty variation brush then let's
		// change our minds and make it visible by default.
		if (record.assetPath == "Assets/Rotorz Tile System/Brushes/Empty Variation.prefab") {
			newBrush.visibility = BrushVisibility.Shown;
			EditorUtility.SetDirty(newBrush);
		}
	}
	
	/// <summary>
	/// Copies common brush properties from v1.x brush to v2.x brush.
	/// </summary>
	/// <remarks>
	/// <para>Also copies material mappings and filters materials to use the
	/// material assets provided by v2.x where possible.</para>
	/// </remarks>
	/// <param name="newBrush">The new brush.</param>
	/// <param name="oldBrush">The old brush.</param>
	private void CopyCommonBrushProperties(Brush newBrush, MonoBehaviour oldBrush) {
		newBrush.visibility = (bool)_fiTileBrush_hideBrush.GetValue(oldBrush)
			? BrushVisibility.Hidden
			: BrushVisibility.Shown;
		newBrush.group = (int)_fiTileBrush_tileGroup.GetValue(oldBrush);

		if (_fiTileBrush_overrideTag != null)
			newBrush.overrideTag = (bool)_fiTileBrush_overrideTag.GetValue(oldBrush);
		newBrush.tag = oldBrush.gameObject.tag;
		if (_fiTileBrush_overrideLayer != null)
			newBrush.overrideLayer = (bool)_fiTileBrush_overrideLayer.GetValue(oldBrush);
		newBrush.layer = oldBrush.gameObject.layer;

		if (_fiTileBrush_category != null)
			newBrush.category = (int)_fiTileBrush_category.GetValue(oldBrush);

		newBrush.applyPrefabTransform = (bool)_fiTileBrush_applyPrefabTransform.GetValue(oldBrush);

		if (_fiTileBrush_scaleMode != null)
			newBrush.scaleMode = (Rotorz.Tile.ScaleMode)_fiTileBrush_scaleMode.GetValue(oldBrush);
		newBrush.transformScale = oldBrush.transform.localScale;
		
		newBrush.Static = oldBrush.gameObject.isStatic;
		newBrush.Smooth = (bool)_fiTileBrush_smooth.GetValue(oldBrush);

		if (_fiTileBrush_userFlags != null)
			_fiBrush_userFlags.SetValue(newBrush, _fiTileBrush_userFlags.GetValue(oldBrush));
		
		// Copy material mappings if applicable.
		// Note: Material mappings do not apply to all types of brush.
		IMaterialMappings materialMappings = newBrush as IMaterialMappings;
		if (materialMappings != null) {
			Material[] materialsFrom = (Material[])_fiTileBrush_materialMapFrom.GetValue(oldBrush);
			Material[] materialsTo = (Material[])_fiTileBrush_materialMapTo.GetValue(oldBrush);
			
			// Use the new default material asset instead of the old one.
			for (int i = 0; i < materialsFrom.Length; ++i)
				materialsFrom[i] = FilterMaterial(materialsFrom[i]);
			
			// Use the new Cave and Grass material assets instead of the old ones.
			for (int i = 0; i < materialsTo.Length; ++i)
				materialsTo[i] = FilterMaterial(materialsTo[i]);
		
			materialMappings.MaterialMappingFrom = materialsFrom;
			materialMappings.MaterialMappingTo = materialsTo;
		}
	}
	
	/// <summary>
	/// Filter material and when using a v1.x material asset, switch to the
	/// new v2.x material asset instead.
	/// </summary>
	/// <param name="oldMaterial">Old material asset.</param>
	/// <returns>
	/// The filtered material.
	/// </returns>
	private Material FilterMaterial(Material oldMaterial) {
		if (useNewAssets) {
			string assetPath = AssetDatabase.GetAssetPath(oldMaterial);
			
			if (assetPath == "Assets/Rotorz Tile System/Materials/Default.mat") {
				Material newMaterial = AssetDatabase.LoadAssetAtPath("Assets/Rotorz/Tile System/Materials/Default.mat", typeof(Material)) as Material;
				if (newMaterial != null)
					return newMaterial;
			}
			
			if (assetPath == "Assets/Rotorz Tile System/Materials/Cave Stone.mat"
				|| assetPath == "Assets/Rotorz Tile System/Materials/Grass Stone.mat")
			{
				Material newMaterial = AssetDatabase.LoadAssetAtPath("Assets/Rotorz/Tile System/Demo/TileBrushes/Materials/" + oldMaterial.name.Replace("Stone", "Platform") + ".mat", typeof(Material)) as Material;
				if (newMaterial != null)
					return newMaterial;
			}
			
			// Use default material if old material is missing.
			if (oldMaterial == null)
				return AssetDatabase.LoadAssetAtPath("Assets/Rotorz/Tile System/Materials/Default.mat", typeof(Material)) as Material;
		}
		
		return oldMaterial;
	}
	
	/// <summary>
	/// Finalize a newly created standalone brush asset.
	/// </summary>
	/// <remarks>
	/// <para>Also moves asset into "Master" folder if needed.</para>
	/// </remarks>
	/// <param name="record">Record for v1.x brush.</param>
	/// <param name="newBrush">The new brush.</param>
	private void FinalizeStandaloneBrush(RtsBrushAssetRecordWrapper record, Brush newBrush) {
		CopyCommonBrushProperties(newBrush, record.brush);
		RtsUpgradedBrushMap.BrushMappings.SetMapping(record.brush, newBrush);
		
		// Was previous brush a master?
		if (record.master) {
			string newAssetPath = AssetDatabase.GetAssetPath(newBrush);
			string masterAssetPath = newAssetPath.Replace(BrushUtility.GetBrushAssetPath(), BrushUtility.GetMasterBrushAssetPath());
			AssetDatabase.MoveAsset(newAssetPath, masterAssetPath);
		}
		
		EditorUtility.SetDirty(newBrush);
	}
	
	#endregion
	
}