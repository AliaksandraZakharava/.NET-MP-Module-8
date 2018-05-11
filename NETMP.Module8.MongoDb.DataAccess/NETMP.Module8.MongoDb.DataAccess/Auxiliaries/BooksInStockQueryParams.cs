using System;

namespace NETMP.Module8.MongoDb.DataAccess.Auxiliaries
{
    public class BooksInStockQueryParams
    {
        public bool SortByTitle { get; set; }

        public bool ShowOnlyTitle { get; set; }

        public bool GetOnlyCount { get; set; }

        public int GetLimitedItemsNumber { get; set; }

        public BooksInStockQueryParams()
        {
            GetLimitedItemsNumber = Int32.MaxValue;
        }
    }
}
