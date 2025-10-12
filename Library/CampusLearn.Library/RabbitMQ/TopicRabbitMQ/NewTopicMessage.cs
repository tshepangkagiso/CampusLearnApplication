namespace CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;

public record NewTopicMessage(
    string Title,
    string ModuleCode,
    int QueryId,
    int StudentId,
    int TutorId,
    DateTime CreatedAt
);
