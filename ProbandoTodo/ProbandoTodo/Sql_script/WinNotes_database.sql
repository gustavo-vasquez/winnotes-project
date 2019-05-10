CREATE DATABASE WinNotes
GO

USE WinNotes
GO

CREATE SCHEMA WinNotes
GO

CREATE TABLE WinNotes.Person (
	PersonID INTEGER IDENTITY(1,1),
	UserName NVARCHAR(MAX) NOT NULL,
	PersonIDEncrypted NVARCHAR(MAX),
	Email NVARCHAR(MAX) NOT NULL,
	Password NVARCHAR(MAX) NOT NULL,
	RegistrationDate DATETIME NOT NULL,
	Active BIT DEFAULT 0,
	LastLoginDate DATETIME NOT NULL,
	PersonalPhrase NVARCHAR(MAX),
	PhraseColor VARCHAR(MAX),
	AvatarImage VARBINARY(MAX),
	AvatarMIMEType VARCHAR(MAX),
	CONSTRAINT PK_Person PRIMARY KEY CLUSTERED (PersonID)
)
GO

-------------------------------------------------------------------------------------------------------

ALTER TABLE WinNotes.Person ADD CONSTRAINT CHK_PERSON_IMAGE_2MB CHECK (DATALENGTH(AvatarImage) <= 2097152)
GO

-------------------------------------------------------------------------------------------------------

CREATE TABLE WinNotes.Folder (
	FolderID INTEGER IDENTITY(1,1),
	Name NVARCHAR(MAX) NOT NULL,
	Details NVARCHAR(MAX) NOT NULL,
	LastModified DATETIME NOT NULL,
	Person_ID INTEGER NOT NULL,
	CONSTRAINT PK_Folder PRIMARY KEY CLUSTERED (FolderID),
	CONSTRAINT FK_PersonFolder FOREIGN KEY (Person_ID) REFERENCES WinNotes.Person (PersonID)
)
GO

CREATE TABLE WinNotes.Note (
	NoteID INTEGER IDENTITY(1,1),
	Title NVARCHAR(MAX) NOT NULL,
	Details NVARCHAR(MAX) NOT NULL,
	ExpirationDate DATETIME NOT NULL,
	Starred BIT DEFAULT 0,
	Completed BIT DEFAULT 0,
	Folder_ID INTEGER NOT NULL,
	Person_ID INTEGER NOT NULL,
	CONSTRAINT PK_Note PRIMARY KEY CLUSTERED (NoteID),
	CONSTRAINT FK_FolderNote FOREIGN KEY (Folder_ID) REFERENCES WinNotes.Folder (FolderID),
	CONSTRAINT FK_PersonNote FOREIGN KEY (Person_ID) REFERENCES WinNotes.Person (PersonID)
)
GO

select * from WinNotes.Person
select * from WinNotes.Folder
select * from WinNotes.Note