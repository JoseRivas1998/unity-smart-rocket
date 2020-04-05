using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    public int maxSpeed;

    private DNA dna;

    private int numGenes;
    public int maxSteps { get; set; }
    private Vector3 initialPosition;

    public bool dead { get; private set; }
    public bool won { get; private set; }
    public bool isActive { get { return !dead && !won; } }

    public int step { get { return dna.step; } }

    public float fitness { get; private set; }

    public Renderer rend;

    private GameObject target;

    private Rigidbody rb;

    private float arenaSize;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initialize(Vector3 initialPosition, int numGenes, int maxSteps, GameObject target, float arenaSize) 
    {
        this.numGenes = numGenes;
        this.maxSteps = maxSteps;
        this.initialPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
        this.dna = new DNA(numGenes);
        this.rb = gameObject.GetComponent<Rigidbody>();
        transform.position = initialPosition;
        this.target = target;
        dead = false;
        won = false;
        this.arenaSize = arenaSize;
    }

    private void move()
    {
        if(dna.hasNext())
        {
            Vector3 acc = dna.next();
            this.rb.velocity = (this.rb.velocity.normalized + acc).normalized * maxSpeed;
        } else
        {
            kill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            move();
            transform.LookAt(transform.position + rb.velocity);
            transform.Rotate(0, -90, 0);
            if(transform.position.x < 0 || transform.position.x > arenaSize ||
                transform.position.y < 0 || transform.position.y > arenaSize ||
                transform.position.z < 0 || transform.position.z > arenaSize)
            {
                this.kill();
            }
            if(this.step > maxSteps)
            {
                this.kill();
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void crossOver(GameObject newObject, Rocket parentA, Rocket parentB)
    {
        Rocket child = newObject.GetComponent<Rocket>();
        child.initialize(this.initialPosition, this.numGenes, this.maxSteps, this.target, this.arenaSize);
        child.dna = parentA.dna.crossover(parentB.dna);
    }

    public void clone(GameObject newObject, Rocket parent)
    {
        Rocket clone = newObject.GetComponent<Rocket>();
        clone.initialize(this.initialPosition, this.numGenes, this.maxSteps, this.target, this.arenaSize);
        clone.dna = dna.copy();
    }

    public void kill()
    {
        this.dead = true;
        rend.enabled = false;
        rb.velocity = Vector3.zero;
    }

    public void mutate(float mutationRate)
    {
        dna.mutate(mutationRate);
    }

    public void calculateFitness()
    {
        if (won)
        {
            fitness = 1f / 16f + (10000f) / (dna.step * dna.step);
        } 
        else
        {
            float distanceToGoal = Vector3.Distance(transform.position, target.transform.position);
            fitness = 1f / (distanceToGoal * distanceToGoal);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Target"))
        {
            won = true;
            rb.velocity = Vector3.zero;
            rend.enabled = false;
        }
        if(other.CompareTag("Obstacle"))
        {
            kill();
        }
    }

}
