using KironTest.Entities;
using KironTest.Models;
using KironTest.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace KironTest.Repositories.NavigationRepositories
{
    public class NavigationRepository
    {
        private readonly ApplicationDbContext _context;

        public NavigationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NavigationModel>> GetNavigationTreeAsync()
        {
            var navigations = await _context.Navigation.ToListAsync();

            if (!navigations.Any())
                return new List<NavigationModel>();

            var rootNavigations = navigations.Where(n => n.ParentID == -1).ToList();

            var result = new List<NavigationModel>();
            foreach (var root in rootNavigations)
            {
                result.Add(BuildHierarchy(root, navigations));
            }

            return result;
        }

        private NavigationModel BuildHierarchy(Navigation item, List<Navigation> allItems)
        {
            var node = new NavigationModel { Text = item.Text };
            foreach (var child in allItems.Where(c => c.ParentID == item.Id))
            {
                node.Children.Add(BuildHierarchy(child, allItems));
            }

            return node;
        }
    }
}
