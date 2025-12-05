FROM ghcr.io/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PaymentsService.csproj", "./"]
RUN dotnet restore "./PaymentsService.csproj"
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM ghcr.io/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PaymentsService.dll"]
