/// <summary>
/// UnityEditor用Util
/// </summary>
using UnityEditor;
using System.Collections.Generic;

namespace org.a2dev.UnityScripts.Util
{
    public static class EditorUtil
    {
        public const string NAME_EDITOR_PREFIX = "a2dev";
        
        /// <summary>
        /// 選択しているアッセトのパスを返却
        /// </summary>
        /// <returns>アセットパス</returns>
        /// <param name=""></param>
		public static List<string> GetSelectionAssetPaths()
        {
            // 選択しているassetsのGuid
            string[] guids = Selection.assetGUIDs;

            // チェック
            List<string> paths = new List<string>();
            if (guids == null || guids.Length == 0)
            {
                return paths;
            }

            // pathに変換
            for (int i = 0; i < guids.Length; i++)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            return paths;
        }

        /// <summary>
        /// Editorウィンドウ作成
        /// </summary>
        /// <returns>作成したwindow</returns>
        /// <param name="title">windowのタイトル</param>
        /// <param name="isUtil">windowをUtilとして扱うフラグ。trueにするとタブがなくなる</param>
        public static T GetWindow<T>(string title, bool isUtil = false) where T : EditorWindow
        {
            T window = EditorWindow.GetWindow(typeof(T), isUtil, title) as T;
            return window;
        }
    }
}
