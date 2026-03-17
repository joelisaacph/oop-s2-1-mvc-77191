using Bogus;
using Library.Domain;
using Library.MVC.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Library.MVC.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Books.Any()) return;

        // 1. Seed Books
        var categories = new[] { "Fiction", "Sci-Fi", "History", "Kids", "Tech" };
        var bookFaker = new Faker<Book>()
            .RuleFor(b => b.Title, f => f.Commerce.ProductName())
            .RuleFor(b => b.Author, f => f.Name.FullName())
            .RuleFor(b => b.Isbn, f => f.Random.Replace("###-##########"))
            .RuleFor(b => b.Category, f => f.PickRandom(categories))
            .RuleFor(b => b.IsAvailable, true);

        var books = bookFaker.Generate(20);
        context.Books.AddRange(books);

        // 2. Seed Members
        var memberFaker = new Faker<Member>()
            .RuleFor(m => m.FullName, f => f.Name.FullName())
            .RuleFor(m => m.Email, f => f.Internet.Email())
            .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber());

        var members = memberFaker.Generate(10);
        context.Members.AddRange(members);

        context.SaveChanges();

        // 3. Seed Loans
        var random = new Random();
        for (int i = 0; i < 15; i++)
        {
            var book = books[i];
            var member = members[random.Next(0, 10)];

            var loan = new Loan
            {
                BookId = book.Id,
                MemberId = member.Id,
                LoanDate = DateTime.Now.AddDays(-random.Next(5, 20)),
                DueDate = DateTime.Now.AddDays(random.Next(-5, 5))
            };

            // Mix of returned and active loans
            if (i < 8)
            {
                loan.ReturnedDate = DateTime.Now;
                book.IsAvailable = true;
            }
            else
            {
                book.IsAvailable = false;
            }
            context.Loans.Add(loan);
        }
        context.SaveChanges();
    }
}