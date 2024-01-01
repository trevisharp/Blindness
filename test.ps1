clear

dotnet nuget delete Blindness 1.0.0 -s ($env:USERPROFILE + "\.nuget\packages") --non-interactive

cd gen
dotnet build -c Release
cd ..

cp .\gen\bin\Release\net7.0\CodeGenerator.exe .\src\buildTransitive\tools\CodeGenerator.exe
cp .\gen\bin\Release\net7.0\CodeGenerator.dll .\src\buildTransitive\tools\CodeGenerator.dll
cp .\gen\bin\Release\net7.0\CodeGenerator.runtimeconfig.json .\src\buildTransitive\tools\CodeGenerator.runtimeconfig.json

cd src
dotnet pack -c Release -o output
cd ..

cd smp
cd sample1
dotnet remove package Blindess
dotnet add package -s ..\..\src\output Blindness
dotnet build
cd ..
cd ..

cd src
rm output -r
cd ..