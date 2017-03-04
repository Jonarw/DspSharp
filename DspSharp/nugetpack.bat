ren project.json pproject.json
nuget pack DspSharp.csproj -Prop Configuration=Release
ren pproject.json project.json
PAUSE