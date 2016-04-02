/// <summary>
/// TapTracer
/// コンポーネントに追加すると、オブジェクトをクリックで動くようになる
/// コンポーネントに追加するだけで、オブジェクトをクリックで動くようにする
/// </summary>
using UnityEngine;
using org.a2dev.UnityScripts.Util.Touch;
using System;

namespace org.a2dev.UnityScripts.Compornents
{
    public class TapTrace : MonoBehaviour
    {
        // プッシュ時のトレース用
        static Action<GameObject, Vector3> onPush = (GameObject go, Vector3 position) => 
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            worldPosition.z = go.transform.position.z;
            go.transform.position = worldPosition;
        };

        /// <summary>
        /// オブジェクトをタップでトレースするようにする
        /// </summary>
        /// <param name="gameObject">トレース処理を設定するGameObject</param>
        public static void SetTapTracer(GameObject gameObject)
        {
            ObjectTouchManager manager = ObjectTouchManager.Instantiate();
            manager.AddEvent(gameObject, null, null, onPush);
        }

        // Use this for initialization
        void Start()
        {
            SetTapTracer(gameObject);
        }
    }

}

