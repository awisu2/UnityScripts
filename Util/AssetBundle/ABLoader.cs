using UnityEngine;
using UnityEngine.Experimental.Networking;
using System.IO;
using System;

//TODO:デストラクタとdispose
namespace org.a2dev.UnityScripts.Util.AssetBundle
{
    // TODO: bundleInfoのほうがいいかも
    abstract public class ABLoader : IDisposable {
		// ロードタイプ
		public enum LoadType
		{
			MANIFEST,
			GAMEOBJECT,
			DEPENDENCE,
		};

		// パス
		protected string _bundleName;
		// ファイル名
		protected string _filename;
		// ロードタイプ
		protected LoadType _type;

		protected UnityEngine.AssetBundle _bundle = null;
		protected bool isUpdate = true;

		// エラー
		protected string _error = null;

		// プロパティ
		public LoadType loadType {
			get{ return _type; }
		}

		// コンストラクタ
		public ABLoader(string bundleName, LoadType type) {
			_bundleName = bundleName;
			_filename = Path.GetFileName (bundleName);
			_type = type;
		}

		// 破棄
		public void Dispose () {
			#if DEBUG
			Debug.Log("Dispose ABLoader");
			#endif
		}

		// 都度チェック用メソッド、返却は、updateが更に必要かどうか
		abstract public bool Update();
		public UnityEngine.AssetBundle GetAssetBundle() {
			return _bundle;
		}

		// エラーの取得
		public string error {
			get{ return _error; }
		}

		// エラーチェック
		public bool IsError() {
			return string.IsNullOrEmpty (_error) == false;
		}
	}

	/* ABLoaderWWW
	 * 
	 * WWWを利用したローダ
	 */
	public class ABLoaderWWW : ABLoader 
	{
		WWW _request = null;

		// TODO:Disposeの使い方・・・？
		// コンストラクタ
		public ABLoaderWWW(string bundleName, LoadType type) : base(bundleName, type) {
			CreateRequest ();
		}

		// デストラクタ
		~ABLoaderWWW() {
			Dispose ();
		}

		// クリア
		public void Dispose()
		{
			if (_request != null) {
				_request.Dispose ();
			}
			base.Dispose ();
		}

		// リクエストの作成
		protected void CreateRequest ()
		{
			string name = Path.GetFileName (_bundleName);
			string url = ABManager.GetUrl (_bundleName);
			if (_type == ABLoader.LoadType.MANIFEST) {
				_request = new WWW (url);
			} else {
				if (ABManager.manifest == null) {
					_request = WWW.LoadFromCacheOrDownload (url, 10);
				} else {
					Hash128 hash = ABManager.manifest.GetAssetBundleHash (name);
					uint crc = 0; // 個別のmanifestに設定されている
					_request = WWW.LoadFromCacheOrDownload (url, hash, crc);
				}
			}
		}

		// 更新
		override public bool Update() {
			if (isUpdate == false)
				return false;

			// 完了チェック
			if (_request.isDone == false)
				return true;

			// エラーチェック
			if (string.IsNullOrEmpty (_request.error)) {
				_bundle = _request.assetBundle;
			} else {
				_error = _request.error;
			}

			// 更新フラグ
			isUpdate = false;
			return false;
		}
	}

	/* ABLoaderWebRequest
	 * 
	 * UnityWebRequestを利用したローダ
	 */
	public class ABLoaderWebRequest : ABLoader 
	{
		UnityWebRequest _request = null;

		// TODO:Disposeの使い方・・・？
		// コンストラクタ
		public ABLoaderWebRequest(string bundleName, LoadType type) : base(bundleName, type) {
			InitRequest ();
		}

		// デストラクタ
		~ABLoaderWebRequest() {
			Dispose ();
		}

		// クリア
		public void Dispose()
		{
			if (_request != null) {
				_request.Dispose ();
			}
			base.Dispose ();
		}

		protected void InitRequest ()
		{
			string uri = ABManager.GetUrl (_bundleName);

			if (_type == ABLoader.LoadType.MANIFEST) {
				_request = UnityWebRequest.GetAssetBundle (uri);
			} else {
				// TODO:crc
				uint crc = 0;
				if (ABManager.manifest == null) {
					_request = UnityWebRequest.GetAssetBundle(uri, crc);
				} else {
					Hash128 hash = ABManager.manifest.GetAssetBundleHash (_bundleName);
					crc = 0; // 個別のmanifestに設定されている
					_request = UnityWebRequest.GetAssetBundle (uri, hash, crc);
				}
			}
			_request.Send ();
		}

		// 更新
		override public bool Update() {
			if (isUpdate == false)
				return false;

			// 完了チェック
			if (_request.isDone == false)
				return true;

			// エラーチェック
			if (_request.isError) {
				_error = _request.error;
			} else {
				// アセットバンドルの読み込み
				_bundle = DownloadHandlerAssetBundle.GetContent (_request);
				if (_bundle == null) {
					_error = "no asset bundle " + _bundleName;
				}
			}
			_request.Dispose ();

			// 更新フラグ
			isUpdate = false;
			return false;
		}
	}
}