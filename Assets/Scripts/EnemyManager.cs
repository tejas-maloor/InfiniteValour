using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private bool _isActive = false;

    private EnemyScript[] enemies;
    public EnemyStruct[] allEnemies;
    public List<int> enemyIndexes;

    private Coroutine AI_Loop_Coroutine;

    public int aliveEnemyCount;

    private ComboCharacter character;

    private void Start()
    {
        enemies = GetComponentsInChildren<EnemyScript>();

        allEnemies = new EnemyStruct[enemies.Length];

        for(int i = 0; i <  allEnemies.Length; i++)
        {
            allEnemies[i].enemyScript = enemies[i];
            allEnemies[i].enemyAvailability = true;
        }

        character = FindObjectOfType<ComboCharacter>();

    }

    public void StartAI()
    {
        AI_Loop_Coroutine = StartCoroutine(AI_Loop(null));
    }

    IEnumerator AI_Loop(EnemyScript enemy)
    {
        if(AliveEnemyCount() == 0 || character.IsDead())
        {
            StopCoroutine(AI_Loop(null));
            yield break;
        }

        yield return new WaitForSeconds(Random.Range(.5f, 1.5f));

        EnemyScript attackingEnemy = RandomEnemyExcludingOne(enemy);

        if (attackingEnemy == null)
            attackingEnemy = RandomEnemy();

        if (attackingEnemy == null)
            yield break;

        yield return new WaitUntil(() => attackingEnemy.IsRetreating() == false);
        yield return new WaitUntil(() => attackingEnemy.IsLockedTarget() == false);
        yield return new WaitUntil(() => attackingEnemy.IsStunned() == false);

        attackingEnemy.SetAttack();

        yield return new WaitUntil(() => attackingEnemy.IsPreparingAttack() == false);

        attackingEnemy.SetRetreat();

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        if (AliveEnemyCount() > 0)
            AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isActive = true;
            StartAI();
        }
    }

    public EnemyScript RandomEnemy()
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyScript randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyScript;

        return randomEnemy;
    }

    public EnemyScript RandomEnemyExcludingOne(EnemyScript exclude)
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability && allEnemies[i].enemyScript != exclude)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyScript randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyScript;

        return randomEnemy;
    }


    public int AvailableEnemyCount()
    {
        int count = 0;
        for(int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                count++;
        }
        return count;
    }

    public int AliveEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript.isActiveAndEnabled)
                count++;
        }
        aliveEnemyCount = count;
        return count;
    }

    public void SetEnemyAvailiability(EnemyScript enemy, bool state)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript == enemy)
                allEnemies[i].enemyAvailability = state;
        }

        AliveEnemyCount();

        if (FindObjectOfType<EnemyDetector>().CurrentTarget() == enemy)
            FindObjectOfType<EnemyDetector>().SetCurrentTarget(null);
    }
}

[System.Serializable]
public struct EnemyStruct
{
    public EnemyScript enemyScript;
    public bool enemyAvailability;
}
