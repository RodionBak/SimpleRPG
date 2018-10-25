using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//класс здания
public class Build : GameUnit {


    [Header("Build")]
    public List<GameObject> lvlStates = new List<GameObject>();
    public GameUnit spawnUnit;
    public Transform spawnPoint;
    float nextSpawnTime = 0f;

	// Use this for initialization
	void Start () {
        InitBuild();
	}

    public void InitBuild()
    {
        InitUnit();
        nextSpawnTime = 1f / attackSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        if (!active) return;

	    if (nextSpawnTime > 0f)
        {
            nextSpawnTime -= Time.deltaTime;
        }
        else
        {
            nextSpawnTime = 1f / attackSpeed;
            BuildUnit();
        }
	}


    //получение нового уровня
    public override void OnLevelUp(int _newLevel)
    {
        base.OnLevelUp(_newLevel);
        int maxLevel = _newLevel;

        if (maxLevel > (lvlStates.Count - 1))
        {
            maxLevel = lvlStates.Count - 1;
        }

        for (int i=0; i<lvlStates.Count;i++)
        {
            lvlStates[i].SetActive(i == maxLevel);
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();

        active = false;
    }


    //создание юнита
    public void BuildUnit()
    {
        if (spawnUnit == null) return;
        if (spawnPoint == null) return;

        GameUnit _newUnit = Instantiate(spawnUnit, spawnPoint.position, spawnPoint.rotation) as GameUnit;
        _newUnit.defaultTargets = defaultTargets;
        _newUnit.Target = Target;
        _newUnit.country = country;
        _newUnit.Level = Level;
        _newUnit.commander = commander;
    }
}
