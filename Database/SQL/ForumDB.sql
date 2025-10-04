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
CREATE TABLE [ForumTopics] (
    [ForumTopicID] int NOT NULL IDENTITY,
    [ForumTopicTitle] nvarchar(200) NOT NULL,
    [ForumTopicDescription] nvarchar(max) NOT NULL,
    [RelatedModuleCode] nvarchar(450) NOT NULL,
    [TopicUpVote] int NOT NULL,
    [ViewCount] int NOT NULL,
    [TopicCreationDate] datetime2 NOT NULL,
    [LastActivity] datetime2 NULL,
    [UserProfileID] int NULL,
    [IsAnonymous] bit NOT NULL,
    [AnonymousName] nvarchar(max) NULL,
    [IsLocked] bit NOT NULL,
    [IsPinned] bit NOT NULL,
    [IsFeatured] bit NOT NULL,
    CONSTRAINT [PK_ForumTopics] PRIMARY KEY ([ForumTopicID])
);

CREATE TABLE [ForumTopicResponses] (
    [ResponseID] int NOT NULL IDENTITY,
    [Comment] nvarchar(max) NOT NULL,
    [MediaContentUrl] nvarchar(max) NULL,
    [ResponseUpVote] int NOT NULL,
    [ResponseCreationDate] datetime2 NOT NULL,
    [UserProfileID] int NULL,
    [IsAnonymous] bit NOT NULL,
    [AnonymousName] nvarchar(max) NULL,
    [ForumTopicID] int NOT NULL,
    CONSTRAINT [PK_ForumTopicResponses] PRIMARY KEY ([ResponseID]),
    CONSTRAINT [FK_ForumTopicResponses_ForumTopics_ForumTopicID] FOREIGN KEY ([ForumTopicID]) REFERENCES [ForumTopics] ([ForumTopicID]) ON DELETE CASCADE
);

CREATE INDEX [IX_ForumTopicResponses_ForumTopicID] ON [ForumTopicResponses] ([ForumTopicID]);

CREATE INDEX [IX_ForumTopicResponses_UserProfileID] ON [ForumTopicResponses] ([UserProfileID]);

CREATE INDEX [IX_ForumTopics_IsPinned] ON [ForumTopics] ([IsPinned]);

CREATE INDEX [IX_ForumTopics_LastActivity] ON [ForumTopics] ([LastActivity]);

CREATE INDEX [IX_ForumTopics_RelatedModuleCode] ON [ForumTopics] ([RelatedModuleCode]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251004010648_InitialCreate', N'9.0.9');

COMMIT;
GO

