
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class ChartOfAccountTemplate : ERPNodeDalRepository
    {
        public ChartOfAccountTemplate(Organization organization) : base(organization)
        {

        }

        private void CreateAsset()
        {
            var CashEquivalentGroup = organization.ChartOfAccount.newFolder(AccountTypes.Asset, AccountSubTypes.Cash, "CA", "เงินสด หรือ เทียบเท่า", "Cash");
            CashEquivalentGroup.AddChild(organization.ChartOfAccount.newItem(AccountTypes.Asset, AccountSubTypes.Cash, "1", "เงินสด", "Cash"));
            organization.ChartOfAccount.newItem(AccountTypes.Asset, AccountSubTypes.AccountReceivable, "1", "ลูกหนี้การค้า", "Account Receivable");
            organization.ChartOfAccount.newItem(AccountTypes.Asset, AccountSubTypes.EarnestPayment, "2", "เงินมัดจำจ่าย", "Paid Deposit");
            organization.ChartOfAccount.newItem(AccountTypes.Asset, AccountSubTypes.Inventory, "3", "สินค้าคงคลัง", "Inventory");
            erpNodeDBContext.SaveChanges();
        }
        private void CreateEquity()
        {
            organization.ChartOfAccount.newItem(AccountTypes.Capital, AccountSubTypes.Stock, "311000", "ทุนที่ชำระแล้ว", "Fully Capital");
            organization.ChartOfAccount.newItem(AccountTypes.Capital, AccountSubTypes.OverStockValue, "312000", "ส่วนเกินทุน", "Share Premium");
            organization.ChartOfAccount.newItem(AccountTypes.Capital, AccountSubTypes.OverStockValue, "323000", "กำไรสะสม", "Retained Earnings");
            organization.ChartOfAccount.newItem(AccountTypes.Capital, AccountSubTypes.OpeningBalance, "340400", "บัญชีสมดุลเปิด", "OpeningBalance");

            erpNodeDBContext.SaveChanges();
        }
        private void CreateExpense()
        {
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.CostOfGoodsSold, "1", "ต้นทุนสินค้าเพื่อขาย", "");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "2", "ซื้อ", "Puechases");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "3", "ค่าขนส่งเข้า", "Freight");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "4", "ส่งคืน", "Purchases Return");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.DiscountGiven, "5", "ส่วนลดจ่าย", "DiscountGiven");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "6", "เงินเดือน", "Salaries");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "7", "ค่าตอบแทนกรรมการ", "Board Members' Compensation");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "8", "ค่าไฟฟ้า", "Electric Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "9", "ค่าน้ำประปา", "Water Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "10", "ค่าโทรศัพท์และติดต่อสื่อสาร", "Telephone and Communication Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "11", "ค่าใช้จ่ายต้องห้าม", "Prohibit Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "12", "ค่าขนส่ง", "Delivery Fees");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "13", "ค่าเช่า", "Rental");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "14", "ค่าซ่อมแซม", "Mantenance and Repair Expanse");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "15", "ค่าส่งเสริมการขาย", "Promotion");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "16", "ค่ารับรอง", "Meals and Entertainment Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "17", "ค่านายหน้า", "Commission Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "18", "ค่าโฆษณา", "Advertising");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "19", "ค่าอากรแสตมป์", "Duty Stamp");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "20", "ค่าภาษีอากรอื่น ๆ", "Other Tax");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "21", "ค่าทำบัญชี", "Accounting Service Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "22", "ค่าสอบบัญชี", "Auditing Fee");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "23", "รายจ่ายเพื่อสนับสนุนการศึกษา", "Donation for Education");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.BankFee, "24", "ค่าธรรมเนียมธนาคาร", "Bank Chargs");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "25", "ค่าธรรมเนียมอื่น", "Other Fees");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "26", "ค่าใช้จ่ายเบ็ดเตล็ด", "Miscellaneous Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "27", "ภาษีซื้อไม่ขอคืน", "Unused Input Tax"); organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "533000", "ดอกเบี้ยจ่าย", "Interest Expense");
            organization.ChartOfAccount.newItem(AccountTypes.Expense, AccountSubTypes.Expense, "28", "ภาษีเงินได้นิติบุคคล", "Income Tax");
            erpNodeDBContext.SaveChanges();
        }
        private void CreateIncome()
        {
            organization.ChartOfAccount.newItem(AccountTypes.Income, AccountSubTypes.Income, "1", "รายได้จากการขาย", "");
            organization.ChartOfAccount.newItem(AccountTypes.Income, AccountSubTypes.Income, "2", "รายได้จากการให้บริการ", "");
            organization.ChartOfAccount.newItem(AccountTypes.Income, AccountSubTypes.DiscountTaken, "3", "ส่วนลดรับ", "Discount Taken");
            organization.ChartOfAccount.newItem(AccountTypes.Income, AccountSubTypes.OverStockValue, "4", "รับคืน", "Purchase Retuens");
            organization.ChartOfAccount.newItem(AccountTypes.Income, AccountSubTypes.Interest, "5", "ดอกเบี้ยรับ", "Interest Income");
            organization.ChartOfAccount.newItem(AccountTypes.Income, AccountSubTypes.InvestmentRevenue, "6", "เงินปันผลและส่วนแบ่งกำไร", "Investment Income");
            erpNodeDBContext.SaveChanges();
        }
        private void CreateLiability()
        {
            organization.ChartOfAccount.newItem(AccountTypes.Liability, AccountSubTypes.AccountPayable, "1", "เจ้าหนี้การค้า", "Accounts Payable");
            organization.ChartOfAccount.newItem(AccountTypes.Liability, AccountSubTypes.CurrentLiability, "2", "เงินกู้ยืม", "Loans");
            organization.ChartOfAccount.newItem(AccountTypes.Liability, AccountSubTypes.CurrentLiability, "3", "ภาษีเงินได้นิติบุคคลค้างจ่าย", "Accrued Income Tax");
            organization.ChartOfAccount.newItem(AccountTypes.Liability, AccountSubTypes.EarnestReceive, "4", "เงินมัดจำรับ", "Receive Deposit");
            organization.ChartOfAccount.newItem(AccountTypes.Liability, AccountSubTypes.OverReceivePayment, "4", "เงินรับชำระเกิน", "Over Receive Payment");
            erpNodeDBContext.SaveChanges();
        }
        public void CreateAccounts()
        {
            Console.WriteLine("> Create Default Account ");
            bool isEmpty = organization.DataItems.Get(Models.Datum.DataItemKey.DefaultAccountCreated) != "1";
            if (isEmpty)
            {
                this.CreateAsset();
                this.CreateLiability();
                this.CreateEquity();
                this.CreateIncome();
                this.CreateExpense();
                organization.DataItems.Set(Models.Datum.DataItemKey.DefaultAccountCreated, "1");
            }
        }
    }
}

