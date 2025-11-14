# 🏠 PROYECTO-INMOBILIARIA

## 📄 Descripción del Proyecto

Este sistema informatiza la gestión de alquileres y administración de propiedades para una agencia inmobiliaria. Permite gestionar propietarios, inquilinos, inmuebles, contratos de alquiler, pagos y usuarios, con funcionalidades avanzadas como informes, auditoría y roles de usuario diferenciados.


## 🕹️ Funcionalidades Principales

### **Gestión de Entidades**
- **Propietarios:** Alta, baja y modificación de propietarios con datos personales y contacto. Relación uno a varios con inmuebles.
- **Inmuebles:** Administración de propiedades (alta, baja, modificación), registro de características (dirección, uso, tipo, ambientes, precio, coordenadas), y suspensión temporal de oferta.
- **Inquilinos:** Alta, baja y modificación de inquilinos, con relación a contratos de alquiler.
- **Contratos:** Creación y gestión de contratos (fechas, monto, inquilino, inmueble), finalizaciones anticipadas con cálculo de multas, y renovación automática.
- **Pagos:** Registro y gestión de pagos asociados a contratos, edición limitada, y anulación mediante cambio de estado.
- **Auditoria** Registro de todos los usuarios que realizaron un contrato.
- **Usuarios** Alta, baja y modificación de usuarios con datos para generar una cuenta que te pertima ingresar al sistema.

### **Roles y Seguridad**
- **Administrador:** Acceso completo , gestión de usuarios y entidades, visualización de auditorías y usuarios.
- **Empleado:** Modificación de perfil personal, sin permisos para gestión de usuarios ni eliminación de entidades.

### **Informes y Consultas**
- Listado de inmuebles por disponibilidad, propietario o filtros.
- Consulta de contratos vigentes, próximos a finalizar, o asociados a inmuebles/inquilinos.
- Consulta y carga de pagos realizados.
- Búsqueda de inmuebles disponibles en rangos de fechas.
- Listado de Propietarios, Inquilinos y Usuarios
- Listado de los Usuarios que generaron un Contrato 

## 🚀 Tecnologías Utilizadas

### **Backend**
- **Framework:** .NET Core
- **Base de Datos:** MySQL (configurable)
- **ORM:** EntityFrameworkCore y extensiones para MySQL

### **Extensiones**
- C# para VS Code

### **Paquetes**
- **Autenticación:** JwtBearer para autenticación JWT
- **Funcionalidades adicionales:** MailKit para envío de correos electrónicos

## 🛠️ Herramientas Requeridas

- **IDE:** Visual Studio Code
- **Generador de Código:** aspnet-codegenerator
- **Depuración:** Postman para pruebas de API

## 💻 ¿Cómo ejecutar este proyecto en tu PC?

1. **Clona el repositorio y entra a la carpeta del proyecto:**
   
   git clone https://github.com/AngelBaroja/Proyecto-Inmobiliaria.git
   cd Proyecto-Inmobiliaria 

2. **Instala las dependencias:**  
   - dotnet restore 

3. **Configura la base de datos:**
   - Copia la base de datos guardada en la capeta "Bd & Diagrama"
   - Edita el archivo `appsettings.json` con tus datos de conexión a MySQL.
     
4. **Inicia el servidor:** 
  - dotnet run


## 👤 Usuarios y Contraseñas de Prueba

| Usuario           | Contraseña | Rol          |
|-------------------|------------|--------------|
| 	angel@mail.com  | 12345      | Administrador|
| 	pepe@mail.com   | 123        | Empleado     |

> Si ingresa con un usuarion con el Rol "Administrador" va a tener acceso completo al sistema.

## 📚 Descripción de las rutas principales

- **/ (Home):** Página principal del sistema, protegida para usuarios autenticados.
- **/login:** Formulario y lógica de inicio de sesión.
- **/propietarios:** Gestión y registro de propietarios.
- **/inmuebles:** Gestión, registro y consulta de inmuebles.
- **/inquilinos:** Gestión, registro y consulta de inquilinos.
- **/contratos:** Gestión, registro y consulta de contratos de alquiler.
- **/pagos:** Gestion, registro y consulta de pagos.
- **/auditoria:** Registro de los usuarios que realizaron los contratos.


## 📦 Dependencias principales del proyecto

- **MySql.Data**
- **Microsoft.AspNetCore.Authentication.JwtBearer**





