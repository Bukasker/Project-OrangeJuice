using System.Collections;
using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private float moveX;
    private float moveY;
    [SerializeField] private float lastMoveX;
    [SerializeField] private float lastMoveY;

    public bool swordEquiped = false;
    public bool isDead = false;

    private bool isDashing = false;
    public bool isAttacking = false;

    void Update()
    {
        HandleMovement();
        HandleInput();
    }

    void HandleMovement()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            lastMoveX = moveX;
            lastMoveY = moveY;
        }

        animator.SetFloat("X", moveX != 0 ? moveX : lastMoveX);
        animator.SetFloat("Y", moveY != 0 ? moveY : lastMoveY);
        animator.SetFloat("Speed", new Vector2(moveX, moveY).sqrMagnitude);
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && swordEquiped && !isAttacking)
        {
            TryAttack();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            // animator.SetTrigger("Dash");
        }
    }

    public void TryAttack()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        float animLength = GetCurrentAttackAnimationLength();
        StartCoroutine(ResetAttackAfter(animLength));
    }

    private IEnumerator ResetAttackAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndAttack();
    }

    private float GetCurrentAttackAnimationLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

    public void AttackFromCombat()
    {
        if (!isAttacking)
        {
            TryAttack();
        }
    }
}
