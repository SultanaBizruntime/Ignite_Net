using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitedotnet
{
    class OptimisticTransaction
    {
        public const string cacheName = "optimisticTransaction";
        public static void IncrementCacheValue(ICache<int,int> cache,int threadId)
        {
            try
            {
                var transaction =cache.Ignite.GetTransactions();
                using(var tx=transaction.TxStart(TransactionConcurrency.Optimistic,TransactionIsolation.Serializable))
                {
                    cache[1]++;
                    Thread.Sleep(TimeSpan.FromSeconds(1.7));
                    tx.Commit();
                }
                Console.WriteLine("Thread {0} successfully incremented the cache value",threadId);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Thread{0} failed to increment cache value" + 
                    "caught an expected optimistic exception {1}", threadId,ex.Message);
            }
        }
    }
}
