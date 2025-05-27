using System.Collections.Generic;
using UnityEngine;

public class BackdoorSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _buttonFace;
    [SerializeField] private Material _greenMat;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxSpawn = 5;

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


        // Clean up destroyed enemies from the list
        enemyList.RemoveAll(enemy => enemy == null);

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

            // Try to get the Key component and call the function if it exists
            if (TryGetComponent<Key>(out Key goKey)) { goKey.rf_CompleteKey(); }
        }
    }

    public void rf_CloseBackdoor()
    {
        if (_IsDoorOpen) { _IsDoorOpen = false; rf_UpdateBackdoorsLeft(); }
        
        // plays close door animation
        _animator.SetBool("Close", true);

        // turns buton face green
        _buttonFace.GetComponent<MeshRenderer>().material = _greenMat;
    }
}

