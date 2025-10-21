using BlazorBootstrap;
using Blazored.Toast.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace VirtualTour.Web.Components.Pages.Admin.User_Manage
{
    partial class UpdateUserAdmin
    {
        public IApiClient ApiClient { get; set; }
        public IToastService ToastService { get; set; }
        public NavigationManager NavigationManager { get; set; }
        [Parameter] public int Id { get; set; }
        public UpdateUserDTO User { get; set; } = new();
        public List<RoleModel> Roles { get; set; } = new List<RoleModel>();
        public Modal Modal;
        public Modal ConfirmModal;

        public UpdateUserAdmin(IApiClient apiClient, IToastService toastService, NavigationManager navigationManager)
        {
            ApiClient = apiClient;
            ToastService = toastService;
            NavigationManager = navigationManager;
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Id != null)
            {
                var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>($"/api/user/getElementById/{Id}");
                if (res != null && res.Success)
                {
                    User = JsonConvert.DeserializeObject<UpdateUserDTO>(res.Data.ToString());
                }
                res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/role/getListUse");
                if (res != null && res.Success == true)
                {
                    Roles = JsonConvert.DeserializeObject<List<RoleModel>>(res.Data.ToString());
                }
                else
                {
                    ToastService.ShowError("Failed to load roles");
                }
            }
        }
        private async Task HandleValidSubmit()
        {
            var response = await ApiClient.PutAsync<BaseResponseModel, UpdateUserDTO>("/api/user/update", User);
            if (response != null && response.Success)
            {
                ToastService.ShowSuccess("User updated successfully");
                NavigationManager.NavigateTo("/admin/user");
            }
            else
            {
                ToastService.ShowError("Failed to update user");
            }
        }
        private async Task HandleResetPassword()
        {
            var response = await ApiClient.PostAsync<BaseResponseModel, string>("/api/auth/reset-password", User.Id.ToString());
            if (response != null && response.Success)
            {
                ConfirmModal.ShowAsync();
                OnHideModalClick();
            }
            else
            {
                ToastService.ShowError("Failed to reset password");
            }
        }
        private async Task OnShowModalClick()
        {
            await Modal.ShowAsync();
        }

        private async Task OnHideModalClick()
        {
            await Modal.HideAsync();
        }
        private async Task OnHideConfirmModalClick()
        {
            await ConfirmModal.HideAsync();
        }
    }
}
