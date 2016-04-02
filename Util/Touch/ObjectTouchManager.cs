/// <summary>
/// ObjectTouchManager
/// </summary>
using UnityEngine;
using System;
using System.Collections.Generic;

namespace org.a2dev.UnityScripts.Util.Touch
{
    public class ObjectTouchManager : MonoBehaviour
    {
        // クラス名
        const string CLASS_NAME = "TouchManager";

        // 押下イベント発生中のインデックス
        const int PUSH_INDEX_NON = -1;

        // 押下ボタンインデックス        
        int pushIndex = PUSH_INDEX_NON;

        // TODO：属性でプロパティ作れないものか？
        [SerializeField]
        private List<TouchEvent> objects;

        /// <summary>
        /// Instantiate
        /// </summary>
        /// <returns>ClickChecker</returns>
        /// <param name="name">GameObject名</param>
        public static ObjectTouchManager Instantiate(string name = CLASS_NAME)
        {
            return GameObjectUtil.GetComponentOrAddWithGameobjectSingleton<ObjectTouchManager>(name);
        }

        /// <summary>
        /// イベントの追加
        /// </summary>
        /// <param name="touchEvent">イベント追加</param>
        public void AddEvent(TouchEvent touchEvent)
        {
            objects.Add(touchEvent);
        }

        /// <summary>
        /// イベントの追加
        /// </summary>
        /// <param name="touchEvent">イベント追加</param>
        public void AddEvent(GameObject go, 
            Action<GameObject, Vector3> onDown = null, 
            Action<GameObject> onUp = null, 
            Action<GameObject, Vector3> onPush = null)
        {
            TouchEvent touchEvent = new TouchEvent(go, onDown, onUp, onPush);
            objects.Add(touchEvent);
        }

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            objects = new List<TouchEvent>();
        }

        /// <summary>
        /// Start
        /// </summary>
        void Start()
        {

        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            // 判定対象がない場合は処理を行わない
            if (objects.Count > 0)
            {
                UpdateClickCheck();
            }
        }

        /// <summary>
        /// クリックの状態チェック
        /// </summary>
        void UpdateClickCheck()
        {
            if (pushIndex != PUSH_INDEX_NON)
            {
                // ボタンアップ
                if (Input.GetMouseButtonUp(0))
                {
                    if(objects[pushIndex].onUp != null)
                    {
                        objects[pushIndex].onUp(objects[pushIndex].gameObject);
                    }
                    pushIndex = PUSH_INDEX_NON;
                }
            }
            else
            {
                // ボタン押下
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit))
                    {
                        GameObject hitGameObject = hit.collider.gameObject;

                        for (int i = 0; i < objects.Count; i++)
                        {
                            if (objects[i].gameObject == hitGameObject)
                            {
                                pushIndex = i;
                                if(objects[pushIndex].onDown != null)
                                {
                                    objects[pushIndex].onDown(objects[pushIndex].gameObject, Input.mousePosition);
                                }
                            }
                        }
                    }
                }
            }

            // push
            if (pushIndex != PUSH_INDEX_NON)
            {
                if(objects[pushIndex].onPush != null)
                {
                    objects[pushIndex].onPush(objects[pushIndex].gameObject, Input.mousePosition);
                }
            }
        }
    }
}

