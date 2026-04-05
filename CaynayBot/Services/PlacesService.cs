using CaynayBot.Models;
using CaynayBot.Repositories;

namespace CaynayBot.Services
{
    public interface IPlaceService
    {
        Task UpdatePlace(Place Place);
        Task AddPlace(Place Place);
        Task<IEnumerable<Place>> GetAllAsync();
        Task AddPlaces(IEnumerable<Place> Places);
        Task DeleteById(int id);
    }


    public class PlaceService : IPlaceService
    {
        private readonly IRepository<Place> _placeService;

        public PlaceService(IRepository<Place> userPlace)
        {
            _placeService = userPlace;
        }

        public async Task UpdatePlace(Place Place)
        {
            await Task.Run(() => _placeService.Update(Place));
            await _placeService.SaveChangesAsync();
        }
        public async Task AddPlace(Place Place)
        {
            await Task.Run(() => _placeService.AddAsync(Place));
            await _placeService.SaveChangesAsync();
        }

        public async Task<IEnumerable<Place>> GetAllAsync()
        {
            var result = await _placeService.GetAllAsync();
            return [.. result];
        }

        public async Task AddPlaces(IEnumerable<Place> resetPlaces)
        {
            await _placeService.AddRangeAsync(resetPlaces);
            await _placeService.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
        {
            var place = await _placeService.GetByIdAsync(id);
            if (place != null) 
            {
                _placeService.Delete(place);
                await _placeService.SaveChangesAsync();
            }
        }
    }
}