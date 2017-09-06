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

alter table WinNotes.Person alter column PersonIDEncrypted NVARCHAR(MAX) NULL
alter table WinNotes.Person alter column AvatarImage VARBINARY(MAX) NULL
alter table WinNotes.Person alter column AvatarMIMEType VARCHAR(MAX) NULL

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


-- OPERACIONES CON STORED PROCEDURES

create procedure sp_login
@email nvarchar(max),
@password nvarchar(max)
as
	begin

		if not exists ( select 1
						from WinNotes.Person
						where Email = @email
						and Password = @password )
		begin
			raiserror('Email y/o contraseña incorrecta',16,1)
			return
		end

		select PersonID, UserName, Email, AvatarImage, AvatarMIMEType, Active
		from WinNotes.Person
		where Email = @email
		and Password = @password

	end
go

drop procedure sp_login

exec sp_login
@email = 'barril.de.duendes@outlook.com', @password = 'Barril3duendes'

select * from WinNotes.Person
select * from WinNotes.Folder
select * from WinNotes.Note


create procedure sp_refreshLoginDate
@id_user integer
as
	begin

		update WinNotes.Person set LastLoginDate = GETDATE()
		where PersonID = @id_user

	end


create procedure sp_verifyUserName
@userName nvarchar(max),
@result bit out
as
	begin				

		if exists (	select 1
					from WinNotes.Person
					where UserName = @userName )
		begin

			set @result = 1
			return

		end

		set @result = 0
		return

	end


DECLARE @result bit
EXEC sp_verifyUserName 'Testing22', @result OUTPUT
SELECT @result




create procedure sp_createNewUser
@userName nvarchar(max),
@email nvarchar(max),
@password nvarchar(max)
as
	begin

		if not exists ( select 1
						from WinNotes.Person
						where UserName = @userName
						and Email = @email )
		begin

			insert into WinNotes.Person(UserName, Email, Password, RegistrationDate, LastLoginDate)
			values(@userName, @email, @password, GETDATE(), GETDATE())

			select top 1 PersonID
			from WinNotes.Person
			order by PersonID desc
			return

		end

		raiserror('Ya existe un usuario con esos datos',16,1)

	end



exec sp_createNewUser 'CosmeFulanito99', 'cosme.fulanito99@gmail.com', 'asdASD123'

select * from WinNotes.Person


create procedure sp_saveEncryptedUserID
@id integer,
@encryptedID nvarchar(max)
as
	begin

		update WinNotes.Person set PersonIDEncrypted = @encryptedID
		where PersonID = @id

	end



create procedure sp_changePersonalPhrase
@userID integer,
@phrase nvarchar(max),
@phraseColor varchar(max)
as
	begin

		if exists ( select 1
					from WinNotes.Person
					where PersonID = @userID )
		begin

			update WinNotes.Person set PersonalPhrase = @phrase, PhraseColor = @phraseColor
			where PersonID = @userID
			return

		end

		raiserror('El usuario proporcionado no existe',16,1)
		return

	end

declare @userID integer, @phrase nvarchar(max), @phraseColor varchar(max)
exec sp_changePersonalPhrase @userID = 1, @phrase = 'Hola soy un mensaje de prueba', @phraseColor = 'green'
select * from WinNotes.Person

create procedure sp_deprueba
as
begin
	raiserror('deberia mostrarlo en el cartel rojo',16,1)
end

exec sp_deprueba


create procedure sp_changePassword
@userID integer,
@currentPassword nvarchar(max),
@newPassword nvarchar(max)
as
	begin
		
		if not exists ( select 1
						from WinNotes.Person
						where PersonID = @userID
						and Password = @currentPassword )
		begin
			raiserror('El usuario no existe o la contraseña actual ingresada es incorrecta',16,1)
			return
		end

		update WinNotes.Person set Password = @newPassword
		where PersonID = @userID

	end


exec sp_changePassword 2, 'asdASD123', 'qweQWE123'

select * from WinNotes.Person