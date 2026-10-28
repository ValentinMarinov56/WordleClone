# Run the API and SQL Server with Docker Compose

The API's `appsettings.json` uses `Server=localhost` and Windows integrated authentication. That works only when SQL Server is reachable from the API process on the same host; it is not the right connection string inside a container. In Compose, the API connects to the database service by its Compose DNS name, `db`, and authenticates with the SQL Server `sa` password. The API container also serves the built React app, so the UI and API share `http://localhost:5101`.

## Start

1. Copy `.env.example` to `.env` and replace both values with strong private values. Do not commit `.env`.
2. Start the containers from the repository root with `docker compose up --build`.
3. Open `http://localhost:5101` for the frontend and API. SQL Server is exposed at `localhost,1433` for optional host tools.

Compose overrides `ConnectionStrings__DefaultConnection` for the API, so the existing local `appsettings.json` does not need to be changed. Keep using the local trusted-connection setting when running the API directly on Windows. To prepare the frontend, run `npm run build` in `WordleClone.Client`, then manually copy the generated `WordleClone.Client/dist` folder into `WordleClone.API` and name the copied folder `dist` (the final path should be `WordleClone.API/dist/index.html`). Do this before `dotnet run` or `docker compose up --build`. For frontend hot reload during development, run Vite separately with `npm run dev`; its `/api` requests are proxied to the API.

## Apply the EF migrations

Starting SQL Server does not automatically create the application schema. Before the first API use, apply the existing EF migrations. With the containers running, run this from the repository root in PowerShell, replacing the values with the same ones in `.env`:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost,1433;Database=WordleCloneDb;User Id=sa;Password=<your-SA-password>;TrustServerCertificate=True;Encrypt=False"
dotnet ef database update --project .\WordleClone.API\WordleClone.API.csproj
Remove-Item Env:\ConnectionStrings__DefaultConnection
```

This uses the API's existing migrations against the Docker SQL Server. `dotnet-ef` must be installed as a tool on the host. The database data is stored in the named `wordle-sql-data` volume and survives container restarts and `docker compose down`. **Do not run `docker compose down -v` unless you intend to delete that database volume and its data.**

The API image expects SQL Server to be available before database-backed requests are made. Compose starts the database container first, but the SQL Server process may take a little while to become ready; if the first request fails, retry once the database container is healthy.
