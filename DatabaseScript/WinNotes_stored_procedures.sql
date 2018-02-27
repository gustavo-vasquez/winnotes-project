-- OPERACIONES CON STORED PROCEDURES


-- USERCONTROLLER

-- Login
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


-- Actualizar la fecha de ultimo login
create procedure sp_refreshLoginDate
@id_user integer
as
	begin

		update WinNotes.Person set LastLoginDate = GETDATE()
		where PersonID = @id_user

	end


-- Comprobar si ya existe el nombre de usuario
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




-- Crear nuevo usuario
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



-- Guardar el id de usuario encriptado
create procedure sp_saveEncryptedUserID
@id integer,
@encryptedID nvarchar(max)
as
	begin

		update WinNotes.Person set PersonIDEncrypted = @encryptedID
		where PersonID = @id

	end




-- Cambiar la frase personal
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



-- Cambiar contraseña del usuario
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



-- Obtener la información de perfil del usuario
create procedure sp_getUserInformation
@userID integer
as
	begin

		if not exists ( select 1
						from WinNotes.Person
						where PersonID = @userID )
		begin
			raiserror('El usuario especificado no existe',16,1)
			return
		end

		select UserName, Email, RegistrationDate, PersonalPhrase, PhraseColor, AvatarImage, AvatarMIMEType
		from WinNotes.Person
		where PersonID = @userID
	end


exec sp_getUserInformation 3

select * from WinNotes.Person



-- Cambiar la imágen de perfil del usuario
create procedure sp_changeAvatar
@userID integer,
@avatarImage varbinary(max),
@mimeType varchar(max)
as
	begin
		
		if not exists ( select 1
						from WinNotes.Person
						where PersonID = @userID )
		begin
			raiserror('El usuario especificado no existe',16,1)
			return
		end

		update WinNotes.Person set AvatarImage = @avatarImage, AvatarMIMEType = @mimeType
		where PersonID = @userID
	end


--------------------------------------------------------------------

-- FOLDERCONTROLLER


-- Obtener las carpetas creadas por el usuario
create procedure sp_getUserFolders
@userID integer
as
	begin

		if not exists ( select 1
						from WinNotes.Person
						where PersonID = @userID )
		begin
			raiserror('El usuario especificado no existe',16,1)
			return
		end		

		select FolderID, Name, Details, LastModified
		from WinNotes.Folder
		where Person_ID = @userID

	end
go


exec sp_getUserFolders 1
select * from WinNotes.Person
select * from WinNotes.Folder



-- Crear nueva carpeta
create procedure sp_createNewFolder
@userID integer,
@name nvarchar(max),
@details nvarchar(max)
as
	begin

		if not exists ( select 1
						from WinNotes.Person
						where PersonID = @userID )
		begin
			raiserror('El usuario especificado no existe',16,1)
			return
		end
		
		if exists ( select 1
					from WinNotes.Folder
					where Name = @name )
		begin
			raiserror('Ya existe una carpeta con ese nombre',16,1)
			return
		end
		
		insert into WinNotes.Folder(Name, Details, LastModified, Person_ID)
		values (@name, @details, GETDATE(), @userID)
	end


exec sp_createNewFolder 3, 'Carpeta de prueba', 'Creada directamente desde sql'

select * from WinNotes.Person
select * from WinNotes.Folder



-- Editar carpeta
create procedure sp_editFolder
@userID integer,
@folderID integer,
@name nvarchar(max),
@details nvarchar(max)
as
	begin

		if not exists ( select 1
						from WinNotes.Folder
						where FolderID = @folderID
						or Person_ID = @userID )
		begin
			raiserror('El usuario o la carpeta no existe',16,1)
			return
		end

		update WinNotes.Folder set Name = @name, Details = @details, LastModified = GETDATE()
		where FolderID = @folderID
		and Person_ID = @userID
	end


exec sp_editFolder 3, 1, 'Probando editar', 'Nuevamente desde sql'

select * from WinNotes.Folder



-- Eliminar carpeta
create procedure sp_removeFolder
@userID integer,
@folderID integer
as
	begin

		if not exists ( select 1
						from WinNotes.Folder
						where FolderID = @folderID
						or Person_ID = @userID )
		begin
			raiserror('El usuario o la carpeta no existe',16,1)
		end

		if exists ( select 1
					from WinNotes.Note
					where Folder_ID = @folderID
					and Person_ID = @userID )
		begin
			delete from WinNotes.Note
			where Folder_ID = @folderID
			and Person_ID = @userID
		end

		delete from WinNotes.Folder
		where FolderID = @folderID
		and Person_ID = @userID

	end
go


exec sp_removeFolder 

select * from WinNotes.Person
select * from WinNotes.Folder
select * from WinNotes.Note




create procedure sp_changeNoteLocation
@userID integer,
@noteID integer,
@selectedFolder nvarchar(max)
as
	begin

		if not exists ( select 1
						from WinNotes.Folder						
						where Person_ID = @userID )
		begin
			raiserror('El usuario no existe',16,1)
		end

		if not exists ( select 1
						from WinNotes.Folder
						where Name = @selectedFolder )
		begin
			raiserror('La carpeta no existe',16,1)
		end

		if not exists ( select 1
						from WinNotes.Note
						where NoteID = @noteID )
		begin
			raiserror('La nota no existe',16,1)
		end


		declare @folderID integer
		set @folderID = ( select FolderID
						  from WinNotes.Folder
						  where Name = @selectedFolder )

		update WinNotes.Note set Folder_ID = @folderID
		where Person_ID = @userID
		and NoteID = @noteID

	end
go



--------------------------------------------------------------------