/// <summary>
/// ScreenのUtil
/// </summary>
using UnityEngine;
using UnityEngine.UI;

namespace org.a2dev.UnityScripts.Util
{
    public static class ScreenUtil
    {
        // iphone5縦のスクリーンサイズ
        public static readonly Vector2 SCREENSIZE_IPHONE5_PORTAL = new Vector2(640f, 1136f);
        // iphone5横のスクリーンサイズ
        public static readonly Vector2 SCREENSIZE_IPHONE5_LANDSCAPE = new Vector2(1136f, 640f);
        
        /// <summary>
        /// キャンバスのスケールを設定
        /// </summary>
        /// <param name="scaler">設定対象となるスケーラ</param>
        /// <param name="resolution">基準サイズ</param>
        /// <param name="isPortal">縦か横か</param>
        public static void setCanvasScaler(CanvasScaler scaler, Vector2 resolution, bool isPortal = true)
        {
                // スクリーンサイズと一緒にする
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = resolution;
                if(isPortal)
                {
                    scaler.matchWidthOrHeight = 0f;
                }
                else
                {
                    scaler.matchWidthOrHeight = 1f;
                }
        }
    }

}
