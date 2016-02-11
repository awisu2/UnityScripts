using UnityEngine;
using System.Collections;
using System.IO;

namespace DownloadTools {
	public class Sample {

		private IEnumerator Downloading(string url) {
			WWW www = new WWW (url);

			//		yield return www;
			while (!www.isDone) {
				float progress = www.progress;
				Debug.Log ((progress * 100f).ToString () + "%");
				yield return null;
			}

			if (!string.IsNullOrEmpty (www.error)) {
				Debug.Log (www.error.ToString ());
			}

			www.Dispose ();
		}

		private void Save(WWW www, string name) {
			byte[] bytes = www.bytes;
			string path = Application.persistentDataPath + "/" + name;
			File.WriteAllBytes(path, bytes);
		}
	}
}
