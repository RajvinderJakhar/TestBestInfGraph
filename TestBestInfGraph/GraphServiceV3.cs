using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBestInfGraph
{
    public class GraphServiceV3
    {
        private static float[,] _hUImps { get; set; }
        /// <summary>
        /// Adjacency Matrix
        /// </summary>
        private static float[,] _adjacencyMatrix { get; set; }
        /// <summary>
        /// Distance Matrix
        /// </summary>
        //private static int[,] _distanceMatrix { get; set; }
        private static int[] _output { get; set; }
        //private static int[] _visitedImps { get; set; }
        private static float[,] _inf_valsArr { get; set; }
        private static int[,] _edgesArr { get; set; }

        private static int _lc = 0;

        #region BFS
        /// <summary>
        /// Floyd Warshall Algorithm for a Graph
        /// </summary>
        [Benchmark]
        public void GetBestInfluencerFWA()
        {
            //var _sa = float.PositiveInfinity;

            //PriorityQueue
            Console.WriteLine("\tFloyd Warshall Algorithm for a Graph");
            Console.WriteLine();

            long st = DateTime.Now.Ticks;
            _lc = 0;
            int _verticesN, _edgesN, _hoursN;

            ReadInput(@"input4.txt", out _verticesN, out _edgesN, out _hoursN);

            _adjacencyMatrix = new float[_verticesN, _verticesN];

            List<int> _res = new List<int>();
            _hUImps = new float[_hoursN, _verticesN];
            _output = new int[_hoursN];


            CreateAdjMatch(_verticesN, _edgesN);
            floydWarshall(_verticesN);
            Console.WriteLine($"\tDepth loop called in floydWarshall algo: {_lc}");
            Console.WriteLine();
            _lc = 0;
            UpdateUserImpactVal(_hoursN, _verticesN);

            long elapsedTicks = DateTime.Now.Ticks - st;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            Console.WriteLine("\tExecuting time sec: " + elapsedSpan.TotalSeconds);

            Console.WriteLine();
            //Console.WriteLine("\t" + String.Join("\t", _output));
            for (int i = 0; i < _hoursN; i++)
            {
                if (i == 0 || (i % 10) == 0)
                    Console.WriteLine();
                Console.Write("\t" + _output[i]);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"\tDepth loop called: {_lc}");
            Console.WriteLine();

        }

        #endregion
        #region Methods
        private void ReadInput(string inputFile, out int _verticesN, out int _edgesN, out int _hoursN)
        {
            _verticesN = 0;
            _edgesN = 0;
            _hoursN = 0;

            try
            {
                var _lines = File.ReadLines(inputFile).ToList();

                //Console.WriteLine("\t" + String.Join(Environment.NewLine + "\t", _lines));
                Console.WriteLine();

                _lines = _lines.Where(x => !String.IsNullOrEmpty(x)).Select(x => x.Trim()).ToList();

                int.TryParse(_lines[0].Split(" ")[0], out _verticesN);
                int.TryParse(_lines[0].Split(" ")[1], out _edgesN);
                int.TryParse(_lines[0].Split(" ")[2], out _hoursN);

                _edgesArr = new int[_edgesN, 3];
                for (int i = 0; i < _edgesN; i++)
                {
                    var _linkItem = _lines[i + 1].Split(" ").ToArray();
                    if (_linkItem != null)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int.TryParse(_linkItem[j], out int _l);
                            _edgesArr[i, j] = _l;
                        }
                    }
                }


                _inf_valsArr = new float[_hoursN, _verticesN];
                for (int i = 0; i < _hoursN; i++)
                {
                    var _hourItem = _lines[i + _edgesN + 1].Split(" ").ToArray();
                    if (_hourItem != null)
                    {
                        for (int j = 0; j < _verticesN; j++)
                        {
                            int.TryParse(_hourItem[j], out int _l);
                            _inf_valsArr[i, j] = _l;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Read input error - " + ex.Message);
            }
        }

        private void CreateAdjMatch(int _verticesN, int _edgesN)
        {
            for (int i = 0; i < _verticesN; i++)
                for (int j = 0; j < _verticesN; j++)
                {
                    if (i != j)
                        _adjacencyMatrix[i, j] = float.PositiveInfinity;
                }

            for (int i = 0; i < _edgesN; i++)
            {
                var _u = _edgesArr[i, 0];
                var _v = _edgesArr[i, 1];
                _adjacencyMatrix[_u, _v] = _edgesArr[i, 2];
            }

            //for (int i = 0; i < Math.Sqrt(_adjMat.Length); i++)
            //{
            //    Console.WriteLine();
            //    for(int j = 0; j < Math.Sqrt(_adjMat.Length); j++)
            //    {
            //        Console.Write("\t" + _adjMat[i, j]);
            //    }
            //}
        }

        /// <summary>
        /// Floyd Warshall Algorithm
        /// </summary>
        /// <param name="_verticesN"></param>
        private void floydWarshall(int _verticesN)
        {
            //_distanceMatrix = _adjacencyMatrix;

            /* Add all vertices one by one to
        the set of intermediate vertices.
        ---> Before start of a iteration,
             we have shortest distances
             between all pairs of vertices
             such that the shortest distances
             consider only the vertices in
             set {0, 1, 2, .. k-1} as
             intermediate vertices.
        ---> After the end of a iteration,
             vertex no. k is added
             to the set of intermediate
             vertices and the set
             becomes {0, 1, 2, .. k} */
            for (int k = 0; k < _verticesN; k++)
            {
                // Pick all vertices as source
                // one by one
                for (int i = 0; i < _verticesN; i++)
                {
                    // Pick all vertices as destination
                    // for the above picked source
                    for (int j = 0; j < _verticesN; j++)
                    {
                        _lc++;
                        // If vertex k is on the shortest
                        // path from i to j, then update
                        // the value of dist[i][j]

                        //_distanceMatrix[i, j] = Math.Min(_distanceMatrix[i, j], _distanceMatrix[i, k] + _distanceMatrix[k, j]);

                        if (_adjacencyMatrix[i, k] + _adjacencyMatrix[k, j] < _adjacencyMatrix[i, j])
                        {
                            _adjacencyMatrix[i, j] = _adjacencyMatrix[i, k] + _adjacencyMatrix[k, j];
                        }
                    }
                }
            }
        }

        private void UpdateUserImpactVal(int _hoursN, int _verticesN)
        {
            try
            {
                for (int _h = 0; _h < _hoursN; _h++)
                {
                    for (int i = 0; i < _verticesN; i++)
                    {
                        var _visitedImps = new int[_verticesN];
                        for (int j = 0; j < _verticesN; j++)
                        {
                            _lc++;
                            int _imp = (int)(_inf_valsArr[_h, i] - _adjacencyMatrix[i, j]);
                            if(_imp > 0)
                            {
                                _visitedImps[j] = _imp;
                            }
                        }

                        _hUImps[_h, i] = _visitedImps.Sum();
                        if (_hUImps[_h, _output[_h]] < _hUImps[_h, i])
                            _output[_h] = i;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
