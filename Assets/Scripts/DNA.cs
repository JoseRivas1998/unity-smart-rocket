using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DNA
{
    private int numGenes;
    private Vector3[] genes;
    public int step { get; private set; }

    public DNA(int numGenes)
    {
        this.numGenes = numGenes;
        this.genes = new Vector3[numGenes];
        this.step = 0;
        randomize(numGenes);
    }

    private void randomize(int numGenes)
    {
        for (int i = 0; i < numGenes; i++)
        {
            this.genes[i] = Random.onUnitSphere;
        }
    }

    public bool hasNext()
    {
        return step < genes.Length;
    }

    public Vector3 next()
    {
        Vector3 next = genes[step++];
        return new Vector3(next.x, next.y, next.z);
    }

    public DNA copy()
    {
        DNA clone = new DNA(this.numGenes);
        for (int i = 0; i < this.numGenes; i++)
        {
            Vector3 gene = this.genes[i];
            clone.genes[i] = new Vector3(gene.x, gene.y, gene.z);
        }
        return clone;
    }

    public DNA crossover(DNA parent)
    {
        DNA child = new DNA(this.numGenes);
        int halfIndex = this.numGenes / 2;
        for (int i = 0; i < this.numGenes; i++)
        {
            Vector3 gene;
            if (i < halfIndex)
            {
                gene = this.genes[i];
            }
            else
            {
                gene = parent.genes[i];
            }
            child.genes[i] = new Vector3(gene.x, gene.y, gene.z);
        }
        return child;
    }

    public void mutate(float mutationRate)
    {
        for (int i = 0; i < this.genes.Length; i++)
        {
            if (Random.Range(0f, 1f) < mutationRate)
            {
                this.genes[i] = Random.onUnitSphere;
            }
        }
    }

}
