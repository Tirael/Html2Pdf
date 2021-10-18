FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

#####################
#PUPPETEER RECIPE
#####################
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install -y google-chrome-unstable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst  \
      --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*
#####################
#END PUPPETEER RECIPE
#####################

WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS http://+
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG Configuration=Release
WORKDIR /src
COPY *.sln ./
COPY . .
RUN dotnet restore
WORKDIR /src
RUN dotnet build -c $Configuration -o /app/build

FROM build AS publish
ARG Configuration=Release
RUN dotnet publish -c $Configuration --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-unstable"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Html2PdfProcessor.dll"]
