using CaynayBot.Models;
using CaynayBot.Repositories;

namespace CaynayBot.Services
{
    public interface IBookingService
    {
        Task UpdateBook(Book Book);
        Task AddBook(Book Book);
        Task <IEnumerable<Book>> GetAllAsync();
        Task AddBooks(List<Book> resetBooks);
    }
    
    
    public class BookingService : IBookingService
    {
        private readonly IRepository<Book> _bookService;

        public BookingService(IRepository<Book> userBook)
        {
            _bookService = userBook;
        }

        public async Task UpdateBook(Book Book)
        {
            await Task.Run(() => _bookService.Update(Book));
            await _bookService.SaveChangesAsync();
        }
        public async Task AddBook(Book Book)
        {
            await Task.Run(() => _bookService.AddAsync(Book));
            await _bookService.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            var result = await _bookService.GetAllAsync();
            return [.. result];
        }

        public async Task AddBooks(List<Book> resetBooks)
        {
            await _bookService.AddRangeAsync(resetBooks);
            await _bookService.SaveChangesAsync();
        }
    }
}