FROM microsoft/dotnet:1.1.2-sdk

WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet publish -c Release -o out

WORKDIR ./SmsAndroidVerification

ENV ASPNETCORE_URLS http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "run"]
