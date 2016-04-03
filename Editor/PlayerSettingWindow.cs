using UnityEngine;
using UnityEditor;
using org.a2dev.UnityScripts.GUIParts;

namespace org.a2dev.UnityScripts.Editor
{
    public class PlayerSettingWindow : BaseEditorWindow<PlayerSettingWindow>
    {
        // window用の名称(クラス名から)
        const string NAME_WINDOW = "PlayerSetting";

        // 初期identifier
        public const string DEFAULT_IDENTIFIER = "com.Company.ProductName";

        // OS毎のdefine設定
        static string defineAlone;
        static string defineAndroid;
        static string defineIos;

        // orientationの選択Index
        static int orientationIndex = -1;

        // オリエンテーション簡単設定項目
        static string[] OrientationSettings = new string[]
        {
            "Default",
            "Rotation",
            "Portrait",
        };

        static bool isInitStatic = false;

        protected static void InitOnceStatic()
        {
            if(isInitStatic == false)
            {
                InitStatic();
                isInitStatic = true;
            }
        }

        protected static void InitStatic()
        {
            // define値の取得
            defineAlone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            defineIos = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        protected override void OnGUI()
        {
            InitOnceStatic();
            OnGUIStatic();
        }
        
        public static void OnGUIStatic() 
        {
            InitOnceStatic();
            
            // 左にスペースを空ける
            GUILayoutUtil.LeftSpace(() =>
            {
                // bundleIdentifier設定
                OnGUIStepPlayerSettingBundleIdentifier();

                // orientation設定
                GUILayout.Label("Orientation");
                OnGUIStepPlayerSettingOrientation();

                GUILayout.Label("Defines Symbols");
                OnGUIStepPlayerSettingDefineSymbols();
            });
        }

        /// <summary>
        /// BundleIdentifierGUI
        /// </summary>
        public static void OnGUIStepPlayerSettingBundleIdentifier()
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
        public static void OnGUIStepPlayerSettingOrientation()
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
            SetOrientationCustom(GUILayout.Toolbar(-1, OrientationSettings));
        }

        /// <summary>
        /// Orientationの設定を行う
        /// </summary>
        /// <param name="index">カスタム設定index</param>
        static void SetOrientationCustom(int index)
        {
            switch (index)
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
        public static void OnGUIStepPlayerSettingDefineSymbols()
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
        static string OnGUIDefinesSymbols(BuildTargetGroup group, string labelName, string define)
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
    }

}
