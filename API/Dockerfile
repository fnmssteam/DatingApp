FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
EXPOSE 8080

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./

# create configuration named Release and output to "out"
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# copy everything that's contained within "build-env" defined above
# then copy what's inside /app/out the root of the container
COPY --from=build-env /app/out .

ENTRYPOINT [ "dotnet", "API.dll" ]