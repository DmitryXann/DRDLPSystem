
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 04/28/2013 18:09:29
-- Generated from EDMX file: C:\Users\Admin\DRDLPSystem\DRDLPSystemDAL\DRDLPModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DRDLP_TEST_DB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_PCHardware]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HardwareSet] DROP CONSTRAINT [FK_PCHardware];
GO
IF OBJECT_ID(N'[dbo].[FK_PCUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSet] DROP CONSTRAINT [FK_PCUser];
GO
IF OBJECT_ID(N'[dbo].[FK_PCDocuments]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentSet] DROP CONSTRAINT [FK_PCDocuments];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentsUserAccess]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserAccessSet] DROP CONSTRAINT [FK_DocumentsUserAccess];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserAccess_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserUserAccess] DROP CONSTRAINT [FK_UserUserAccess_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserAccess_UserAccess]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserUserAccess] DROP CONSTRAINT [FK_UserUserAccess_UserAccess];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentsUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentSet] DROP CONSTRAINT [FK_DocumentsUser];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentsUser1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentSet] DROP CONSTRAINT [FK_DocumentsUser1];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentPathPC]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentPathSet] DROP CONSTRAINT [FK_DocumentPathPC];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentDocumentPath]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocumentPathSet] DROP CONSTRAINT [FK_DocumentDocumentPath];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AdministratorSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdministratorSet];
GO
IF OBJECT_ID(N'[dbo].[HardwareSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HardwareSet];
GO
IF OBJECT_ID(N'[dbo].[PCSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PCSet];
GO
IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[DocumentSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DocumentSet];
GO
IF OBJECT_ID(N'[dbo].[UserAccessSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAccessSet];
GO
IF OBJECT_ID(N'[dbo].[DocumentPathSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DocumentPathSet];
GO
IF OBJECT_ID(N'[dbo].[UserUserAccess]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserUserAccess];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AdministratorSet'
CREATE TABLE [dbo].[AdministratorSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Login] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [NeedsToChangePassword] bit  NOT NULL
);
GO

-- Creating table 'HardwareSet'
CREATE TABLE [dbo].[HardwareSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HardwareID] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [OtherInfo] nvarchar(max)  NOT NULL,
    [Type] tinyint  NOT NULL,
    [PC_Id] int  NOT NULL
);
GO

-- Creating table 'PCSet'
CREATE TABLE [dbo].[PCSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Valid] bit  NOT NULL,
    [PCHardwareBasedID] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Login] nvarchar(max)  NOT NULL,
    [Valid] bit  NOT NULL,
    [PC_Id] int  NOT NULL
);
GO

-- Creating table 'DocumentSet'
CREATE TABLE [dbo].[DocumentSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DocumentID] nvarchar(max)  NOT NULL,
    [LastChange] datetime  NOT NULL,
    [Version] bigint  NOT NULL,
    [PC_Id] int  NOT NULL,
    [LastUserAccess_Id] int  NOT NULL,
    [LastUserAccessWithChanges_Id] int  NOT NULL
);
GO

-- Creating table 'UserAccessSet'
CREATE TABLE [dbo].[UserAccessSet] (
    [AccessType] tinyint  NOT NULL,
    [Documents_Id] int  NOT NULL
);
GO

-- Creating table 'DocumentPathSet'
CREATE TABLE [dbo].[DocumentPathSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Path] nvarchar(max)  NOT NULL,
    [PC_Id] int  NOT NULL,
    [Document_Id] int  NOT NULL
);
GO

-- Creating table 'AccessLogSet'
CREATE TABLE [dbo].[AccessLogSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccessType] tinyint  NOT NULL,
    [AccessDateTime] datetime  NOT NULL,
    [LogEntryProcessed] bit  NOT NULL,
    [Document_Id] int  NOT NULL,
    [User_Id] int  NOT NULL,
    [PC_Id] int  NOT NULL,
    [DocumentPath_Id] int  NOT NULL,
    [UserAccess_AccessType] tinyint  NOT NULL,
    [Hardware_Id] int  NOT NULL
);
GO

-- Creating table 'UserUserAccess'
CREATE TABLE [dbo].[UserUserAccess] (
    [Users_Id] int  NOT NULL,
    [UserAccesses_AccessType] tinyint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AdministratorSet'
ALTER TABLE [dbo].[AdministratorSet]
ADD CONSTRAINT [PK_AdministratorSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HardwareSet'
ALTER TABLE [dbo].[HardwareSet]
ADD CONSTRAINT [PK_HardwareSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PCSet'
ALTER TABLE [dbo].[PCSet]
ADD CONSTRAINT [PK_PCSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DocumentSet'
ALTER TABLE [dbo].[DocumentSet]
ADD CONSTRAINT [PK_DocumentSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [AccessType] in table 'UserAccessSet'
ALTER TABLE [dbo].[UserAccessSet]
ADD CONSTRAINT [PK_UserAccessSet]
    PRIMARY KEY CLUSTERED ([AccessType] ASC);
GO

-- Creating primary key on [Id] in table 'DocumentPathSet'
ALTER TABLE [dbo].[DocumentPathSet]
ADD CONSTRAINT [PK_DocumentPathSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AccessLogSet'
ALTER TABLE [dbo].[AccessLogSet]
ADD CONSTRAINT [PK_AccessLogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Users_Id], [UserAccesses_AccessType] in table 'UserUserAccess'
ALTER TABLE [dbo].[UserUserAccess]
ADD CONSTRAINT [PK_UserUserAccess]
    PRIMARY KEY NONCLUSTERED ([Users_Id], [UserAccesses_AccessType] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [PC_Id] in table 'HardwareSet'
ALTER TABLE [dbo].[HardwareSet]
ADD CONSTRAINT [FK_PCHardware]
    FOREIGN KEY ([PC_Id])
    REFERENCES [dbo].[PCSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PCHardware'
CREATE INDEX [IX_FK_PCHardware]
ON [dbo].[HardwareSet]
    ([PC_Id]);
GO

-- Creating foreign key on [PC_Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [FK_PCUser]
    FOREIGN KEY ([PC_Id])
    REFERENCES [dbo].[PCSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PCUser'
CREATE INDEX [IX_FK_PCUser]
ON [dbo].[UserSet]
    ([PC_Id]);
GO

-- Creating foreign key on [PC_Id] in table 'DocumentSet'
ALTER TABLE [dbo].[DocumentSet]
ADD CONSTRAINT [FK_PCDocuments]
    FOREIGN KEY ([PC_Id])
    REFERENCES [dbo].[PCSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PCDocuments'
CREATE INDEX [IX_FK_PCDocuments]
ON [dbo].[DocumentSet]
    ([PC_Id]);
GO

-- Creating foreign key on [Documents_Id] in table 'UserAccessSet'
ALTER TABLE [dbo].[UserAccessSet]
ADD CONSTRAINT [FK_DocumentsUserAccess]
    FOREIGN KEY ([Documents_Id])
    REFERENCES [dbo].[DocumentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentsUserAccess'
CREATE INDEX [IX_FK_DocumentsUserAccess]
ON [dbo].[UserAccessSet]
    ([Documents_Id]);
GO

-- Creating foreign key on [Users_Id] in table 'UserUserAccess'
ALTER TABLE [dbo].[UserUserAccess]
ADD CONSTRAINT [FK_UserUserAccess_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [UserAccesses_AccessType] in table 'UserUserAccess'
ALTER TABLE [dbo].[UserUserAccess]
ADD CONSTRAINT [FK_UserUserAccess_UserAccess]
    FOREIGN KEY ([UserAccesses_AccessType])
    REFERENCES [dbo].[UserAccessSet]
        ([AccessType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserAccess_UserAccess'
CREATE INDEX [IX_FK_UserUserAccess_UserAccess]
ON [dbo].[UserUserAccess]
    ([UserAccesses_AccessType]);
GO

-- Creating foreign key on [LastUserAccess_Id] in table 'DocumentSet'
ALTER TABLE [dbo].[DocumentSet]
ADD CONSTRAINT [FK_DocumentsUser]
    FOREIGN KEY ([LastUserAccess_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentsUser'
CREATE INDEX [IX_FK_DocumentsUser]
ON [dbo].[DocumentSet]
    ([LastUserAccess_Id]);
GO

-- Creating foreign key on [LastUserAccessWithChanges_Id] in table 'DocumentSet'
ALTER TABLE [dbo].[DocumentSet]
ADD CONSTRAINT [FK_DocumentsUser1]
    FOREIGN KEY ([LastUserAccessWithChanges_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentsUser1'
CREATE INDEX [IX_FK_DocumentsUser1]
ON [dbo].[DocumentSet]
    ([LastUserAccessWithChanges_Id]);
GO

-- Creating foreign key on [PC_Id] in table 'DocumentPathSet'
ALTER TABLE [dbo].[DocumentPathSet]
ADD CONSTRAINT [FK_DocumentPathPC]
    FOREIGN KEY ([PC_Id])
    REFERENCES [dbo].[PCSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentPathPC'
CREATE INDEX [IX_FK_DocumentPathPC]
ON [dbo].[DocumentPathSet]
    ([PC_Id]);
GO

-- Creating foreign key on [Document_Id] in table 'DocumentPathSet'
ALTER TABLE [dbo].[DocumentPathSet]
ADD CONSTRAINT [FK_DocumentDocumentPath]
    FOREIGN KEY ([Document_Id])
    REFERENCES [dbo].[DocumentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentDocumentPath'
CREATE INDEX [IX_FK_DocumentDocumentPath]
ON [dbo].[DocumentPathSet]
    ([Document_Id]);
GO

-- Creating foreign key on [Document_Id] in table 'AccessLogSet'
ALTER TABLE [dbo].[AccessLogSet]
ADD CONSTRAINT [FK_DocumentAccessLog]
    FOREIGN KEY ([Document_Id])
    REFERENCES [dbo].[DocumentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentAccessLog'
CREATE INDEX [IX_FK_DocumentAccessLog]
ON [dbo].[AccessLogSet]
    ([Document_Id]);
GO

-- Creating foreign key on [User_Id] in table 'AccessLogSet'
ALTER TABLE [dbo].[AccessLogSet]
ADD CONSTRAINT [FK_UserAccessLog]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccessLog'
CREATE INDEX [IX_FK_UserAccessLog]
ON [dbo].[AccessLogSet]
    ([User_Id]);
GO

-- Creating foreign key on [PC_Id] in table 'AccessLogSet'
ALTER TABLE [dbo].[AccessLogSet]
ADD CONSTRAINT [FK_AccessLogPC]
    FOREIGN KEY ([PC_Id])
    REFERENCES [dbo].[PCSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessLogPC'
CREATE INDEX [IX_FK_AccessLogPC]
ON [dbo].[AccessLogSet]
    ([PC_Id]);
GO

-- Creating foreign key on [DocumentPath_Id] in table 'AccessLogSet'
ALTER TABLE [dbo].[AccessLogSet]
ADD CONSTRAINT [FK_DocumentPathAccessLog]
    FOREIGN KEY ([DocumentPath_Id])
    REFERENCES [dbo].[DocumentPathSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentPathAccessLog'
CREATE INDEX [IX_FK_DocumentPathAccessLog]
ON [dbo].[AccessLogSet]
    ([DocumentPath_Id]);
GO

-- Creating foreign key on [UserAccess_AccessType] in table 'AccessLogSet'
ALTER TABLE [dbo].[AccessLogSet]
ADD CONSTRAINT [FK_UserAccessAccessLog]
    FOREIGN KEY ([UserAccess_AccessType])
    REFERENCES [dbo].[UserAccessSet]
        ([AccessType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccessAccessLog'
CREATE INDEX [IX_FK_UserAccessAccessLog]
ON [dbo].[AccessLogSet]
    ([UserAccess_AccessType]);
GO

-- Creating foreign key on [Hardware_Id] in table 'AccessLogSet'
ALTER TABLE [dbo].[AccessLogSet]
ADD CONSTRAINT [FK_HardwareAccessLog]
    FOREIGN KEY ([Hardware_Id])
    REFERENCES [dbo].[HardwareSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HardwareAccessLog'
CREATE INDEX [IX_FK_HardwareAccessLog]
ON [dbo].[AccessLogSet]
    ([Hardware_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------