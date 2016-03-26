/// <summary>
/// Downloadの補助を行う
/// </summary>
using UnityEngine;
using System.Collections;
using System;

namespace org.a2dev.UnityScripts.Util
{
    public class DownloadUtil
    {
        /// <summary>
        /// WWWs the download.
        /// </summary>
        /// <returns>The download.</returns>
        /// <param name="url">URL.</param>
        /// <param name="actDownloaded">Act downloaded.</param>
        /// <param name="actError">Act error.</param>
        /// <param name="actProgress">Act progress.</param>
        public IEnumerator WWWDownload(string url, Action<WWW> actDownloaded,
            Action<string> actError = null, Action<float> actProgress = null)
        {
            // ダウンロード開始
            WWW www = new WWW(url);

            // ダウンロード中
            while (!www.isDone)
            {
                if (actProgress != null)
                {
                    actProgress(www.progress);
                }
                yield return null;
            }

            // ダウンロード中
            if (!string.IsNullOrEmpty(www.error))
            {
                if (actError != null)
                {
                    actError(www.error.ToString());
                }
                yield return null;
            }

            actDownloaded(www);

            // 開放
            www.Dispose();
        }
    }
}
