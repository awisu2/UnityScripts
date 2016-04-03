/// <summary>
/// UnityEditor用Util
/// </summary>
using UnityEngine;
using UnityEditor;
using System;

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
        
        public static void Label(string text, bool isbold = false)
        {
            GUIStyle style = null;
            if(isbold)
            {
                style = EditorStyles.boldLabel;
            }
            GUILayout.Label(text, style);
        }
        
        /// <summary>
        /// 左側にスペースを開ける
        /// </summary>
        /// <param name="actGUI">表示するGUI</param>
        /// <param name="space">左側のスペース幅</param>
        public static void LeftSpace(Action acuGUI, float space = 5f)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(space));
            GUILayout.Space(1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            acuGUI();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();            
        }
    }
}
