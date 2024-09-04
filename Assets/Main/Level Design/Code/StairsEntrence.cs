using UnityEngine;
using UnityEngine.Events;

public class StairsEntrence : MonoBehaviour
{
	[SerializeField] private Collider2D stairsCollider;

	[SerializeField] private UnityEvent stairsEventEnter;
	[SerializeField] private UnityEvent stairsButtonEventExit;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			if (stairsCollider != null)
				stairsCollider.isTrigger = true;

			stairsEventEnter.Invoke();
		}
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			if (stairsCollider != null)
				stairsCollider.isTrigger = false;

			stairsButtonEventExit.Invoke();
		}
	}
}
