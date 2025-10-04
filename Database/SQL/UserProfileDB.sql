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
CREATE TABLE [Modules] (
    [ModuleID] int NOT NULL IDENTITY,
    [ModuleName] nvarchar(100) NOT NULL,
    [ModuleCode] nvarchar(20) NOT NULL,
    [ProgramType] int NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Modules] PRIMARY KEY ([ModuleID])
);

CREATE TABLE [UserProfiles] (
    [UserProfileID] int NOT NULL IDENTITY,
    [UserRole] int NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Surname] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [ProfilePictureUrl] nvarchar(max) NOT NULL,
    [Qualification] int NOT NULL,
    [StudentNumber] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastLogin] datetime2 NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_UserProfiles] PRIMARY KEY ([UserProfileID])
);

CREATE TABLE [Logins] (
    [LoginID] int NOT NULL IDENTITY,
    [Email] nvarchar(450) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastPasswordChange] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [FailedLoginAttempts] int NOT NULL,
    [LockoutEnd] datetime2 NULL,
    [UserProfileID] int NOT NULL,
    CONSTRAINT [PK_Logins] PRIMARY KEY ([LoginID]),
    CONSTRAINT [FK_Logins_UserProfiles_UserProfileID] FOREIGN KEY ([UserProfileID]) REFERENCES [UserProfiles] ([UserProfileID]) ON DELETE CASCADE
);

CREATE TABLE [Students] (
    [StudentID] int NOT NULL IDENTITY,
    [UserProfileID] int NOT NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([StudentID]),
    CONSTRAINT [FK_Students_UserProfiles_UserProfileID] FOREIGN KEY ([UserProfileID]) REFERENCES [UserProfiles] ([UserProfileID]) ON DELETE CASCADE
);

CREATE TABLE [Tutors] (
    [TutorID] int NOT NULL IDENTITY,
    [UserProfileID] int NOT NULL,
    [IsAdmin] bit NOT NULL,
    [AdminSince] datetime2 NULL,
    CONSTRAINT [PK_Tutors] PRIMARY KEY ([TutorID]),
    CONSTRAINT [FK_Tutors_UserProfiles_UserProfileID] FOREIGN KEY ([UserProfileID]) REFERENCES [UserProfiles] ([UserProfileID]) ON DELETE CASCADE
);

CREATE TABLE [StudentModules] (
    [StudentID] int NOT NULL,
    [ModuleID] int NOT NULL,
    [SubscribedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_StudentModules] PRIMARY KEY ([StudentID], [ModuleID]),
    CONSTRAINT [FK_StudentModules_Modules_ModuleID] FOREIGN KEY ([ModuleID]) REFERENCES [Modules] ([ModuleID]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentModules_Students_StudentID] FOREIGN KEY ([StudentID]) REFERENCES [Students] ([StudentID]) ON DELETE CASCADE
);

CREATE TABLE [TutorModules] (
    [TutorID] int NOT NULL,
    [ModuleID] int NOT NULL,
    [QualifiedSince] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_TutorModules] PRIMARY KEY ([TutorID], [ModuleID]),
    CONSTRAINT [FK_TutorModules_Modules_ModuleID] FOREIGN KEY ([ModuleID]) REFERENCES [Modules] ([ModuleID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TutorModules_Tutors_TutorID] FOREIGN KEY ([TutorID]) REFERENCES [Tutors] ([TutorID]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Logins_Email] ON [Logins] ([Email]);

CREATE UNIQUE INDEX [IX_Logins_UserProfileID] ON [Logins] ([UserProfileID]);

CREATE UNIQUE INDEX [IX_Modules_ModuleCode] ON [Modules] ([ModuleCode]);

CREATE INDEX [IX_StudentModules_ModuleID] ON [StudentModules] ([ModuleID]);

CREATE UNIQUE INDEX [IX_Students_UserProfileID] ON [Students] ([UserProfileID]);

CREATE INDEX [IX_TutorModules_ModuleID] ON [TutorModules] ([ModuleID]);

CREATE UNIQUE INDEX [IX_Tutors_UserProfileID] ON [Tutors] ([UserProfileID]);

CREATE UNIQUE INDEX [IX_UserProfiles_Email] ON [UserProfiles] ([Email]);

CREATE UNIQUE INDEX [IX_UserProfiles_StudentNumber] ON [UserProfiles] ([StudentNumber]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251004010428_InitialCreate', N'9.0.9');

COMMIT;
GO

