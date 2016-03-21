using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace A2Unity.Utility.AssetBundle {
	public class ABManager : MonoBehaviour {
		static Dictionary<string, ABLoader> loaders;
		static Dictionary<string, UnityEngine.AssetBundle> bundles;
		static Dictionary<string, string> errors;

		static AssetBundleManifest _manifest = null;

		static string _host = "";
		static string _platformName = "";

		static ABManager _manager = null;

		// ホスト
		public static string host {
			set{ _host = value; }
		}

		// マニフェスト
		public static AssetBundleManifest manifest {
			set{ _manifest = value; }
			get{ return _manifest; }
		}
		public static ABManager GetInstance{
			get{ return _manager; }
		}

		public static string platformName{
			get{ return _platformName; }
		}

		// エラーの取得
		public static string GetErrorByName(string name) {
			if (errors.ContainsKey (name)) {
				return errors[name];
			}
			return "";
		}

		// 初期化
		public static void Initialize(string host, string gameObjectName = "ABManager")
		{
			if (InitializeABGameObject(gameObjectName)) {
				loaders = new Dictionary<string, ABLoader> ();
				bundles = new Dictionary<string, UnityEngine.AssetBundle> ();
				errors = new Dictionary<string, string> ();
				_host = host;
				_platformName = Util.GetPlatformName (GetPlatformRunning());
				#if DEBUG
				DebugLog("[Initialize] host:" + _host);
				#endif
			}
		}

		public void Start()
		{
		}

		public void Update()
		{
			UpdateLoaders ();
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
		public static string GetUrl(string fileName) {
			return _host + "/" + _platformName + "/" + fileName;
		}

		// AssetBundleのロード開始(２重ダウンロード)
		public static bool StartLoadAssetBundle(string path, ABLoader.LoadType type = ABLoader.LoadType.GAMEOBJECT)
		{
			if (type == ABLoader.LoadType.MANIFEST) {
				// 既にダウンロードしている
				if (_manifest != null) {
					errors.Add (path, "already manifest file " + _platformName);
					return false;
				}
			}

			// ダウンロード中またはすでにダウンロード済みチェック
			// TODO:カウントアップ？
			if (loaders.ContainsKey (path) || bundles.ContainsKey (path)) {
				return true;
			}

			// ダウンロードを開始して終了を待つ
//			loaders.Add (path, new ABLoaderWWW (path, type));
			loaders.Add (path, new ABLoaderWebRequest (path, type));

			// 依存関係のあるファイルもロード
			if (type != ABLoader.LoadType.MANIFEST) {
				StartLoadDependence (path);
			}

			return true;
		}

		// マニフェストのダウンロード
		public static IEnumerator StartLoadManifest() {
			StartLoadAssetBundle (_platformName, ABLoader.LoadType.MANIFEST);
			string error = "";
			while (isDownloaded (_platformName, out error, ABLoader.LoadType.MANIFEST) == false) {
				yield return null;
			}
		}

		// 依存アセットバンドルダウンロード
		private static void StartLoadDependence(string name) {
			if (_manifest == null) {
				return;
			}
			string[] dependencies = _manifest.GetAllDependencies(name);
			for (int i = 0; i < dependencies.Length; i++) {
				StartLoadAssetBundle (dependencies [i], ABLoader.LoadType.DEPENDENCE);
			}
		}

		// ダウンロードの状態を監視終わっていればAssetBundleを取得して、
		private static void UpdateLoaders() {
			List<string> deleteKeys = new List<string> ();

			foreach (var keyValue in loaders) {
				string name = keyValue.Key;
				ABLoader loader = keyValue.Value;

				// 更新
				if (loader.Update ()) {
					continue;
				}

				// エラーチェック
				if (loader.IsError()) {
					errors.Add (name, loader.error);
					#if DEBUG
					Debug.LogError(loader.error);
					#endif
					deleteKeys.Add (name);
					continue;
				}

				// AssetBundleの取得とリストへの追加
				UnityEngine.AssetBundle bundle = loader.GetAssetBundle();
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
				if (loaders [name].loadType == ABLoader.LoadType.MANIFEST) {
					// AssetBundleの中から各種objectを読み込む
					_manifest = bundle.LoadAsset ("AssetBundleManifest",
						typeof(AssetBundleManifest)) as AssetBundleManifest;
				} else {
					// バンドルに追加
					bundles.Add (name, bundle);
				}

				deleteKeys.Add(name);
			}

			// 削除
			foreach (string key in deleteKeys) {
				loaders [key].Dispose ();
				loaders.Remove (key);
			}
		}

		// ダウンロード完了チェック
		//
		// ダウンロードが完了していればtrueを返却
		// エラーチェックは別途行なうこと
		public static bool isDownloaded(string name, out string error, ABLoader.LoadType type = ABLoader.LoadType.GAMEOBJECT) {
			error = "";

			// ダウンロード中
			if (loaders.ContainsKey(name)) {
				return false;
			}

			if (type == ABLoader.LoadType.MANIFEST) {
				if (_manifest == null)
					return false;
			} else {
				// ダウンロードは終わっているがエラー
				if(errors.ContainsKey (name)) {
					error = errors[name];
					return true;
				}
				// ダウンロードされていなければfalse
				if (bundles.ContainsKey (name) == false) {
					return false;
				}

				// 依存関係のファイルが全てロードされているかチェック
				if (_manifest != null) {
					string[] dependencies = _manifest.GetAllDependencies(name);
					for (int i = 0; i < dependencies.Length; i++) {
						if (isDownloaded (dependencies [i], out error) == false) {
							return false;
						}
					}
				}
			}

			return true;
		}

		// ダウンロードしたAssetbundleからAssetの取得
		// TODO: yieldで普通にロードを待ったりしているので、一旦考慮の必要あり
		// TODO: いきなりGameObjectでロードしているので、違う使い方もある
		public static IEnumerator LoadAssets(string bundleName, string assetName) {
			ABManager.StartLoadAssetBundle (bundleName);

			string error = null;
			while (!isDownloaded (bundleName, out error)) {
				yield return null;
			}
			// エラーチェック
			if (string.IsNullOrEmpty (error) == false) {
				Debug.LogError ("[LoadError] " + bundleName + " : " + errors [bundleName]);
				yield break;
			}

			UnityEngine.AssetBundle bundle = bundles [bundleName];

			// AssetBundleの中から各種objectを読み込む(非同期)
			AssetBundleRequest request = bundle.LoadAssetAsync (assetName, typeof(GameObject));
			yield return request;

			GameObject obj = request.asset as GameObject;

			// 不要インスタンスの削除
			UnloadBundle (bundleName);

			// インスタンス化
			if (obj == null) {
				Debug.Log ("[asset is null] " + bundleName + " -> " + assetName);
			} else {
				var ins = GameObject.Instantiate (obj);
				ins.name = assetName;
			}
		}

		// 不要になったアセットバンドルのクリア
		public static void UnloadBundle(string name, bool isDependencies = true)
		{
			if(!bundles.ContainsKey(name)) {
				return;
			}

			// 依存しているバンドルもクリア
			if(isDependencies) {
				if (_manifest != null) {
					string[] dependencies = _manifest.GetAllDependencies(name);
					for (int i = 0; i < dependencies.Length; i++) {
						UnloadBundle (dependencies[i]);
					}
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
