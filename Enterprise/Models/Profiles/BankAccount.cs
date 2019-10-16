using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCore.Enterprise.Models.Profiles
{

    [Table("ERP_Profiles_BankAccount")]
    public class ProfileBankAccount
    {
        [Key]
        public Guid Id { get; set; }

        [Column("ProfileId")]
        public Guid? ProfileGuid { get; set; }

        [ForeignKey("ProfileGuid")]
        public virtual Profiles.Profile Profile { get; set; }
        public String Name { get; set; }
        public String Number { get; set; }
        public Models.Financial.AccountType Type { get; set; }
        public String BankName { get; set; }
        public Guid? BankProfileGuid { get; set; }
        public void Update(ProfileBankAccount bankAccount)
        {
            this.Name = bankAccount.Name;
            this.BankName = bankAccount.BankName;
            this.Number = bankAccount.Number;
        }

        // [ForeignKey("BankProfileGuid")]
        // public virtual Profiles.Profile BankProfile { get; set; }

        public ProfileBankAccount()
        {
            Id = Guid.NewGuid();
        }
    }
}