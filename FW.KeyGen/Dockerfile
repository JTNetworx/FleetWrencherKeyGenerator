# Use the official .NET image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application and build it
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Set the entry point to the application
ENTRYPOINT ["./FleetWrencherKeyGen"]

# Optional: Set default command if no arguments are passed
CMD ["--help"]