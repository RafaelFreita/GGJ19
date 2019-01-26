using UnityEngine;

public class RespawnController : MonoBehaviour
{

    public GameObject playerPrefab;
    public RespawnObject respawnObject;
    public float timeToDestroy;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject, timeToDestroy);
        Instantiate(playerPrefab, respawnObject.spawnPoint);
    }
}
