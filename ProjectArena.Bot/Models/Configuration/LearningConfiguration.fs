namespace ProjectArena.Bot.Models.Configuration

type LearningConfiguration = {
    IsLearning: bool
    BatchSize: int
    ModelsAmount: int
    SuccessfulModelsAmount: int
    MutationProbability: float
    TimeTillSurrender: float32

    VictoryPerformanceCoefficient: float
    EnemyPowerPerformanceCoefficient: float
    PlayerPowerPerformanceCoefficient: float
    
    MagnifyingHiddenNeuronsCount: int
    CommandHiddenNeuronsCount: int
}