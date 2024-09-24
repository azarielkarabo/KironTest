namespace KironTest.Entities
{
    public class RegionHoliday
    {
        public int Id { get; set; }
       
        public int RegionId { get; set; }
      
        public int HolidayId { get; set; }

        public Region Region { get; set; }

        public Holiday Holiday { get; set; }
    }

}
