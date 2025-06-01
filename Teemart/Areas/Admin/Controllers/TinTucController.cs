using Nhom9.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Nhom9.Areas.Admin.Controllers
{
    public class TinTucController : Controller
    {
        private Nhom9DB db = new Nhom9DB();

        // GET: Admin/TinTuc
        public ActionResult Index()
        {
            return View(db.TinTucs.OrderByDescending(t => t.NgayDang).ToList());
        }

        // GET: Admin/TinTuc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
                return HttpNotFound();

            return View(tinTuc);
        }

        // GET: Admin/TinTuc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/TinTuc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TinTuc model, HttpPostedFileBase HinhAnhFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Gán ngày đăng
                    model.NgayDang = DateTime.Now;

                    // Xử lý upload hình ảnh
                    if (HinhAnhFile != null && HinhAnhFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(HinhAnhFile.FileName);
                        string extension = Path.GetExtension(HinhAnhFile.FileName);

                        // Tạo tên file tránh trùng
                        fileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

                        // Thư mục lưu ảnh
                        string folderPath = Server.MapPath("~/Images/TinTuc/");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Đường dẫn vật lý lưu file
                        string filePath = Path.Combine(folderPath, fileName);
                        HinhAnhFile.SaveAs(filePath);

                        // Lưu đường dẫn tương đối vào model, **bỏ dấu / đầu tiên**
                        // Ví dụ: "Images/TinTuc/filename.jpg"
                        model.HinhAnh = "Images/TinTuc/" + fileName;
                    }
                    else
                    {
                        // Nếu không upload hình thì có thể để null hoặc mặc định
                        model.HinhAnh = null;
                    }

                    db.TinTucs.Add(model);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm tin tức: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "ModelState không hợp lệ.");
            }

            return View(model);
        }



        // GET: Admin/TinTuc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
                return HttpNotFound();

            return View(tinTuc);
        }

        // POST: Admin/TinTuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTin,TieuDe,NoiDung,HinhAnh,NgayDang,MoTaNgan,IsActive")] TinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tinTuc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tinTuc);
        }

        // GET: Admin/TinTuc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
                return HttpNotFound();

            return View(tinTuc);
        }

        // POST: Admin/TinTuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TinTuc tinTuc = db.TinTucs.Find(id);
            db.TinTucs.Remove(tinTuc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
