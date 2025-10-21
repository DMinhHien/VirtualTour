using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualTour.BL.Repositories;
using VirtualTour.Model;

namespace VirtualTour.BL.Services
{
    public interface ISectionService
    {
        Task<List<SectionModel>> GetAllSectionsAsync();
        Task<SectionModel> GetSectionByIdAsync(int id);
        Task CreateSectionAsync(SectionModel section);
        Task UpdateSectionAsync(SectionModel section);
        Task DeleteSectionAsync(int id);
    }
    public class SectionService: ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        public SectionService(ISectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }
        public async Task<List<SectionModel>> GetAllSectionsAsync()
        {
            return await _sectionRepository.GetAllSectionsAsync();
        }
        public async Task<SectionModel> GetSectionByIdAsync(int id)
        {
            return await _sectionRepository.GetSectionByIdAsync(id);
        }
        public async Task CreateSectionAsync(SectionModel section)
        {
            await _sectionRepository.CreateSectionAsync(section);
        }
        public async Task UpdateSectionAsync(SectionModel section)
        {
            await _sectionRepository.UpdateSectionAsync(section);
        }
        public async Task DeleteSectionAsync(int id)
        {
            await _sectionRepository.DeleteSectionAsync(id);
        }
    }
}
