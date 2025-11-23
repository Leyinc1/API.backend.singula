# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy all project files
COPY . .

# Restore dependencies
RUN dotnet restore API.backend.singula/API.backend.singula.csproj

# Publish the application
RUN dotnet publish API.backend.singula/API.backend.singula.csproj -c Release -o out

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose the port and run the application
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "API.backend.singula.dll"]