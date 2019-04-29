
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/17/2014 06:43:33
-- Generated from EDMX file: C:\TFS\psod-msgq-2\Source\M5\working\Sixeyed.MessageQueue\Sixeyed.MessageQueue.Integration\Data\UserModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Sixeyed.MessageQueue];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmailAddress] nvarchar(max)  NOT NULL,
    [IsUnsubscribed] bit  NOT NULL,
    [UnsubscribedAt] datetime  NULL
);
GO

-- Creating table 'UserEvents'
CREATE TABLE [dbo].[UserEvents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EventCode] nvarchar(20)  NOT NULL,
    [RecordedAt] datetime  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserEvents'
ALTER TABLE [dbo].[UserEvents]
ADD CONSTRAINT [PK_UserEvents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'UserEvents'
ALTER TABLE [dbo].[UserEvents]
ADD CONSTRAINT [FK_UserUserEvent]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserEvent'
CREATE INDEX [IX_FK_UserUserEvent]
ON [dbo].[UserEvents]
    ([User_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------