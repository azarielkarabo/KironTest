using System.ComponentModel.DataAnnotations;

namespace KironTest.Entities
{
    public class Holiday
    {
        public int Id { get; set; }
      
        [Required]
        public string Title { get; set; }
       
        [Required]
        public DateTime Date { get; set; }
       
        public string Notes { get; set; }
       
        public bool Bunting { get; set; }

        public ICollection<RegionHoliday> RegionHolidays { get; set; }
    }

}
