using System;
using System.Collections.Generic;

namespace NETMP.Module8.MongoDb.DataAccess.Models
{
    public class Book : IEquatable<Book>
    {
        private List<string> _genre;
        private int _count;
        private int? _publishingYear;

        public string Name { get; set; }

        public string Author { get; set; }

        public List<string> Genre
        {
            get { return _genre; }
            set { _genre = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentException("Negative count value.");
                }

                _count = value;
            }
        }

        public int? PublishingYear
        {
            get { return _count; }
            set
            {
                if (value.HasValue && value > DateTime.UtcNow.Year)
                {
                    throw new ArgumentException("Publishing year should be less then the current year.");
                }

                _publishingYear = value;
            }
        }

        public Book()
        {
            Genre = new List<string>();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() % Author.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Book))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            Book item = (Book)obj;

            return Equals(item);
        }

        public bool Equals(Book other)
        {
            return Name == other?.Name && Author == other?.Author;
        }
    }
}
