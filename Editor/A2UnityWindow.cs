/// <summary>
/// 便利機能UnityEditorWindow
/// </summary>
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using org.a2dev.UnityScripts.Util;

namespace org.a2dev.UnityScripts.Editor
{
    public class A2UnityWindow : EditorWindow
    {
        public const string NAME_WINDOW = "a2dev";

        /// タイムスケールメニュー用名前設定
        string[] timeScaleNames = new string[] { "0.1", "0.5", "x1", "x2", "x5", "x10" };

        /// タイムスケールメニュー用スケール設定
        float[] timeScaleScales = new float[] { 0.1f, 0.5f, 1f, 2f, 5f, 10f };

        // タイムスケールメニュー用選択中index
        int selectTimeScaleToolbar = 2;

        // 最後に設定されていたタイムスケール
        float lastTimeScale = 1f;

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Window/" + EditorUtil.NAME_EDITOR_PREFIX + "/a2devWindow")]
        static void Init()
        {
            EditorUtil.GetWindow<A2UnityWindow>("NAME_WINDOW").Show();
        }

        /// <summary>
        /// Window内表示
        /// </summary>
        void OnGUI()
        {
            // タイムスケール
            GUILayout.Label("timescale　x" + Time.timeScale.ToString("0.00"));
            OnGUITimeScale();

            // キャンバスのノーマライズ化ボタン
            GUILayout.Label("ugui");
            OnGUINormalizeCanvas();
            
            // Step処理
            OnGUIStep();
        }

        void OnGUIStep()
        {
            if (GUILayout.Button("Step"))
            {
                Debug.Log("step", this);
                EditorApplication.Step();
            }
        }

        /// <summary>
        /// タイムスケール
        /// </summary>
        void OnGUITimeScale()
        {
            // メニュー以外でのTimeScale変更に合わせる
            int index = -1;
            for (int i = 0; i < timeScaleScales.Length; i++)
            {
                if (timeScaleScales[i] == Time.timeScale)
                {
                    index = i;
                    break;
                }
            }

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
            scale = GUILayout.HorizontalSlider(Time.timeScale, 0f, 10f);
            if (scale != lastTimeScale)
            {
                Time.timeScale = scale;
            }

            lastTimeScale = Time.timeScale;
        }

        /// <summary>
        /// キャンバスのノーマライズ化ボタン
        /// </summary>        
        void OnGUINormalizeCanvas()
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
