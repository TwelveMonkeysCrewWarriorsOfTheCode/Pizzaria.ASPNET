﻿using Microsoft.AspNetCore.Mvc;
using PizzariaASP.Models;
using PizzariaDAL;
using PizzariaDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzariaASP.Controllers
{
    public class PlatController : Controller
    {
        private readonly PizzariaContext _dc;

        public PlatController(PizzariaContext dc)
        {
            _dc = dc;
        }
        public IActionResult Index([FromQuery]int? filtre)
        {
            PlatCategorieModel model = new PlatCategorieModel
            {
                Filtre = filtre,
                Plats = _dc.Plats
                        .Where(p => p.CategorieId == filtre || filtre == null)
                        .Select(p => new PlatModel
                {
                    Id = p.Id,
                    Nom = p.Nom,
                    prix = p.Prix,
                    Image = p.Image,
                    CategorieNom = p.Categorie.Nom
                }),
                
                Categories = _dc.categories
                
                .Select(c => new CategorieModel
                {
                    Id = c.Id,
                    Nom = c.Nom
                })
            };
            return View(model);
        }

        public IActionResult Create()
        {
            PlatAddModel model = new PlatAddModel
            {
                Categories = _dc.categories.Select(c => new CategorieModel {
                    Id = c.Id,
                    Nom = c.Nom
                })
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(PlatAddModel form)
        {
            if (ModelState.IsValid)
            {
                Plat plat = new Plat {
                    Nom = form.Nom,
                    Prix = decimal.Parse(form.Prix.Replace('.', ',')),
                    Description = form.Description,
                    CategorieId = form.CategorieId
                };

                _dc.Plats.Add(plat);
                _dc.SaveChanges();
                TempData["success"] = "Enregistrement effectué!";
                // Rediriger vers la page d'index
                return RedirectToAction("Index");
            }
            return View(form);
        }

        public IActionResult Delete(int id)
        {
            // Récupérer le plat dont l'id est celui passé en paramètre
            Plat toDelete = _dc.Plats.Find(id);
            if (toDelete == null)
            {
                return NotFound();
            }
            _dc.Plats.Remove(toDelete);
            _dc.SaveChanges();
            TempData["success"] = $"Le plat {toDelete.Nom} a été supprimé!";
            return RedirectToAction("Index");
        }

        public IActionResult update(int id)
        {
            // Récupérer l'objet dont l'id est celui passé en paramètre
            Plat toUpdate = _dc.Plats.Find(id);
            PlatUpdateModel platModel = new PlatUpdateModel
            {
                Nom = toUpdate.Nom,
                Prix = toUpdate.Prix.ToString(),
                Description = toUpdate.Description,
                Image = toUpdate.Image
            };
            return View(platModel);
        }

        [HttpPost]
        public IActionResult Update(int id, PlatUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                Plat toUpdate = _dc.Plats.Find(id);
                toUpdate.Nom = model.Nom;
                toUpdate.Prix = decimal.Parse(model.Prix.Replace('.', ','));
                toUpdate.Description = model.Description;
                toUpdate.Image = model.Image;
                _dc.SaveChanges();
                TempData["success"] = $"La mise à jour a bien eu lieu!";
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
