# FindIt.API

Backend de **FindIt**, una API REST para gestionar objetos perdidos y encontrados, usuarios y solicitudes.

## Tecnologías

- ASP.NET Core Web API con **.NET 10**
- **PostgreSQL** y **Entity Framework Core** (Npgsql)
- Autenticación mediante **JWT**
- Documentación interactiva de la API con **Scalar**

## Requisitos

Antes de empezar, instalar:

1. [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
2. [PostgreSQL](https://www.postgresql.org/download/) (el proyecto se trabajó con PostgreSQL 18)
3. [Git](https://git-scm.com/downloads)
4. Visual Studio, Visual Studio Code u otro editor (opcional)

Asegúrate de que el servicio de PostgreSQL esté iniciado y de conocer la contraseña de tu usuario de PostgreSQL.

## Cómo levantar el backend por primera vez

Todos los comandos se ejecutan desde una terminal en la **carpeta raíz del proyecto**, donde está `FindIt.API.csproj`.

### 1. Clonar el repositorio

~~~bash
git clone https://github.com/abrahanv60-bit/FindIt.API.git
cd FindIt.API
~~~

### 2. Restaurar los paquetes de .NET

~~~bash
dotnet restore
~~~

### 3. Preparar PostgreSQL

En pgAdmin 4 o psql, crea una base de datos local llamada **`findit_db`**.

Por ejemplo, desde el editor SQL de PostgreSQL (conectado a otra base de datos):

~~~sql
CREATE DATABASE findit_db;
~~~

Si la base ya existe, **no vuelvas a crearla**.

### 4. Configurar la conexión y JWT

Este proyecto **no guarda la cadena de conexión en `appsettings.json`**: usa **.NET User Secrets**, configurados por separado en la computadora de cada desarrollador.

Reemplaza `TU_CONTRASENA` por tu contraseña local de PostgreSQL y ejecuta:

~~~bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=findit_db;Username=postgres;Password=TU_CONTRASENA"
~~~

Si PostgreSQL utiliza otro puerto o usuario, cambia `Port` o `Username` según tu instalación.

Configura además las opciones para generar y validar tokens JWT:

~~~bash
dotnet user-secrets set "Jwt:Key" "REEMPLAZA_ESTO_POR_UNA_CLAVE_ALEATORIA_Y_SEGURA_DE_AL_MENOS_32_CARACTERES"
dotnet user-secrets set "Jwt:Issuer" "FindIt.API"
dotnet user-secrets set "Jwt:Audience" "FindIt.Client"
~~~

**Importante:** sustituye el texto de `Jwt:Key` por una clave secreta propia, larga y aleatoria. No compartas ni subas al repositorio tus contraseñas o claves JWT. El proyecto ya tiene un `UserSecretsId` en `FindIt.API.csproj`, por lo que no hace falta ejecutar `dotnet user-secrets init`.

### 5. Aplicar las migraciones de la base de datos

Si no tienes instalada la herramienta de Entity Framework Core:

~~~bash
dotnet tool install --global dotnet-ef --version 10.0.0
~~~

Si ya la tienes instalada, omite ese comando. Si tu versión es anterior a 10, actualízala:

~~~bash
dotnet tool update --global dotnet-ef --version 10.0.0
~~~

Luego ejecuta:

~~~bash
dotnet ef database update
~~~

Esto aplica las migraciones que están en la carpeta `Migrations/` y crea las tablas del proyecto (usuarios, objetos y solicitudes) en `findit_db`.

### 6. Ejecutar la API

~~~bash
dotnet run --launch-profile http
~~~

Con el perfil HTTP incluido en el proyecto, la API se inicia en:

- **API:** http://localhost:5247
- **Scalar (documentación):** http://localhost:5247/scalar
- **Especificación OpenAPI:** http://localhost:5247/openapi/v1.json

Scalar está disponible cuando la aplicación se ejecuta en el entorno `Development`, como se configura en `Properties/launchSettings.json`.

## Arrancar el proyecto los siguientes días

Una vez realizados los pasos anteriores, solo abre una terminal en `FindIt.API` y ejecuta:

~~~bash
dotnet run --launch-profile http
~~~

No necesitas volver a crear la base de datos ni configurar los User Secrets cada vez. Si descargas cambios que incluyen nuevas migraciones, vuelve a ejecutar `dotnet ef database update`.

## Principales rutas de la API

| Ruta | Descripción |
| --- | --- |
| `/api/auth/registro` | Registrar un usuario |
| `/api/auth/login` | Iniciar sesión y obtener un token JWT |
| `/api/auth/perfil` | Consultar el perfil del usuario autenticado |
| `/api/objetos` | Consultar y gestionar objetos perdidos/encontrados |
| `/api/usuarios` | Consultar usuarios (requiere autenticación) |
| `/api/solicitudes` | Crear, consultar y gestionar solicitudes (requiere autenticación) |

Algunas operaciones requieren enviar el encabezado `Authorization: Bearer TU_TOKEN`. Puedes probar los endpoints desde Scalar o desde una herramienta como Postman. Al iniciar con una base nueva, primero registra un usuario mediante `POST /api/auth/registro`; los usuarios de otras computadoras **no se copian automáticamente** desde GitHub.

## Problemas comunes

- **Error de conexión a PostgreSQL:** verifica que PostgreSQL esté iniciado y que `Host`, `Port`, `Database`, `Username` y `Password` sean correctos en tus User Secrets.
- **`Falta configurar Jwt:Key`:** ejecuta los comandos de configuración JWT del paso 4.
- **`dotnet ef` no se reconoce:** instala `dotnet-ef` siguiendo el paso 5 y abre una nueva terminal si fuera necesario.
- **La API funciona, pero faltan tablas:** ejecuta `dotnet ef database update`.
- **El puerto 5247 está ocupado:** cierra el proceso que lo esté usando o ajusta el puerto de lanzamiento. Si cambias el puerto, utiliza ese nuevo valor para entrar a Scalar.

> **Seguridad:** los User Secrets se usan para desarrollo local; no son un sistema de administración de secretos para producción. No subas claves reales, contraseñas ni cadenas de conexión a GitHub.
