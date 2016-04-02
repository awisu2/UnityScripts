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

        // 最後に設定されていたタイムスケール
        float lastTimeScale = 1f;

        // 行間用の高さ
        const float SPACE_HEIGHT = 10f;

        // orientationの選択Index
        int orientationIndex = -1;

        public const string DEFAULT_IDENTIFIER = "com.Company.ProductName";

        // オリエンテーション簡単設定
        string[] OrientationSettings = new string[]
        {
            "Default",
            "Rotation",
            "Portrait",
        };

        // 初期化フラグ
        bool isInitialize = false;

        // playerSettingのトグル表示フラグ
        bool isShowPlaySettings = false;

        // OS毎のdefine設定
        string defineAlone;
        string defineAndroid;
        string defineIos;

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
                OnGUIStepPlayerSetting();
            }

            // スクロールビューEnd            
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Init()
        {
            // define値の取得
            defineAlone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            defineIos = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
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

        void OnGUIStepPlayerSetting()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(5f));
            GUILayout.Space(1f);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            // bundleIdentifier設定
            OnGUIStepPlayerSettingBundleIdentifier();

            // orientation設定
            GUILayout.Label("Orientation");
            OnGUIStepPlayerSettingOrientation();

            GUILayout.Label("Defines Symbols");
            OnGUIStepPlayerSettingDefineSymbols();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// BundleIdentifierGUI
        /// </summary>
        void OnGUIStepPlayerSettingBundleIdentifier()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bundle Identifier", GUILayout.Width(90f));

            string identifier = PlayerSettings.bundleIdentifier;
            bool isSame = identifier == DEFAULT_IDENTIFIER;
            GuiStyleUtil.TextField style = null;
            if (isSame)
            {
                style = GuiStyleUtil.TextField.GetInstance().SetTextColor(Color.red);
            }
            identifier = GUILayout.TextField(identifier);
            if (identifier != PlayerSettings.bundleIdentifier)
            {
                PlayerSettings.bundleIdentifier = identifier;
            }
            if (isSame)
            {
                style.Reset();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// オリエンテーションGUI
        /// </summary>
        void OnGUIStepPlayerSettingOrientation()
        {
            // Default Orientation
            orientationIndex = OrientationUtil.GetUIOrientationIndex(PlayerSettings.defaultInterfaceOrientation);
            orientationIndex = EditorGUILayout.Popup(orientationIndex, OrientationUtil.OrientationStrings);
            PlayerSettings.defaultInterfaceOrientation = OrientationUtil.GetUIOrientation(orientationIndex);

            // Orientationの個別設定(AutoRotationを選択していない時はdisableにする)
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(PlayerSettings.defaultInterfaceOrientation != UIOrientation.AutoRotation);
            bool isSet = PlayerSettings.allowedAutorotateToLandscapeLeft;
            if (isSet != GUILayout.Toggle(isSet, "left"))
            {
                PlayerSettings.allowedAutorotateToLandscapeLeft = !isSet;
            }
            isSet = PlayerSettings.allowedAutorotateToLandscapeRight;
            if (isSet != GUILayout.Toggle(isSet, "right"))
            {
                PlayerSettings.allowedAutorotateToLandscapeRight = !isSet;
            }
            isSet = PlayerSettings.allowedAutorotateToPortrait;
            if (isSet != GUILayout.Toggle(isSet, "up"))
            {
                PlayerSettings.allowedAutorotateToPortrait = !isSet;
            }
            isSet = PlayerSettings.allowedAutorotateToPortraitUpsideDown;
            if (isSet != GUILayout.Toggle(isSet, "down"))
            {
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = !isSet;
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            // 設定
            switch (GUILayout.Toolbar(-1, OrientationSettings))
            {
                case 0:
                    {
                        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
                        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                        PlayerSettings.allowedAutorotateToLandscapeRight = true;
                        PlayerSettings.allowedAutorotateToPortrait = true;
                        PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
                        break;
                    }
                case 1:
                    {
                        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
                        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                        PlayerSettings.allowedAutorotateToLandscapeRight = true;
                        PlayerSettings.allowedAutorotateToPortrait = false;
                        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                        break;
                    }
                case 2:
                    {
                        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
                        PlayerSettings.allowedAutorotateToPortrait = true;
                        PlayerSettings.allowedAutorotateToPortraitUpsideDown = true;
                        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
                        PlayerSettings.allowedAutorotateToLandscapeRight = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// define設定表示
        /// </summary>
        void OnGUIStepPlayerSettingDefineSymbols()
        {
            defineAlone = OnGUIDefinesSymbols(BuildTargetGroup.Standalone, "Alone", defineAlone);
            defineIos = OnGUIDefinesSymbols(BuildTargetGroup.iOS, "iOS", defineIos);
            defineAndroid = OnGUIDefinesSymbols(BuildTargetGroup.Android, "Android", defineAndroid);

            // 全設定が同時の場合同時に更新が可能
            bool isSame = (defineAlone == defineIos && defineAlone == defineAndroid);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!isSame);
            GUILayout.Label("All", GUILayout.Width(60f));
            string define = "-";
            if (isSame)
            {
                define = defineAlone;
            }
            string defineInput = GUILayout.TextField(define);
            if (defineInput != define)
            {
                defineAlone = defineInput;
                defineAndroid = defineInput;
                defineIos = defineInput;
            }
            EditorGUI.BeginDisabledGroup(isSame && define == PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
            if (GUILayout.Button("Set", GUILayout.Width(40f)))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineInput);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineInput);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineInput);

                // フォーマットエラーがあった時用に書き換え  
                defineInput = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
                defineAlone = defineInput;
                defineIos = defineInput;
                defineAndroid = defineInput;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            // 一括クリアボタン            
            if (GUILayout.Button("Clear", GUILayout.Width(50f)))
            {
                defineAlone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
                defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
                defineIos = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
            }
        }

        // 各define値の更新用テキスト表示
        string OnGUIDefinesSymbols(BuildTargetGroup group, string labelName, string define)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(labelName, GUILayout.Width(60f));
            define = GUILayout.TextField(define);
            EditorGUI.BeginDisabledGroup(define == PlayerSettings.GetScriptingDefineSymbolsForGroup(group));
            if (GUILayout.Button("Set", GUILayout.Width(40f)))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, define);
                define = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            return define;
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
