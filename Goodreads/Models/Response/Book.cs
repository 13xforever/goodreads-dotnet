﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Goodreads.Extensions;

namespace Goodreads.Models.Response
{
    /// <summary>
    /// This class models a single book as defined by the Goodreads API.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Book : ApiResponse
    {
        /// <summary>
        /// The Goodreads Id for this book.
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// The title of this book.
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// The description of this book.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// The ISBN of this book.
        /// </summary>
        public string Isbn { get; protected set; }

        /// <summary>
        /// The ISBN13 of this book.
        /// </summary>
        public string Isbn13 { get; protected set; }

        /// <summary>
        /// The ASIN of this book.
        /// </summary>
        public string Asin { get; protected set; }

        /// <summary>
        /// The Kindle ASIN of this book.
        /// </summary>
        public string KindleAsin { get; protected set; }

        /// <summary>
        /// The marketplace Id of this book.
        /// </summary>
        public string MarketplaceId { get; protected set; }

        /// <summary>
        /// The country code of this book.
        /// </summary>
        public string CountryCode { get; protected set; }

        /// <summary>
        /// The cover image for this book.
        /// </summary>
        public string ImageUrl { get; protected set; }

        /// <summary>
        /// The small cover image for this book.
        /// </summary>
        public string SmallImageUrl { get; protected set; }

        /// <summary>
        /// The date this book was published.
        /// </summary>
        public DateTime? PublicationDate { get; protected set; }

        /// <summary>
        /// The publisher of this book.
        /// </summary>
        public string Publisher { get; protected set; }

        /// <summary>
        /// The language code of this book.
        /// </summary>
        public string LanguageCode { get; protected set; }

        /// <summary>
        /// Signifies if this is an eBook or not.
        /// </summary>
        public bool IsEbook { get; protected set; }

        /// <summary>
        /// The average rating of this book by Goodreads users.
        /// </summary>
        public decimal AverageRating { get; protected set; }

        /// <summary>
        /// The number of pages in this book.
        /// </summary>
        public int Pages { get; protected set; }

        /// <summary>
        /// The format of this book.
        /// </summary>
        public string Format { get; protected set; }

        /// <summary>
        /// Brief information about this edition of the book.
        /// </summary>
        public string EditionInformation { get; protected set; }

        /// <summary>
        /// The count of all Goodreads ratings for this book.
        /// </summary>
        public int RatingsCount { get; protected set; }

        /// <summary>
        /// The count of all reviews that contain text for this book.
        /// </summary>
        public int TextReviewsCount { get; protected set; }

        /// <summary>
        /// The Goodreads Url for this book.
        /// </summary>
        public string Url { get; protected set; }

        /// <summary>
        /// The aggregate information for this work across all editions of the book.
        /// </summary>
        public Work Work { get; protected set; }

        /// <summary>
        /// The list of authors that worked on this book.
        /// </summary>
        public List<AuthorSummary> Authors { get; protected set; }

        /// <summary>
        /// The most popular shelf names this book appears on. This is a
        /// dictionary of shelf name -> count.
        /// </summary>
        public Dictionary<string, int> PopularShelves { get; protected set; }

        /// <summary>
        /// The list of book links tracked by Goodreads.
        /// This is usually a list of libraries that the user can borrow the book from.
        /// </summary>
        public List<BookLink> BookLinks { get; protected set; }

        /// <summary>
        /// The list of buy links tracked by Goodreads.
        /// This is usually a list of third-party sites that the
        /// user can purchase the book from.
        /// </summary>
        public List<BookLink> BuyLinks { get; protected set; }

        /// <summary>
        /// Summary information about similar books to this one.
        /// </summary>
        public List<BookSummary> SimilarBooks { get; protected set; }

        internal string DebuggerDisplay
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "Book: Id: {0}, Title: {1}",
                    Id,
                    Title);
            }
        }

        internal override void Parse(XElement element)
        {
            Id = element.ElementAsInt("id");
            Title = element.ElementAsString("title");
            Isbn = element.ElementAsString("isbn");
            Isbn13 = element.ElementAsString("isbn13");
            Asin = element.ElementAsString("asin");
            KindleAsin = element.ElementAsString("kindle_asin");
            MarketplaceId = element.ElementAsString("marketplace_id");
            CountryCode = element.ElementAsString("country_code");
            ImageUrl = element.ElementAsString("image_url");
            SmallImageUrl = element.ElementAsString("small_image_url");
            PublicationDate = element.ElementAsGoodreadsDate("publication");
            Publisher = element.ElementAsString("publisher");
            LanguageCode = element.ElementAsString("language_code");
            IsEbook = element.ElementAsBool("is_ebook");
            Description = element.ElementAsString("description");
            AverageRating = element.ElementAsDecimal("average_rating");
            Pages = element.ElementAsInt("num_pages");
            Format = element.ElementAsString("format");
            EditionInformation = element.ElementAsString("edition_information");
            RatingsCount = element.ElementAsInt("ratings_count");
            TextReviewsCount = element.ElementAsInt("text_reviews_count");
            Url = element.ElementAsString("url");

            // Initialize and parse the work information
            Work = new Work();
            Work.Parse(element.Element("work"));

            // Parse out the authors of this book
            var authorsRoot = element.Element("authors");
            if (authorsRoot != null)
            {
                var authorElements = authorsRoot.Descendants("author");
                if (authorElements != null && authorElements.Count() > 0)
                {
                    Authors = new List<AuthorSummary>();

                    foreach (var authorElement in authorElements)
                    {
                        var authorSummary = new AuthorSummary();
                        authorSummary.Parse(authorElement);
                        Authors.Add(authorSummary);
                    }
                }
            }

            // Parse out the popular shelves this book appears on
            var popularShelvesElement = element.Element("popular_shelves");
            if (popularShelvesElement != null)
            {
                var shelfElements = popularShelvesElement.Descendants("shelf");
                if (shelfElements != null && shelfElements.Count() > 0)
                {
                    PopularShelves = new Dictionary<string, int>();

                    foreach (var shelfElement in shelfElements)
                    {
                        var shelfName = shelfElement.Attribute("name").Value;
                        var shelfCountValue = shelfElement.Attribute("count").Value;

                        int shelfCount = 0;
                        int.TryParse(shelfCountValue, out shelfCount);

                        PopularShelves.Add(shelfName, shelfCount);
                    }
                }
            }

            // Parse out the book links
            var bookLinksElement = element.Element("book_links");
            if (bookLinksElement != null)
            {
                var bookLinkElements = bookLinksElement.Descendants("book_link");
                if (bookLinkElements != null && bookLinkElements.Count() > 0)
                {
                    BookLinks = new List<BookLink>();

                    foreach (var bookLinkElement in bookLinkElements)
                    {
                        var bookLink = new BookLink();
                        bookLink.Parse(bookLinkElement);
                        bookLink.FixBookLink(Id);
                        BookLinks.Add(bookLink);
                    }
                }
            }

            // Parse out the buy links
            var buyLinksElement = element.Element("buy_links");
            if (buyLinksElement != null)
            {
                var buyLinkElements = buyLinksElement.Descendants("buy_link");
                if (buyLinkElements != null && buyLinkElements.Count() > 0)
                {
                    BuyLinks = new List<BookLink>();

                    foreach (var buyLinkElement in buyLinkElements)
                    {
                        var buyLink = new BookLink();
                        buyLink.Parse(buyLinkElement);
                        buyLink.FixBookLink(Id);
                        BuyLinks.Add(buyLink);
                    }
                }
            }

            // Parse out the similar books
            var similarBooksElement = element.Element("similar_books");
            if (similarBooksElement != null)
            {
                var bookElements = element.Descendants("book");
                if (bookElements != null && bookElements.Count() > 0)
                {
                    SimilarBooks = new List<BookSummary>();

                    foreach (var bookElement in bookElements)
                    {
                        var bookSummary = new BookSummary();
                        bookSummary.Parse(bookElement);
                        SimilarBooks.Add(bookSummary);
                    }
                }
            }
        }
    }
}
