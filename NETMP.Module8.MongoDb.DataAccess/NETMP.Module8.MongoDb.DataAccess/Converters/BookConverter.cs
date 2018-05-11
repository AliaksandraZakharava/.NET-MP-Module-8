using System.Linq;
using MongoDB.Bson;
using NETMP.Module8.MongoDb.DataAccess.Models;

namespace NETMP.Module8.MongoDb.DataAccess.Converters
{
    public static class BookConverter
    {
        public static BsonDocument ConvertBookToDocument(Book book)
        {
            if(book == null)
            {
                return null;
            }

            return new BsonDocument()
            {
                [BooksTableFieldNames.Name] = BsonValuesConverter.ConvertStringToBsonValue(book.Name),
                [BooksTableFieldNames.Author] = BsonValuesConverter.ConvertStringToBsonValue(book.Author),
                [BooksTableFieldNames.Genre] = new BsonArray(book.Genre),
                [BooksTableFieldNames.Count] = book.Count,
                [BooksTableFieldNames.PublishingYear] = BsonValuesConverter.ConvertNullableToBsonValue(book.PublishingYear)
            };
        }

        public static Book ConvertDocumentToBook(BsonDocument document)
        {
            if (document == null)
            {
                return null;
            }

            return new Book
            {
                Name = BsonValuesConverter.ConvertBsonValueToString(document[BooksTableFieldNames.Name]),
                Author = BsonValuesConverter.ConvertBsonValueToString(document[BooksTableFieldNames.Author]),
                Genre = document[BooksTableFieldNames.Genre].AsBsonArray.Select(doc => doc.AsString).ToList(),
                Count = document[BooksTableFieldNames.Count].AsInt32,
                PublishingYear = document[BooksTableFieldNames.PublishingYear].AsNullableInt32
            };
        }
    }
}
