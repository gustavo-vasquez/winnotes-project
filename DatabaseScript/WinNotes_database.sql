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

-- Muestra un listado de todas las tablas de la base con las propiedades [datetype][max_length][precision][scale][is_nullable][is_ansi_padded]
SELECT OBJECT_SCHEMA_NAME(T.[object_id],DB_ID()) AS [Schema],   
        T.[name] AS [table_name], AC.[name] AS [column_name],   
        TY.[name] AS system_data_type, AC.[max_length],  
        AC.[precision], AC.[scale], AC.[is_nullable], AC.[is_ansi_padded]  
FROM sys.[tables] AS T   
  INNER JOIN sys.[all_columns] AC ON T.[object_id] = AC.[object_id]  
 INNER JOIN sys.[types] TY ON AC.[system_type_id] = TY.[system_type_id] AND AC.[user_type_id] = TY.[user_type_id]   
WHERE T.[is_ms_shipped] = 0  
ORDER BY T.[name], AC.[column_id]

-------------------------------------------------------------------------------------------------------

alter table WinNotes.Person alter column PersonIDEncrypted NVARCHAR(MAX)
alter table WinNotes.Person alter column AvatarImage VARBINARY(MAX)
alter table WinNotes.Person alter column AvatarMIMEType VARCHAR(MAX)

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