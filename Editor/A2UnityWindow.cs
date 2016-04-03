/// <summary>
/// 便利機能UnityEditorWindow
/// </summary>
using UnityEngine;
using UnityEditor;
using org.a2dev.UnityScripts.GUIParts;

namespace org.a2dev.UnityScripts.Editor
{
    public class A2UnityWindow : BaseEditorWindow<A2UnityWindow>
    {
        public const string NAME_WINDOW = "a2dev";

        // 最後に設定されていたタイムスケール
        float lastTimeScale = 1f;

        // 行間用の高さ
        const float SPACE_HEIGHT = 10f;

        // playerSettingのトグル表示フラグ
        bool isShowPlaySettings = false;

        Vector2 scrollPosition;

        /// <summary>
        /// 
        /// </summary>
        [MenuItem(WINDOW_PREFIX + "a2devWindow")]
        static void OpenWindow()
        {
            BaseEditorWindow<A2UnityWindow>.OpenWindow();
        }

        /// <summary>
        /// Window内表示
        /// </summary>
        protected override void OnGUI()
        {
            // スクロールビュー
            scrollPosition = GUILayoutUtil.ScrollView(scrollPosition, () =>
            {
                // タイムスケール
                GUILayoutUtil.Label("timescale　x" + Time.timeScale.ToString("0.00"), true);
                GUIParts.GameParts.OnGUITimeScale();

                // キャンバスのノーマライズ化ボタン
                GUILayoutUtil.Label("ugui", true);
                Parts.UIParts.OnGUINormalizeCanvas();

                // 適当にウィンドウを開くようのラベル
                GUILayoutUtil.Label("Any Windows", true);

                // シーンリストメニュー
                if (GUILayout.Button("Scenes List"))
                {
                    SceneListWindow.OpenWindow();
                }

                // PlayerSetting
                GUILayoutUtil.Horisozontal(() =>
                {
                    isShowPlaySettings = EditorGUILayout.Foldout(isShowPlaySettings, "PlayerSetting");
                    GUILayoutUtil.Button("Opne", () =>
                    {
                        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
                    }, 40f);
                });
                if (isShowPlaySettings)
                {
                    PlayerSettingWindow.OnGUIStatic();
                }
            });
        }
    }
}
