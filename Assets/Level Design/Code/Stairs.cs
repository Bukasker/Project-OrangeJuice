using UnityEngine;

public class Stairs : MonoBehaviour
{
	public Vector2 stairDirection = Vector2.up;  // Kierunek schodów, domyœlnie w górê

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			PlayerController player = collision.GetComponent<PlayerController>();
			if (player != null)
			{
				player.EnterStairs(stairDirection);  // Wywo³anie publicznej metody
			}
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			PlayerController player = collision.GetComponent<PlayerController>();
			if (player != null)
			{
				player.LeaveStairs();  // Wywo³anie publicznej metody
			}
		}
	}

	void OnDrawGizmos()
	{
		// Opcjonalnie: rysowanie kierunku schodów w edytorze, aby zobaczyæ ustawienie
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + (Vector3)stairDirection * 2);
	}
}
