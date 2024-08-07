FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
EXPOSE 7700
WORKDIR /src
COPY Colir.Exceptions/*.csproj Colir.Exceptions/
COPY Colir.DAL/*.csproj Colir.DAL/
COPY Colir.BLL/*.csproj Colir.BLL/
COPY Colir.WebApi/*.csproj Colir.WebApi/
RUN dotnet restore Colir.WebApi/Colir.WebApi.csproj
COPY . .
RUN dotnet publish "Colir.WebApi/Colir.WebApi.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-amd64 AS final
WORKDIR /src
COPY --from=build /publish .

CMD dotnet tool install --global dotnet-ef --version 6.0.25
ENV PATH="${PATH}:/root/.dotnet/tools"
CMD dotnet ef database update

ENTRYPOINT [ "dotnet", "Colir.WebApi.dll", "--urls", "http://+:7700" ]