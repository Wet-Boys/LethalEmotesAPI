var target = Argument("target", "Default");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Restore")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=Restore");
});

Task("Build")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=Build");
});

Task("PatchNetcode")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=PatchNetcode");
});

Task("BuildAndPatch")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=BuildAndPatch");
});

Task("Deploy")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=Deploy");
});

Task("DeployUnity")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=DeployUnity");
});

Task("BuildThunderstore")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=BuildThunderstore");
});

Task("Default")
.Does(() => {
   StartProcess("dotnet", "run --project ./build/Build.csproj --target=Default");
});


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);