using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
	[SerializeField] private Animator animator; 

	private float moveX;
	private float moveY;
	[SerializeField] private float lastMoveX;
	[SerializeField] private float lastMoveY;

    public bool IsAttacking = false;
    public bool swordEquiped = false;
	public bool isDead = false;
	private bool isDashing = false;

    [SerializeField] private int maxAttacks = 3;
    [SerializeField] private float resetTime = 0.1f; 

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
            currentAttacks = maxAttacks;

        Debug.Log("Current Attacks: " + currentAttacks);

        IsAttacking = true; // <- dodane
        animator.SetBool("IsAttacking", true);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(ResetAttackCounterAfterTime());
    }

    IEnumerator ResetAttackCounterAfterTime()
    {
        yield return new WaitForSeconds(resetTime);
        currentAttacks = 0;
        IsAttacking = false; // <- dodane
        animator.SetBool("IsAttacking", false);
        Debug.Log("Attack counter reset");
    }

}
