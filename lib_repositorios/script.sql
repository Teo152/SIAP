CREATE DATABASE SIAP;
GO

USE SIAP

CREATE TABLE ROLES (
id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
tipo VARCHAR(100),
descripcion VARCHAR(100)


);


CREATE TABLE USUARIOS (
    id INT  NOT NULL PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    apellido VARCHAR(100) ,
    email VARCHAR(150) NULL, 
    contraseña VARCHAR(100),
    telefono INT,
    fecha_nacimiento DATETIME NULL, -- solo anfitrion
    rol INT REFERENCES ROLES (ID),
   
    edad INT NULL -- solo huesped

);




CREATE TABLE MENSAJES (
id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
texto VARCHAR(500),
remitente INT REFERENCES USUARIOS (id),
destinatario INT REFERENCES USUARIOS (id),
);

CREATE TABLE PROPIEDADES ( -- mefalta la variable de disponibilidad de falso o verdadaero
    id INT  NOT NULL PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    direccion VARCHAR(100) ,
    capacidad INT,
    tipo_de_propiedad VARCHAR(100),
    precio DECIMAL (10,2),
    estanciaminima VARCHAR(100), -- comfirmar si va aparte de reglas de negocio y de que tipo es 
    reglas_propiedad VARCHAR (500),
    usuario INT REFERENCES USUARIOS (id)


);

ALTER TABLE PROPIEDADES
ADD Imagen VARCHAR(255) NULL;



CREATE TABLE BUSQUEDAS (
id INT  NOT NULL PRIMARY KEY IDENTITY(1,1),
fecha_deseada DATETIME null,
fecha_fin DATETIME null,
Cantidad_huesped  int null,
ciudad VARCHAR(100) null,
precio_min DECIMAL (10,2) null,
precio_max DECIMAL (10,2) null,
TipoPropiedad NVARCHAR(50) null,
usuario INT REFERENCES USUARIOS (id)
);

CREATE TABLE ESTADOS(
id INT  NOT NULL PRIMARY KEY IDENTITY(1,1),
tipo VARCHAR (100)
);

CREATE TABLE RESERVAS(
id INT  NOT NULL PRIMARY KEY IDENTITY(1,1),
fecha_creacion DATETIME,
fecha_deseada DATETIME,
fecha_fin DATETIME,
cantidad_huesped INT,
costo_total DECIMAL (10,2),
usuario INT REFERENCES USUARIOS (id),
propiedad INT REFERENCES PROPIEDADES (id),
estado INT REFERENCES ESTADOS (id)
);

CREATE TABLE PAGOS(
id INT  NOT NULL PRIMARY KEY IDENTITY(1,1),
usuario INT REFERENCES USUARIOS (id),
codigo VARCHAR(100),
monto DECIMAL(10,2),
metodo VARCHAR(100),
reserva INT REFERENCES RESERVAS(id),
fecha_pago DATETIME
);



CREATE TABLE RESENAS(
id INT  NOT NULL PRIMARY KEY IDENTITY(1,1),
fecha_creacion DATETIME,
comentario VARCHAR(200),
clasificacion int,
propiedad INT REFERENCES PROPIEDADES (id),
reserva INT REFERENCES RESERVAS (id)

);
