using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Player Settings")]
    [SerializeField] private PlayerAnimationsController playerAnimationsController;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
	[SerializeField] private float runSpeed = 5f;
	private float moveInputX;
	private float moveInputY;
	private Vector2 movement;


	[Header("Stairs Settings")]
	[SerializeField] private float stairsSpeedMovement = 5;
	private bool isOnStairs;
	private float stairsDirectionMovement;


	private void Update()
	{
		moveInputX = Input.GetAxisRaw("Horizontal");
		moveInputY = Input.GetAxisRaw("Vertical");

        if (playerAnimationsController != null && playerAnimationsController.IsAttacking)
        {
            playerRigidbody2D.velocity = Vector2.zero;
            return;
        }

        StairsMovement();

		if (Mathf.Abs(moveInputX) < 0.1f && Mathf.Abs(moveInputY) < 0.1f)
		{
			playerRigidbody2D.velocity = Vector2.zero;
		}
		else
		{
			playerRigidbody2D.velocity = movement;
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

}
