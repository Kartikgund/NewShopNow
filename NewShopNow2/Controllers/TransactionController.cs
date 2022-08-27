using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopNowBL2.Models;
using NewShopNowBL2.Repository;
using NewShopNow2.ViewModel;
using Rotativa;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace NewShopNow2.Controllers
{
    public class TransactionController : Controller
    {
        TransactionRepo TR = new TransactionRepo();
        StockRepo SR = new StockRepo();
        CustomerRepo CR = new CustomerRepo();
        ErrorLogRepo ER = new ErrorLogRepo();
        // GET: Transaction
        public ActionResult BuyProduct()
        {
            return View();
        }
        public ActionResult SaveTransactions(string TotalQty, string TotalAmount, string PayMethod, string totalDisc, string totalgst, List<tblTransactionItem> TItems,string CustName,string CustMobile, string custEmail)
        {
            tblTransaction result = new tblTransaction();
            try
            {
                //save transaction
                tblTransaction objData = new tblTransaction();
                objData.TotalQty = Convert.ToInt32(TotalQty);
                objData.InvoiceAmount = Convert.ToDecimal(TotalAmount);
                objData.PaymentMethod = PayMethod;
                objData.InvoiceNo = TR.GenerateId();
                objData.GST = Convert.ToDecimal(totalgst);
                objData.TotalDiscount = Convert.ToDecimal(totalDisc);

                //
                tblCustomer objCust = new tblCustomer();
                objCust.CustomerName = CustName;
                objCust.MobileNo = CustMobile;


                List<tblTransactionItem> lstItems = TItems;

                result = TR.SaveTransactions(objData, lstItems, objCust);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string absoluteurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                ER.AddException(ex, absoluteurl);
                return View("Error");
            }
        

           

        }

        public ActionResult GetProductById(string Id)
        {
            tblStock stock = SR.FindProductById(Convert.ToInt32(Id));
            return Json(stock, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult BillPreview(string InvoiceNo, string hide, string receiver)
        {
            //InvoiceNo = "6cae7dcfd0e4607b";
            TransactionModel transactionModel = new TransactionModel();

            transactionModel.lstItems = TR.GetTransactionItemsByInvoiceNo(InvoiceNo);
            transactionModel.objTransaction = TR.GetTransactionByInvoiceNo(InvoiceNo);
            transactionModel.objCustmore = CR.getCustomerById(transactionModel.objTransaction.CustomerId);

            ViewBag.receiver = receiver;
            ViewBag.Hide = hide;

            return PartialView("_BillPreview", transactionModel);
        }

        public ActionResult PrintInvoice(string InvoiceNo, string receiver)
        {
            var a = new ViewAsPdf();
            a.ViewName = "_BillPreview";
            TransactionModel transactionModel = new TransactionModel();
            transactionModel.lstItems = TR.GetTransactionItemsByInvoiceNo(InvoiceNo);
            transactionModel.objTransaction = TR.GetTransactionByInvoiceNo(InvoiceNo);
            transactionModel.objCustmore = CR.getCustomerById(transactionModel.objTransaction.CustomerId);

            ViewBag.InvoiceId = InvoiceNo;
            ViewBag.Hide = 1;

            a.Model = transactionModel;
            var pdfBytes = a.BuildFile(this.ControllerContext);

            // Optionally save the PDF to server in a proper IIS location.
            var fileName = transactionModel.objCustmore.CustomerName + transactionModel.objTransaction.InvoiceNo + ".pdf";
            var path = Server.MapPath("~/Temp/" + fileName);
            //var path = "https://liacrm.com/CRM_Publish/Temp/" + fileName;
            System.IO.File.WriteAllBytes(path, pdfBytes);

            // return ActionResult
            MemoryStream ms = new MemoryStream(pdfBytes);


            //Email sending
            try
            {
                var senderEmail = new MailAddress("kartikgund2@gmail.com", "Kartik");
                var receiverEmail = new MailAddress(receiver, "Receiver");
                var password = "rgpljlpevjrezdpq";
                var sub = "Invoice For Your Purchase";
                var body = "Dear " + transactionModel.objCustmore.CustomerName + " Thank you for your purchase your Invoice is here, Visit Again!!";

                MailMessage message = new MailMessage();
                message.To.Add(receiver);// Email-ID of Receiver  
                message.Subject = sub;// Subject of Email  
                message.From = senderEmail;// Email-ID of Sender  
                message.IsBodyHtml = true;
                Attachment data = new Attachment(ms, fileName, "application/pdf");
                message.Attachments.Add(data);
                message.Body = body;
                SmtpClient SmtpMail = new SmtpClient();

                SmtpMail.Host = "smtp.gmail.com";
                SmtpMail.Port = 587;
                SmtpMail.EnableSsl = true;
                SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpMail.UseDefaultCredentials = false;
                SmtpMail.Credentials = new NetworkCredential(senderEmail.Address, password);
                SmtpMail.Send(message);
                return RedirectToAction("BuyProduct");
            }
            catch (Exception ex)
            {
                string absoluteurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                ER.AddException(ex, absoluteurl);
                return View("Error");
            }

            
        }

        /*public ActionResult Dummy()
        {
            string InvoiceNo = "6cae7dcfd0e4607b";
            TransactionModel transactionModel = new TransactionModel();

            transactionModel.lstItems = TR.GetTransactionItemsByInvoiceNo(InvoiceNo);
            transactionModel.objTransaction = TR.GetTransactionByInvoiceNo(InvoiceNo);
            transactionModel.objCustmore = CR.getCustomerById(transactionModel.objTransaction.CustomerId);

            ViewBag.receiver = "kartikgund2@gmail.com";
            ViewBag.Hide = null;

            return View("DummyView", transactionModel);
        }*/
    }
}
