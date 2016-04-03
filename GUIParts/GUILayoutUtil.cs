/// <summary>
/// GUILayoutUtil.cs
/// GUIと、GUILayoutのUtil
/// UnityEngine側からも利用することを考え、UnityEditorは利用禁止
/// </summary>
using UnityEngine;
using System;

namespace org.a2dev.UnityScripts.GUIParts
{
    public static class GUILayoutUtil
    {
        // 太文字
        static GUIStyle _styleBold;
        public static GUIStyle styleBold
        {
            get
            {
                if (_styleBold == null)
                {
                    _styleBold = new GUIStyle(GUI.skin.label);
                    _styleBold.fontStyle = FontStyle.Bold;
                }
                return _styleBold;
            }
        }

        // 太文字
        static GUIStyle _styleItalic;
        public static GUIStyle styleItalic
        {
            get
            {
                if (_styleItalic == null)
                {
                    _styleItalic = new GUIStyle(GUI.skin.label);
                    _styleItalic.fontStyle = FontStyle.Italic;
                }
                return _styleItalic;
            }
        }

        /// <summary>
        /// ラベル表示
        /// </summary>
        /// <param name="text">表示文字列</param>
        /// <param name="isBold"></param>
        public static void Label(string text, bool isBold = false, float width = 0f)
        {
            // 太文字
            GUIStyle style = null;
            if (isBold)
            {
                style = styleBold;
            }

            // 幅
            GUILayoutOption option = null;
            if (width > 0f)
            {
                option = GUILayout.Width(width);
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

        public class Toolbar
        {
            int selected;
            string[] texts;
            GUILayoutOption[] options;

            public Toolbar(string[] texts, int selected = -1, GUILayoutOption[] options = null)
            {
                this.texts = texts;
                this.selected = selected;
                this.options = options;
            }

            public int Update()
            {
                selected = GUILayout.Toolbar(selected, texts, options);
                return selected;
            }
        }

        public static Vector2 ScrollView(Vector2 position, Action actGUI)
        {
            position = GUILayout.BeginScrollView(position);
            actGUI();
            GUILayout.EndScrollView();

            return position;
        }

        public static void Horisozontal(Action actGUI)
        {
            GUILayout.BeginHorizontal();
            actGUI();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// ボタン
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="actGUI">押下時のファンクション</param>
        /// <param name="width">ボタン幅</param>
        public static void Button(string text, Action actGUI, float width = 0f)
        {
            if (width > 0f)
            {
                if (GUILayout.Button(text, GUILayout.Width(width)))
                {
                    actGUI();
                }
            }
            else
            {
                if (GUILayout.Button(text))
                {
                    actGUI();
                }
            }
        }

    }

}

