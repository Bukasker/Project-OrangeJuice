using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
	public int currentFloor = 0;

	void Update()
	{
		// Zaktualizuj warstw� renderowania postaci w zale�no�ci od pozycji Z
		GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.y * 100) + currentFloor * 1000;
	}
}
