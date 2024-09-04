using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Player Settings")]
	[SerializeField] private Rigidbody2D rigidbody;
	[SerializeField] private float runSpeed = 5f;
	private float moveInputX;
	private float moveInputY;
	private Vector2 movement;

	[Header("Jump Settings")]
	[SerializeField] private List<Collider2D> collidersToDisable;
	[SerializeField] private float disableDuration = 0.5f;

	[Header("Stairs Settings")]
	[SerializeField] private float stairsSpeedMovement = 5;
	private bool isOnStairs;
	private float stairsDirectionMovement;


	[Header("Keybinds")]
	[SerializeField] private KeyCode jumpKey = KeyCode.Space;


	private void Update()
	{
		moveInputX = Input.GetAxisRaw("Horizontal");
		moveInputY = Input.GetAxisRaw("Vertical");

		StairsMovement();
		Jump();
		if (Mathf.Abs(moveInputX) < 0.1f && Mathf.Abs(moveInputY) < 0.1f)
		{
			rigidbody.velocity = Vector2.zero;
		}
		else
		{
			rigidbody.velocity = movement;
		}
	}

	public void PlayerIsOnStairs(float direction)
	{
		stairsDirectionMovement = direction;
		isOnStairs = true;
	}
	public void PlayerIsOffStairs()
	{
		stairsDirectionMovement = 0;
		isOnStairs = false;
	}
	private void StairsMovement()
	{
		if (isOnStairs)
		{
			if (stairsDirectionMovement == 1)
			{
				if (moveInputX == 1)
				{
					movement = new Vector2(moveInputX, 1).normalized * (runSpeed + stairsSpeedMovement);
				}
				else
				{
					movement = new Vector2(moveInputX, -1).normalized * (runSpeed + stairsSpeedMovement);
				}
			}
			if (stairsDirectionMovement == -1)
			{
				if (moveInputX == -1)
				{
					movement = new Vector2(moveInputX, 1).normalized * (runSpeed + stairsSpeedMovement);
				}
				else
				{
					movement = new Vector2(moveInputX, -1).normalized * (runSpeed + stairsSpeedMovement);
				}
			}

			if (stairsDirectionMovement == 0)
			{
				movement = new Vector2(moveInputX, moveInputY).normalized * (runSpeed + stairsSpeedMovement);
			}
			else if (moveInputX == 0)
			{
				movement = new Vector2(moveInputX, moveInputY).normalized * runSpeed;
			}
		}
		else
		{
			movement = new Vector2(moveInputX, moveInputY).normalized * runSpeed;
		}
	}

	public void Jump()
	{
		if (Input.GetKeyDown(jumpKey))
		{
			StartCoroutine(DisableCollidersCoroutine());
		}
	}
	private IEnumerator DisableCollidersCoroutine()
	{
		// Wy³¹cz wszystkie collidery w liœcie
		foreach (var collider in collidersToDisable)
		{
			if (collider != null)
			{
				collider.enabled = false;
			}
		}

		// Czekaj przez okreœlony czas
		yield return new WaitForSeconds(disableDuration);

		// W³¹cz ponownie wszystkie collidery w liœcie
		foreach (var collider in collidersToDisable)
		{
			if (collider != null)
			{
				collider.enabled = true;
			}
		}
	}

}
