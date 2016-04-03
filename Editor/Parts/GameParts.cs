using UnityEngine;
using UnityEditor;

namespace org.a2dev.UnityScripts.Editor.Parts
{
    public static class GameParts
    {
        /// <summary>
        /// stepボタンの代替ボタン
        /// </summary>
        public static void OnGUIStep()
        {
            if (GUILayout.Button("Step"))
            {
                EditorApplication.Step();
            }
        }

    }

}

