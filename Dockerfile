# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the Razor Class Library to match the Linux project reference path
COPY --from=razorlib / ../bgbahasajerman_RazorClassLibrary/

# Copy the main project files
COPY bgbahasajerman_FullBlazorWebApp/bgbahasajerman_FullBlazorWebApp.csproj bgbahasajerman_FullBlazorWebApp/
COPY bgbahasajerman_FullBlazorWebApp.Client/bgbahasajerman_FullBlazorWebApp.Client.csproj bgbahasajerman_FullBlazorWebApp.Client/

# Copy the rest of the source
COPY . .

# Build and publish (OS detection will automatically use Linux path)
RUN dotnet publish bgbahasajerman_FullBlazorWebApp/bgbahasajerman_FullBlazorWebApp.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:80
EXPOSE 80
ENTRYPOINT ["dotnet", "bgbahasajerman_FullBlazorWebApp.dll"]
