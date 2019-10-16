using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Financial.Transfer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.ChartOfAccount
{

    [Table("ERP_Accounts")]
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Account Parent { get; set; }
        public virtual ICollection<Account> Child { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(30)]
        public String No { get; set; }

        [MaxLength(255)]
        public String Name { get; set; }

        public String CodeName { get; set; }

        public string Code => this.IsFolder == false ?  String.Format("{0}{1}", this.Type.ToString().Substring(0, 1).ToUpper(), this.No):  null;
          
        public bool IsLiquidity { get; set; }

        [MaxLength(500)]
        public String Description { get; set; }

        public void AddChild(Account accountItem)
        {
            accountItem.ParentId = this.Id;
        }

        [DefaultValue(false)]
        public bool IsFolder { get; set; }



        public AccountTypes Type { get; set; }

        [Column("AccountSubType")]
        public AccountSubTypes? SubEnumType { get; set; }


        public String SpanBadge
        {

            get
            {
                switch (this.Type)
                {
                    case AccountTypes.Asset:
                        return "<span class=\"badge badge-success\">A</span>";
                    case AccountTypes.Liability:
                        return "<span class=\"badge badge-warning\">L</span>";
                    case AccountTypes.Capital:
                        return "<span class=\"badge badge-primary\">C</span>";
                    case AccountTypes.Income:
                        return "<span class=\"badge badge-info\">I</span>";
                    case AccountTypes.Expense:
                        return "<span class=\"badge badge-danger\">E</span>";
                    default:
                        return "";
                }
            }
        }

        public String SpanCodeBadge
        {
            get
            {
                string returnBadge = "";
                switch (this.Type)
                {
                    case AccountTypes.Asset:
                        returnBadge = "<span class=\"badge\"> {0}</span>";
                        break;
                    case AccountTypes.Liability:
                        returnBadge = "<span class=\"badge\"> {0}</span>";
                        break;
                    case AccountTypes.Capital:
                        returnBadge = "<span class=\"badge\"> {0}</span>";
                        break;
                    case AccountTypes.Income:
                        returnBadge = "<span class=\"badge\"> {0}</span>";
                        break;
                    case AccountTypes.Expense:
                        returnBadge = "<span class=\"badge\"> {0}</span>";
                        break;
                }

                return string.Format(returnBadge, this.Code);

            }
        }





        [DefaultValue(false)]
        public bool IsPreviewDisplay { get; set; }


        // public BankAccountType BankAccountType { get; set; }
        public String BankAccountNumber { get; set; }

        public String BankAccountBankName { get; set; }

        public String BankAccountBranch { get; set; }

        public bool IsCashEquivalent { get; set; }

        public LedgerPostStatus PostStatus { get; set; }



        #region OpeningBalance

        [Column(TypeName = "Money")]
        public Decimal? OpeningBalance { get; set; }
        public Decimal OpeningDebitBalance { get; set; }
        public Decimal OpeningCreditBalance { get; set; }




        #endregion

        [DisplayFormat(DataFormatString = "N2")]
        public Decimal CurrentBalance
        {
            get
            {
                switch (Type)
                {
                    case AccountTypes.Asset:
                    case AccountTypes.Expense:
                        return (this.Debit) - (Credit) ;

                    case AccountTypes.Liability:
                    case AccountTypes.Capital:
                    case AccountTypes.Income:
                    default:
                        return (Credit) - (this.Debit);
                }
            }
        }



        [Column("BalanceRecordDate")]
        public DateTime? CurrentBalanceRecordDate { get; set; }


        [Column(TypeName = "Money")]
        [DisplayFormat(DataFormatString = "N2")]
        
        public Decimal Credit { get; set; }

        [Column(TypeName = "Money")]
        [DisplayFormat(DataFormatString = "N2")]
        
        public Decimal Debit { get; set; }





        public Decimal? ProfitMultiplyAmount { get; set; }
        public string EnglishName { get; set; }
        public int Order { get; set; }

        public Account()
        {
            this.Id = Guid.NewGuid();
        }

        public Account(AccountTypes type, AccountSubTypes subType, bool folder, string AccountNo, string accountName, Account parent)
        {
            this.Id = Guid.NewGuid();
            this.Type = type;
            this.SubEnumType = subType;
            this.IsFolder = folder;
            this.No = AccountNo;
            this.Name = accountName;
        }

        public void ClearBalance()
        {
            this.Debit = 0;
            this.Credit = 0;
            this.CurrentBalanceRecordDate = DateTime.Today;
        }

    }
}