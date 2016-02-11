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
		public static bool StartLoadAssetBundle(string name, LoadType type = LoadType.GAMEOBJECT)
		{
			#if DEBUG
			DebugLog("StartLoadAssetBundle : " + name);
			#endif
			if (type == LoadType.MANIFEST) {
				// すでにダウンロードしている
				if (_manifest != null) {
					// TODO:エラーリターンをうまく扱いたい(マニフェストがないのでエラー)
					return false;
				}
			}

			// ダウンロード中またはすでにダウンロード済みチェック
			if (wwws.ContainsKey (name) || bundles.ContainsKey (name)) {
				return true;
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

			return true;
		}

		// マニフェストのダウンロード
		public static void StartLoadManifest() {
			StartLoadAssetBundle (platformName, LoadType.MANIFEST);
		}

		// 依存アセットバンドルダウンロード
		public static void StartLoadDependence(string name) {
			if (_manifest == null) {
				return;
			}
			string[] dependencies = _manifest.GetAllDependencies(name);
			for (int i = 0; i < dependencies.Length; i++) {
				StartLoadAssetBundle (dependencies [i], LoadType.DEPENDENCE);
			}
		}

		// ダウンロードの状態を監視
		private static void CheckDownloads() {
			List<string> deleteKeys = new List<string> ();

			foreach (var keyValue in wwws) {
				string name = keyValue.Key;
				WWW www = wwws [name];

				#if DEBUG
				DebugLog ("[CheckDownload] " + name);
				#endif

				// ダウンロード終了チェック
				if (!www.isDone) {
					continue;
				}

				// エラーチェック
				if(!string.IsNullOrEmpty(www.error)) {
					#if DEBUG
					DebugLog ("[download error] " + name + " " + www.error);
					#endif
					errors.Add (name, www.error);
					deleteKeys.Add (name);
					continue;
				}

				// AssetBundleの取得とリストへの追加
				AssetBundle bundle = www.assetBundle;
				if (bundle == null) {
					#if DEBUG
					DebugLog ("[download error] " + name + " " + "no assetBundle");
					#endif
					errors.Add (name, "no assetBundle");
					deleteKeys.Add (name);
					continue;
				}

				// タイプで処理分け
				#if DEBUG
				DebugLog ("[downloaded] " + name);
				#endif
				if (wwwInfos [name].loadtype == LoadType.MANIFEST) {
					// AssetBundleの中から各種objectを読み込む
					_manifest = bundle.LoadAsset ("AssetBundleManifest",
						typeof(AssetBundleManifest)) as AssetBundleManifest;
				} else {
					// バンドルに追加
					bundles.Add (name, bundle);
				}

				// ダウンロード管理のクリア
				deleteKeys.Add(name);
			}

			// 削除
			foreach (string key in deleteKeys) {
				RemoveWWW (key);
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
			#if DEBUG
			DebugLog ("[isDownloaded] " + name);
			#endif
			// ダウンロード中
			if (wwws.ContainsKey(name)) {
				return false;
			}

			if (type == LoadType.MANIFEST) {
				if (_manifest == null)
					return false;
			} else {
				if (!bundles.ContainsKey (name) && !errors.ContainsKey (name)) {
					if (errors.ContainsKey(name)) {
						Debug.Log ("bundle error " + errors[name]);
					}
					return false;
				}

				// 依存関係のファイルが全てロードされているかチェック
				if (_manifest != null) {
					string[] dependencies = _manifest.GetAllDependencies(name);
					for (int i = 0; i < dependencies.Length; i++) {
						if (!isDownloaded (dependencies [i])) {
							return false;
						}
					}
				}
			}

			#if DEBUG
			DebugLog ("[isDownloaded] " + name + " check OK");
			#endif
			return true;
		}

		// ダウンロードしたAssetbundleからAssetの取得
		// TODO: yieldで普通にロードを待ったりしているので、一旦考慮の必要あり
		// TODO: いきなりGameObjectでロードしているので、違う使い方もある
		public static IEnumerator LoadAssets(string bundleName, string assetName) {
			ABManager.StartLoadAssetBundle (bundleName);

			while (!ABManager.isDownloaded (bundleName)) {
				yield return null;
			}

			if (!bundles.ContainsKey (bundleName))
				yield break;

			AssetBundle bundle = bundles [bundleName];

			// AssetBundleの中から各種objectを読み込む(非同期)
			AssetBundleRequest request = bundle.LoadAssetAsync (assetName, typeof(GameObject));
			yield return request;

			GameObject obj = request.asset as GameObject;

			// 不要インスタンスの削除
			UnloadBundle (bundleName);

			// インスタンス化
			GameObject.Instantiate (obj);
		}

		// 不要になったアセットバンドルのクリア
		public static void UnloadBundle(string name, bool isDependencies = true)
		{
			if(!bundles.ContainsKey(name)) {
				return;
			}

			// 依存しているバンドルもクリア
			if(isDependencies) {
				string[] dependencies = _manifest.GetAllDependencies(name);
				for (int i = 0; i < dependencies.Length; i++) {
					UnloadBundle (dependencies[i]);
				}
			}

			// クリア
			bundles[name].Unload(false);
			bundles.Remove (name);
		}

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

		// デバッグログ
		#if DEBUG
		private static void DebugLog(string log) {
		Debug.Log (log);
		}
		#endif
	}
}
