/// <summary>
/// ScriptableObject用Util
/// </summary>
using UnityEngine;
using UnityEditor;
using System.IO;
using org.a2dev.UnityScripts.Util;

namespace org.a2dev.UnityScript.Editor
{
	public class ScriptableObjectManager : UnityEditor.Editor
	{
		/// <summary>
		/// Creates the scriptable object.
		/// </summary>
		[MenuItem("Window/A2Unity/LogResourcPathSelection", false, 0)]
		public static void LogResourcPathSelection()
		{
			// 選択しているAsset
			string[] paths = EditorUtil.GetSelectionAssetPaths();
            for(int i = 0; i < paths.Length; i++)
            {
				// Resourcesディレクトリ取得
				string pathResource = ConvertResourcesPath (paths[i]);
                
				if (string.IsNullOrEmpty (pathResource)) {
					continue;
				}

				// ログ
				Debug.Log (pathResource);
			}
		}

		/// Resourcesディレクトリ名
		private const string NameResoucesDir = "Resources/";

		/// Resourcesディレクトリ名の文字列長
		private static readonly int LengthDirNameResouces = NameResoucesDir.Length;

		/// <summary>
		/// Resourcesディレクトリのパスに変換
		/// </summary>
		/// <returns>The resources path.</returns>
		/// <param name="path">Path. <c>""</c>Resourcesディレクトリでない場合は</param>
		private static string ConvertResourcesPath(string path)
		{
			string ResourcesPath = "";

			int idx = path.LastIndexOf (NameResoucesDir);
			if (idx >= 0) {
				ResourcesPath = path.Substring (LengthDirNameResouces + idx);
				ResourcesPath = Path.GetFileNameWithoutExtension (ResourcesPath);
			}

			return ResourcesPath;
		}


	}


}
