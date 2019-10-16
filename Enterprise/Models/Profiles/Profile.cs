using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERPCore.Enterprise.Models.Profiles
{
    [Table("ERP_Profiles")]

    public class Profile
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? CoreProfileId { get; set; }

        public ProfileType ProfileType { get; set; }


        [Column("IsSelfOrganization")]
        public bool isSelfOrganization { get; set; }

        [Column("IsRDVerify")]
        public bool isRDVerify { get; set; }


        public Security.Enums.AccessLevel AccessLevel { get; set; }


        public String TitleName { get; set; }
        public String Name { get; set; }
        public String ShotName { get; set; }
        public String DisplayName => String.Format("{0} {1}", this.TitleName, this.Name).Trim().Replace("-", "");


        public String DisplayHeaderTemplate => "{0} สาขา {0}";
        public String DisplayHeader => this.DisplayName + " สาขา " + this.PrimaryAddress.Title +
            Environment.NewLine + "เลขประจำตัวผู้เสียภาษี " + this.TaxNumber + Environment.NewLine + this.PrimaryAddress.AddressLine + Environment.NewLine + this.PhoneNumber;






        [DefaultValue(EnumLanguage.en)]
        public EnumLanguage localizedLanguage { get; set; }



        public byte[] Picture { get; set; }

        [MaxLength(50)]
        public String TaxNumber { get; set; }
        public String Detail { get; set; }
        public String WebSite { get; set; }

        [MaxLength(100)]
        [Index]
        public String Email { get; set; }
        public String FaceBook { get; set; }
        public String LineID { get; set; }


        public String PhoneNumber { get; set; }
        public String FaxNumber { get; set; }
        public DateTime? CreatedDate { get; set; }


        public Guid? CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual ProfileCountry Country { get; set; }

        public virtual ICollection<ProfileBankAccount> BankAccounts { get; set; }
        public virtual ICollection<ProfileGroup> Groups { get; set; }
        public virtual ICollection<HistoryItem> Histories { get; set; }
        public virtual ICollection<ProfileAddress> Addresses { get; set; }


        public virtual Equity.Investor Investor { get; set; }
        public virtual Employees.Employee Employee { get; set; }
        public virtual Customers.Customer Customer { get; set; }
        public virtual Suppliers.Supplier Supplier { get; set; }



   



        public ProfileAddress PrimaryAddress
        {
            get
            {
                var primaryAddress = Addresses.Where(a => a.IsPrimary).FirstOrDefault();
                if (primaryAddress != null)
                    return primaryAddress;

                var defaultBranch = Addresses.Where(a => a.Number == "00000").FirstOrDefault();
                if (defaultBranch != null)
                    return defaultBranch;

                if (Addresses.Count > 0)
                    return Addresses.FirstOrDefault();

                return new ProfileAddress();
            }
        }

        public string Pin { get; set; }



        public Profile()
        {
            this.Id = Guid.NewGuid();
        }

        public void AssignCoreProfileId(Guid coreProfileGuid) => this.CoreProfileId = coreProfileGuid;

        internal void Update(Profile profile)
        {
            this.TitleName = (profile.TitleName ?? "-").ToLower();
            this.Name = profile.Name ?? "NA";
            this.ShotName = profile.ShotName ?? "NA";

            this.TaxNumber = (profile.TaxNumber ?? "-").ToLower();
            this.PhoneNumber = (profile.PhoneNumber ?? "-").ToLower();
            this.Email = (profile.Email ?? "-").ToLower();
            this.WebSite = (profile.WebSite ?? "-").ToLower();
            this.FaceBook = (profile.FaceBook ?? "-").ToLower();
            this.Pin = profile.Pin?.Trim();
            this.AccessLevel = profile.AccessLevel;
        }
    }
}
