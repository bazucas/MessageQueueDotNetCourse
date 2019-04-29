
SET QUOTED_IDENTIFIER OFF;
GO
USE [Sixeyed.MessageQueue];
GO

INSERT INTO [dbo].[Users] (  
    [EmailAddress] ,
    [IsUnsubscribed])
VALUES ('elton@sixeyed.com', 0);
GO
