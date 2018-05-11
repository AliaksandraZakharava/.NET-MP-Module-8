using NETMP.Module8.MongoDb.DataAccess.Models;
using System.Collections.Generic;

namespace NETMP.Module8.MongoDb.Tests
{
    public class TestData
    {
        public static List<Book> Books = new List<Book>
        {
            new Book {Name = "Hobbit", Author ="Tolkien", Count = 5, Genre = new List<string>{ "fantasy" }, PublishingYear = 2014 },
            new Book {Name = "Lord of the rings", Author ="Tolkien", Count = 3, Genre = new List<string>{ "fantasy" }, PublishingYear = 2015 },
            new Book {Name = "Kolobok", Count = 10, Genre = new List<string>{ "kids" }, PublishingYear = 2000 },
            new Book {Name = "Repka", Count = 11, Genre = new List<string>{ "kids" }, PublishingYear = 2000 },
            new Book {Name = "Dyadya Stiopa", Author ="Mihalkov", Count = 1, Genre = new List<string>{ "kids" }, PublishingYear = 2001 }
        };

        public static Book Book = new Book
        {
            Name = "Hamlet",
            Author = "W. Shakespeare",
            Count = 10,
            Genre = new List<string> { "Classics", "Tragedy" },
            PublishingYear = 2004
        };
    }
}
