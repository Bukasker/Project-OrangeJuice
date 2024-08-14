using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
	public int currentFloor = 0;

	void Update()
	{
		// Zaktualizuj warstwê renderowania postaci w zale¿noœci od pozycji Z
		GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.y * 100) + currentFloor * 1000;
	}
}
