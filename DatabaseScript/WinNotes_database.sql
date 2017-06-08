CREATE SCHEMA WinNotes
GO

CREATE TABLE WinNotes.Person (
	PersonID INTEGER IDENTITY(1,1),
	UserName NVARCHAR(MAX) NOT NULL,
	PersonIDEncrypted NVARCHAR(MAX) NOT NULL,
	Email NVARCHAR(MAX) NOT NULL,
	Password NVARCHAR(MAX) NOT NULL,
	RegistrationDate DATETIME NOT NULL,
	Active BIT DEFAULT 0,
	LastLoginDate DATETIME NOT NULL,
	PersonalPhrase NVARCHAR(MAX),
	PhraseColor VARCHAR(MAX),
	AvatarImage VARBINARY(MAX) NOT NULL,
	AvatarMIMEType VARCHAR(MAX) NOT NULL,
	CONSTRAINT PK_Person PRIMARY KEY CLUSTERED (PersonID)
)
GO

ALTER TABLE WinNotes.Person
ADD AvatarImage VARBINARY(MAX)

ALTER TABLE WinNotes.Person
DROP COLUMN EmailEncrypted
GO

ALTER TABLE WinNotes.Person
ADD AvatarMIMEType VARCHAR(MAX)

ALTER TABLE WinNotes.Person
ADD PhraseColor VARCHAR(MAX)

ALTER TABLE WinNotes.Person
ADD PersonIDEncrypted NVARCHAR(MAX)

select * from WinNotes.Person

/* Cuando el check da falso tira un error masomenos asi: The INSERT statement conflicted with the CHECK constraint "CHK_T_Column__2MB". The conflict occurred in database "TestDB", table "dbo.T", column 'VarB'. */
ALTER TABLE WinNotes.Person ADD CONSTRAINT CHK_PERSON_IMAGE_2MB CHECK (DATALENGTH(AvatarImage) <= 2097152)
GO

SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'WinNotes'
  AND TABLE_NAME = 'Person'
  AND COLUMN_NAME = 'EmailConfirmed'

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