﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facturacion.Filtro;
using Facturacion.Models;

namespace Facturacion.Controllers
{
    public class CarouselController : Controller
    {

        // GET: Carousel
        [AutorizarUsuario(idOperacion: 6)]
        public ActionResult Index()
        {
            using (DB_ImagenesEntities1 db = new DB_ImagenesEntities1())
            {
                var listaDeImagenes = db.Imagenes.ToList();
                return View(listaDeImagenes);
            }
        }

        // Renderizar vista subir imagen
        public ActionResult Create()
        {
            return View();
        }

        // Subir nueva imagen
        [HttpPost]
        public ActionResult CreateImage(Imagenes oImagen, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        byte[] imagenData = null;
                        using (var imagen = new BinaryReader(upload.InputStream))
                        {
                            imagenData = imagen.ReadBytes(upload.ContentLength);
                        }
                        oImagen.imagen = imagenData;
                    }

                    using (DB_ImagenesEntities1 db = new DB_ImagenesEntities1())
                    {
                        db.Imagenes.Add(oImagen);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View();
        }

        // Renderizar vista eliminar
        public ActionResult Delete(int? id)
        {
            using (DB_ImagenesEntities1 db = new DB_ImagenesEntities1())
            {
                Imagenes imagenes = db.Imagenes.Find(id);
                if (imagenes == null)
                {
                    return HttpNotFound();
                }

                return View(imagenes);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (DB_ImagenesEntities1 db = new DB_ImagenesEntities1())
            {
                Imagenes imagen = db.Imagenes.Find(id);
                db.Imagenes.Remove(imagen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }


        // Convertir de binario a imagen.
        public ActionResult convertirImagen(int id)
        {
            using (DB_ImagenesEntities1 db = new DB_ImagenesEntities1())
            {
                var imagen = (from i in db.Imagenes
                              where i.id == id
                              select i.imagen).FirstOrDefault();
                return File(imagen, "Imagenes/jpg");
            }
        }
    }
}