using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetBundleTools {
	public class ABManager : MonoBehaviour {
		static Dictionary<string, WWW> wwws;
		static Dictionary<string, AssetBundle> bundles;
		static Dictionary<string, string> errors;

		static string _host = "";
		static string platformName = "";

		public static string host {
			set{ _host = value; }
		}

		// 初期化
		public static void Initialize(string host, string gameObjectName = "ABManager")
		{
			if (InitializeABGameObject(gameObjectName)) {
				wwws = new Dictionary<string, WWW> ();
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
		}

		// 現在稼働しているプラットフォームの取得
		private static RuntimePlatform GetPlatformRunning()
		{
			#if UNITY_EDITOR && UNITY_IOS
			return RuntimePlatform.IPhonePlayer;
			#elif UNITY_EDITOR && UNITY_ANDROID
			return RuntimePlatform.Android;
			#else
			return Application.platform;
			#endif
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

			return true;
		}

		// 補完されたurlの取得
		private static string GetUrl(string fileName) {
			return _host + "/" + platformName + "/" + fileName;
		}

		// アセットバンドルのダウンロードと読み込み
		public static IEnumerator AssetBundleLoad (string assetBundleName, string assetName, int version = 1) {
			string url = GetUrl(assetBundleName);

			// ダウンロードを開始して終了を待つ
			WWW www = WWW.LoadFromCacheOrDownload (url, version);
			wwws.Add (assetBundleName, www);
			while (!www.isDone) {
				#if DEBUG
				Debug.Log ("[downloading] " + assetBundleName + " " + (www.progress * 100f).ToString () + "%");
				#endif
				yield return null;
			}

			// エラーチェック
			if(!string.IsNullOrEmpty(www.error)) {
				errors.Add (assetBundleName, www.error);
				#if DEBUG
				Debug.Log ("[download error] " + assetBundleName + " " + www.error);
				#endif
				yield break;
			}

			// AssetBundleの抽出
			AssetBundle bundle = wwws [assetBundleName].assetBundle;
			if (bundle == null) {
				errors.Add (assetBundleName, wwws[assetBundleName].error);
				yield break;
			}
			bundles.Add (assetBundleName, bundle);

			// ダウンロード処理のクリア
			wwws [assetBundleName].Dispose();
			wwws.Remove (assetBundleName);

			// AssetBundleの中から各種objectを読み込む(非同期)
			AssetBundleRequest request = bundle.LoadAssetAsync (assetName, typeof(GameObject));
			yield return request;

			GameObject obj = request.asset as GameObject;

			// 不要になったバンドルのクリア
			bundle.Unload(false);

			// インスタンス化
			GameObject.Instantiate (obj);
		}	
	}
}
