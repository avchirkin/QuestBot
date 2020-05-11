//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using CoreBot;
//using ScenarioBot.Domain;
//using Shields.GraphViz.Components;
//using Shields.GraphViz.Models;
//using Shields.GraphViz.Services;
//
//namespace ScenarioBot.Service
//{
//    public class GraphvizScenarioGenerator
//    {
//        public static async Task<string> Generate(Scenario scenario)
//        {
//            var statements = new List<Statement>();
//            foreach (var item in scenario.Collection)
//            {
//                foreach (var branch in item.PossibleBranches)
//                {
//                    statements.Add(new EdgeStatement(item.Id, branch.GoToId, new Dictionary<Id, Id> { { "label", $"{branch.Answer}" } }.ToImmutableDictionary()));
//                }
//                statements.Add(new EdgeStatement(item.Id, item.ElseBranch, new Dictionary<Id, Id> { { "label", "else" } }.ToImmutableDictionary()));
//            }
//
//            var graph = Graph.Undirected.AddRange(statements);
//            IRenderer renderer = new Renderer(@"C:\projects\bot\graphviz-2.38\release\bin");
//            var cProjectsBotOutGraphPng = $@"C:\projects\bot\out\{Guid.NewGuid()}.png";
//            using (Stream file = File.Create(cProjectsBotOutGraphPng))
//            {
//                await renderer.RunAsync(
//                    graph, file,
//                    RendererLayouts.Dot,
//                    RendererFormats.Png,
//                    CancellationToken.None);
//            }
//
//            return cProjectsBotOutGraphPng;
//        }
//    }
//}

