using Microsoft.AspNetCore.Mvc;
using WebApplicationKendim.Utility;
using System.Collections.Generic;
using WebApplicationKendim.Models;

public class SepetController : Controller
{
    public IActionResult Index()
    {
        var sepet = TempData["Sepet"] as List<Kitap>;
        return View(sepet);
    }

    [HttpPost]
    public IActionResult KitapEkle(int id, string KitapAdi,string tanim, string yazar,int KitapTuruId)
    {
        var sepet = TempData["Sepet"] as List<Kitap> ?? new List<Kitap>();

        sepet.Add(new Kitap { Id = id, KitapAdi = KitapAdi, Yazar = yazar,Tanim=tanim,KitapTuruId=KitapTuruId  });

        TempData["Sepet"] = sepet;
        TempData["basarili"] = "Kitap sepete eklendi";

        return RedirectToAction("Kitap");
    }
}
