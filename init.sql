IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'arsenal')
	BEGIN
		CREATE DATABASE arsenal;
	END;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'arsenal')
	BEGIN
		USE arsenal;
		IF NOT EXISTS (Select name from sys.tables where name = 'types')
                        create table types(
                        type_id smallint identity(10,1),
                        name varchar(50),
                        primary key(type_id));

		IF NOT EXISTS (Select name from sys.tables where name = 'weapons')
			create table weapons(
			weapon_id smallint identity(100,1),
			stock smallint,
  			ammo smallint,
  			name varchar(100),
  			type_id smallint,
  			created date,
  			primary key(weapon_id),
			foreign key(type_id) references types(type_id));
	END
GO


