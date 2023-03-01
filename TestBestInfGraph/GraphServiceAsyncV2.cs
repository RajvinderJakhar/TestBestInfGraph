using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBestInfGraph
{
    internal class GraphServiceAsyncV2
    {
        private static int[,] _hUImps { get; set; }
        private static int[,] _adjMat { get; set; }
        private static int[] _output { get; set; }
        //private static int[] _visitedImps { get; set; }
        private static int[,] _inf_valsArr { get; set; }
        private static int[,] _linksArr { get; set; }

        private static int _lc = 0;

        #region DFS Async
        /// <summary>
        /// Depth First Search or DFS for a Graph
        /// </summary>
        public async void GetBestInfluencerDFSAsync()
        {
            //PriorityQueue
            Console.WriteLine("\tAsync DFS using array");
            Console.WriteLine();

            long st = DateTime.Now.Ticks;
            _lc = 0;
            int _userN, _linksN, _hoursN;

            ReadInputAsync(@"input4.txt", out _userN, out _linksN, out _hoursN);

            _adjMat = new int[_userN, _userN];

            List<int> _res = new List<int>();
            _hUImps = new int[_hoursN, _userN];
            _output = new int[_hoursN];


            CreateAdjMatch(_linksN);
            //st = DateTime.Now.Ticks;
            ProcessImpectDFSAsync(_userN, _hoursN, _linksN);

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

        private async void ProcessImpectDFSAsync(int _userN, int _hoursN, int _linksN)
        {
            var _tasksList = new List<Task<bool>>();
            for (int _h = 0; _h < _hoursN; _h++)
            {
                //Console.WriteLine($"h-{_h}");
                _tasksList.Add(ProcessHourImpectDFSAsync(_h, _userN));
            }
            Task.WhenAll(_tasksList);
        }

        private async Task<bool> ProcessHourImpectDFSAsync(int _h, int _userN)
        {
            try
            {
                var _tasksList = new List<Task<int[]>>();
                for (int _u = 0; _u < _userN; _u++)
                {
                    //Console.WriteLine($"h-{_h}, u-{_u}");
                    int inf_val = _inf_valsArr[_h, _u];

                    if (inf_val > 0)
                    {
                        var _dfsUtil = new DFS_UtilAsync();
                        _tasksList.Add(_dfsUtil.UpdateUserImpactValAsync(_h, _u, _userN, inf_val, _adjMat));
                        //await UpdateUserImpactValAsync(_h, _u, _userN, inf_val, _u, _visitedImps2);
                    }

                }
                var _tasksRes = await Task.WhenAll(_tasksList);

                for (int i = 0; i < _tasksRes.Length; i++)
                {
                    var _res = _tasksRes[i];
                    _hUImps[_h, _res[1]] = _res[2];
                    if (_hUImps[_h, _output[_h]] < _hUImps[_h, _res[1]])
                        _output[_h] = _res[1];
                    _lc += _res[3];
                    //Console.WriteLine("\t" + $"h-{_h}, U-{_cu}, imp-{_hUImps[_h, _cu]}");
                }




                //Console.WriteLine();
                //Console.WriteLine("\t" + $"********* ------------- h-{_h}, U-{_output[_h]} imp-{_hUImps[_h, _output[_h]]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }
        #endregion

        #region BFS Async
        /// <summary>
        /// Breadth First Search or DFS for a Graph
        /// </summary>
        //[Benchmark]
        public async void GetBestInfluencerBFSAsync()
        {
            //PriorityQueue
            Console.WriteLine("\tAsync BFS using array");
            Console.WriteLine();

            long st = DateTime.Now.Ticks;
            _lc = 0;
            int _userN, _linksN, _hoursN;

            ReadInputAsync(@"input4.txt", out _userN, out _linksN, out _hoursN);

            _adjMat = new int[_userN, _userN];

            List<int> _res = new List<int>();
            _hUImps = new int[_hoursN, _userN];
            _output = new int[_hoursN];


            CreateAdjMatch(_linksN);
            //st = DateTime.Now.Ticks;
            ProcessImpectBFSAsync(_userN, _hoursN, _linksN);

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

        private async Task ProcessImpectBFSAsync(int _userN, int _hoursN, int _linksN)
        {
            var _tasksList = new List<Task>();
            Parallel.For(0, _hoursN, _h =>
            {
                var _task = ProcessHourImpectBFSAsync(_h, _userN);
                _tasksList.Add(_task);
            });

            //for (int _h = 0; _h < _hoursN; _h++)
            //{
            //    //Console.WriteLine($"h-{_h}");
            //    var _task = ProcessHourImpectBFSAsync(_h, _userN);
            //    _tasksList.Add(_task);
            //}
            Task.WhenAll(_tasksList);
        }

        //[Benchmark]
        private async Task ProcessHourImpectBFSAsync(int _h, int _userN)
        {
            try
            {
                //Thread.Sleep(1000);
                var _tasksList = new List<Task<int[]>>();
                //Parallel.For(0, _userN, _u =>
                //{
                //    int inf_val = _inf_valsArr[_h, _u];

                //    if (inf_val > 0)
                //    {
                //        var _bfsUtil = new BFS_UtilAsync();
                //        _tasksList.Add(_bfsUtil.UpdateUserImpactValAsync(_h, _u, _userN, inf_val, _adjMat));
                //    }
                //});
                for (int _u = 0; _u < _userN; _u++)
                {
                    //Console.WriteLine($"h-{_h}, u-{_u}");
                    int inf_val = _inf_valsArr[_h, _u];

                    if (inf_val > 0)
                    {
                        var _bfsUtil = new BFS_UtilAsync();
                        _tasksList.Add(_bfsUtil.UpdateUserImpactValAsync(_h, _u, _userN, inf_val, _adjMat));
                        //await UpdateUserImpactValAsync(_h, _u, _userN, inf_val, _u, _visitedImps2);
                    }
                }
                var _tasksRes = await Task.WhenAll(_tasksList);

                for (int i = 0; i < _tasksRes.Length; i++)
                {
                    var _res = _tasksRes[i];
                    _hUImps[_h, _res[1]] = _res[2];
                    if (_hUImps[_h, _output[_h]] < _hUImps[_h, _res[1]])
                        _output[_h] = _res[1];
                    _lc += _res[3];
                    //Console.WriteLine("\t" + $"h-{_h}, U-{_cu}, imp-{_hUImps[_h, _cu]}");
                }




                //Console.WriteLine();
                //Console.WriteLine("\t" + $"********* ------------- h-{_h}, U-{_output[_h]} imp-{_hUImps[_h, _output[_h]]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
        #region Methods
        //[Benchmark]
        private void ReadInputAsync(string inputFile, out int _userN, out int _linksN, out int _hoursN)
        {
            _userN = 0;
            _linksN = 0;
            _hoursN = 0;

            try
            {
                var _lines = File.ReadLines(inputFile).ToList();

                //Console.WriteLine("\t" + String.Join(Environment.NewLine + "\t", _lines));
                Console.WriteLine();

                _lines = _lines.Where(x => !String.IsNullOrEmpty(x)).Select(x => x.Trim()).ToList();

                int.TryParse(_lines[0].Split(" ")[0], out _userN);
                int.TryParse(_lines[0].Split(" ")[1], out _linksN);
                int.TryParse(_lines[0].Split(" ")[2], out _hoursN);

                _linksArr = new int[_linksN, 3];
                for (int i = 0; i < _linksN; i++)
                {
                    var _linkItem = _lines[i + 1].Split(" ").ToArray();
                    if (_linkItem != null)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int.TryParse(_linkItem[j], out int _l);
                            _linksArr[i, j] = _l;
                        }
                    }
                }


                _inf_valsArr = new int[_hoursN, _userN];
                for (int i = 0; i < _hoursN; i++)
                {
                    var _hourItem = _lines[i + _linksN + 1].Split(" ").ToArray();
                    if (_hourItem != null)
                    {
                        for (int j = 0; j < _userN; j++)
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

        //[Benchmark]
        private void CreateAdjMatch(int _linksN)
        {
            for (int i = 0; i < _linksN; i++)
            {
                var _u = _linksArr[i, 0];
                var _v = _linksArr[i, 1];
                _adjMat[_u, _v] = _linksArr[i, 2];
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
        #endregion
    }

    internal class BFS_UtilAsync
    {
        //[Benchmark]
        public async Task<int[]> UpdateUserImpactValAsync(int _h, int _u, int _userN, int inf_val, int[,] _adjMat)
        {
            int[] _res = new int[4];
            int _lc = 0;        //For debug only how many depth loop called
            try
            {
                var _visitedImps = new int[_userN];
                var _q = new Queue<int>();
                _q.Enqueue(_u);
                while (_q.Count > 0)
                {
                    int _qu = _q.Dequeue();
                    for (int _v = 0; _v < _userN; _v++)
                    {
                        _lc++;
                        int _fadingVal = _adjMat[_qu, _v];
                        if (_fadingVal != 0)
                        {
                            var _imp = _visitedImps[_qu] == 0 ? inf_val - _fadingVal : _visitedImps[_qu] - _fadingVal;
                            if (_imp > 0 && _visitedImps[_v] < _imp && _v != _u)
                            {
                                //Console.WriteLine("\t" + $"{_cu} -> From {_u} to {_v} --- {inf_val} - {_fadingVal} = {_imp}");
                                _visitedImps[_v] = _imp;
                                _q.Enqueue(_v);
                            }
                        }
                    }
                }

                _res[0] = _h;
                _res[1] = _u;
                _res[2] = _visitedImps.Sum(); ;
                _res[3] = _lc;      // For debug only


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return _res;
        }
    }

    internal class DFS_UtilAsync
    {
        //[Benchmark]
        public async Task<int[]> UpdateUserImpactValAsync(int _h, int _u, int _userN, int inf_val, int[,] _adjMat)
        {
            int[] _res = new int[4];
            int _lc = 0;        //For debug only how many depth loop called
            try
            {
                var _visitedImps = new int[_userN];
                var _s = new Stack<int>();        //For DFS
                _s.Push(_u);
                while (_s.Count > 0)
                {
                    int _su = _s.Pop();
                    for (int _v = 0; _v < _userN; _v++)
                    {
                        _lc++;
                        int _fadingVal = _adjMat[_su, _v];
                        if (_fadingVal != 0)
                        {
                            var _imp = _visitedImps[_su] == 0 ? inf_val - _fadingVal : _visitedImps[_su] - _fadingVal;
                            if (_imp > 0 && _visitedImps[_v] < _imp && _v != _u)
                            {
                                //Console.WriteLine("\t" + $"{_u} -> From {_su} to {_v} --- {inf_val} - {_fadingVal} = {_imp}");
                                _visitedImps[_v] = _imp;
                                _s.Push(_v);
                            }
                        }
                    }
                }

                _res[0] = _h;
                _res[1] = _u;
                _res[2] = _visitedImps.Sum(); ;
                _res[3] = _lc;      // For debug only


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return _res;
        }
    }

}
