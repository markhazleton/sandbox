using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// Base view model containing common properties inherited from BaseEntity
    /// </summary>
    public abstract class BaseViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime UpdatedAt { get; set; }

        [Display(Name = "Created By")]
        [MaxLength(200)]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Modified By")]
        [MaxLength(200)]
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
