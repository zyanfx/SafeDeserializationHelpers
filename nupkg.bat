@echo off

msbuild SafeDeserializationHelpers.sln /t:Rebuild /m /fl /flp:LogFile=msbuild.log /p:Configuration=Release

nuget pack Zyan.SafeDeserializationHelpers.nuspec