namespace KironTest.Models
{
    public class NavigationModel
    {
        public string Text { get; set; }
        public List<NavigationModel> Children { get; set; } = new List<NavigationModel>();
    }
}
