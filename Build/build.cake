/////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var projectDirectory = Directory("../NewRelic.Service.CustomEvents");
var testDirectory = Directory("../NewRelic.Service.CustomEvents.Tests");
var solution = "../NewRelic.Service.CustomEvents.sln";

//////////////////////////////////////////////////////////////////////
// SETTINGS
//////////////////////////////////////////////////////////////////////

var projectSettings = new DotNetCoreCleanSettings
{
    Framework = "netstandard2.0",
    Configuration = configuration
};

var testSettings = new DotNetCoreCleanSettings
{
    Framework = "netcoreapp2.0",
    Configuration = configuration
};

var packSettings = new DotNetCorePackSettings
{
    Configuration = configuration,
    OutputDirectory = "../Publish"
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleaning the build directory.")
    .Does(() =>
    {
        DotNetCoreClean(projectDirectory, projectSettings);
        DotNetCoreClean(testDirectory, testSettings);
    });

Task("Restore")
    .Description("Restoring NuGet packages.")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Build")
    .Description("Building the solution.")
    .Does(() =>
    {
        DotNetCoreBuild(solution);
    });

Task("Test")
    .Description("Runs Unit tests.")
    .Does(() =>
    {
        DotNetCoreTest(testDirectory);
    });

Task("Pack")
    .Description("Packs Nuget package.")
    .Does(() =>
    {
        DotNetCorePack(projectDirectory, packSettings);
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack")
    .Finally (() =>
    {
        Information("Complete");
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);