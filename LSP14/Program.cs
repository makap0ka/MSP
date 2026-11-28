using System;
using System.Collections;

namespace _13_Cinema
{
    enum Genre
    {
        Comedy,
        Horror,
        Adventure,
        Drama,
        Action,
        Fantasy,
        SciFi
    }

    class Director : ICloneable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int BirthYear { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, born in {BirthYear}";
        }
    }

    class Movie : IComparable<Movie>, ICloneable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Director Director { get; set; }
        public string Country { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public short Rating { get; set; }

        public object Clone()
        {
            Movie clone = (Movie)this.MemberwiseClone();

            clone.Director = (Director)this.Director.Clone();

            return clone;
        }

        public int CompareTo(Movie? other)
        {
            return Title.CompareTo(other?.Title);
        }

        public override string ToString()
        {
            return $"Title: {Title}\n" +
                   $"Description: {Description}\n" +
                   $"Director: {Director}\n" +
                   $"Country: {Country}\n" +
                   $"Genre: {Genre}\n" +
                   $"Year: {Year}\n" +
                   $"Rating: {Rating}/10\n";
        }
    }

    class CompareByRating : IComparer<Movie>
    {
        public int Compare(Movie? x, Movie? y)
        {
            return x!.Rating.CompareTo(y!.Rating);
        }
    }

    class CompareByYear : IComparer<Movie>
    {
        public int Compare(Movie? x, Movie? y)
        {
            return x!.Year.CompareTo(y!.Year);
        }
    }

    class Cinema : IEnumerable
    {
        Movie[] movies;
        string address;

        public Cinema()
        {
            address = "Main Street, 15";

            movies = new Movie[]
            {
                new Movie
                {
                    Title = "The Avengers",
                    Description = "Superheroes save the world.",
                    Director = new Director
                    {
                        FirstName = "Joss",
                        LastName = "Whedon",
                        BirthYear = 1964
                    },
                    Country = "USA",
                    Genre = Genre.Action,
                    Year = 2012,
                    Rating = 8
                },

                new Movie
                {
                    Title = "Titanic (i've belive i can flyyyyyyyyyy)",
                    Description = "A love story during a ship disaster.",
                    Director = new Director
                    {
                        FirstName = "James",
                        LastName = "Cameron",
                        BirthYear = 1954
                    },
                    Country = "USA",
                    Genre = Genre.Drama,
                    Year = 1997,
                    Rating = 9
                },

                new Movie
                {
                    Title = "The Mask",
                    Description = "A man finds a magical mask.",
                    Director = new Director
                    {
                        FirstName = "Chuck",
                        LastName = "Russell",
                        BirthYear = 1958
                    },
                    Country = "USA",
                    Genre = Genre.Comedy,
                    Year = 1994,
                    Rating = 7
                },

                new Movie
                {
                    Title = "Parry Hotter",
                    Description = "A young wizard studies magic.",
                    Director = new Director
                    {
                        FirstName = "Chris",
                        LastName = "Columbus",
                        BirthYear = 1958
                    },
                    Country = "UK",
                    Genre = Genre.Fantasy,
                    Year = 2001,
                    Rating = 9
                },
            };
        }

        public IEnumerator GetEnumerator()
        {
            return movies.GetEnumerator();
        }

        public void PrintAll()
        {
            foreach (Movie movie in movies)
            {
                Console.WriteLine(movie);
            }
        }

        public void Sort()
        {
            Array.Sort(movies);
        }

        public void Sort(IComparer<Movie> comparer)
        {
            Array.Sort(movies, comparer);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Cinema cinema = new Cinema();

            Console.WriteLine("========== ALL MOVIES ==========");
            cinema.PrintAll();

            Console.WriteLine("========== SORT BY TITLE ==========");
            cinema.Sort();
            cinema.PrintAll();

            Console.WriteLine("========== SORT BY RATING ==========");
            cinema.Sort(new CompareByRating());
            cinema.PrintAll();

            Console.WriteLine("========== SORT BY YEAR ==========");
            cinema.Sort(new CompareByYear());
            cinema.PrintAll();

            Console.WriteLine("========== ENUMERABLE ==========");

            foreach (Movie movie in cinema)
            {
                Console.WriteLine(movie.Title);
            }

            Console.WriteLine("\n========== CLONE MOVIE ==========");

            Movie original = new Movie
            {
                Title = "Original Movie",
                Description = "Original description",
                Director = new Director
                {
                    FirstName = "John",
                    LastName = "Smith",
                    BirthYear = 1970
                },
                Country = "USA",
                Genre = Genre.Drama,
                Year = 2020,
                Rating = 8
            };

            Movie copy = (Movie)original.Clone();

            copy.Title = "Copied Movie";
            copy.Director.FirstName = "Michael";

            Console.WriteLine("Original:");
            Console.WriteLine(original);

            Console.WriteLine("Copy:");
            Console.WriteLine(copy);

            Console.ReadKey();
        }
    }
}