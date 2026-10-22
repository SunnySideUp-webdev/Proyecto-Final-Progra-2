
--creamos la base de datos autoManagerDMS--
create database AutoManagerDMS

use AutoManagerDMS

--creamos la tabla dbo.Repuesto--
create table dbo.Repuesto
(
	idrepuesto int IDENTITY(1,1) primary key,
	codigo varchar(15) not null unique,
	nombre varchar(50) not null,
	descripcion varchar(100) not null,
	stock int not null default 0,
	marcacarro varchar(50) not null,
	modelocarro varchar(50) not null,
	aniocarro int not null,

	--generamos la restriccion en donde el stock no puede quedar en negativo
	constraint chk_stock_negativo check(stock >= 0)
);

select * from dbo.Repuesto