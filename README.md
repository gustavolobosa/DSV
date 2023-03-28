# DSV


#SQL QUERY

CREATE DATABASE BBDDVerificable
USE BBDDVerificable
GO

CREATE TABLE formularios (
   num_atencion INT PRIMARY KEY IDENTITY(1,1),
   cne VARCHAR(50) NULL,
   comuna VARCHAR(50) NULL,
   manzana VARCHAR(50) NULL,
   predio VARCHAR(50) NULL,
   fojas INT NULL,
   fecha_inscripcion DATE NULL,
   num_inscripcion INT NULL
);
GO

CREATE TABLE enajenantes (
   id INT PRIMARY KEY IDENTITY(1,1),
   num_atencion INT NULL,
   run_rut VARCHAR(50) NULL,
   porcentaje_derecho FLOAT NULL,
   no_acreditado FLOAT NULL,
   FOREIGN KEY (num_atencion) REFERENCES formularios(num_atencion)
);
GO

CREATE TABLE adquirentes (
   id INT PRIMARY KEY IDENTITY(1,1),
   num_atencion INT NULL,
   run_rut VARCHAR(50) NULL,
   porcentaje_derecho FLOAT NULL,
   no_acreditado FLOAT NULL,
   FOREIGN KEY (num_atencion) REFERENCES formularios(num_atencion)
);
GO
 
CREATE TABLE multipropietarios (
   id INT PRIMARY KEY IDENTITY(1,1),
   comuna VARCHAR(50) NOT NULL,
   manzana VARCHAR(50) NOT NULL,
   predio VARCHAR(50) NOT NULL,
   run_rut VARCHAR(50) NULL,
   porcentaje_derecho FLOAT NULL,
   fojas INT NULL,
   fecha_inscripcion DATE NULL,
   num_inscripcion INT NULL,
   vigencia_inicial DATE NULL,
   vigencia_final DATE NULL,

);
GO
