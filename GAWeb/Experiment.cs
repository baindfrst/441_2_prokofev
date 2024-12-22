using System;
using System.Threading.Tasks;
using System.Linq;
using lib;
using System.Diagnostics;

public class Experiment
{
    public ExperimentParameters Config { get; private set; }
    public int countCity { get; private set; }
    public int[][] Matrix { get; private set; }
    public Genom Best { get; private set; }
    public double BestScore { get; private set; }
    public long Epochs { get; private set; }
    public List<Population> Populations { get; private set; }

    public Experiment(ExperimentParameters parameters)
    {
        Config = parameters;
        countCity = parameters.countCity;
        Matrix = GenerateMatrix(countCity);
        Debug.WriteLine(Matrix);
        Populations = new List<Population>();
        for (int i = 0; i < parameters.countThreads; i++) Populations.Add(new Population(parameters.countPopulation, Matrix, parameters.lr));
        Best = Populations.Select(p => p.bestGen).OrderBy(g => g.GenomScore).FirstOrDefault();
        BestScore = Best.GenomScore;
        Epochs = 0;
    }

    public Result RunStep()
    {
        Epochs++;
        bool new_best = false;
        foreach (var population in Populations)
        {
            population.GenNewEpoch();
            int ind_best = population.CalculateSolutionsLenght();
            if (BestScore > population.genomsresult.Min())
            {
                new_best = true;
                BestScore = population.genomsresult.Min();
                Best = population.genArray[ind_best].ClonePopulation();
            }
        }
        return new Result { Best = Best.ToString(), BestScore = BestScore, Epochs = Epochs, new_best = new_best };
    }

    public static int[][] GenerateMatrix(int size)
    {
        Random random = new Random();
        int[][] matrix = new int[size][];
        for (int i = 0; i < size; i++)
        {
            matrix[i] = new int[size];
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                int distance = random.Next(1, 100);
                matrix[i][j] = distance;
                matrix[j][i] = distance;
            }
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                for (int k = j + 1; k < size; k++)
                {
                    if (matrix[i][j] + matrix[j][k] < matrix[i][k])
                    {
                        matrix[i][k] = matrix[i][j] + matrix[j][k] + 1;
                        matrix[k][i] = matrix[i][k];
                    }
                }
            }
        }

        return matrix;
    }

}

public class Result
{
    public String Best { get; set; }
    public double BestScore { get; set; }
    public long Epochs { get; set; }
    public bool new_best { get; set; }
}
