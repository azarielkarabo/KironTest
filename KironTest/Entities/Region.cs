using System.ComponentModel.DataAnnotations;

namespace KironTest.Entities
{
    public class Region
    {
        public int Id { get; set; }
       
        [Required]
        public string Name { get; set; }

        public ICollection<RegionHoliday> RegionHolidays { get; set; }
    }

}
