using UnityEngine;
using UnityEditor;
using System.IO;

namespace org.a2dev.UnityScript.Editor {
	public static class ScriptableObjectUtil {
		/// <summary>
		/// ScriptableObjectの作成
		/// </summary>
		/// <returns>The scriptable object.</returns>
		/// <param name="name">savePath under dataPath</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void CreateScriptableObject<T>(string savePath, bool isRenew = false) where T : ScriptableObject
		{
			// ファイル存在チェック
			string assetPath = "Assets/" + savePath;
			string fullPath = Application.dataPath + "/" + savePath;
			if (File.Exists (fullPath)) {
				if (isRenew) {
					AssetDatabase.DeleteAsset (assetPath);
				} else {
					return;
				}
			}

			// 作成
			T obj = ScriptableObject.CreateInstance<T> ();
			AssetDatabase.CreateAsset (obj, assetPath);
			AssetDatabase.SaveAssets ();
		}
	}
}
