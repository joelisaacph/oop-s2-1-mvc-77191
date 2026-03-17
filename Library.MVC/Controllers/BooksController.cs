using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.MVC.Data;
using Library.Domain;

namespace Library.MVC.Controllers;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Books
    public async Task<IActionResult> Index(string searchString, string category)
    {
        // Compose the query
        var books = _context.Books.AsQueryable();

        // 1. Search by Title or Author
        if (!string.IsNullOrEmpty(searchString))
        {
            books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
        }

        // 2. Filter by Category
        if (!string.IsNullOrEmpty(category))
        {
            books = books.Where(b => b.Category == category);
        }

        // Get unique categories for the dropdown menu
        var categoryList = await _context.Books
            .Select(b => b.Category)
            .Distinct()
            .ToListAsync();

        ViewBag.Categories = categoryList;

        return View(await books.ToListAsync());
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);

        if (book == null) return NotFound();

        return View(book);
    }

    // Standard CRUD Actions (Create, Edit, Delete) should follow here...
}