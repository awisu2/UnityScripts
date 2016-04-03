using UnityEngine;

namespace org.a2dev.UnityScripts.GUIParts
{
    public static class GameParts
    {
        // タイムスケール用設定
        static readonly float[] timeScaleScales = new float[] { 0.1f, 0.2f, 1f, 2f, 5f, 10f };
        static readonly string[] timeScaleNames = new string[] { "0.1", "0.5", "x1", "x2", "x5", "x10" };

        // タイムスケール最終スケール
        static float lastTimeScale = 1f;
        
        // timeScaleからボタンに対応するindexを取得
        static int GetIndexTimeScaleSetting(float timeScale)
        {
            int index = -1;
            for (int i = 0; i < timeScaleScales.Length; i++)
            {
                if (timeScaleScales[i] == timeScale)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// タイムスケール設定表示
        /// </summary>
        public static void OnGUITimeScale()
        {
            // メニュー以外でのTimeScale変更に合わせる
            int index = GetIndexTimeScaleSetting(Time.timeScale);

            // タイムスケール用メニュー
            index = GUILayout.Toolbar(index, timeScaleNames);
            float scale = lastTimeScale;
            if (index >= 0)
            {
                scale = timeScaleScales[index];
            }
            if (scale != lastTimeScale)
            {
                Time.timeScale = scale;
            }

            // スライドバーでのタイムスケール
            scale = GUILayout.HorizontalSlider(scale, 0f, 10f);
            if (scale != lastTimeScale)
            {
                Time.timeScale = scale;
            }

            lastTimeScale = scale;
        }
        
    }

}

