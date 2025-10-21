using Blazored.Toast.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace VirtualTour.Web.Components.Pages.Admin.User_Manage
{
    partial class CreateUserAdmin
    {
        public IApiClient ApiClient { get; set; }
        public IToastService ToastService { get; set; }
        private ReqUserCreate NewUser { get; set; } = new ReqUserCreate
        { 
            Gender = "None", 
            PhoneNumber = ""
        };
        public NavigationManager NavigationManager { get; set; }
        public List<RoleModel> Roles { get; set; } = new List<RoleModel>();
        private string ConfirmPassword { get; set; } = string.Empty;
        private bool isSubmitting;
        public CreateUserAdmin(IApiClient apiClient, IToastService toastService, NavigationManager navigationManager)
        {
            ApiClient = apiClient;
            ToastService = toastService;
            NavigationManager = navigationManager;
        }
        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("newId", out var newIdStr)
                && int.TryParse(newIdStr, out int parsedId))
            {
                NewUser.Id = parsedId;
            }
            else
            {
                NewUser.Id = 0;
            }
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/role/getListUse");
            if (res != null && res.Success == true)
            {
                Roles = JsonConvert.DeserializeObject<List<RoleModel>>(res.Data.ToString());
            }
            else
            {
                ToastService.ShowError("Failed to load roles");
            }
        }

        private async Task HandleValidSubmit()
        {
            if (isSubmitting)
                return;

            isSubmitting = true;
            bool isEmail = NewUser.Email.Contains("@") &&
            NewUser.Email.EndsWith(".com", StringComparison.OrdinalIgnoreCase);
            if (NewUser.PasswordHash != ConfirmPassword)
            {
                ToastService.ShowError("Password not confirmed!");
            }
            else if (string.IsNullOrEmpty(NewUser.Email) || !isEmail)
            {
                ToastService.ShowError("Invalid email format!");
            }
            else if (string.IsNullOrEmpty(ConfirmPassword))
            {
                ToastService.ShowError("Please confirm your password!");
            }
            else
            {
                var res = await ApiClient.PostAsync<BaseResponseModel, ReqUserCreate>("/api/user/create", NewUser);
                if (res != null && res.Success == true)
                {
                    ToastService.ShowSuccess("User created successfully");
                    NavigationManager.NavigateTo("/admin/user");
                }
                else
                {
                    ToastService.ShowError("Failed to create user (Email or Username has existed)");
                }
            }
            isSubmitting = false;
        }
    }
}

