using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScenarioBot.Domain;
using ScenarioBot.Repository;

namespace ScenarioBot.Service
{
    public class ScenarioService : IScenarioService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly ILogger<ScenarioService> _logger;
        private readonly IDictionary<string, Scenario> _store;

        public ScenarioService(ILogger<ScenarioService> logger,
            IAnswerRepository answerRepository)
        {
            _logger = logger;
            _answerRepository = answerRepository;
            _store = new Dictionary<string, Scenario>(StringComparer.CurrentCultureIgnoreCase);
        }

        public async Task<IList<string>> GetNotCompletedScenarioNames(UserId teamId)
        {
            var loadedScenarioNames = _store.Select(x => x.Key.ToLower()).ToArray();
            var completedScenarioNames = await _answerRepository.GetCompletedScenarioIds(teamId);
            return loadedScenarioNames.Except(completedScenarioNames).ToList();
        }

        public async Task<ScenarioDetails> GetLastScenarioDetailsExceptGameOver(UserId userId, string scenarioId)
        {
            var answer = await _answerRepository.GetLastAddedAnswerFromNotCompletedScenario(userId, scenarioId);
            if (answer == null)
                // значит у пользователя нет начатого сценария
                return null;

            var puzzle = GetNextPuzzle(userId, answer.ScenarioId, answer.PuzzleId, answer.ActualAnswer);
            return new ScenarioDetails
            {
                ScenarioId = answer.ScenarioId,
                UserId = userId,
                LastPuzzleDetails = new PuzzleDetails(puzzle)
            };
        }

        public bool IsExist(string scenarioId)
        {
            return _store.ContainsKey(scenarioId);
        }


        public Puzzle GetNextPuzzle(UserId teamId, string scenarioId, string lastPuzzleId, string lastAnswer)
        {
            var scenario = _store[scenarioId];

            if (string.IsNullOrEmpty(lastPuzzleId))
                return scenario.Collection.First(x =>
                    string.Equals(x.Id, Puzzle.RootId, StringComparison.CurrentCultureIgnoreCase));

            var puzzle = scenario.Collection.First(x =>
                string.Equals(x.Id, lastPuzzleId, StringComparison.CurrentCultureIgnoreCase));

            // для корректной работы переключения между сценариями,
            // когда один начат и не закончан, переключились на другой, снова вернулись 
            if (string.IsNullOrEmpty(lastAnswer)) return puzzle;

            var puzzleId = puzzle.GetNextPossibleBranchId(lastAnswer);

            var retPuzzle = scenario.Collection.FirstOrDefault(x =>
                string.Equals(x.Id, puzzleId, StringComparison.CurrentCultureIgnoreCase));

            if (retPuzzle == null)
            {
                _logger.LogError("Seems, that infinite loop detected in scenario {@scenarioId} in the puzzle {@puzzleId}." +
                                 "One of the reasons is unfinished node in the scenario", puzzleId, scenarioId);
                retPuzzle = puzzle;
            }

            return retPuzzle;
        }

        public bool IsOver(string scenarioId, string lastPuzzleId)
        {
            var scenario = _store[scenarioId];
            var puzzle = scenario.Collection.First(x =>
                string.Equals(x.Id, lastPuzzleId, StringComparison.CurrentCultureIgnoreCase));
            return puzzle.IsLastPuzzle;
        }
        
        public async Task<bool> IsOverByUserAsync(UserId userId, string scenarioId)
        {
            var isOver = await _answerRepository.IsScenarioCompletedByAsync(userId, scenarioId);
            return isOver;
        }

        public void LoadAll()
        {
            var dr = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "raw_data/2020.DotNext.Spb"));
            var scs = dr.GetFiles("*.json").Select(x => Load(x.FullName))
                .Where(x => x != null)
                .ToList();
            var join = string.Join(", ",scs.Select(x=>x.ScenarioId));
            _logger.LogInformation("Loaded scenarios: {@join}", join);
        }

        private Scenario Load(string path)
        {
            try
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
                var value = File.ReadAllText(path);
                var scenario = JsonConvert.DeserializeObject<Scenario>(value);
                _store[scenario.ScenarioId] = scenario;
                return scenario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "While loading scenario {@path}", path);
            }

            return null;
        }
    }
}