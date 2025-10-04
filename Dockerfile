FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/HotelBooking.Web/HotelBooking.Web.csproj", "src/HotelBooking.Web/"]
COPY ["src/HotelBooking.Application/HotelBooking.Application.csproj", "src/HotelBooking.Application/"]
COPY ["src/HotelBooking.Domain/HotelBooking.Domain.csproj", "src/HotelBooking.Domain/"]
COPY ["src/HotelBooking.Infrastructure/HotelBooking.Infrastructure.csproj", "src/HotelBooking.Infrastructure/"]

RUN dotnet restore "src/HotelBooking.Web/HotelBooking.Web.csproj"

COPY . .

WORKDIR "/src/src/HotelBooking.Web"
RUN dotnet build "HotelBooking.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelBooking.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "HotelBooking.Web.dll"]