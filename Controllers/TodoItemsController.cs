using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodoList.Models;

namespace TodoList.Controllers
{
    public class TodoItemsController : Controller
    {
        private readonly AppDbContext _context;

        public TodoItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TodoItems
        public async Task<IActionResult> Index(string categoryFilter, Priority? priorityFilter, string sortOrder)
        {
            ViewData["CategoryFilter"] = categoryFilter;
            ViewData["PriorityFilter"] = priorityFilter;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PrioritySortParam"] = sortOrder == "priority_desc" ? "priority_asc" : "priority_desc";

            var items = _context.ToDoItems.AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                items = items.Where(t => t.Category == categoryFilter);
            }

            if (priorityFilter.HasValue)
            {
                items = items.Where(t => t.Priority == priorityFilter.Value);
            }

            // Sorting
            items = sortOrder switch
            {
                "priority_desc" => items.OrderByDescending(t => (int) t.Priority),
                "priority_asc" => items.OrderBy(t => (int) t.Priority),
                _ => items.OrderBy(t => t.Title)
            };

            // Get distinct categories and priorities for dropdowns
            ViewBag.Categories = await _context.ToDoItems.Select(t => t.Category).Distinct().ToListAsync();
            ViewBag.Priorities = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToList();
            Console.WriteLine($"PriorityFilter: {ViewData["PriorityFilter"]}");
            Console.WriteLine($"CategoryFilter: {ViewData["CategoryFilter"]}");
            return View(await items.ToListAsync());

        }

        // GET: TodoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.ToDoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // GET: TodoItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TodoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IsCompleted,Category,Priority")] TodoItem todoItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                // Toast Message
                TempData["ToastMessage"] = "Task created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }

        // GET: TodoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.ToDoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }
            return View(todoItem);
        }

        // POST: TodoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IsCompleted,Category,Priority")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    TempData["ToastMessage"] = "Task edited successfully";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }

        // GET: TodoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.ToDoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // POST: TodoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.ToDoItems.FindAsync(id);
            if (todoItem != null)
            {
                _context.ToDoItems.Remove(todoItem);
                TempData["ToastMessage"] = " Task deleted successfully";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }

        [HttpPost]
        // Inserting Ajax Script
        public async Task<IActionResult> ToggleCompleteAjax([FromBody] int id)
        {
            var item = await _context.ToDoItems.FindAsync(id);
            if (item == null)
            {
                return Json(new { success = false });
            }

            item.IsCompleted = !item.IsCompleted;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isCompleted = item.IsCompleted });
        }
    }
}
