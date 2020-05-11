CREATE TABLE [dbo].[Test] (
    [Id]                      uniqueidentifier    NOT NULL,
    [Name]                    nvarchar(200)       NOT NULL,
    PRIMARY KEY (Id),
);

INSERT INTO [dbo].[Test] ([Id], [Name])
VALUES (NEWID(), 'Test Entry');