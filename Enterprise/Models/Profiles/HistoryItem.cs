


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCore.Enterprise.Models.Profiles
{

    [Table("ERP_Profiles_HistoryItems")]
    public class HistoryItem
    {

        public HistoryItem()
        {
            RecordDate = DateTime.Today;
        }


        [Key]
        public Guid Id { get; set; }
        public DateTime RecordDate { get; set; }



        [Column("HistoryRelatedProfileId")]
        public Guid? ProfileGuid { get; set; }
        [ForeignKey("ProfileGuid")]
        public virtual Profiles.Profile Profile { get; set; }



        public String Title { get; set; }
        public String Detail { get; set; }
        public Guid KeyId { get; set; }


        [Column("Url")]
        public string Url { get; set; }
        public string Type { get; set; }
    }
}