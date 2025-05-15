using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGate : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("chuj");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
