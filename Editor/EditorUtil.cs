/// <summary>
/// UnityEditor用Util
/// </summary>
using UnityEditor;

namespace org.a2dev.UnityScripts.Util
{
    public static class EditorUtil
    {
        /// <summary>
        /// 選択しているアッセトのパスを返却
        /// </summary>
        /// <returns>アセットパス</returns>
		public static string[] GetSelectionAssetPaths()
        {
            // 選択しているassetsのGuid
            string[] guids = Selection.assetGUIDs;

            // チェック
            if (guids == null || guids.Length == 0)
            {
                return null;
            }

            // pathに変換
            string[] paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return paths;
        }
        
        

    }
}
