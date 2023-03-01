// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using TestBestInfGraph;



GraphService _graphService = new GraphService();
//_graphService.GetBestInfluencerBFS();
//_graphService.GetBestInfluencerDFSAsync();
//_graphService.GetBestInfluencerDFS();
//_graphService.GetBestInfluencerDFS_List();

//_graphService.GetBestInfluencerBFS();


GraphServiceV2 _graphServiceV2 = new GraphServiceV2();
//_graphServiceV2.GetBestInfluencerDFS();
//_graphServiceV2.GetBestInfluencerBFS();

GraphServiceV3 _graphServiceV3 = new GraphServiceV3();
_graphServiceV3.GetBestInfluencerFWA();

var summary = BenchmarkRunner.Run<GraphServiceV3>();
Console.WriteLine(summary);
