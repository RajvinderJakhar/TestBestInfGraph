using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBestInfGraph
{
    public class GraphService
    {
        private static int[,] _hUImps { get; set; }
        private static int[,] _adjMat { get; set; }
        private static int[] _output { get; set; }
        private static int[] _visitedImps { get; set; }
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

            #region Read input
            var _lines = File.ReadLines(@"input3.txt").ToList();

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

            #endregion

            _adjMat = new int[_userN, _userN];

            List<int> _res = new List<int>();
            _hUImps = new int[_hoursN, _userN];
            _output = new int[_hoursN];

            //int maxThreads;
            //int completionPortThreads;
            //ThreadPool.GetMaxThreads(out maxThreads, out completionPortThreads);

            //int availableThreads;
            //ThreadPool.GetAvailableThreads(out availableThreads, out completionPortThreads);

            

            CreateAdjMatch(_linksN);
            //st = DateTime.Now.Ticks;
            ProcessImpectAsync(_userN, _hoursN, _linksN);

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

        private async void ProcessImpectAsync(int _userN, int _hoursN, int _linksN)
        {
            var _tasksList = new List<Task<bool>>();
            for (int _h = 0; _h < _hoursN; _h++)
            {
                Console.WriteLine($"h-{_h}");
                _tasksList.Add(ProcessHourImpectAsync(_h, _userN));
            }
            Task.WhenAll(_tasksList);
        }

        private async Task<bool> ProcessHourImpectAsync(int _h, int _userN)
        {
            try
            {
                var _tasksList = new List<Task<bool>>();
                for (int _u = 0; _u < _userN; _u++)
                {
                    Console.WriteLine($"h-{_h}, u-{_u}");
                    var _visitedImps2 = new int[_userN];
                    int inf_val = _inf_valsArr[_h, _u];
                    
                    if (inf_val > 0)
                    {
                        
                        _tasksList.Add(UpdateUserImpactValAsync(_h, _u, _userN, inf_val, _u, _visitedImps2));
                        //await UpdateUserImpactValAsync(_h, _u, _userN, inf_val, _u, _visitedImps2);
                    }
                    
                }
                Task.WhenAll(_tasksList);
                //Console.WriteLine("\t" + $"********* ------------- h-{_h}, U-{_output[_h]} imp-{_hUImps[_h, _output[_h]]}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        private async Task<bool> UpdateUserImpactValAsync(int _h, int _u, int _userN, int inf_val, int _cu, int[] _visitedImps2, bool _isSC = false)
        {
            try
            {
                //if (!_isSC)
                //    Console.WriteLine("\t" + $"h {_h} u {_u} --- {inf_val} ");
                for (int _v = 0; _v < _userN; _v++)
                {
                    _lc++;

                    
                    //Console.WriteLine();
                    //Console.Write("\t" + $"From {_u} to {_v} --- {inf_val} ");
                    if (_adjMat[_u, _v] != 0 && (!_isSC || _v != _cu))
                    {
                        int _fadingVal = _adjMat[_u, _v];
                        var _imp = inf_val - _fadingVal;
                        if (_imp > 0)
                        {
                            //Console.Write($" - {_fadingVal} = {_imp}");
                            //Console.WriteLine("\t" + $"{_cu} -> From {_u} to {_v} --- {inf_val} - {_fadingVal} = {_imp}");
                            if (_visitedImps2[_v] < _imp)
                            {
                                _visitedImps2[_v] = _imp;
                                if (!_isSC || _v != _cu)
                                    await UpdateUserImpactValAsync(_h, _v, _userN, _imp, _cu, _visitedImps2, _isSC: true);
                            }

                        }
                    }
                }


                if (!_isSC)
                {
                    _hUImps[_h, _cu] += _visitedImps2.Sum();

                    if (_hUImps[_h, _output[_h]] < _hUImps[_h, _cu])
                        _output[_h] = _cu;

                    //Console.WriteLine("\t" + $"h-{_h}, U-{_cu}, imp-{_hUImps[_h, _cu]}");
                    //Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }
        #endregion

        #region DFS

        //[Benchmark]
        public void GetBestInfluencerDFS()
        {
            Console.WriteLine("\tDFS using array");
            Console.WriteLine();

            long st = DateTime.Now.Ticks;
            _lc = 0;
            int _userN, _linksN, _hoursN;

            #region Read input
            var _lines = File.ReadLines(@"input4.txt").ToList();

            //Console.WriteLine("\t" + String.Join(Environment.NewLine + "\t", _lines));

            _lines = _lines.Where(x => !String.IsNullOrEmpty(x)).Select(x => x.Trim()).ToList();

            int.TryParse(_lines[0].Split(" ")[0], out _userN);
            int.TryParse(_lines[0].Split(" ")[1], out _linksN);
            int.TryParse(_lines[0].Split(" ")[2], out _hoursN);

            _linksArr = new int[_linksN, 3];
            for (int i = 0; i < _linksN; i++)
            {
                var _linkItem = _lines[i + 1].Split(" ").ToArray();
                if(_linkItem != null)
                {
                    for(int j = 0; j < 3; j++)
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

            #endregion

            _adjMat = new int[_userN, _userN];

            List<int> _res = new List<int>();
            _hUImps = new int[_hoursN, _userN];
            _output = new int[_hoursN];

            CreateAdjMatch(_linksN);
            ProcessImpect(_userN, _hoursN, _linksN);

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

        private static void ProcessImpect(int _userN, int _hoursN, int _linksN)
        {
            for (int i = 0; i < _hoursN; i++)
            {
                for (int j = 0; j < _userN; j++)
                {
                    _visitedImps = new int[_userN];
                    int inf_val = _inf_valsArr[i, j];

                    if (inf_val > 0)
                    {
                        UpdateUserImpactVal(i, j, _userN, inf_val, j);
                        if (_hUImps[i, _output[i]] < _hUImps[i, j])
                            _output[i] = j;

                        //Console.WriteLine("\t" + $"h-{i}, U-{j}, imp-{_hUImps[i, j]}");
                        //Console.WriteLine();
                    }
                }
                //Console.WriteLine("\t" + $"********* ------------- h-{i}, U-{_output[i]} imp-{_hUImps[i, _output[i]]}");
            }
        }

        private static void UpdateUserImpactVal(int _h, int _u, int _userN, int inf_val, int _cu, bool _isSC = false)
        {
            try
            {
                for (int _v = 0; _v < _userN; _v++)
                {
                    _lc++;
                    if (_adjMat[_u, _v] != 0 && (!_isSC || _v != _cu))
                    {
                        int _fadingVal = _adjMat[_u, _v];
                        var _imp = inf_val - _fadingVal;
                        if (_imp > 0)
                        {
                            //Console.WriteLine("\t" + $"{_cu} -> From {_u} to {_v} --- {inf_val} - {_fadingVal} = {_imp}");
                            if (_visitedImps[_v] < _imp)
                            {
                                _visitedImps[_v] = _imp;
                                if (!_isSC || _v != _cu)
                                    UpdateUserImpactVal(_h, _v, _userN, _imp, _cu, _isSC: true);
                            }
                        }
                    }
                }


                if (!_isSC)
                {
                    _hUImps[_h, _cu] += _visitedImps.Sum();

                    if (_hUImps[_h, _output[_h]] < _hUImps[_h, _cu])
                        _output[_h] = _cu;

                    //Console.WriteLine("\t" + $"h-{_h}, U-{_cu}, imp-{_hUImps[_h, _cu]}");
                    //Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        #endregion

        #region DFS Using lists

        private static List<List<int>> inf_vals { get; set; } = new List<List<int>>();
        private static List<List<int>> _links { get; set; } = new List<List<int>>();
        public void GetBestInfluencerDFS_List()
        {
            Console.WriteLine("\tDFS using list");
            Console.WriteLine();

            _lc = 0;
            long st = DateTime.Now.Ticks;

            int _userN, _linksN, _hoursN;
            var _lines = File.ReadLines(@"input4.txt").ToList();

            //Console.WriteLine("\t" + String.Join(Environment.NewLine + "\t", _lines));

            _lines = _lines.Where(x => !String.IsNullOrEmpty(x)).Select(x => x.Trim()).ToList();

            int.TryParse(_lines[0].Split(" ")[0], out _userN);
            int.TryParse(_lines[0].Split(" ")[1], out _linksN);
            int.TryParse(_lines[0].Split(" ")[2], out _hoursN);

            _links = new List<List<int>>();
            for (int i = 1; i <= _linksN; i++)
            {
                var _linkItem = _lines[i].Split(" ").ToList();
                _links.Add(_linkItem.Select(int.Parse).ToList());
            }

            inf_vals = new List<List<int>>();
            for (int i = _linksN + 1; i < (_linksN + 1 + _hoursN); i++)
            {
                var _hourItem = _lines[i].Split(" ").ToList();
                inf_vals.Add(_hourItem.Select(int.Parse).ToList());
            }


            List<int> _res = new List<int>();
            _hUImps = new int[_hoursN, _userN];
            _output = new int[_hoursN];

            ProcessImpect_UsingList(_userN, _hoursN);

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

        private static void ProcessImpect_UsingList(int _userN, int _hoursN)
        {
            for (int i = 0; i < _hoursN; i++)
            {
                //Console.WriteLine();
                //Console.WriteLine();

                for (int j = 0; j < _userN; j++)
                {
                    _visitedImps = new int[_userN];
                    int inf_val = inf_vals[i][j];

                    if (inf_val > 0)
                    {
                        UpdateUserImpactVal_UL(i, j, _userN, inf_val, j);
                        if (_hUImps[i, _output[i]] < _hUImps[i, j])
                            _output[i] = j;

                        //Console.WriteLine("\t" + $"h-{i}, U-{j}, imp-{_hUImps[i, j]}");
                        //Console.WriteLine();
                    }
                }
                //Console.WriteLine("\t" + $"********* ------------- h-{i}, U-{_output[i]} imp-{_hUImps[i, _output[i]]}");
            }
        }

        private static void UpdateUserImpactVal_UL(int _h, int _u, int _userN, int inf_val, int _cu, bool _isSC = false)
        {
            try
            {
                var _lnks = _links.Where(x => x[0] == _u).ToList();
                if (_lnks != null && _lnks.Count > 0)
                {
                    foreach (var _lnk in _lnks)
                    {
                        _lc++;

                        int _fadingVal = _lnk[2];
                        var _imp = inf_val - _fadingVal;
                        int _vU = _lnk[1];
                        
                        if (_imp > 0 && _vU != _cu)
                        {
                            //Console.WriteLine("\t" + $"From {_u} to {_vU} --- {inf_val} - {_fadingVal}, = {_imp}");
                            if (_visitedImps[_vU] < _imp)
                            {
                                _visitedImps[_vU] = _imp;
                                UpdateUserImpactVal_UL(_h, _vU, _userN, _imp, _cu, _isSC: true);
                            }
                        }
                    }
                }

                if (!_isSC)
                    _hUImps[_h, _cu] += _visitedImps.Sum();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        #endregion

        #region BFS

        /// <summary>
        /// Breadth First Search or BFS for a Graph
        /// </summary>
        //[Benchmark]
        public void GetBestInfluencerBFS()
        {
            Console.WriteLine("\tBFS using array");
            Console.WriteLine();

            long st = DateTime.Now.Ticks;
            _lc = 0;
            int _userN, _linksN, _hoursN;

            #region Read input
            var _lines = File.ReadLines(@"input4.txt").ToList();

            //Console.WriteLine("\t" + String.Join(Environment.NewLine + "\t", _lines));

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

            #endregion

            _adjMat = new int[_userN, _userN];

            List<int> _res = new List<int>();
            _hUImps = new int[_hoursN, _userN];
            _output = new int[_hoursN];

            CreateAdjMatch(_linksN);
            ProcessImpectBFS(_userN, _hoursN, _linksN);

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

        private static void ProcessImpectBFS(int _userN, int _hoursN, int _linksN)
        {
            for (int i = 0; i < _hoursN; i++)
            {
                for (int j = 0; j < _userN; j++)
                {
                    _visitedImps = new int[_userN];
                    int inf_val = _inf_valsArr[i, j];

                    if (inf_val > 0)
                    {
                        UpdateUserImpactValBFS(i, j, _userN, inf_val);
                        if (_hUImps[i, _output[i]] < _hUImps[i, j])
                            _output[i] = j;

                        //Console.WriteLine("\t" + $"h-{i}, U-{j}, imp-{_hUImps[i, j]}");
                        //Console.WriteLine();
                    }
                }
                //Console.WriteLine("\t" + $"********* ------------- h-{i}, U-{_output[i]} imp-{_hUImps[i, _output[i]]}");
            }
        }

        private static void UpdateUserImpactValBFS(int _h, int _u, int _userN, int inf_val)
        {
            try
            {
                var _q = new Queue<int>();
                //var _q = new Stack<int>();        //For stack   // For DFS
                _q.Enqueue(_u);
                //_q.Push(_u);
                while (_q.Count > 0)
                {
                    int _qu = _q.Dequeue();
                    //int _qu = _q.Pop();     //For stack   
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
                                //_q.Push(_v);
                            }
                        }
                    }
                }

                _hUImps[_h, _u] += _visitedImps.Sum();

                if (_hUImps[_h, _output[_h]] < _hUImps[_h, _u])
                    _output[_h] = _u;

                //Console.WriteLine("\t" + $"h-{_h}, U-{_cu}, imp-{_hUImps[_h, _cu]}");
                //Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        #endregion

        #region Methods
        private static void CreateAdjMatch(int _linksN)
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
}
