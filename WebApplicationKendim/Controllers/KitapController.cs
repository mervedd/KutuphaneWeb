using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationKendim.Models;
using WebApplicationKendim.Models.ViewModels;
using WebApplicationKendim.Utility;

namespace WebApplicationKendim.Controllers
{
    public class KitapController : Controller
    {
        private readonly IKiralamaRepository _kiralamaRepository;
        private readonly IKitapRepository _kitapRepository;
        private readonly IKitapTurleriRepository _kitapTurleriRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public KitapController(
            IKitapRepository kitapRepository,
            IKitapTurleriRepository kitapTurleriRepository,
            IWebHostEnvironment webHostEnvironment,
            IKiralamaRepository kiralamaRepository)
        {
            _kitapRepository = kitapRepository;
            _kitapTurleriRepository = kitapTurleriRepository;
            _webHostEnvironment = webHostEnvironment;
            _kiralamaRepository = kiralamaRepository;
        }

        [Authorize(Roles = "Admin, Ogrenci")]
        public IActionResult Index()
        {
            var kitapListesi = _kitapRepository.GetAll(includeProps: "Kitapturleri").ToList();
            return View(kitapListesi);
        }

        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult EkleGuncelle(int? id)
        {
            var kitapTuruList = _kitapTurleriRepository.GetAll().Select(k => new SelectListItem
            {
                Text = k.Ad,
                Value = k.Id.ToString()
            });

            ViewBag.KitapTuruList = kitapTuruList;

            if (id == null || id == 0)
            {
                return View(new Kitap());
            }

            var kitap = _kitapRepository.Get(u => u.Id == id);
            if (kitap == null)
            {
                return NotFound();
            }

            return View(kitap);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult EkleGuncelle(Kitap kitap, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string kitapPath = Path.Combine(_webHostEnvironment.WebRootPath, "img");

                if (file != null)
                {
                    string filePath = Path.Combine(kitapPath, file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    kitap.ResimUrl = $"/img/{file.FileName}";
                }

                if (kitap.Id == 0)
                {
                    _kitapRepository.Ekle(kitap);
                    TempData["basarili"] = "Yeni Kitap Başarıyla Eklendi.";
                }
                else
                {
                    _kitapRepository.Guncelle(kitap);
                    TempData["basarili"] = "Kitap Başarıyla Güncellendi.";
                }

                _kitapRepository.Kaydet();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.KitapTuruList = _kitapTurleriRepository.GetAll().Select(k => new SelectListItem
            {
                Text = k.Ad,
                Value = k.Id.ToString()
            });

            return View(kitap);
        }

        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult Sil(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var kitap = _kitapRepository.Get(u => u.Id == id);
            if (kitap == null)
            {
                return NotFound();
            }

            return View(kitap);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult SilPOST(int? id)
        {
            var kitap = _kitapRepository.Get(u => u.Id == id);
            if (kitap != null)
            {
                _kitapRepository.Sil(kitap);
                _kitapRepository.Kaydet();
                TempData["basarili"] = "Kitap Başarıyla Silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = UserRoles.Role_Ogrenci)]
        [HttpPost]
        public IActionResult Sepet(int ogrenciId, int kitapId)
        {
            var kitap = _kitapRepository.Get(u => u.Id == kitapId);
            if (kitap == null)
            {
                return NotFound();
            }

            var kiralamaKaydi = new Kiralama
            {
                OgrenciId = ogrenciId,
                KitapId = kitapId,
                Kitap = kitap
            };

            _kiralamaRepository.Ekle(kiralamaKaydi);
            _kiralamaRepository.Kaydet();

            // Fetch or initialize the cart from TempData
            var sepet = TempData.Get<SepetViewModel>("Sepet") ?? new SepetViewModel();
            sepet.OgrenciId = ogrenciId;
            sepet.Kitaplar.Add(kitap);

            // Save the cart back to TempData
            TempData.Put("Sepet", sepet);

            TempData["basarili"] = "Kitap başarıyla sepete eklendi.";
            return RedirectToAction(nameof(Sepet));
        }

        [Authorize(Roles = UserRoles.Role_Ogrenci)]
        [HttpGet]
        public IActionResult Sepet()
        {
            var sepet = TempData.Get<SepetViewModel>("Sepet") ?? new SepetViewModel();
            return View(sepet);
        }
    }
}
