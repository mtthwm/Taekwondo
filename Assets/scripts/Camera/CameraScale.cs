using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour {
    public SpriteRenderer ground;

	// Use this for initialization
	void Start () {
        Camera.main.orthographicSize = ground.bounds.size.x * Screen.height / Screen.width * 0.5f;
	}
}
