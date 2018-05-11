using MongoDB.Bson;
using MongoDB.Driver;
using NETMP.Module8.MongoDb.DataAccess;
using NETMP.Module8.MongoDb.DataAccess.Auxiliaries;
using NETMP.Module8.MongoDb.DataAccess.Converters;
using NETMP.Module8.MongoDb.DataAccess.Models;
using NETMP.Module8.MongoDb.DataAccess.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NETMP.Module8.MongoDb.Tests
{
    public class DataAccessTests : IDisposable
    {
        private readonly BooksRepository _repository;
        private readonly DatabaseContext _context;

        private readonly FilterDefinitionBuilder<BsonDocument> _filter;

        public DataAccessTests()
        {
            _context = new DatabaseContext();
            _repository = new BooksRepository(_context);

            _filter = Builders<BsonDocument>.Filter;

            InitContext();
        }

        public void Dispose()
        {
            _context.DropCollection(TablesNames.BooksTableName);
        }

        [Fact]
        public void AddBook_BookIsNull_ThrowsNullArgumentException()
        {
            Book book = null;

            Assert.Throws<ArgumentNullException>(() => _repository.AddBook(book));
        }

        [Fact]
        public void AddBook_ValidBook_AddsBookToTable()
        {
            var itemsBeforeInsert = GetAllCount();

            _repository.AddBook(TestData.Book);

            var itemsAfterInsert = GetAllCount();

            Assert.True(itemsAfterInsert - itemsBeforeInsert == 1);
        }

        [Fact]
        public void AddBooks_BooksListIsNull_ThrowsNullArgumentException()
        {
            List<Book> books = null;

            Assert.Throws<ArgumentNullException>(() => _repository.AddBooks(books));
        }

        [Fact]
        public void AddBooks_ValidBooksList_AddsBooksToTable()
        {
            _context.DropCollection(TablesNames.BooksTableName);

            var itemsBeforeInsert = GetAllCount();

            _repository.AddBooks(TestData.Books);

            var itemsAfterInsert = GetAllCount();

            Assert.True(itemsAfterInsert - itemsBeforeInsert > 1);
        }

        [Fact]
        public void GetBooksInStock_QueryParamsAreNull_ThrowsArgumentNullException()
        {
            BooksInStockQueryParams queryParams = null;

            Assert.Throws<ArgumentNullException>(() => _repository.GetBooksInStock(queryParams));
        }

        [Fact]
        public void GetBooksInStock_NoParamsAreSet_ReturnsAllBooksWithCountMoreThanZero()
        {
            var expectedResult = TestData.Books.Count(book => book.Count > 1);

            var queryParams = new BooksInStockQueryParams();

            var actualResult = _repository.GetBooksInStock(queryParams).Items.Count;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetBooksInStock_SortByTitleFilterIsSet_ReturnsAllBooksWithCountMoreThanZeroSortedbyTitle()
        {
            var expectedResult = TestData.Books.Where(book => book.Count > 1)
                                               .OrderBy(book => book.Name);

            var queryParams = new BooksInStockQueryParams { SortByTitle = true};

            var actualResult = _repository.GetBooksInStock(queryParams).Items;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetBooksInStock_ShowOnlyTitleFilterIsSet_ReturnsOnlyTitlesOfAllBooksWithCountMoreThanZero()
        {
            var expectedResult = TestData.Books.Where(book => book.Count > 1)
                                               .Select(book => book.Name).ToList();

            var queryParams = new BooksInStockQueryParams { ShowOnlyTitle = true };

            var actualResult = _repository.GetBooksInStock(queryParams).Titles;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetBooksInStock_GetOnlyCountIsSet_ReturnsOnlyCountOfAllBooksWithCountMoreThanZero()
        {
            var expectedResult = TestData.Books.Count(book => book.Count > 1);

            var queryParams = new BooksInStockQueryParams { GetOnlyCount = true };

            var result = _repository.GetBooksInStock(queryParams);

            var actualResultBooks = result.Items;
            var actualResultCount = result.AgregateValue;

            Assert.Empty(actualResultBooks);
            Assert.Equal(expectedResult, actualResultCount);
        }

        [Fact]
        public void GetBooksInStock_GetLimitedItemsNumberIsSet_ReturnslimitedNumberBooksWithCountMoreThanZero()
        {
            var booksLimit = 2;

            var expectedResult = TestData.Books.Where(book => book.Count > 0).Take(booksLimit).ToList();

            var queryParams = new BooksInStockQueryParams { GetLimitedItemsNumber = booksLimit };

            var actualResult = _repository.GetBooksInStock(queryParams).Items;

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void GetBookWithLimitCount_NotDefinedLimitCount_ThrowsArgumentException()
        {
            var limitCountValue = BookLimitCountValues.NotDefined;

            Assert.Throws<ArgumentException>(() => _repository.GetBookWithLimitCount(limitCountValue));
        }

        [Fact]
        public void GetBookWithLimitCount_MaxLimitCount_ReturnsBookWithMaxCount()
        {
            var limitCountValue = BookLimitCountValues.MaxCount;

            var expectedResult = TestData.Books.Aggregate((book1, book2) => book1.Count > book2.Count ? book1 : book2);

            var actualResult = _repository.GetBookWithLimitCount(limitCountValue);

            Assert.Equal(expectedResult.Name, actualResult.Name);
        }

        [Fact]
        public void GetBookWithLimitCount_MinLimitCount_ReturnsBookWithMinCount()
        {
            var limitCountValue = BookLimitCountValues.MinCount;

            var expectedResult = TestData.Books.Aggregate((book1, book2) => book1.Count < book2.Count ? book1 : book2);

            var actualResult = _repository.GetBookWithLimitCount(limitCountValue);

            Assert.Equal(expectedResult.Name, actualResult.Name);
        }

        [Fact]
        public void GetAuthorsList_ReturnsListOfDistinctAuthors()
        {
            var expectedResult = TestData.Books.Select(book => book.Author).Distinct().ToList();

            var actualResult = _repository.GetAuthorsList();

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void IncrementBooksCount_IncrementsAllBooksCount()
        {
            var booksNumber = TestData.Books.Count();
            var booksCountSumBeforeUpdate = GetAllCountSum();

            _repository.IncrementBooksCount();

            var booksCountSumAfterUpdate = GetAllCountSum();


            Assert.Equal(booksCountSumAfterUpdate - booksCountSumBeforeUpdate, booksNumber);
        }

        [Fact]
        public void AddFavorityGenreToFantasyBooks_AddFavorityGenreToBooksOfFantasyGenre()
        {
            var favorityGenre = "favority";

            _repository.AddFavorityGenreToFantasyBooks();

            var actualResult = GetBooksOfFantasyGenre();

            actualResult.ForEach(book => Assert.Contains(favorityGenre, book.Genre));

            _repository.AddFavorityGenreToFantasyBooks();

            actualResult.ForEach(book => Assert.Equal(1, book.Genre.Count(item => item == favorityGenre)));
        }

        [Fact]
        public void GetBooksWithNoAuthorDefined_ReturnsBooksWithNoAuthorSet()
        {
            var expectedResult = TestData.Books.Where(book => string.IsNullOrEmpty(book.Author)).ToList();

            var actualResult = _repository.GetBooksWithNoAuthorDefined();

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void DeleteBooksWithCountMoreThanValue_DeletesBooksWithCountValueMoreThanDefined()
        {
            var countValue = 3;

            var preDeleteCount = TestData.Books.Count();

            _repository.DeleteBooksWithCountMoreThanValue(countValue);

            var postDeleteCount = GetAllCount();

            Assert.True(preDeleteCount > postDeleteCount);
        }

        [Fact]
        public void DeleteAllBooks_DeletesAllBooks()
        {
            var preDeleteCount = TestData.Books.Count();

            _repository.DeleteAllBooks();

            var postDeleteCount = GetAllCount();

            Assert.True(preDeleteCount > postDeleteCount);
            Assert.Equal(0, postDeleteCount);
        }

        #region Additional methods

        private void InitContext()
        {
            _context.Books.InsertMany(TestData.Books.Select(book => BookConverter.ConvertBookToDocument(book)));
        }

        private int GetAllCount()
        {
            return _context.Books.Find(_filter.Empty).ToEnumerable().Count();
        }

        private int GetAllCountSum()
        {
            return _context.Books.Find(_filter.Empty).ToList()
                                 .Select(BookConverter.ConvertDocumentToBook)
                                 .Sum(book => book.Count);
        }

        private List<Book> GetBooksOfFantasyGenre()
        {
            var filter = _filter.Regex(BooksTableFieldNames.Genre, "fantasy");

            return _context.Books.Find(filter).ToEnumerable()
                                              .Select(doc => BookConverter.ConvertDocumentToBook(doc))
                                              .ToList();
        }

        #endregion
    }
}
