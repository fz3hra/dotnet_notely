# 1️⃣ Use .NET SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy source code and build
COPY . ./
RUN dotnet build -c Release
RUN dotnet publish -c Release -o /publish

# 2️⃣ Use a smaller runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy built app from build stage
COPY --from=build /publish .

# Expose application port
EXPOSE 8080

# Start the application
CMD ["dotnet", "dotnet_notely.dll", "--urls", "http://0.0.0.0:8080"]
