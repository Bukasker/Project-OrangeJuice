using UnityEngine;

public class Stairs : MonoBehaviour
{
	public Vector2 stairDirection = Vector2.up;  // Kierunek schod�w, domy�lnie w g�r�

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			PlayerController player = collision.GetComponent<PlayerController>();
			if (player != null)
			{
				player.EnterStairs(stairDirection);  // Wywo�anie publicznej metody
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
				player.LeaveStairs();  // Wywo�anie publicznej metody
			}
		}
	}

	void OnDrawGizmos()
	{
		// Opcjonalnie: rysowanie kierunku schod�w w edytorze, aby zobaczy� ustawienie
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + (Vector3)stairDirection * 2);
	}
}
