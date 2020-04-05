using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{

    public int populationSize;
    public int numGenes;
    public GameObject target;
    public GameObject rocketPrefab;
    public float arenaSize;
    public float mutationRate;

    private Rocket[] rockets;
    private int maxSteps;
    private float fitnessSum;

    // Start is called before the first frame update
    void Start()
    {
        rockets = new Rocket[populationSize];
        maxSteps = numGenes;
        for (int i = 0; i < rockets.Length; i++)
        {
            rockets[i] = createRocket();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.one * (arenaSize / 2f), Vector3.one * arenaSize);
    }

    void LateUpdate()
    {
        if(isAllInactive())
        {
            calculateFitness();
            naturalSelection();
            mutate();
        }
    }

    public bool isAllInactive()
    {
        foreach (Rocket rocket in rockets)
        {
            if (rocket.isActive) return false;
        }
        return true;
    }

    public void calculateFitness()
    {
        foreach (Rocket rocket in rockets)
        {
            rocket.calculateFitness();
        }
    }

    public void naturalSelection()
    {
        Rocket[] newRockets = new Rocket[rockets.Length];
        calculateFitnessSum();

        int best = getBestRocket();
        Rocket bestRocket = createRocket();
        bestRocket.clone(bestRocket.gameObject, rockets[best]);
        newRockets[0] = bestRocket;

        calcMaxSteps();

        for (int i = 1; i < rockets.Length; i++)
        {
            Rocket parentA = selectParent();
            Rocket parentB = selectParent();
            newRockets[i] = createRocket();
            newRockets[i].crossOver(newRockets[i].gameObject, parentA, parentB);
            newRockets[i].maxSteps = maxSteps;
        }

        for(int i = 0; i < rockets.Length; i++)
        {
            Destroy(rockets[i].gameObject);
        }

        rockets = newRockets;

    }

    private Rocket createRocket()
    {
        GameObject rocket = GameObject.Instantiate(rocketPrefab);
        Rocket r = rocket.GetComponent<Rocket>();
        r.initialize(this.transform.position, this.numGenes, this.maxSteps, target, this.arenaSize);
        return r;
    }

    public void calculateFitnessSum()
    {
        fitnessSum = 0;
        foreach (Rocket rocket in rockets)
        {
            fitnessSum += rocket.fitness;
        }
    }

    public void mutate()
    {
        for (int i = 1; i < rockets.Length; i++)
        {
            rockets[i].mutate(this.mutationRate);
        }
    }

    private void calcMaxSteps()
    {
        int lowestSteps = numGenes;
        foreach (Rocket rocket in rockets)
        {
            if(rocket.won && rocket.step < lowestSteps)
            {
                lowestSteps = rocket.step;
            }
        }
        maxSteps = lowestSteps;
    }

    public Rocket selectParent()
    {
        float rand = (float)(Random.Range(0f, 1f) * fitnessSum);
        float runningSum = 0;
        foreach (Rocket rocket in rockets)
        {
            runningSum += rocket.fitness;
            if(runningSum >= rand)
            {
                return rocket;
            }
        }
        return null;
    }

    public int getBestRocket()
    {
        int bestRocket = 0;
        float bestFitness = 0;
        for (int i = 0; i < rockets.Length; i++)
        {
            Rocket rocket = rockets[i];
            if(rocket.fitness > bestFitness)
            {
                bestRocket = i;
                bestFitness = rocket.fitness;
            }
        }
        return bestRocket;
    }

    public Vector3 avgPosition()
    {
        Vector3 sum = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        int numPoints = 1;
        foreach (Rocket rocket in rockets)
        {
            if(rocket.isActive)
            {
                sum += rocket.transform.position;
                numPoints++;
            }
        }
        return sum / numPoints;
    }

}
