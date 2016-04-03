/// <summary>
/// 便利機能UnityEditorWindow
/// </summary>
using UnityEngine;
using UnityEditor;
using org.a2dev.UnityScripts.Util;
using System.Collections.Generic;

namespace org.a2dev.UnityScripts.Editor
{
    public class SceneListWindow : BaseEditorWindow<SceneListWindow>
    {
        const string NAME_WINDOW = "SceneList";
        Vector2 scrollPosition = Vector2.zero;

        bool isInitialize = false;

        List<SceneInfo> infos = new List<SceneInfo>();

        EditorBuildSettingsScene[] buildScenes;

        // シーン情報
        public class SceneInfo
        {
            public string path { get; private set; }
            public SceneAsset asset { get; private set; }
            public EditorBuildSettingsScene buildScene { get; set; }

            public SceneInfo(string path)
            {
                this.path = path;
                asset = AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)) as SceneAsset;
                buildScene = null;
            }
        }

        /// <summary>
        /// ウィンドウを開く
        /// </summary>        
        [MenuItem(WINDOW_PREFIX + "SceneList")]
        public static void OpenWindow()
        {
            BaseEditorWindow<SceneListWindow>.OpenWindow();
        }

        /// <summary>
        /// OnGUI
        /// </summary>        
        protected override void OnGUI()
        {
            InitOnce();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            OnGUIMenusButtons();
            OnGUISceneList();

            GUILayout.EndScrollView();
        }

        protected override void Init()
        {
            // ビルド設定のシーン一覧を取得
            buildScenes = EditorBuildSettings.scenes;

            // アッセト内のシーン一覧を取得
            infos.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t:Scene"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // 表示用情報の作成
                SceneInfo info = new SceneInfo(path);
                info.buildScene = getBuildSceneIsExists(path);

                // 追加
                infos.Add(info);
            }
        }

        void OnGUIMenusButtons()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("reset", GUILayout.Width(100f)))
            {
                Init();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// シーンリスト表示
        /// </summary>
        void OnGUISceneList()
        {
            foreach (SceneInfo info in infos)
            {
                GUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(info.asset, typeof(ScriptableObject), true, GUILayout.Width(180f));

                EditorGUI.BeginDisabledGroup(true);
                GUILayout.TextField(info.path);
                EditorGUI.EndDisabledGroup();

                // ビルドシーンへの追加、削除ボタン
                GUILayoutOption option = GUILayout.Width(50f);
                if (info.buildScene == null)
                {
                    if (GUILayout.Button("add", option))
                    {
                        EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(info.path, true);
                        ArrayUtility.Add(
                            ref buildScenes,
                            buildScene
                        );
                        EditorBuildSettings.scenes = buildScenes;
                        info.buildScene = buildScene;
                    }
                }
                else
                {
                    if (GUILayout.Button("remove", option))
                    {
                        ArrayUtility.Remove(
                            ref buildScenes,
                            info.buildScene
                        );
                        EditorBuildSettings.scenes = buildScenes;
                        info.buildScene = null;
                    }
                }

                GUILayout.EndHorizontal();
            }
        }

        // ビルドシーンが存在する場合返却
        EditorBuildSettingsScene getBuildSceneIsExists(string path)
        {
            for (int i = 0; i < buildScenes.Length; i++)
            {
                if (buildScenes[i].path == path)
                {
                    return buildScenes[i];
                }
            }
            return null;
        }
    }

}