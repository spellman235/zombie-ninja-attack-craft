// Copyright (c) 2011-2014 Rotorz Limited. All rights reserved.

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Type = System.Type;

public sealed class RtsBrushAssetRecordWrapper {

	#region Reflection

	private static Type _tyBrushAssetRecord;
	private static FieldInfo _fiBrushAssetRecord_brush;
	private static FieldInfo _fiBrushAssetRecord_displayName;
	private static PropertyInfo _piBrushAssetRecord_DisplayName;
	private static FieldInfo _fiBrushAssetRecord_filePath;
	private static PropertyInfo _piBrushAssetRecord_AssetPath;
	private static FieldInfo _fiBrushAssetRecord_master;
	private static PropertyInfo _piBrushAssetRecord_IsMaster;

	static RtsBrushAssetRecordWrapper() {
		BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		_tyBrushAssetRecord = Type.GetType("Rotorz.TileSystem.Editor.BrushAssetRecord,RotorzTileSystemEditor");
		_fiBrushAssetRecord_brush = _tyBrushAssetRecord.GetField("brush", instanceBindingFlags);
		_fiBrushAssetRecord_displayName = _tyBrushAssetRecord.GetField("displayName", instanceBindingFlags);
		_piBrushAssetRecord_DisplayName = _tyBrushAssetRecord.GetProperty("DisplayName", instanceBindingFlags);
		_fiBrushAssetRecord_filePath = _tyBrushAssetRecord.GetField("filePath", instanceBindingFlags);
		_piBrushAssetRecord_AssetPath = _tyBrushAssetRecord.GetProperty("AssetPath", instanceBindingFlags);
		_fiBrushAssetRecord_master = _tyBrushAssetRecord.GetField("master", instanceBindingFlags);
		_piBrushAssetRecord_IsMaster = _tyBrushAssetRecord.GetProperty("IsMaster", instanceBindingFlags);
	}

	#endregion

	public readonly MonoBehaviour brush;
	public readonly string displayName;
	public readonly string assetPath;
	public readonly bool master;

	public RtsBrushAssetRecordWrapper(object tilePrefab) {
		brush = (MonoBehaviour)_fiBrushAssetRecord_brush.GetValue(tilePrefab);
		displayName = (string)(_fiBrushAssetRecord_displayName != null
			? _fiBrushAssetRecord_displayName.GetValue(tilePrefab)
			: _piBrushAssetRecord_DisplayName.GetValue(tilePrefab, null));
		assetPath = (string)(_fiBrushAssetRecord_filePath != null
			? _fiBrushAssetRecord_filePath.GetValue(tilePrefab)
			: _piBrushAssetRecord_AssetPath.GetValue(tilePrefab, null));
		master = (bool)(_fiBrushAssetRecord_master != null
			? _fiBrushAssetRecord_master.GetValue(tilePrefab)
			: _piBrushAssetRecord_IsMaster.GetValue(tilePrefab, null));
	}

}

public sealed class RtsBrushDatabaseWrapper {

	#region Singleton

	internal static RtsBrushDatabaseWrapper _instance;

	public static RtsBrushDatabaseWrapper Instance {
		get {
			if (_instance == null)
				_instance = new RtsBrushDatabaseWrapper();
			return _instance;
		}
	}

	#endregion

	private Type _databaseType;
	private object _rawDatabase;

	public List<RtsBrushAssetRecordWrapper> records;

	public bool IsExistingDatabaseAvailable {
		get { return _rawDatabase != null; }
	}

	public RtsBrushDatabaseWrapper() {
		records = new List<RtsBrushAssetRecordWrapper>();

		_databaseType = Type.GetType("Rotorz.TileSystem.Editor.BrushDatabase,RotorzTileSystemEditor");
		if (_databaseType == null)
			return;

		PropertyInfo piInstance = _databaseType.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
		if (piInstance == null) {
			piInstance = _databaseType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
			if (piInstance == null)
				return;
		}

		_rawDatabase = piInstance.GetValue(null, null);

		Update();
	}

	public void Rescan() {
		if (!IsExistingDatabaseAvailable)
			return;

		MethodInfo miRebuild = _databaseType.GetMethod("Rebuild", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(bool) }, null);
		if (miRebuild != null) {
			miRebuild.Invoke(_rawDatabase, new object[] { false });
		}
		else {
			miRebuild = _databaseType.GetMethod("Rebuild", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(bool), typeof(bool) }, null);
			miRebuild.Invoke(_rawDatabase, new object[] { false, false });
		}

		Update();
	}

	private void Update() {
		records.Clear();

		if (!IsExistingDatabaseAvailable)
			return;

		PropertyInfo piTilePrefabs = _databaseType.GetProperty("tilePrefabs", BindingFlags.Public | BindingFlags.Instance);
		IEnumerable tilePrefabs = (IEnumerable)piTilePrefabs.GetValue(_rawDatabase, null);
		if (tilePrefabs == null)
			return;

		foreach (object tilePrefab in tilePrefabs)
			records.Add(new RtsBrushAssetRecordWrapper(tilePrefab));
	}

}
