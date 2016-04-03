using UnityEngine;
using UnityEngine.UI;
using org.a2dev.UnityScripts.Util;

namespace org.a2dev.UnityScripts.Editor.Parts
{
    public static class UIParts
    {
        /// <summary>
        /// キャンバスのノーマライズ化ボタン
        /// </summary>        
        public static void OnGUINormalizeCanvas()
        {
            // ボタン表示
            if (GUILayout.Button("Create & NormalizeCanvas"))
            {
                // Objectの存在確認
                GameObject go;
                if (GameObjectUtil.IsExistsComponent<CanvasScaler>(out go) == false)
                {
                    go = GameObjectUtil.CreateCanvas();
                }

                // スクリーンサイズと一緒にする
                CanvasScaler scaler = go.GetComponent<CanvasScaler>();
                ScreenUtil.setCanvasScaler(scaler, ScreenUtil.SCREENSIZE_IPHONE5_LANDSCAPE, false);
            }
        }
    }

}
