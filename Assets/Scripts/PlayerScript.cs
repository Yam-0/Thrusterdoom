using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneController))]
public class PlayerScript : MonoBehaviour
{
	private Vector2 input = Vector2.zero;
	private DroneController droneController;

	void Start()
	{
		droneController = GetComponent<DroneController>();
	}

	void Update()
	{
		input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		input.y = Mathf.Max(0, input.y);
		droneController.SetMoveInput(input);
		droneController.SetBoostingInput(Input.GetKey(KeyCode.LeftShift));
		droneController.SetGlideInput(Input.GetKey(KeyCode.Space));
	}
}
