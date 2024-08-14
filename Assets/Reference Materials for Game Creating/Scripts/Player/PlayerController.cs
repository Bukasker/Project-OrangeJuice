using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Rigidbody2D rigidbody;
	[SerializeField] private float runSpeed = 5f;

	public float stairSpeedMultiplier = 0.5f;  // Jak szybko gracz porusza siê w górê lub w dó³ po schodach

	public bool isOnStairs = false;

	public Vector2 stairDirection;

	private Vector2 movement;


	// Definiowanie zdarzeñ
	public event Action<Vector2> OnEnterStairs;
	public event Action OnExitStairs;

	private void Update()
	{
		float moveInputX = Input.GetAxisRaw("Horizontal");
		float moveInputY = Input.GetAxisRaw("Vertical");

		movement = new Vector2(moveInputX, moveInputY).normalized * runSpeed;

		if (Mathf.Abs(moveInputX) < 0.1f && Mathf.Abs(moveInputY) < 0.1f)
		{
			rigidbody.velocity = Vector2.zero;
		}
		else
		{
			rigidbody.velocity = movement;
		}

		if (isOnStairs)
		{
			transform.Translate(new Vector2(movement.x, movement.x * stairDirection.y * stairSpeedMultiplier) * runSpeed * Time.deltaTime);
		}
		else
		{
			// Normalny ruch gracza
			transform.Translate(movement * runSpeed * Time.deltaTime);
		}
	}

	// Metoda, która bêdzie wywo³ywana przez zdarzenie OnEnterStairs
	public void SetStairMovement(Vector2 direction)
	{
		stairDirection = direction;
		isOnStairs = true;
	}

	// Metoda, która bêdzie wywo³ywana przez zdarzenie OnExitStairs
	public void ExitStairs()
	{
		isOnStairs = false;
	}

	private void OnEnable()
	{
		// Subskrybowanie zdarzeñ
		OnEnterStairs += SetStairMovement;
		OnExitStairs += ExitStairs;
	}

	private void OnDisable()
	{
		// Odsubskrybowanie zdarzeñ
		OnEnterStairs -= SetStairMovement;
		OnExitStairs -= ExitStairs;
	}

}
