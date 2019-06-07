Create package:
- Make sure nuget.exe is present inside the NuGet directory of the WrapPanel library;
- Open Developer Command Prompt and navigate to the NuGet directory;
- Run command 'nuget pack WrapPanel.nuspec'.

Publish package:
- Open Developer Command Prompt and navigate to the NuGet directory;
- Run command 'nuget push WrapPanel.UWP.<VERSION>.nupkg <API_KEY>'