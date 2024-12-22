using System;
using System.Collections.Generic;
using System.Threading;

public class ExperimentStore
{
    public static readonly object _lock = new object();
    public static Dictionary<Guid, Experiment> Experiments = new Dictionary<Guid, Experiment>();
    public static Dictionary<Guid, CancellationTokenSource> EvolutionTokens = new Dictionary<Guid, CancellationTokenSource>();

    public static Experiment NewExperiment(Guid id, ExperimentParameters parameters)
    {
        var experiment = new Experiment(parameters);
        Experiments[id] = experiment;
        return experiment;
    }

    public static bool GetExperiments(Guid id, out Experiment experiment)
    {
        return Experiments.TryGetValue(id, out experiment);
    }

    public static bool GetEvolutionTokens(Guid id)
    {
        return EvolutionTokens.ContainsKey(id);
    }
}
