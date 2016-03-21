using UnityEngine;
using UnityEditor;
using System.IO;

namespace A2Unity.Editor {
	public class ExportAssetbundle  
	{
		const string OutputFolder = "AssetBundles";

		const string MenuPath = "Assets/ExportAssetbundle";

		[MenuItem(MenuPath + "/ActiveTarget")]
		static void Export()
		{
			BuildAssetBundles (EditorUserBuildSettings.activeBuildTarget);
		}

		[MenuItem(MenuPath + "/iOS")]
		static void Export_iOS()
		{
			BuildAssetBundles (BuildTarget.iOS);
		}

		[MenuItem(MenuPath + "/Android")]
		static void Export_Android()
		{
			BuildAssetBundles (BuildTarget.Android);
		}

		[MenuItem(MenuPath + "/Multi")]
		static void Export_Multi()
		{
			BuildTarget[] targets = new BuildTarget[]{BuildTarget.iOS, BuildTarget.Android};
			foreach (var target in targets) {
				BuildAssetBundles (target);
			}
		}

		// アセットバンドルの作成
		private static void BuildAssetBundles(BuildTarget target) {
			string name = GetPlatformNameByTarget (target);
			string dir = Application.dataPath + "/../" + OutputFolder + "/" + name;

			BuildAssetBundleOptions options = BuildAssetBundleOptions.None;

			// ディレクトリ作成
			if(!Directory.Exists(dir)) {
				Directory.CreateDirectory (dir);
			}

			BuildPipeline.BuildAssetBundles (dir, options, target);
		}

		// platformからtargetを取得(targetを利用できるのはEditorのみなので変換できるように)
		private static RuntimePlatform GetPlatformByTarget(BuildTarget target)
		{
			RuntimePlatform platform;
			switch (target) {
			case BuildTarget.iOS:
				{
					platform = RuntimePlatform.IPhonePlayer;
					break;
				}
			case BuildTarget.Android:
				{
					platform = RuntimePlatform.Android;
					break;
				}
			case BuildTarget.StandaloneOSXIntel:
			default:
				{
					platform = RuntimePlatform.OSXPlayer;
					break;
				}
			}
			return platform;
		}

		// ターゲットからプラットフォームに対応する名前を取得
		private static string GetPlatformNameByTarget(BuildTarget target)
		{
			RuntimePlatform platform = GetPlatformByTarget (target);
			return GetPlatformName (platform);
		}

		// platformから名前を取得
		private static string GetPlatformName(RuntimePlatform platform) 
		{
			string name = "";

			switch (platform) {
			case RuntimePlatform.IPhonePlayer:
				{
					name = "iOS";
					break;
				}
			case RuntimePlatform.Android:
				{
					name = "Android";
					break;
				}
			case RuntimePlatform.OSXPlayer:
			default:
				{
					name = "OSX";
					break;
				}
			}
			return name;
		}
	}
}
