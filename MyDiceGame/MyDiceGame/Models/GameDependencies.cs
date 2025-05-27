
    public class GameDependencies
    {
        public List<Dice> DiceList { get; }
        public IOutputPrinter Printer { get; }
        public IFairRandomGenerator Random { get; }
        public IHmacGenerator HmacGenerator { get; }
        public IProbabilityCalculator Calculator { get; }
        public IProbabilityVisualizer Visualizer { get; }
        public IHmacTurnHandler TurnHandler { get; }

        public GameDependencies(
            List<Dice> diceList,
            IOutputPrinter printer,
            IFairRandomGenerator random,
            IHmacGenerator hmacGenerator,
            IProbabilityCalculator calculator,
            IProbabilityVisualizer visualizer,
            IHmacTurnHandler turnHandler)
        {
            DiceList = diceList;
            Printer = printer;
            Random = random;
            HmacGenerator = hmacGenerator;
            Calculator = calculator;
            Visualizer = visualizer;
            TurnHandler = turnHandler;
        }
    }