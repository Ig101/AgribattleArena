namespace ProjectArena.Bot.Models.Configuration

type LearningConfiguration = {
    IsLearning: bool
    ModelsAmount: int
    SuccessfulModelsAmount: int
    LearningMutationProbability: float
    WorkingMutationProbability: float
}