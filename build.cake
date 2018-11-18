///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// EXTERNAL NUGET TOOLS
//////////////////////////////////////////////////////////////////////

#Tool "xunit.runner.console"

var artifactsDir = Directory("./artifacts");
var solutionPath = "./Markeli.Half.sln";
var framework = "netstandard2.0";

var isMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master",
    BuildSystem.TravisCI.Environment.Build.Branch);

Task("Clean")
    .Does(() => 
    {            
        DotNetCoreClean(solutionPath);        
        DirectoryPath[] cleanDirectories = new DirectoryPath[] {
            artifactsDir
        };
    
        CleanDirectories(cleanDirectories);
    
        foreach(var path in cleanDirectories) { EnsureDirectoryExists(path); }
    
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        var settings = new DotNetCoreBuildSettings
          {
              Configuration = configuration
          };
          
        DotNetCoreBuild(
            solutionPath,
            settings);
    });

Task("UnitTests")
    .IsDependentOn("Build")
    .Does(() =>
    {        
        Information("UnitTests task...");
        var projects = GetFiles("./tests/**/*csproj");
        foreach(var project in projects)
        {
            Information(project);
            
            DotNetCoreTest(
                project.FullPath,
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = false
                });
        }
    });
     
Task("Pack")
    .IsDependentOn("UnitTests")
    .Does(() =>
    {        
         Information("Packing to nupkg...");
         var settings = new DotNetCorePackSettings
          {
              Configuration = configuration,
              OutputDirectory = artifactsDir
          };
         
          DotNetCorePack(solutionPath, settings);
    });
    
Task("Publish")
.IsDependentOn("Pack")
.WithCriteria(() => BuildSystem.TravisCI.IsRunningOnTravisCI && isMasterBranch)
.Does(()=> 
{
   var settings = new DotNetCoreNuGetPushSettings
    {
        Source = "https://api.nuget.org/v3/",
        ApiKey = EnvironmentVariable("Nuget_API_KEY")
    };
   Information(isMasterBranch);
   DotNetCoreNuGetPush("./artifacts/*nupkg", settings);
    
});
 
Task("Default")
    .IsDependentOn("Publish");
    
RunTarget(target);
