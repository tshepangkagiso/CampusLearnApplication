IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [FAQs] (
    [FAQID] int NOT NULL IDENTITY,
    [FrequentlyAskedQuestion] nvarchar(500) NOT NULL,
    [Answer] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsPublished] bit NOT NULL,
    [ViewCount] int NOT NULL,
    [TutorID] int NOT NULL,
    [ModuleCode] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_FAQs] PRIMARY KEY ([FAQID])
);

CREATE TABLE [QueryTopics] (
    [QueryTopicID] int NOT NULL IDENTITY,
    [QueryTopicTitle] nvarchar(200) NOT NULL,
    [QueryTopicDescription] nvarchar(max) NOT NULL,
    [RelatedModuleCode] nvarchar(450) NOT NULL,
    [TopicCreationDate] datetime2 NOT NULL,
    [LastActivity] datetime2 NULL,
    [IsResolved] bit NOT NULL,
    [IsUrgent] bit NOT NULL,
    [StudentID] int NOT NULL,
    CONSTRAINT [PK_QueryTopics] PRIMARY KEY ([QueryTopicID])
);

CREATE TABLE [QueryResponses] (
    [QueryResponseID] int NOT NULL IDENTITY,
    [Comment] nvarchar(max) NOT NULL,
    [MediaContentUrl] nvarchar(max) NULL,
    [ResponseCreationDate] datetime2 NOT NULL,
    [IsSolution] bit NOT NULL,
    [HelpfulVotes] int NOT NULL,
    [TutorID] int NOT NULL,
    [QueryTopicID] int NOT NULL,
    CONSTRAINT [PK_QueryResponses] PRIMARY KEY ([QueryResponseID]),
    CONSTRAINT [FK_QueryResponses_QueryTopics_QueryTopicID] FOREIGN KEY ([QueryTopicID]) REFERENCES [QueryTopics] ([QueryTopicID]) ON DELETE CASCADE
);

CREATE INDEX [IX_FAQs_ModuleCode] ON [FAQs] ([ModuleCode]);

CREATE INDEX [IX_QueryResponses_QueryTopicID] ON [QueryResponses] ([QueryTopicID]);

CREATE INDEX [IX_QueryTopics_IsResolved] ON [QueryTopics] ([IsResolved]);

CREATE INDEX [IX_QueryTopics_RelatedModuleCode] ON [QueryTopics] ([RelatedModuleCode]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251004010606_InitialCreate', N'9.0.9');

COMMIT;
GO

