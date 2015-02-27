// Copyright (c) 2011-2014 Rotorz Limited. All rights reserved.

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

/// <summary>
/// An asset that maps v1.x brushes to their v2.x counterparts that are generated
/// when brushes are upgraded.
/// </summary>
/// <remarks>
/// <para>These mappings are used when upgrading tile systems, however may also be
/// useful if you need to write custom upgrade scripts.</para>
/// <para>The asset is saved to 'Assets/TileBrushes/BrushMappingsV1toV2.asset'.</para>
/// </remarks>
public sealed class RtsUpgradedBrushMap : ScriptableObject {

	// This field can also be initialised by `RtsBrushUpgraderUtility.BrushMappings`.
	internal static RtsUpgradedBrushMap _brushMap;

	/// <summary>
	/// Gets brush mapping asset.
	/// </summary>
	/// <remarks>
	/// <para>Gets a value of <c>null</c> if brush mappings asset does not exist.</para>
	/// </remarks>
	public static RtsUpgradedBrushMap BrushMappings {
		get {
			if (_brushMap == null) {
				_brushMap = AssetDatabase.LoadAssetAtPath("Assets/TileBrushes/BrushMappingsV1toV2.asset", typeof(RtsUpgradedBrushMap)) as RtsUpgradedBrushMap;
				if (_brushMap != null)
					_brushMap.Cleanup();
			}
			return _brushMap;
		}
	}
	
	// V1.x to V2.x brush mappings.
	[SerializeField]
	private List<Object> oldBrushes = new List<Object>();
	[SerializeField]
	private List<Rotorz.Tile.Brush> newBrushes = new List<Rotorz.Tile.Brush>();
	
	// V1.x atlas material to V2.x tileset mappings.
	[SerializeField]
	private List<Material> oldAtlasMaterials = new List<Material>();
	[SerializeField]
	private List<Rotorz.Tile.Tileset> newTilesets = new List<Rotorz.Tile.Tileset>();
	
	/// <summary>
	/// Set mapping from old V1.x brush to new V2.x brush.
	/// </summary>
	/// <param name="oldBrush">Old brush.</param>
	/// <param name="newBrush">New brush.</param>
	public void SetMapping(Object oldBrush, Rotorz.Tile.Brush newBrush) {
		int oldIndex = oldBrushes.IndexOf(oldBrush);
		if (oldIndex == -1) {
			oldBrushes.Add(oldBrush);
			newBrushes.Add(newBrush);
		}
		else {
			newBrushes[oldIndex] = newBrush;
		}
		
		EditorUtility.SetDirty(this);
	}
	
	/// <summary>
	/// Lookup replacement brush from a V1.x brush.
	/// </summary>
	/// <param name="oldBrush">Old brush.</param>
	/// <returns>
	/// The new brush or a value of <c>null</c> if no mapping is defined.
	/// </returns>
	public Rotorz.Tile.Brush Lookup(Object oldBrush) {
		int oldIndex = oldBrushes.IndexOf(oldBrush);
		return oldIndex != -1
			? newBrushes[oldIndex]
			: null;
	}
	
	/// <summary>
	/// Set mapping from old V1.x atlas material to new v2.x tileset.
	/// </summary>
	/// <param name="oldAtlasMaterial">Old atlas material.</param>
	/// <param name="newTileset">New tileset.</param>
	public void SetMapping(Material oldAtlasMaterial, Rotorz.Tile.Tileset newTileset) {
		int oldIndex = oldAtlasMaterials.IndexOf(oldAtlasMaterial);
		if (oldIndex == -1) {
			oldAtlasMaterials.Add(oldAtlasMaterial);
			newTilesets.Add(newTileset);
		}
		else {
			newTilesets[oldIndex] = newTileset;
		}
		
		EditorUtility.SetDirty(this);
	}
	
	/// <summary>
	/// Lookup replacement tileset asset from a V1.x atlas material.
	/// </summary>
	/// <param name="oldAtlasMaterial">Old atlas material.</param>
	/// <returns>
	/// The new tileset asset or a value of <c>null</c> if no mapping is defined.
	/// </returns>
	public Rotorz.Tile.Tileset Lookup(Material oldAtlasMaterial) {
		int oldIndex = oldAtlasMaterials.IndexOf(oldAtlasMaterial);
		return oldIndex != -1
			? newTilesets[oldIndex]
			: null;
	}
	
	/// <summary>
	/// Cleanup broken mappings if assets have been deleted.
	/// </summary>
	public void Cleanup() {
		int nullIndex;
		
		nullIndex = oldBrushes.IndexOf(null);
		while (nullIndex != -1) {
			oldBrushes.RemoveAt(nullIndex);
			newBrushes.RemoveAt(nullIndex);
			nullIndex = oldBrushes.IndexOf(null);
		}
		
		nullIndex = newBrushes.IndexOf(null);
		while (nullIndex != -1) {
			oldBrushes.RemoveAt(nullIndex);
			newBrushes.RemoveAt(nullIndex);
			nullIndex = newBrushes.IndexOf(null);
		}
		
		nullIndex = oldAtlasMaterials.IndexOf(null);
		while (nullIndex != -1) {
			oldAtlasMaterials.RemoveAt(nullIndex);
			newTilesets.RemoveAt(nullIndex);
			nullIndex = oldAtlasMaterials.IndexOf(null);
		}
		
		nullIndex = newTilesets.IndexOf(null);
		while (nullIndex != -1) {
			oldAtlasMaterials.RemoveAt(nullIndex);
			newTilesets.RemoveAt(nullIndex);
			nullIndex = newTilesets.IndexOf(null);
		}
		
		EditorUtility.SetDirty(this);
	}
	
}