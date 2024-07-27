# Use the official ASP.NET Core runtime as a parent image

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set the timezone
RUN ln -sf /usr/share/zoneinfo/Europe/Warsaw /etc/localtime
RUN echo "Europe/Warsaw" > /etc/timezone

# Set the locale for Polish signs
RUN apt-get update && apt-get install -y locales
RUN sed -i -e 's/# pl_PL.UTF-8 UTF-8/pl_PL.UTF-8 UTF-8/' /etc/locale.gen && locale-gen
ENV LANG pl_PL.UTF-8
ENV LANGUAGE pl_PL:pl
ENV LC_ALL pl_PL.UTF-8

ENV DOTNET_RUNNING_IN_CONTAINER=true

# Copy the certificates
COPY acl_openstack_authorization/certificates/ /app/certificates/

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["acl_openstack_authorization/acl_openstack_authorization.csproj", "acl_openstack_authorization/"]
RUN dotnet restore "./acl_openstack_authorization/acl_openstack_authorization.csproj"
COPY . .
WORKDIR "/src/acl_openstack_authorization"
RUN dotnet build "acl_openstack_authorization.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "acl_openstack_authorization.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "acl_openstack_authorization.dll"]
