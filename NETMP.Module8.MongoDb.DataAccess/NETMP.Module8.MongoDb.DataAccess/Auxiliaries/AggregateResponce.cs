using System.Collections.Generic;

namespace NETMP.Module8.MongoDb.DataAccess.Auxiliaries
{
    public class AggregateResponce<T>
    {
        public List<T> Items { get; set; }

        public List<string> Titles { get; set; }

        public int AgregateValue { get; set; }

        public AggregateResponce()
        {
            Items = new List<T>();
            Titles = new List<string>();
        }
    }
}
