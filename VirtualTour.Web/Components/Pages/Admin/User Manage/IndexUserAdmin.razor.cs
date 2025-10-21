using BlazorBootstrap;
using Blazored.Toast.Services;
using VirtualTour.Model;
using VirtualTour.Web.Authentication;
using MudBlazor;
using Newtonsoft.Json;

namespace VirtualTour.Web.Components.Pages.Admin.User_Manage
{
    partial class IndexUserAdmin
    {
        public IApiClient ApiClient { get; set; }
        public List<UserModel> Users { get; set; }
        public Modal Modal { get; set; }
        public Modal RefreshModal { get; set; }
        public int DeleteID { get; set; }
        public int RefreshID { get; set; }
        public int NewUserId { get; set; } = 0;
        public int currentUserId { get; set; } = 0;
        MudDataGrid<UserModel> UserGrid;
        ApiAuthenticationStateProvider AuthStateProvider;

        private IToastService ToastService { get; set; }
        public IndexUserAdmin(IApiClient apiClient, IToastService toastService,ApiAuthenticationStateProvider apiAuthenticationStateProvider)
        {
            ApiClient = apiClient;
            ToastService = toastService;
            AuthStateProvider = apiAuthenticationStateProvider;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadUser();
            NewUserId = (Users != null && Users.Any()) ? Users.Max(u => u.Id) + 1 : 1;
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            currentUserId =int.Parse(user.FindFirst(c => c.Type == 
            System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty);
        }
        protected async Task LoadUser()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/user/getListUse");
            if (res != null && res.Success == true)
            {
                Users = JsonConvert.DeserializeObject<List<UserModel>>(res.Data.ToString());
            }

        }
        protected async Task HandleDelete()
        {
            var res = await ApiClient.DeleteAsync<BaseResponseModel>($"/api/user/delete/{DeleteID}");
            if (res != null && res.Success)
            {
                ToastService.ShowSuccess("Delete User successfully");
                await LoadUser();
                await UserGrid.ReloadServerData();
                OnHideModalClick();
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
        private async Task OnHideRefreshModalClick()
        {
            await RefreshModal.HideAsync();
        }
        private async Task RefreshApiKey()
        {
            if (RefreshID != currentUserId)
            {
                var res = await ApiClient.PostAsync<BaseResponseModel, object>($"/api/user/refreshApiKey/{RefreshID}", null);
                if (res != null && res.Success)
                {
                    ToastService.ShowSuccess("Refresh API Key successfully");
                    await LoadUser();
                    await UserGrid.ReloadServerData();
                }
                else
                {
                    ToastService.ShowError(res != null ? res.ErrorMessage : "Error refreshing API Key");
                }
            }
            else
            {
                await RefreshModal.ShowAsync();
            }
        }
        private async Task RefreshOwnApiKey()
        {
            var res = await ApiClient.PostAsync<BaseResponseModel, object>($"/api/user/refreshApiKey/{RefreshID}", null);
            if (res != null && res.Success)
            {
                ToastService.ShowSuccess("Refresh API Key successfully");
                await RefreshModal.HideAsync();
                Navigation.NavigateTo("/admin/logout");
            }
            else
            {
                ToastService.ShowError(res != null ? res.ErrorMessage : "Error refreshing API Key");
            }
        }
    }
}
