﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Lucene.Net.Index;

namespace TicketDesk.Domain.Search.Lucene
{
    internal class LuceneIndexManager : LuceneSearchConnector, ISearchIndexManager
    {
        internal LuceneIndexManager(string indexLocation)
            : base(indexLocation)
        {
        }

        public async Task<bool> RunStartupIndexMaintenanceAsync()
        {
            return await Task.FromResult(true);//nothing to do for lucene, the indexes is built as a side-effect of adding documents
        }

        public Task<bool> AddItemsToIndexAsync(IEnumerable<SearchQueueItem> items)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var indexWriter = await GetIndexWriterAsync();
                    foreach (var item in items)
                    {
                        UpdateIndexForItem(indexWriter, item);
                    }
                    indexWriter.Optimize();
                    indexWriter.Dispose();
                } // ReSharper disable once EmptyGeneralCatchClause
                catch 
                { 
                    //TODO: log this somewhere
                }
                return true;
            });
        }

        public Task<bool> RemoveIndexAsync()
        {
            try
            {
                if (IndexLocation != "ram")
                {
                    var directoryInfo = new DirectoryInfo(IndexLocation);
                    Parallel.ForEach(directoryInfo.GetFiles(), file => file.Delete());
                }
                else
                {
                    TdIndexDirectory.Dispose();
                }
            } // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                //TODO: log this somewhere
            }
            return Task.FromResult(true);
        }

        private static void UpdateIndexForItem(IndexWriter indexWriter, SearchQueueItem item)
        {
            indexWriter.UpdateDocument(
                new Term("id", item.Id.ToString(CultureInfo.InvariantCulture)),
                item.ToLuceneDocument());
        }

    }
}
