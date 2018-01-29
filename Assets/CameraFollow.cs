using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public GameObject player;
	// the offset of the camera to centrate the player in the X axis
	public float offsetX = 2;
	// the offset of the camera to centrate the player in the Z axis
	public float offsetZ = 12;
	public float maximumDistance = 2;
	public float playerVelocity = 10;
	public float positionY = 20;
	public float positionX = 0;
	public float positionZ = 0;
	private float movementX;
	private float movementZ;
	
	public Camera followCamera;

	void Update()
	{
		movementX = (player.transform.position.x +offsetX - this.transform.position.x)/maximumDistance;
		movementZ = (player.transform.position.z +offsetZ - this.transform.position.z)/maximumDistance;
		this.transform.position += new Vector3((movementX*playerVelocity*Time.deltaTime), 0, (movementZ*playerVelocity*Time.deltaTime));
	}

}
