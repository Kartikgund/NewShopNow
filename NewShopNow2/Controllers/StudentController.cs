using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using NewShopNowBL2.Models;

namespace NewShopNow2.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult ListStudent()
        {
            List<tblStudent> lstProject = new List<tblStudent>();
            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("~/XML/StudentList.xml"));
            DataView dvPrograms;
            dvPrograms = ds.Tables[0].DefaultView;
            dvPrograms.Sort = "ID";
            foreach (DataRowView dr in dvPrograms)
            {
                tblStudent model = new tblStudent();
                model.Id = Convert.ToInt32(dr[0]);
                model.StudentName = Convert.ToString(dr[1]);
                model.Course = Convert.ToString(dr[2]);
                model.College = Convert.ToString(dr[3]);
                lstProject.Add(model);
            }
            if (lstProject.Count > 0)
            {
                return View(lstProject);
            }
            return View();
        }

        public ActionResult AddStudent()
        {
            tblStudent objStudent = new tblStudent();
            return View(objStudent);
        }

        [HttpPost]
        public ActionResult AddEditStudent(tblStudent mdl)
        {
            if (mdl.Id > 0)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XML/StudentList.xml"));
                var items = (from item in xmlDoc.Descendants("tblStudent") select item).ToList();
                XElement selected = items.Where(p => p.Element("ID").Value == mdl.Id.ToString()).FirstOrDefault();
                selected.Remove();
                xmlDoc.Save(Server.MapPath("~/XML/StudentList.xml"));
                xmlDoc.Element("StudentDetails").Add(new XElement("tblStudent", new XElement("ID", mdl.Id), new XElement("Name", mdl.StudentName), new XElement("Course", mdl.Course), new XElement("College", mdl.College)));
                xmlDoc.Save(Server.MapPath("~/XML/StudentList.xml"));
                return RedirectToAction("ListStudent", "Student");
            }
            else
            {
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.Load(Server.MapPath("~/XML/StudentList.xml"));
                XmlNodeList nodelist = oXmlDocument.GetElementsByTagName("tblStudent");
                var x = oXmlDocument.GetElementsByTagName("ID");
                int Max = 0;
                foreach (XmlElement item in x)
                {
                    int EId = Convert.ToInt32(item.InnerText.ToString());
                    if (EId > Max)
                    {
                        Max = EId;
                    }
                }
                Max = Max + 1;
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XML/StudentList.xml"));
                xmlDoc.Element("StudentDetails").Add(new XElement("tblStudent", new XElement("ID", Max), new XElement("Name", mdl.StudentName), new XElement("Course", mdl.Course), new XElement("College", mdl.College)));
                xmlDoc.Save(Server.MapPath("~/XML/StudentList.xml"));
                return RedirectToAction("ListStudent", "Student");
            }
        }

        public ActionResult GetDetailsById(int Id)
        {
            tblStudent objStudent = new tblStudent();
            XDocument oXmlDocument = XDocument.Load(Server.MapPath("~/XML/StudentList.xml"));
            var items = (from item in oXmlDocument.Descendants("tblStudent")
                         where Convert.ToInt32(item.Element("ID").Value) == Id
                         select new tblStudent
                         {
                             Id = Convert.ToInt32(item.Element("ID").Value),
                             StudentName = item.Element("Name").Value,
                             Course = item.Element("Course").Value,
                             College = item.Element("College").Value,
                         }).SingleOrDefault();
            if (items != null)
            {
                objStudent.Id = items.Id;
                objStudent.StudentName = items.StudentName;
                objStudent.Course = items.Course;
                objStudent.College = items.College;
            }

            return View("AddStudent", objStudent);
        }
        public ActionResult Delete(int Id)
        {
            if (Id > 0)
            {
                XDocument xmlDoc = XDocument.Load(Server.MapPath("~/XML/StudentList.xml"));
                var items = (from item in xmlDoc.Descendants("tblStudent") select item).ToList();
                XElement selected = items.Where(p => p.Element("ID").Value == Id.ToString()).FirstOrDefault();
                selected.Remove();
                xmlDoc.Save(Server.MapPath("~/XML/StudentList.xml"));
            }
            return RedirectToAction("ListStudent", "Student");
        }

    }   
}