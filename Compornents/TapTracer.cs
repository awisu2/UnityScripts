/// <summary>
/// TapTracer
/// </summary>
using UnityEngine;
using org.a2dev.UnityScripts.Util;

public class TapTracer : MonoBehaviour {
    bool isPush = false;
    
	// Use this for initialization
	void Start () {
        GameObjectUtil.GetComponentOrAdd<BoxCollider>(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        // ボタンタップ
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit)){
                GameObject obj = hit.collider.gameObject;
                if(obj == gameObject)
                {
                    isPush = true;
                    Debug.Log(obj.name);
                }
            }
        }

        // ボタンアップを検知
        if(Input.GetMouseButtonUp(0))
        {
            if(isPush)
            {
                isPush = false;
            }
        }
        
        if(isPush)
        {
            // 座標をトレースする
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = transform.position.z;
            transform.position = worldPosition;
        }
	}
}
