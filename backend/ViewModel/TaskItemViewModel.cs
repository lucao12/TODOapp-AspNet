using System.ComponentModel.DataAnnotations;

namespace Teste.ViewModel
{
    public class TaskCreateViewModel
    {
        [Required(ErrorMessage = "Campo título é de preenchimento obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Campo descrição é de preenchimento obrigatório")]
        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Campo prioridade é de preenchimento obrigatório")]
        public string Priority { get; set; }
        public bool isCompleted { get; set; }
        [Required(ErrorMessage = "Campo data é de preenchimento obrigatório")]
        public DateTime DateLimit { get; set; }
    }
    public class TaskUpdateViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public bool? isCompleted { get; set; }
        public DateTime? DateLimit { get; set; }
    }
}
