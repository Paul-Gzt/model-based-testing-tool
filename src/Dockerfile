FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build-env
WORKDIR /POC

COPY ProofOfConcept.Cli ./ProofOfConcept.Cli
COPY ProofOfConcept.Core ./ProofOfConcept.Core
COPY ProofOfConcept.Infrastructure ./ProofOfConcept.Infrastructure
COPY ProofOfConcept.sln .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine
WORKDIR /POC

COPY --from=build-env /POC/out .
COPY /Models ./Models
ENTRYPOINT ["dotnet", "ProofOfConcept.Cli.dll", "./Models/Test.model", "--modeling-mode"]
