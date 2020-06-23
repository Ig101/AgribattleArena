namespace ProjectArena.Bot.Models.Configuration

type LearningConfiguration = {
    IsLearning: bool
    ModelsAmount: int
    SuccessfulModelsAmount: int
    LearningMutationProbability: float
    WorkingMutationProbability: float
    TimeTillSurrender: float32

    VictoryPerformanceCoefficient: float
    EnemyPowerPerformanceCoefficient: float
    PlayerPowerPerformanceCoefficient: float
    
    MagnifyingHiddenNeuronsCount: int
    CommandHiddenNeuronsCount: int
}