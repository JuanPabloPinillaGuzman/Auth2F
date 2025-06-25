# Auth2F

Aplicación .NET Core que implementa Autenticación de Dos Factores (2FA) siguiendo una arquitectura limpia.

## Estructura del Proyecto

La solución está organizada en varios proyectos siguiendo los principios de arquitectura limpia:

- **ApiAuth2F**: Capa de API que contiene los controladores y configuraciones específicas de la API.
- **Application**: Contiene la lógica de la aplicación, DTOs e interfaces de servicios.
- **Domain**: Alberga las entidades principales de negocio y la lógica de dominio.
- **Infrastructure**: Maneja el acceso a datos, servicios externos y aspectos de infraestructura.
- **ConsoleAuth2FExample**: Aplicación de consola de ejemplo que demuestra el uso.


### Instalación

1. Clona el repositorio:
   ```bash
   git clone <url-del-repositorio>
   cd Auth2F
   ```

2. Restaura los paquetes NuGet:
   ```bash
   dotnet restore
   ```

3. Actualiza las cadenas de conexión en `ApiAuth2F/appsettings.json` para que apunten a tu base de datos.

4. Ejecuta las migraciones de la base de datos (si aplica):
   ```bash
   cd ApiAuth2F
   dotnet ef database update
   ```

5. Ejecuta la aplicación:
   ```bash
   dotnet run --project ApiAuth2F
   ```

## Endpoints de la API

### Autenticación

- `POST /api/auth/register`: Registrar un nuevo usuario
- `POST /api/auth/login`: Autenticarse y obtener token JWT
- `POST /api/auth/enable-2fa`: Habilitar autenticación de dos factores
- `POST /api/auth/verify-2fa`: Verificar código de autenticación de dos factores

## Desarrollo

### Compilar la Solución

```bash
dotnet build
```

### Ejecutar Pruebas

```bash
dotnet test
```

### Estilo de Código

Este proyecto sigue las [convenciones de codificación de .NET](https://docs.microsoft.com/es-es/dotnet/csharp/fundamentals/coding-style/coding-conventions).

## Cómo Contribuir

1. Haz un fork del repositorio
2. Crea una rama para tu funcionalidad (`git checkout -b feature/MiNuevaFuncionalidad`)
3. Guarda tus cambios (`git commit -m 'Añadir alguna funcionalidad'`)
4. Sube los cambios a tu rama (`git push origin feature/MiNuevaFuncionalidad`)
5. Abre una Pull Request



## Reconocimientos

- Principios de Arquitectura Limpia
- .NET Core
- Entity Framework Core
