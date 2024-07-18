using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Name com 3 a 40 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Slug é obrigatório")]

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Slug com 3 a 20 caracteres")]
        public string Slug { get; set; }
    }
}
