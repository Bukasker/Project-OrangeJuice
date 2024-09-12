using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerAnimations : MonoBehaviour
{
	[SerializeField] private Animator animator; 
	//[SerializeField] private Rigidbody2D playerRigidbody2D;

	private float moveX;
	private float moveY;
	[SerializeField] private float lastMoveX;
	[SerializeField] private float lastMoveY;

	public float jumpForce = 5f;

	public bool swordEquiped = false;
	public bool isDead = false;
	private bool isDashing = false;

	public int maxAttacks = 3; 
	public float resetTime = 0.21f; 

	private int currentAttacks = 0; 
	private Coroutine attackCoroutine; 

	void Update()
	{
		moveX = Input.GetAxisRaw("Horizontal");
		moveY = Input.GetAxisRaw("Vertical");

		if (moveX != 0 || moveY != 0)
		{
			lastMoveX = moveX;
			lastMoveY = moveY;

			animator.SetFloat("X", moveX);
			animator.SetFloat("Y", moveY);
		}
		else
		{
			animator.SetFloat("X", lastMoveX);
			animator.SetFloat("Y", lastMoveY);
		}

		animator.SetFloat("Speed", new Vector2(moveX, moveY).sqrMagnitude);

		if (Input.GetMouseButtonDown(0))
		{
			if (swordEquiped)
			{
				RegisterAttack();
				animator.SetBool("IsAttacking", true);
				animator.SetInteger("AttackCount", currentAttacks);
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
		{
			animator.SetTrigger("Dash");
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			animator.SetTrigger("IsJumping");
		}

		if (swordEquiped)
		{
			animator.SetBool("IsSwordEquiped", true);
		}
		else
		{
			animator.SetBool("IsSwordEquiped", false);
		}
		if (isDead)
		{
			animator.SetBool("IsDead", true);
		}
		else
		{
			animator.SetBool("IsDead", false);
		}
		
	}

	void RegisterAttack()
	{
		currentAttacks++; 

		if (currentAttacks > maxAttacks)
		{
			currentAttacks = maxAttacks; 
		}

		Debug.Log("Current Attacks: " + currentAttacks);

		if (attackCoroutine != null)
		{
			StopCoroutine(attackCoroutine);
		}
		attackCoroutine = StartCoroutine(ResetAttackCounterAfterTime());
	}

	IEnumerator ResetAttackCounterAfterTime()
	{
		//yield return new WaitForSeconds(0.21f);

		yield return new WaitForSeconds(resetTime);
		currentAttacks = 0;
		animator.SetBool("IsAttacking", false);
		Debug.Log("Attack counter reset");
	}
}
