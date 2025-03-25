using UnityEngine;

public class BackdoorSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;   // Prefab do inimigo a ser spawnado
    public float spawnInterval = 3f;   // Intervalo entre os spawns
    public int maxSpawn = 5;           // Limite máximo de inimigos simultâneos
    public int health = 150;           // Vida do spawner (maior que a dos inimigos)

    private int currentSpawn = 0;
    private float timer = 0f;

    void Update()
    {
        // Se ainda não atingiu o limite, tenta spawnar inimigos
        if (currentSpawn < maxSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
    }

    void SpawnEnemy()
    {
        // Ajuste a posição do spawn se necessário (aqui é posicionado próximo ao spawner)
        Vector3 spawnPos = transform.position + new Vector3(1f, 0, 0);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentSpawn++;
    }

    // Método que pode ser chamado pelos inimigos ao morrer para diminuir a contagem atual
    public void OnEnemyDeath()
    {
        currentSpawn--;
    }
}

