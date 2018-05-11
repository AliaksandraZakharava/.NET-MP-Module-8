using MongoDB.Bson;
using MongoDB.Driver;
using NETMP.Module8.MongoDb.DataAccess.Auxiliaries;
using NETMP.Module8.MongoDb.DataAccess.Converters;
using NETMP.Module8.MongoDb.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NETMP.Module8.MongoDb.DataAccess
{
    public class BooksRepository
    {
        private readonly DatabaseContext _context;
        private readonly FilterDefinitionBuilder<BsonDocument> _filter;
        private readonly UpdateDefinitionBuilder<BsonDocument> _update;

        public BooksRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _filter = Builders<BsonDocument>.Filter;
            _update = Builders<BsonDocument>.Update;
        }

        public void AddBook(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            _context.Books.InsertOne(BookConverter.ConvertBookToDocument(book));
        }

        public void AddBooks(List<Book> books)
        {
            if(books == null)
            {
                throw new ArgumentNullException(nameof(books));
            }

            var documents = books.Select(BookConverter.ConvertBookToDocument);

            _context.Books.InsertMany(documents);
        }

        public AggregateResponce<Book> GetBooksInStock(BooksInStockQueryParams queryParams)
        {
            if (queryParams == null)
            {
                throw new ArgumentNullException(nameof(queryParams));
            }

            var response = new AggregateResponce<Book>();

            var filter = _filter.Gt(BooksTableFieldNames.Count, 1);

            var filteredResult = _context.Books.Find(filter).Limit(queryParams.GetLimitedItemsNumber);

            if (queryParams.ShowOnlyTitle)
            {
                filteredResult = filteredResult.Project(Builders<BsonDocument>.Projection.Include(BooksTableFieldNames.Name));
            }

            if (queryParams.SortByTitle)
            {
                var sortFilter = new BsonDocument().Add(BooksTableFieldNames.Name, 1);

                filteredResult = filteredResult.Sort(sortFilter);
            }

            if (!queryParams.GetOnlyCount)
            {
                if (queryParams.ShowOnlyTitle)
                {
                    response.Titles = filteredResult.ToEnumerable()
                                                    .Select(doc => doc[BooksTableFieldNames.Name].AsString)
                                                    .ToList();
                }
                else
                {
                    response.Items = filteredResult.ToEnumerable()
                                                   .Select(BookConverter.ConvertDocumentToBook)
                                                   .ToList();
                }
            }

            response.AgregateValue = filteredResult.ToEnumerable().Count();

            return response;
        }

        /// <summary>
        /// Get book with max or min Count value
        /// </summary>
        /// <param name="limitValue">BookLimitCountValues enum value: NotDefined, MinCount or MaxCount</param>
        /// <returns></returns>
        public Book GetBookWithLimitCount(BookLimitCountValues limitValue)
        {
            if(limitValue == BookLimitCountValues.NotDefined)
            {
                throw new ArgumentException("Limit value is not defined.");
            }

            var filter = _filter.Empty;

            var filteredResult = limitValue == BookLimitCountValues.MaxCount
                                 ? _context.Books.Find(filter).SortByDescending(doc => doc[BooksTableFieldNames.Count]).Limit(1)
                                 : _context.Books.Find(filter).SortBy(doc => doc[BooksTableFieldNames.Count]).Limit(1);

            return BookConverter.ConvertDocumentToBook(filteredResult.ToEnumerable().First());
        }

        public List<string> GetAuthorsList()
        {
            return _context.Books.Distinct<string>(BooksTableFieldNames.Author, _filter.Empty).ToList();
        }

        public List<Book> GetBooksWithNoAuthorDefined()
        {
            var filter = _filter.Eq(BooksTableFieldNames.Author, BsonNull.Value);

            return _context.Books.Find(filter).ToEnumerable().Select(BookConverter.ConvertDocumentToBook).ToList();
        }

        public void IncrementBooksCount()
        {
            var filter = _filter.Empty;
            var update = _update.Inc(BooksTableFieldNames.Count, 1);

            _context.Books.UpdateMany(filter, update);
        }

        public void AddFavorityGenreToFantasyBooks()
        {
            var filter = _filter.Regex(BooksTableFieldNames.Genre, "fantasy") &
                         _filter.Regex(BooksTableFieldNames.Genre, "^((?!favority).)*$");

            var update = _update.AddToSet(BooksTableFieldNames.Genre, "favority");

            _context.Books.UpdateMany(filter, update);
        }

        public void DeleteBooksWithCountMoreThanValue(int count)
        {
            var filter = _filter.Gt(BooksTableFieldNames.Count, count);

            _context.Books.DeleteMany(filter);
        }

        public void DeleteAllBooks()
        {
            _context.Books.DeleteMany(_filter.Empty);
        }
    }
}
