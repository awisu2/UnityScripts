﻿/// <summary>
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

        // 最後に設定されていたタイムスケール
        float lastTimeScale = 1f;

        // 行間用の高さ
        const float SPACE_HEIGHT = 10f;

        // 初期化フラグ
        bool isInitialize = false;

        // playerSettingのトグル表示フラグ
        bool isShowPlaySettings = false;

        Vector2 scrollPosition;

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Window/" + EditorUtil.NAME_EDITOR_PREFIX + "/a2devWindow")]
        static void OpenWindow()
        {
            EditorUtil.GetWindow<A2UnityWindow>(NAME_WINDOW).Show();
        }


        /// <summary>
        /// Window内表示
        /// </summary>
        void OnGUI()
        {
            if (!isInitialize)
            {
                Init();
                isInitialize = true;
            }

            // スクロールビュー
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // タイムスケール
            GUILayout.Label("timescale　x" + Time.timeScale.ToString("0.00"), EditorStyles.boldLabel);
            OnGUITimeScale();

            // キャンバスのノーマライズ化ボタン
            GUILayout.Label("ugui", EditorStyles.boldLabel);
            OnGUINormalizeCanvas();

            // シーンリストメニュー
            GUILayout.Label("Any Windows", EditorStyles.boldLabel);
            OnGUISceanList();

            // PlayerSetting
            GUILayout.BeginHorizontal();
            isShowPlaySettings = EditorGUILayout.Foldout(isShowPlaySettings, "PlayerSetting");
            if (GUILayout.Button("Opne", GUILayout.Width(40f)))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
            }
            GUILayout.EndHorizontal();

            if (isShowPlaySettings)
            {
                PlayerSettingWindow.OnGUI();
            }

            // スクロールビューEnd            
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Init()
        {
        }

        // stepボタン
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



        // シーンリスト
        void OnGUISceanList()
        {
            if (GUILayout.Button("Scenes List"))
            {
                SceneListWindow.OpenWindow();
            }
        }
    }
}
