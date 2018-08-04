FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Miqo.Config/*.csproj ./Miqo.Config/
COPY Miqo.Config.CreateKeys/*.csproj ./Miqo.Config.CreateKeys/
COPY Miqo.Config.Tests/*.csproj ./Miqo.Config.Tests/
RUN dotnet restore

# copy and build everything else
COPY Miqo.Config/. ./Miqo.Config/
COPY Miqo.Config.CreateKeys/. ./Miqo.Config.CreateKeys/
COPY Miqo.Config.Tests/. ./Miqoo.Config.Tests/
RUN dotnet build

FROM build AS testrunner
WORKDIR /app/Miqo.Config.Tests
COPY Miqo.Config.Tests/. .
RUN dotnet test
ENTRYPOINT ["dotnet", "test","--logger:trx"]
