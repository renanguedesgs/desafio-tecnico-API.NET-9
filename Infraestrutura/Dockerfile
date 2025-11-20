FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY *.sln ./
COPY src/Api/*.csproj ./src/Api/

RUN dotnet restore ./src/Api/Api.csproj

COPY . .

RUN dotnet publish ./src/Api/Api.csproj -c Release -o /app/publish /p:UseAppHost=false
#=======
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet","Api.dll"]

