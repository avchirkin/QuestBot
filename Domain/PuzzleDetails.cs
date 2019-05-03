using System;

namespace CoreBot
{
    /// <summary>
    /// Класс для передачи данных из диалога в диалог 
    /// </summary>
    public class PuzzleDetails
    {
        // пустой конструктор необходим для дессериализации bot framework
        public PuzzleDetails()
        {

        }
        
        public PuzzleDetails(Puzzle puzzle)
        {
            ScenarioId = "1";
            PuzzleId = puzzle.Id;
            Question = puzzle.Question;
            ExpectedAnswer = puzzle.Answer;
            WaitUntilReceiveRightAnswer = puzzle.WaitUntilReceiveRightAnswer;
        }

        public string ScenarioId { get; set; }
        public string PuzzleId { get; set; }
        public string Question { get; set; }
        public string ExpectedAnswer { get; set; }
        public string ActualAnswer { get; set; }
        public bool? WaitUntilReceiveRightAnswer { get; set; }

        public bool IsRight => string.Equals(ExpectedAnswer, ActualAnswer, StringComparison.CurrentCultureIgnoreCase);
    }
}