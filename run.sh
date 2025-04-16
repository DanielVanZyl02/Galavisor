cd publish
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
export ASPNETCORE_HTTP_PORTS=80
chmod +X GalavisorApi
sudo -E nohup ./GalavisorApi > /dev/null 2>&1 & exit