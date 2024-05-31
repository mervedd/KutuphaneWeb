namespace WebApplicationKendim.Models.ViewModels
{
    public class SepetViewModel
    {
        public int OgrenciId { get; set; }
        public List<Kitap> Kitaplar { get; set; }

        public SepetViewModel()
        {
            Kitaplar = new List<Kitap>();
        }

    }
}
