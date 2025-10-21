using System.ComponentModel.DataAnnotations;

namespace VirtualTour.Model
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int InActive { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } 
        public string AvatarUrl { get; set; }
        public string CompanyEmail { get; set; }
        public string ManagerName { get; set; }
        public string HashKey { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
    public class ReqLoginDTO
    {
        [Required(ErrorMessage = "Email or Username is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class ReqLoginUsernameDTO
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class  LoginResponse
    {  
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string ApiKey { get; set; }
        public string AvatarUrl { get; set; }
        public string CompanyEmail { get; set; }
        public string ManagerName { get; set; }
    }
    public class UpdateUserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class RepUserFetch
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public int InActive { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string AvatarUrl { get; set; }
        public string HashKey { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
    public class ReqUserCreate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; } 
    }
}
