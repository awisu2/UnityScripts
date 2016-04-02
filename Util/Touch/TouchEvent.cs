/// <summary>
/// TouchEvent
/// </summary>
using UnityEngine;
using System;

namespace org.a2dev.UnityScripts.Util.Touch
{
    public class TouchEvent
    {
        /// gameObject
        public GameObject gameObject{ get; private set; }

        /// onDown
        public Action<GameObject, Vector3> onDown{ get; private set; }

        /// onUp
        public Action<GameObject> onUp{ get; private set; }

        /// onPush
        public Action<GameObject, Vector3> onPush{ get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gameObject">対象となるGameObject</param>
        /// <param name="onDown">ボタンダウン時イベント</param>
        /// <param name="onUp">ボタンアップ時イベント</param>
        /// <param name="onPush">ボタンプッシュ時イベント</param>
        public TouchEvent(GameObject gameObject,
            Action<GameObject, Vector3> onDown = null,
            Action<GameObject> onUp = null,
            Action<GameObject, Vector3> onPush = null)
        {
            this.gameObject = gameObject;
            this.onDown = onDown;
            this.onUp = onUp;
            this.onPush = onPush;

            // コンポーネント追加
            GameObjectUtil.GetComponentOrAdd<BoxCollider>(this.gameObject);
        }

    }

}
