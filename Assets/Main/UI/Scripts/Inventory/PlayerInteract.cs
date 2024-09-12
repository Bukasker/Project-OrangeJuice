using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerInteract : MonoBehaviour
{
	[Header("Interact Settings")]
	[SerializeField] private Camera playerCamera;
	[SerializeField] private List<GameObject> itemObjects;
	[SerializeField] private GameObject interactObject;

	private ItemPickUp itemPickUp;
	public GameObject focusedItem;

	[Header("Animation Settings")]
	[SerializeField] private Transform playerTransform;
	[SerializeField] private float objectSpeed = 5f;

	[Header("Cursor Events")]
	[SerializeField] private UnityEvent changeToReguralCursorEvent;
	[SerializeField] private UnityEvent changeToHoverCursorEvent;
	[SerializeField] private UnityEvent changeToClickCursorEvent;
	private bool mouseButtonIsPressed;

	private void Start()
	{
		changeToReguralCursorEvent?.Invoke();
	}
	private void Update()
	{
		CheckForInteractable();
		if (Input.GetMouseButtonDown(0)) // Sprawdzenie, czy lewy przycisk myszy zosta³ klikniêty
		{
			if (interactObject != null)
			{
				mouseButtonIsPressed=true;
				InteractWithObject();
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			mouseButtonIsPressed = false;
			changeToReguralCursorEvent?.Invoke();	
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			if (itemObjects != null)
			{
				RemoveFirstItemOnList();
			}
		}
	}
	public void RemoveFirstItemOnList()
	{
		if (itemObjects.Count != 0)
		{
			focusedItem = itemObjects[0];
			itemPickUp = focusedItem.GetComponent<ItemPickUp>();
			Inventory.Instance.AddItemToClosestSlot(itemPickUp.Item);

			Destroy(focusedItem);
			itemObjects.Remove(focusedItem);

		}
	}
	private void InteractWithObject()
	{
		if (interactObject != null)
		{
			changeToClickCursorEvent?.Invoke();
			Debug.Log("Interact with " + interactObject.name);
		}
	}
	private void OnTriggerEnter2D(Collider2D col)
	{
		if(col != null)
		{
			if (col.CompareTag("Item"))
			{
				itemObjects.Add(col.gameObject);
			}
			if (col.CompareTag("Gold"))
			{
				var transform = col.gameObject.GetComponent<Transform>();
				//Add gold to invetory
				StartCoroutine(MoveToPosition(transform, col.gameObject));
			}
		}
	}
	private void OnTriggerExit2D(Collider2D col)
	{
		if (col != null)
		{
			if (col.gameObject != null)
			{
				if (col.CompareTag("Item"))
				{
					itemObjects.Remove(col.gameObject);
				}
			}
		}
	}
	private IEnumerator MoveToPosition(Transform objectToMove, GameObject objectToDestroy)
	{
		while (objectToMove != null && playerTransform != null &&
			   Vector3.Distance(objectToMove.position, playerTransform.position) > 0.1f)
		{
			Vector3 direction = playerTransform.position - objectToMove.position;
			direction.Normalize();
			objectToMove.position += direction * objectSpeed * Time.deltaTime;
			yield return null;
		}

		if (objectToMove != null)
		{
			objectToMove.position = playerTransform.position;
			Destroy(objectToDestroy);
		}
	}

	void CheckForInteractable()
	{
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if (hit.collider != null)
		{
			if (hit.collider.CompareTag("Interact"))
			{
				interactObject = hit.collider.gameObject;
				if (!mouseButtonIsPressed)
				{
					changeToHoverCursorEvent.Invoke();
				}
			}
			else
			{
				changeToReguralCursorEvent.Invoke();
				interactObject = null;
			}
		}
		else
		{
			changeToReguralCursorEvent.Invoke();
			interactObject = null;
		}
	}

}
