/// <summary>
/// GameObject Util
/// </summary>
using UnityEngine;
using UnityEngine.UI;

namespace org.a2dev.UnityScripts.Util
{
    public class GameObjectUtil
    {
        /// <summary>
        /// コンポーネントが存在するか確認
        /// </summary>
        /// <returns>存在判定フラグ</returns>
        /// <param name="gameObject">hitしたGameObject</param>
        public static bool IsExistsComponent<T>(out GameObject gameObjcect) where T : Object
        {
            // default
            gameObjcect = null;

            // check exists
            foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                T component = go.GetComponent<T>();
                if (component != null)
                {
                    gameObjcect = go;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// canvasの作成
        /// </summary>
        /// <returns>作成したGameObject</returns>
        public static GameObject CreateCanvas()
        {
            GameObject go = new GameObject("Canvas");
            Canvas canvas = go.AddComponent(typeof(Canvas)) as Canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent(typeof(CanvasScaler));
            go.AddComponent(typeof(GraphicRaycaster));

            return go;
        }
        
        /// <summary>
        /// コンポーネントを取得、存在しなければ追加
        /// </summary>
        /// <returns>Component</returns>
        /// <param name="gameObjcect">Componentを追加するComponent</param>
        public static T GetComponentOrAdd<T> (GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if(component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
    }
}
