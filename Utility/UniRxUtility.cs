#if ENABLE_UNIRX
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections.Generic;

namespace A2Unity.Utility
{
	public class UniRxUtility : MonoBehaviour{
		// インスタンス
		private static UniRxUtility instance = null;

		// インスタンス取得
		public static UniRxUtility GetInstance()
		{
			if (instance != null) {
				return instance;
			}

			// ゲームオブジェクト
			GameObject go = new GameObject ("UniRxUtility");
			go.AddComponent<UniRxUtility> ();
			instance = go.GetComponent<UniRxUtility> ();

			return instance;
		}

		/// <summary>
		/// ボタンの押下イベント
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="actDown">ボタンが押された時のAction</param>
		/// <param name="actUp">ボタンが離された時のAction</param>
		/// <param name="actPush">ボタンが押されている間のAction</param>
		public void SetButtonPushing(Button button, Action actDown = null, Action actUp = null, Action actPush = null)
		{
			ReactiveProperty<bool> flag = new ReactiveProperty<bool> (false);

			// ボタンダウン
			button.OnPointerDownAsObservable ()
				.Subscribe (_ => {
					flag.Value = true;
					if (actDown != null) {
						actDown ();
					}
				});

			// ボタンアップ
			button.OnPointerUpAsObservable ()
				.Subscribe (_ => {
					flag.Value = false;
					if (actUp != null) {
						actUp ();
					}
				});

			// 押している間のイベント
			this.UpdateAsObservable ()
				.Select (_ => flag)
				.Where (_ => _.Value)
				.Subscribe (_ => actPush ());
		}

		/// <summary>
		/// 特定のサイトからのダウンロード
		/// </summary>
		/// <param name="url">url</param>
		/// <param name="actDownloaded">ダウンロード後Action</param>
		/// <param name="actError">エラー時Action</param>
		/// <param name="timeOut">タイムアウト時間</param>
		public void WWWDownload(string url, Action<WWW> actDownloaded, Action<string> actError = null, float timeOut = 30f)
		{
			// ダウンロード
			CreateIObservableWWW (url, timeOut)
				.Subscribe (www => {
						actDownloaded (www);
					}
					, ex => {
						if (actError != null) {
							actError (ex.ToString ());
						}
					}
				);
		}

		/// <summary>
		/// 複数サイトからのダウロードが全て完了した時に処理を実行する
		/// </summary>
		/// <param name="url">url</param>
		/// <param name="actDownloaded">ダウンロード後Action</param>
		/// <param name="actError">エラー時Action</param>
		/// <param name="timeOut">タイムアウト時間</param>
		public void WWWDownloads(List<string> urls, Action<WWW[]> actDownloaded, Action<string> actError = null, float timeOut = 30f)
		{
			IObservable<WWW>[] observables = new IObservable<WWW>[urls.Count];
			for (int i = 0; i < urls.Count; i++) {
				observables [i] = CreateIObservableWWW (urls [i], timeOut);
			}

			// 複数のダウンロード
			Observable
				.WhenAll (observables)
				.Subscribe (_ => {
						actDownloaded (_);
					}
					, ex => {
						if (actError != null) {
							actError (ex.ToString ());
						}
					}
				);
		}

		/// <summary>
		/// ObservableWWWを作成
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="timeOut">タイムアウト時間</param>
		public IObservable<WWW> CreateIObservableWWW(string url, float timeOut)
		{
			return ObservableWWW.GetWWW (url)
				.Timeout (TimeSpan.FromSeconds (timeOut));
		}

		/// <summary>
		/// 接地イベントを取得
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="actOnGround">Act on ground.</param>
		public void SetOnGround(CharacterController character, Action<CharacterController> actOnGround)
		{
			character
				.ObserveEveryValueChanged(_ => _.isGrounded)
				.Where(x => x)
				.ThrottleFrame(5) // 一定フレームのマージンを取って斜面でのisGroundedの細かい変動を防ぐ
				.Subscribe (_ => {
					actOnGround(character);
				});
		}

		/// <summary>
		/// 一定時間内にイベントが特定回数発行された時に発火
		/// </summary>
		/// <param name="observable">イベント</param>
		/// <param name="clickCount">発行回数</param>
		/// <param name="milliSeconds">待機時間</param>
		public void SetAnyCount(IObservable<Unit> observable, int count, int milliSeconds, Action<IObservable<Unit>> actCounted)
		{
			observable
				.Buffer(observable.Throttle(TimeSpan.FromMilliseconds(milliSeconds)))
				.Where(x => x.Count >= count)
				.Subscribe(_ => {
					actCounted(observable);
				});
		}
	}
}

#endif