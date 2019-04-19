using Reptile.Dispose;
using System;
using System.Threading.Tasks;

namespace Reptile.App
{
    class Program
    {
        //static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        static void Main(string[] args)
        {
            CommoditySearch search = new CommoditySearch();
            Task.Factory.StartNew(() => { search.Start(); });
            Console.ReadLine();
        }
    }
}
