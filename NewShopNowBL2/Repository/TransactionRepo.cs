using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewShopNowBL2.Models;
using System.Data.Entity.Migrations;
using System.Data.Entity;

namespace NewShopNowBL2.Repository
{
    public class TransactionRepo
    {
        StockRepo SR = new StockRepo();
        CustomerRepo custRepo = new CustomerRepo();



        public string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

       
        public tblTransaction SaveTransactions(tblTransaction transaction, List<tblTransactionItem> TItems, tblCustomer objCust)
        {
            // bool Result = false;
            using (DPTContext context = new DPTContext())
            {
                using (DbContextTransaction trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var CResult = custRepo.getCustomerByMobileNo(objCust.MobileNo);
                        if (CResult == null)
                        {
                            objCust.CreatedDate = DateTime.Now;
                            objCust.CreatedBy = 3;
                            context.tblCustomers.AddOrUpdate(objCust);
                            context.SaveChanges();
                            transaction.CustomerId = objCust.Id;
                        }
                        else
                        {
                            transaction.CustomerId = CResult.Id;
                        }

                        // user.RoleId = 2;
                        transaction.InvoiceDate = DateTime.Now;
                        transaction.CreatedDate = DateTime.Now;
                        transaction.CreatedBy = 4;
                        context.tblTransactions.AddOrUpdate(transaction);
                        context.SaveChanges();

                        //save transaction item
                        foreach (tblTransactionItem T in TItems)
                        {
                            tblTransactionItem objItem = new tblTransactionItem();
                            objItem.InvoiceId = transaction.InvoiceNo;
                            objItem.ProductId = T.ProductId;
                            objItem.Price = T.Price;
                            objItem.Qty = T.Qty;
                            context.tblTransactionItems.AddOrUpdate(objItem);
                            context.SaveChanges();

                            //update stock
                            tblStock stock = SR.FindProductById(objItem.ProductId);
                            stock.ProductQty -= Convert.ToInt32(objItem.Qty);
                            SR.saveProduct(stock);

                        }
                        trans.Commit();
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        trans.Rollback();
                        throw ex;
                    }
                }
                
                return transaction;
            }
        }
        public List<tblTransactionItem> GetTransactionItemsByInvoiceNo(string invoiceNo)
        {
            List<tblTransactionItem> lstItem = new List<tblTransactionItem>();
            using (DPTContext context = new DPTContext())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                lstItem = context.tblTransactionItems.Where(x => x.InvoiceId == invoiceNo).ToList();
                foreach (tblTransactionItem item in lstItem)
                {
                    item.tblStock = context.tblStocks.Where(y => y.Id == item.ProductId).FirstOrDefault();
                }
            }
            return lstItem;
        }

        public tblTransaction GetTransactionByInvoiceNo(string invoiceNo)
        {
            tblTransaction objData = new tblTransaction();
            using (DPTContext context = new DPTContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                objData = context.tblTransactions.Where(x => x.InvoiceNo == invoiceNo).FirstOrDefault();

            }
            return objData;
        }
    }
}

