using System.ComponentModel.DataAnnotations;

namespace Teste.ViewModel
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Campo email é de preenchimento obrigatório")]
        [EmailAddress(ErrorMessage = "Campo email deve ser um endereço de email válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo senha é de preenchimento obrigatório")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\p{P}\p{S}])[A-Za-z\d\p{P}\p{S}]{8,}$", ErrorMessage = "A senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial")]
        public string Password { get; set; }
    }
    public class UserSignUpViewModel
    {
        [Required(ErrorMessage = "Campo nome é de preenchimento obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,100}$", ErrorMessage = "Nome contém caracteres inválidos")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Campo email é de preenchimento obrigatório")]
        [EmailAddress(ErrorMessage = "Campo email deve ser um endereço de email válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo senha é de preenchimento obrigatório")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\p{P}\p{S}])[A-Za-z\d\p{P}\p{S}]{8,}$", ErrorMessage = "A senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; }
    }
    public class UserUpdateViewModel
    {
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,100}$", ErrorMessage = "Nome contém caracteres inválidos")]
        public string? Name { get; set; }
        [EmailAddress(ErrorMessage = "Campo email deve ser um endereço de email válido")]
        public string? Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\p{P}\p{S}])[A-Za-z\d\p{P}\p{S}]{8,}$", ErrorMessage = "A senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial")]
        public string OldPassword { get; set; }
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A nova senha deve ter entre 8 e 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\p{P}\p{S}])[A-Za-z\d\p{P}\p{S}]{8,}$", ErrorMessage = "A senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial")]
        public string NewPassword { get; set; }
    }
    public class UserUpdateAdminViewModel
    {
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,100}$", ErrorMessage = "Nome contém caracteres inválidos")]
        public string? Name { get; set; }
        [EmailAddress(ErrorMessage = "Campo email deve ser um endereço de email válido")]
        public string? Email { get; set; }
        [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\p{P}\p{S}])[A-Za-z\d\p{P}\p{S}]{8,}$", ErrorMessage = "A senha deve conter no mínimo 8 caracteres, com pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial")]
        public string? Password { get; set; }
    }
}
