# The `FROM` instruction specifies the base image. You are
# extending the `mcr.microsoft.com/dotnet/aspnet` image.

FROM mcr.microsoft.com/dotnet/aspnet

# Next, this Dockerfile creates a directory for your application
WORKDIR /app
COPY src/ContainerDemoApp/bin/Release/PublishOutput .

# The final instruction copies the site you published earlier into the container.
ENTRYPOINT ["dotnet", "ContainerDemoApp.dll"]
