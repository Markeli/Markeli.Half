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
    .WithCriteria(() => BuildSystem.TravisCI.IsRunningOnTravisCI && isMasterBranch)
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
     
Task("Default")
    .IsDependentOn("Pack");
    
RunTarget(target);
