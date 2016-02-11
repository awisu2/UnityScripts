using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetBundleTools {
	// TODO: bundleInfoのほうがいいかも
	public class WWWInfo {
		protected ABManager.LoadType _loadtype;

		public ABManager.LoadType loadtype {
			get{ return _loadtype; }
		}

		public WWWInfo (ABManager.LoadType loadtype) {
			_loadtype = loadtype;
		}
	}

	public class ABManager : MonoBehaviour {
		// ロードタイプ
		public enum LoadType
		{
			MANIFEST,
			GAMEOBJECT,
			DEPENDENCE,
		};

		static Dictionary<string, WWW> wwws;
		static Dictionary<string, WWWInfo> wwwInfos;
		static Dictionary<string, AssetBundle> bundles;
		static Dictionary<string, string> errors;

		static AssetBundleManifest _manifest = null;

		static string _host = "";
		static string platformName = "";

		static ABManager _manager = null;

		// ホスト
		public static string host {
			set{ _host = value; }
		}

		// マニフェスト
		public static AssetBundleManifest manifest {
			set{ _manifest = value; }
		}

		public static ABManager GetInstance{
			get{ return _manager; }
		}

		// 初期化
		public static void Initialize(string host, string gameObjectName = "ABManager")
		{
			if (InitializeABGameObject(gameObjectName)) {
				wwws = new Dictionary<string, WWW> ();
				wwwInfos = new Dictionary<string, WWWInfo> ();
				bundles = new Dictionary<string, AssetBundle> ();
				errors = new Dictionary<string, string> ();
				_host = host;
				platformName = Util.GetPlatformName (GetPlatformRunning());
			}
		}

		public void Start()
		{
		}

		public void Update()
		{
			CheckDownloads ();
		}

		// ダウンロードなどの管理のためのGameObject化
		private static bool InitializeABGameObject(string name)
		{
			GameObject go = GameObject.Find (name);
			if (go != null) {
				return false;
			}

			go = new GameObject(name, typeof(ABManager));
			DontDestroyOnLoad(go);

			// インスタンスの取得
			_manager = go.GetComponent <ABManager>();

			return true;
		}

		// 補完されたurlの取得
		private static string GetUrl(string fileName) {
			return _host + "/" + platformName + "/" + fileName;
		}

		// AssetBundleのロード開始(２重ダウンロード)
		public static void StartLoadAssetBundle(string name, LoadType type = LoadType.GAMEOBJECT)
		{
			if (type == LoadType.GAMEOBJECT) {
				// TODO:エラーリターンをうまく扱いたい
				if (_manifest == null) {
					return;
				}
			}

			string url = GetUrl(name);

			// ダウンロードを開始して終了を待つ
			WWW www = null;
			if (type == LoadType.MANIFEST) {
				www = new WWW (url);
			} else {
				www = WWW.LoadFromCacheOrDownload (url, 1);
			}
			wwws.Add (name, www);
			wwwInfos.Add (name, new WWWInfo (type));

			// 依存関係のあるファイルもロード
			if (type != LoadType.MANIFEST) {
				StartLoadDependence (name);
			}
		}

		// マニフェストのダウンロード
		public static void StartLoadManifest() {
			StartLoadAssetBundle (platformName, LoadType.MANIFEST);
		}

		// 依存アセットバンドルダウンロード
		public static void StartLoadDependence(string name) {
			string[] dependencies = _manifest.GetAllDependencies(name);
			for (int i = 0; i < dependencies.Length; i++) {
				#if DEBUG
				DebugLog ("[Load Dependence] " + dependencies [i]);
				#endif
				StartLoadAssetBundle (name, LoadType.DEPENDENCE);
			}
		}

		// ダウンロードの状態を監視
		private static void CheckDownloads() {
			foreach (var keyValue in wwws) {
				string name = keyValue.Key;
				WWW www = wwws [name];

				// ダウンロード終了チェック
				if (!www.isDone) {
					continue;
				}

				// エラーチェック
				if(!string.IsNullOrEmpty(www.error)) {
					errors.Add (name, www.error);
					RemoveWWW (name);
					#if DEBUG
					DebugLog ("[download error] " + name + " " + www.error);
					#endif
				}

				// AssetBundleの取得とリストへの追加
				AssetBundle bundle = www.assetBundle;
				if (bundle == null) {
					errors.Add (name, "no assetBundle");
				}

				// バンドルに追加
				// TODO: マニフェストは不要かも
				bundles.Add (name, bundle);

				// タイプで処理分け
				if (wwwInfos [name].loadtype == LoadType.MANIFEST) {
					// AssetBundleの中から各種objectを読み込む
					_manifest = bundle.LoadAsset ("AssetBundleManifest",
						typeof(AssetBundleManifest)) as AssetBundleManifest;
				}

				// ダウンロード管理のクリア
				RemoveWWW(name);
			}
		}

		// 削除
		private static void RemoveWWW(string name) {
			if (!wwws.ContainsKey (name)) {
				return;
			}
			wwws [name].Dispose ();
			wwws.Remove (name);

			// infoも一緒に削除
			wwwInfos.Remove (name);
		}

		// ダウンロード状況の取得
		public static float GetProgress(string abName) {
			if (wwws.ContainsKey (abName)) {
				return wwws [abName].progress;
			}
			return -1f;
		}

		// ダウンロード完了チェック
		// TODO:エラーの扱いは別
		public static bool isDownloaded(string name = "", LoadType type = LoadType.GAMEOBJECT) {
			// ダウンロード中
			if (wwws.ContainsKey) {
				return false;
			}

			if (type == LoadType.MANIFEST) {
				if (_manifest == null)
					return false;
			} else {
				if (!bundles.ContainsKey (name) && errors.ContainsKey (name)) {
					return false;
				}
			}

			return true;
		}
//
//		// アセットバンドルのダウンロードと読み込み
//		public IEnumerator AssetBundleLoad (string assetBundleName, string assetName, int version = 1, bool isdpend = false) {
//			string url = GetUrl(assetBundleName);
//
//			// ダウンロードを開始して終了を待つ
//			WWW www = WWW.LoadFromCacheOrDownload (url, version);
//			wwws.Add (assetBundleName, www);
//			while (!www.isDone) {
//				DebugLog ("[downloading] " + assetBundleName + " " + (www.progress * 100f).ToString () + "%");
//				yield return null;
//			}
//
//			// エラーチェック
//			if(!string.IsNullOrEmpty(www.error)) {
//				errors.Add (assetBundleName, www.error);
//				DebugLog ("[download error] " + assetBundleName + " " + www.error);
//				yield break;
//			}
//
//			// AssetBundleの抽出
//			AssetBundle bundle = wwws [assetBundleName].assetBundle;
//			if (bundle == null) {
//				errors.Add (assetBundleName, wwws[assetBundleName].error);
//				yield break;
//			}
//			bundles.Add (assetBundleName, bundle);
//
//			// ダウンロード処理のクリア
//			wwws [assetBundleName].Dispose();
//			wwws.Remove (assetBundleName);
//
//		// TODO:依存関係のダウンロードの場合はここで終わり
//			if (isdpend) {
//				AssetBundleLoadDependencies (assetBundleName);
//				yield break;
//			}
//
//			// AssetBundleの中から各種objectを読み込む(非同期)
//			AssetBundleRequest request = bundle.LoadAssetAsync (assetName, typeof(GameObject));
//			yield return request;
//
//			GameObject obj = request.asset as GameObject;
//
//			// 不要になったバンドルのクリア
//			bundle.Unload(false);
//
//			// 依存アッセトのロード
//			AssetBundleLoadDependencies (assetBundleName);
//
//			// インスタンス化
//			GameObject.Instantiate (obj);
//		}
//
//		// アセットバンドルのダウンロードと読み込み
//		public static IEnumerator ManifestLoad (string manifestName) {
//			string url = GetUrl(manifestName);
//
//			// ダウンロードを開始して終了を待つ
//			WWW www = new WWW(url);
//			wwws.Add (manifestName, www);
//			while (!www.isDone) {
//				DebugLog ("[downloading] " + manifestName + " " + (www.progress * 100f).ToString () + "%");
//				yield return null;
//			}
//
//			// エラーチェック
//			if(!string.IsNullOrEmpty(www.error)) {
//				errors.Add (manifestName, www.error);
//				DebugLog ("[download error] " + manifestName + " " + www.error);
//				yield break;
//			}
//
//			// AssetBundleの抽出
//			AssetBundle bundle = wwws [manifestName].assetBundle;
//			if (bundle == null) {
//				errors.Add (manifestName, wwws[manifestName].error);
//				yield break;
//			}
//
//			// ダウンロード処理のクリア
//			wwws [manifestName].Dispose();
//			wwws.Remove (manifestName);
//
//			// AssetBundleの中から各種objectを読み込む(非同期)
//			AssetBundleRequest request = bundle.LoadAssetAsync ("AssetBundleManifest", typeof(AssetBundleManifest));
//			yield return request;
//
//			_manifest = request.asset as AssetBundleManifest;
//		}
//
//		// 依存アセットバンドルダウンロード
//		public void AssetBundleLoadDependencies(string bundlename) {
//			string[] dependencies = _manifest.GetAllDependencies(bundlename);
//			for (int i = 0; i < dependencies.Length; i++) {
//				Debug.Log (dependencies [i]);
//				StartCoroutine(AssetBundleLoad (dependencies [i], "", 1, true));
//			}
//		}

		#if DEBUG
		private static void DebugLog(string log) {
			Debug.Log (log);
		}
		#endif

		// 現在稼働しているプラットフォームの取得
		private static RuntimePlatform GetPlatformRunning()
		{
			#if (UNITY_EDITOR && UNITY_IOS)
			return RuntimePlatform.IPhonePlayer;
			#elif (UNITY_EDITOR && UNITY_ANDROID)
			return RuntimePlatform.Android;
			#else
			return Application.platform;
			#endif
		}
	}
}
