/// <summary>
/// TouchComponent
/// OSによらずタッチ判定を行う
/// </summary>
using UnityEngine;

namespace org.a2dev.UnityScripts.Compornents
{
    public class TouchStatusComponent : MonoBehaviour
    {
        // 各種タップステータス
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        TouchStatus[] statuses = new TouchStatus[5];
#else
        TouchStatus[] statuses = new TouchStatus[1];
#endif

        public delegate void OnDownHandler(TouchStatus[] position);
        public delegate void OnUpHandler(TouchStatus[] position);
        public delegate void OnPushHandler(TouchStatus[] position);

        public event OnDownHandler OnDown;
        public event OnUpHandler OnUp;
        public event OnPushHandler OnPush;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            Init();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Init()
        {
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i] = new TouchStatus();
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            for(int i = 0; i < Input.touchCount; i++)
            {
                TouchPhase phase = Input.GetTouch(i).phase;
                
                if(statuses[i].push)
                {
                    statuses[i].down = false;
                    if (phase == TouchPhase.Ended)
                    {
                        statuses[i].SetUp();
                    }
                }
                else
                {
                    statuses[i].up = false;
                    if (phase == TouchPhase.Began)
                    {
                        statuses[i].SetDown();
                    }
                }
            }            
#else
            if (statuses[0].push)
            {
                statuses[0].down = false;
            }
            else
            {
                statuses[0].SetNon();
                if (Input.GetMouseButtonDown(0))
                {
                    statuses[0].SetDown();
                    if (OnDown != null)
                        OnDown(statuses);
                }
            }
            
            if (statuses[0].push)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    statuses[0].SetUp();
                    if (OnUp != null)
                        OnUp(statuses);
                }
                else
                {
                    if(OnPush != null)
                        OnPush(statuses);
                }
            }
#endif
        }

        /// <summary>
        /// ステータスの取得
        /// </summary>
        /// <returns>TouchStatus</returns>
        /// <param name="index">取得するステータスのインデックス</param>
        public TouchStatus GetStatus(int index = 0)
        {
            return statuses[index];
        }

        public string ToStringStatus()
        {
            string str = "";
            for (int i = 0; i < statuses.Length; i++)
            {
                string s = "";
                if (statuses[i].non)
                {
                    s += "non,";
                }
                if (statuses[i].down)
                {
                    s += "down,";
                }
                if (statuses[i].push)
                {
                    s += "push,";
                }
                if (statuses[i].up)
                {
                    s += "up,";
                }

                // 
                if (string.IsNullOrEmpty(s) == false)
                {
                    str += i.ToString() + " : " + s.Substring(0, s.Length - 1) + "; ";
                }
            }
            return str;
        }
    }
}

public class TouchStatus
{
    public bool non { get; private set; }
    public bool down { get; set; }
    public bool push { get; private set; }
    public bool up { get; set; }

    public TouchStatus()
    {
        SetNon();
    }

    public void SetNon()
    {
        non = true;
        down = false;
        push = false;
        up = false;
    }

    public void SetDown()
    {
        non = false;
        down = true;
        push = true;
        up = false;
    }

    public void SetUp()
    {
        non = false;
        down = false;
        push = false;
        up = true;
    }
}