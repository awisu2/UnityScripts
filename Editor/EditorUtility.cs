using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace A2Unity.Editor
{
	public static class EditorUtility 
	{
		public static List<string> GetSelectionAssetPaths()
		{
			// 選択しているassetsのGuid
			string[] guids = Selection.assetGUIDs;

			// チェック
			List<string> paths = new List<string> ();
			if (guids == null || guids.Length == 0) {
				return paths;
			}

			// pathに変換
			for (int i = 0; i < guids.Length; i++) {
				paths.Add (AssetDatabase.GUIDToAssetPath (guids [i]));
			}

			return paths;
		}
	}
}
