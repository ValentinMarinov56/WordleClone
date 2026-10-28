# Build the API. Copy the client's built dist folder to WordleClone.API/dist before building.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY WordleClone.API/WordleClone.API.csproj WordleClone.API/
RUN dotnet restore WordleClone.API/WordleClone.API.csproj
COPY WordleClone.API/ WordleClone.API/
RUN dotnet publish WordleClone.API/WordleClone.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Run the published API
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WordleClone.API.dll"]
