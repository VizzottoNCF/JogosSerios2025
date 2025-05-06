using System.Collections.Generic;
using UnityEngine;

public class BackdoorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;   // Prefab do inimigo a ser spawnado
    [SerializeField] private float spawnInterval = 3f;   // Intervalo entre os spawns
    [SerializeField] private int maxSpawn = 5;           // Limite máximo de inimigos simultâneos
    [SerializeField] private int health = 150;           // Vida do spawner (maior que a dos inimigos)

    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    [SerializeField] private bool _IsDoorOpen = true;
    [SerializeField] private bool  _HasUpdated = false;

    private float timer = 0f;

    

    private void Update()
    {

        // if door is open, spawns enemies
        if (_IsDoorOpen)
        {
            // Se ainda não atingiu o limite, tenta spawnar inimigos
            if (enemyList.Count < maxSpawn)
            {
                timer += Time.deltaTime;
                if (timer >= spawnInterval)
                {
                    SpawnEnemy();
                    timer = 0f;
                }
            }
        }
        
        // in case there are still enemies
        if (!_IsDoorOpen && !_HasUpdated)
        {
            rf_UpdateBackdoorsLeft();
        }

    }

    private void SpawnEnemy()
    {
        // Ajuste a posição do spawn se necessário (aqui é posicionado próximo ao spawner)
        Vector3 spawnPos = transform.position + new Vector3(1f, 0, 0);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        enemyList.Add(newEnemy);
    }

    private void rf_UpdateBackdoorsLeft()
    {
        if (!_IsDoorOpen && enemyList.Count == 0 && !_HasUpdated)
        {
            _HasUpdated = true;

            // gets finish point script and reduces the amount of backdoors left to be closed
            FinishPoint finishPoint = GameObject.FindWithTag("Finish").GetComponent<FinishPoint>();

            // decreases backdoors left by 1
            finishPoint.LevelObjectives.BackdoorLeft--;
        }
    }

    public void rf_CloseBackdoor()
    {
        if (_IsDoorOpen) { _IsDoorOpen = false; rf_UpdateBackdoorsLeft(); }

        //TODO:
        // when sprite exists, place animator parameter here 
        
    }
}

