using UnityEditor;

public class BaseEditorWindow<T> : EditorWindow where T : EditorWindow
{
    public static string className = typeof(T).Name;

    /// <summary>
    /// ウィンドウを開く
    /// </summary>
    /// <returns>ウィンドウのインスタンスクラス</returns>
    /// <param name="title">タイトル</param>
    /// <param name="isUtil">ユーティリティWindowフラグ(trueにするとタブがなくなる)</param>
    public static T OpenWindow(string title = null, bool isUtil = false)
    {
        if(string.IsNullOrEmpty(title))
        {
            title = className;
        }
        return EditorWindow.GetWindow(typeof(T), isUtil, title) as T;
    }
}
