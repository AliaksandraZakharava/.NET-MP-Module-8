using MongoDB.Bson;
using MongoDB.Driver;
using NETMP.Module8.MongoDb.DataAccess.Tables;

namespace NETMP.Module8.MongoDb.DataAccess
{
    public class DatabaseContext
    {
        public const string ConnectionString = "mongodb://localhost:27017";
        public const string DatabaseName = "BooksDB";

        public readonly MongoClient _client;
        public readonly IMongoDatabase _database;

        public DatabaseContext()
        {
            var _client = new MongoClient(ConnectionString);

            _database = _client.GetDatabase(DatabaseName);
        }

        public void DropCollection(string collectionName)
        {
            _database.DropCollection(collectionName);
        }

        public IMongoCollection<BsonDocument> Books
        {
            get { return _database.GetCollection<BsonDocument>(TablesNames.BooksTableName); }
        }
    }
}
