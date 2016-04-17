using UnityEngine;

public class RotationComponent : MonoBehaviour {

	// 一周に掛ける時間
	public float rotateTime = 1f;

	public Vector3 rotateRate = new Vector3 (0.5f, 1f, 0.8f);

	// 一周の単位
	private float CYCLE = 360f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float rate = Time.deltaTime / rotateTime;

		// 回転		 
		transform.Rotate(rotateRate, rate * CYCLE);
	}
}