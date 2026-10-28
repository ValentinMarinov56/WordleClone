# WordleClone

A full-stack, Wordle-inspired word guessing game. Play six-guess rounds, track points and wins, check the leaderboard, and share feedback. The app uses a React and JavaScript client with an ASP.NET Core API and SQL Server database.

## Tech Stack

### Client
- **React 19** and **JavaScript (JSX)**
- **Vite** for development and production builds
- **Axios** for API requests

### API and data
- **.NET 10** and **ASP.NET Core Web API**
- **Entity Framework Core** with **SQL Server**
- **JWT bearer authentication**

### Infrastructure
- **Docker** and **Docker Compose**

## Features

- **Word guessing game**: Guess a five-letter word in up to six attempts, with color feedback for each letter.
- **Accounts**: Register, sign in, and manage your account.
- **Player stats**: Earn points for wins and keep a record of games won.
- **Leaderboard**: Compare player points and wins.
- **Word pool**: Signed-in users can add words to the game pool.
- **Reviews**: Browse and submit player reviews.

## Project Structure

```text
WordleClone/
├── WordleClone.API/           # ASP.NET Core API, EF Core models, and migrations
├── WordleClone.API.Tests/     # API tests
├── WordleClone.Client/        # React + JavaScript + Vite client
├── compose.yaml               # API and SQL Server containers
├── DOCKER.md                  # Docker and database setup details
├── WordleClone.slnx           # .NET solution
└── README.md
```

## Prerequisites

- **.NET 10 SDK**
- **Node.js** and npm
- **SQL Server** for local development, or **Docker Desktop** with Docker Compose for the containerized setup
- The `dotnet-ef` tool to apply database migrations from the command line

## Getting Started

### Option 1: Run with Docker Compose

1. Copy `.env.example` to `.env` and replace the example values with strong, private values. Never commit `.env`.
2. Build the client and stage its output where the API serves static files:

   ```powershell
   cd WordleClone.Client
   npm install
   npm run build
   Copy-Item -Recurse -Force .\dist ..\WordleClone.API\dist
   cd ..
   ```

3. Start the API and SQL Server:

   ```powershell
   docker compose up --build
   ```

4. Apply the database migrations before using the app for the first time. Follow the PowerShell instructions in [DOCKER.md](DOCKER.md#apply-the-ef-migrations).
5. Open [http://localhost:5101](http://localhost:5101).

Stop the containers with `docker compose down`. The SQL Server data is stored in a named volume and persists when containers stop. Avoid `docker compose down -v` unless you intend to delete that data. See [DOCKER.md](DOCKER.md) for additional details and troubleshooting notes.

### Option 2: Run Locally

#### API

1. Ensure SQL Server is running and update `ConnectionStrings:DefaultConnection` in `WordleClone.API/appsettings.json` if your local connection differs.
2. From the repository root, apply the migrations and start the API:

   ```powershell
   dotnet ef database update --project .\WordleClone.API\WordleClone.API.csproj
   dotnet run --project .\WordleClone.API\WordleClone.API.csproj
   ```

The API listens at `http://localhost:5101`. In Development, Swagger UI is available at [http://localhost:5101/swagger](http://localhost:5101/swagger).

#### Client

In another terminal:

```powershell
cd WordleClone.Client
npm install
npm run dev
```

Vite serves the client at [http://localhost:5173](http://localhost:5173) and proxies `/api` requests to the local API.

## Useful Commands

| Area | Command | Purpose |
| --- | --- | --- |
| Client | `npm run dev` | Start the Vite development server |
| Client | `npm run build` | Type-check and create a production build in `dist/` |
| Client | `npm run lint` | Run Oxlint |
| API | `dotnet build WordleClone.slnx` | Build the solution |
| API | `dotnet test WordleClone.slnx` | Run the .NET tests |
| Database | `dotnet ef database update --project .\WordleClone.API\WordleClone.API.csproj` | Apply EF Core migrations |

## API Endpoints

The API uses `/api` routes. Main endpoints include:

| Area | Routes | Access |
| --- | --- | --- |
| Authentication | `POST /api/login/authenticate` | Public |
| Users | `GET /api/user`, `GET /api/user/{id}`, `POST /api/user` | Public |
| User stats and account | `PUT /api/user`, `DELETE /api/user/{id}` | Authenticated |
| Words | `GET /api/wordl`, `GET /api/wordl/{id}` | Public |
| Word pool management | `POST /api/wordl`, `DELETE /api/wordl/{id}` | Authenticated |
| Reviews | `GET /api/review`, `GET /api/review/{id}` | Public |
| Review management | `POST /api/review`, `DELETE /api/review/{id}` | Authenticated; deletion is limited to the review's author |

For request examples, see [WordleClone.API/WordleClone.API.http](WordleClone.API/WordleClone.API.http).

## Configuration

- Local API database connection: `WordleClone.API/appsettings.json` (`ConnectionStrings:DefaultConnection`).
- Docker Compose secrets: `.env`, based on `.env.example` (`SA_PASSWORD` and `JWT_SECRET`).
- The API also reads the JWT signing secret from the `SECRET` environment variable. Compose maps `JWT_SECRET` to this setting.

Use strong, unique secrets for any shared or deployed environment. Do not use the development fallback secret in production.

## License

This project is available under the MIT License. See [LICENSE](LICENSE) for details.
