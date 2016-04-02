using UnityEngine;
using UnityEditor;

namespace org.a2dev.UnityScripts.Editor
{
    public static class GuiStyleUtil
    {
        public class TextField
        {
            // singletonインスタンス
            static TextField instance = null;

            // 初期テキスト色
            public Color defaultTextColor { get; private set; }

            // インスタンス取得    
            public static TextField GetInstance()
            {
                if (instance == null)
                {
                    instance = new TextField();
                }
                return instance;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            TextField()
            {
                defaultTextColor = EditorStyles.textField.normal.textColor;
            }

            /// <summary>
            /// リセット
            /// </summary>
            /// <returns>インスタンス</returns>
            public TextField Reset()
            {
                GUIStyleState state = EditorStyles.textField.normal;
                state.textColor = defaultTextColor;
                return this;
            }

            /// <summary>
            /// テキスト色設定
            /// </summary>
            /// <returns>インスタンス</returns>
            /// <param name="textColor">テキストカラー</param>
            public TextField SetTextColor(Color textColor)
            {
                EditorStyles.textField.normal.textColor = textColor;
                return this;
            }
        }
    }
}
