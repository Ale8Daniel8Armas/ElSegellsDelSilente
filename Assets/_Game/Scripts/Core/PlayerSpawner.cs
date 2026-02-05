using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        string targetID = GameManager.instance.nextSpawnPointID;

        Debug.Log("GameManager dice que debo ir a: '" + targetID + "'");

        if (string.IsNullOrEmpty(targetID)) return;

        GameObject spawnPoint = GameObject.Find(targetID);

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            GameManager.instance.nextSpawnPointID = "";
        }
        else
        {
            Debug.LogWarning("No se encontró el punto de aparición: " + targetID);
        }
    }    
}