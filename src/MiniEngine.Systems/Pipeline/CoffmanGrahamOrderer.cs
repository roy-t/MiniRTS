using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.Systems.Pipeline
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Coffman%E2%80%93Graham_algorithm
    /// </summary>
    public static class CoffmanGrahamOrderer
    {
        public static List<List<SystemSpec>> DivideIntoStages(List<SystemSpec> systemSpecs)
        {
            var expanded = ExpandRequirements(systemSpecs);
            var ordered = Order(expanded);
            var stages = CreateStages(ordered);

            //return SingleThreaded(stages);

            SplitStagesWithMixedParallelism(stages);
            return stages;
        }

        private static List<List<SystemSpec>> SingleThreaded(List<List<SystemSpec>> stages)
        {
            var single = new List<List<SystemSpec>>();

            foreach (var stage in stages)
            {
                foreach (var system in stage)
                {
                    var sequentialStage = new List<SystemSpec>(new[] { system });
                    single.Add(sequentialStage);
                }
            }
            return single;
        }

        private static List<SystemSpec> Order(IReadOnlyList<SystemSpec> systemSpecs)
        {
            var orderedSystemSpecs = systemSpecs.Where(systemSpec => systemSpec.RequiredResources.Count == 0).ToList();
            var unorderedSystemSpecs = systemSpecs.Where(systemSpec => systemSpec.RequiredResources.Count > 0).ToList();

            while (unorderedSystemSpecs.Count > 0)
            {
                var candidate = GetNextCandidate(unorderedSystemSpecs, orderedSystemSpecs);

                if (candidate == null)
                {
                    throw new Exception("Unsatisfiable dependency or cycle detected");
                }

                orderedSystemSpecs.Add(candidate);
                unorderedSystemSpecs.Remove(candidate);
            }

            return orderedSystemSpecs;
        }

        private static IReadOnlyList<SystemSpec> ExpandRequirements(IReadOnlyList<SystemSpec> systemSpecs)
        {
            var producedResources = systemSpecs.SelectMany(s => s.ProducedResources)
                                               .GroupBy(r => r.Resource)
                                               .ToDictionary(gr => gr.Key, gr => gr.ToList());

            var expanded = new List<SystemSpec>();
            foreach (var spec in systemSpecs)
            {
                if (spec.RequiredResources.Any(r => r.SubResource == SystemSpec.MatchAllSubResources))
                {
                    spec.ExpandRequiredResource(producedResources);
                }

                expanded.Add(spec);
            }
            return expanded;
        }

        private static List<List<SystemSpec>> CreateStages(List<SystemSpec> orderedSystemSpecs)
        {
            var stages = new List<List<SystemSpec>>();
            var produced = new List<ResourceState>();

            var currentStage = new List<SystemSpec>();

            foreach (var systemSpec in orderedSystemSpecs)
            {
                if (AllRequirementsHaveBeenProduced(systemSpec, produced))
                {
                    currentStage.Add(systemSpec);
                }
                else
                {
                    produced.AddRange(GetProducedResource(currentStage));
                    if (AllRequirementsHaveBeenProduced(systemSpec, produced))
                    {
                        stages.Add(currentStage);
                        currentStage = new List<SystemSpec>() { systemSpec };
                    }
                    else
                    {
                        throw new Exception($"Algorithm error, did you forget to first call {nameof(CoffmanGrahamOrderer)}.{nameof(Order)}?");
                    }
                }
            }

            if (currentStage.Count > 0)
            {
                stages.Add(currentStage);
            }

            return stages;
        }

        private static void SplitStagesWithMixedParallelism(List<List<SystemSpec>> stages)
        {
            for (var i = 0; i < stages.Count; i++)
            {
                var stage = stages[i];

                while (stage.Count > 1 && stage.Any(s => !s.AllowParallelism))
                {
                    var sequentialSystem = stage.First(s => !s.AllowParallelism);
                    stage.Remove(sequentialSystem);

                    var sequentialStage = new List<SystemSpec>(new[] { sequentialSystem });
                    stages.Insert(i++, sequentialStage);
                }
            }
        }

        private static bool AllRequirementsHaveBeenProduced(SystemSpec systemSpec, List<ResourceState> produced)
            => systemSpec.RequiredResources.All(resource => produced.Contains(resource));

        private static List<ResourceState> GetProducedResource(List<SystemSpec> systemSpecs)
            => systemSpecs.SelectMany(systemSpec => systemSpec.ProducedResources).ToList();

        private static SystemSpec? GetNextCandidate(List<SystemSpec> unorderedSystemSpecs, List<SystemSpec> orderedSystemSpecs)
        {
            var maxDistance = int.MinValue;
            SystemSpec? candidate = null;

            foreach (var systemSpec in unorderedSystemSpecs)
            {
                // The best candidate has the largest distance from the items that produce their
                // requirements so that it does not have to wait long for what it needs.
                var minDistance = int.MaxValue;
                foreach (var requirement in systemSpec.RequiredResources)
                {
                    var distance = DistanceToProducer(requirement, orderedSystemSpecs);
                    if (distance == null)
                    {
                        goto NextCandidate;
                    }

                    minDistance = Math.Min(distance.Value, minDistance);
                }

                if (minDistance > maxDistance)
                {
                    maxDistance = minDistance;
                    candidate = systemSpec;
                }

            NextCandidate: { }
            }

            return candidate;
        }

        private static int? DistanceToProducer(ResourceState requirement, List<SystemSpec> orderedSystemSpecs)
        {
            var producer = orderedSystemSpecs.Where(systemSpec => systemSpec.ProducedResources.Contains(requirement)).FirstOrDefault();

            if (producer != null)
            {
                var insertAt = orderedSystemSpecs.Count;
                return insertAt - orderedSystemSpecs.IndexOf(producer);
            }

            return null;
        }
    }
}
