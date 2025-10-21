using BlazorBootstrap;
using Blazored.Toast.Services;
using VirtualTour.Model;
using VirtualTour.Web.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
namespace VirtualTour.Web.Components.Pages.Admin.QR_Code
{
    partial class QRCodeManage
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        IApiClient ApiClient { get; set; }

        List<QRCodeModel> qRCodeModels;
        QRCodeCreateRequest newQRCodeModel = new();
        QRCodeModel editingQRCode;
        QRCodeModel SelectedQRCode;
        MudDataGrid<QRCodeModel> QRCodeGrid;
        ApiAuthenticationStateProvider AuthStateProvider;

       /* List<WifiModel> wifi;
        WifiModel newWifi = new WifiModel {  CreatedAt = DateTime.Now, LastUpdated = DateTime.Now };
        WifiModel editingWifi;
        WifiModel SelectedQRCode;
        MudDataGrid<WifiModel> WifiGrid;*/

        private string RoleName { get; set; }
        IToastService ToastService { get; set; }
        IWebHostEnvironment Env;

        private readonly NavigationManager _navigationManager;
        private readonly string _configuredBaseUrl;
        Modal ModalDelete;
        Modal ModalCreate;
        Modal ModalEdit;
        Modal ModalQrCode;      
        bool IsReversed;

        public int DeleteId { get; set; }
        public QRCodeManage(IApiClient apiClient, IToastService toastService, 
            IWebHostEnvironment env, NavigationManager navigationManager, ApiAuthenticationStateProvider authStateProvider,
            IConfiguration configuration)
        {
            ToastService = toastService;
            ApiClient = apiClient;
            Env = env;
            _navigationManager = navigationManager;
            AuthStateProvider = authStateProvider;
            _configuredBaseUrl = configuration["BaseUrl"];
        }

        private bool _firstRender = true;
        protected override async Task OnInitializedAsync()
        {
            if (_firstRender)
            {
                _firstRender = false;
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                string Id = user.FindFirst(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
                var response = await ApiClient.GetFromJsonAsync<BaseResponseModel>($"api/user/getElementById/{Id}");
                if (response != null && response.Success)
                {
                    var repUser = JsonConvert.DeserializeObject<RepUserFetch>(response.Data.ToString());
                    if (repUser != null)
                    {
                        RoleName = repUser.RoleName;
                    }
                }
                await LoadWifi();
                StateHasChanged();
            }
        }
        private void OnSelectedItemChanged(QRCodeModel item)
        {
            SelectedQRCode = item;
            Console.WriteLine("Selected: " + item?.QRName);
        }
        async Task LoadWifi()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("api/QRCodes");
            if (res != null && res.Success)
            {
                qRCodeModels = JsonConvert.DeserializeObject<List<QRCodeModel>>(res.Data.ToString());
                Console.WriteLine("Loaded QR Count: " + qRCodeModels?.Count);
            }
            else
            {
                ToastService.ShowError("Failed to load wifi.");
            }
        }


        /* async Task CreateWifiOld()
         {
             var res = await ApiClient.PostAsync<BaseResponseModel, WifiModel>("api/wifi/create", newWifi);
             if (res != null && res.Success == true)
             {
                 ToastService.ShowSuccess("Wifi created successfully.");
                 OnHideModalCreateClick();
                 await LoadWifi();
             }
             await WifiGrid.ReloadServerData();
         }*/
        private async Task CreateWifi()
        {
            try
            {
                var result = await ApiClient.PostAsync<BaseResponseModel, QRCodeCreateRequest>(
                    "api/QRCodes/create",
                    newQRCodeModel
                );

                if (result != null && result.Success)
                {
                    ToastService.ShowSuccess("Wifi created successfully.");
                    OnHideModalCreateClick();
                    await LoadWifi();
                    await QRCodeGrid.ReloadServerData();
                    return;
                }

                ToastService.ShowError(result?.ErrorMessage ?? "Unknown error occurred while creating wifi.");
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error creating wifi: " + ex.Message);
            }
        }

        async Task HandleDelete()
        {

            var res = await ApiClient.DeleteAsync<BaseResponseModel>($"api/QRCodes/delete/{DeleteId}");
            if (res != null && res.Success)
            {
                ToastService.ShowSuccess("Delete Role successfully");
                OnHideModalDeleteClick();
                await LoadWifi();
            }
            else
            {
                ToastService.ShowError("Failed to delete role.");
            }
            await QRCodeGrid.ReloadServerData();

        }
        async Task EditWifi(QRCodeModel qRCodeModel)
        {
            ModalEdit.ShowAsync();
            editingQRCode = new QRCodeModel
            {
                IDQR = qRCodeModel.IDQR,
                QRName = qRCodeModel.QRName,
                QRType = qRCodeModel.QRType,
                SSIDName = qRCodeModel.SSIDName,
                WifiPassHash = qRCodeModel.WifiPassHash,
                ChangeOnDays = qRCodeModel.ChangeOnDays,
                SecurityType= qRCodeModel.SecurityType,
                QRContent = qRCodeModel.QRContent,
            };
            if (qRCodeModel.IsChange == false)
            {
                editingQRCode.ChangeOnDays = "None";
            }
        }
        async Task UpdateWifi(QRCodeModel qRCodeModel)
        {
            var res = await ApiClient.PutAsync<BaseResponseModel, QRCodeModel>("api/QRCodes/update", qRCodeModel);
            if (res != null && res.Success == true)
            {
                ModalEdit.HideAsync();
                editingQRCode = null;
                ToastService.ShowSuccess($"Update '{qRCodeModel.SSIDName}' successfully");
                await LoadWifi();
            }
            await QRCodeGrid.ReloadServerData();
        }
        private async Task OnShowModalDeleteClick()
        {
            await ModalDelete.ShowAsync();
        }

        private async Task OnHideModalDeleteClick()
        {
            await ModalDelete.HideAsync();
        }
        private async Task OnHideModalCreateClick()
        {
            await ModalCreate.HideAsync();
        }
        private async Task OnHideModalEditClick()
        {
            await ModalEdit.HideAsync();
        }
        void ToggleCreateCard()
        {
            ModalCreate.ShowAsync();
            newQRCodeModel = new QRCodeCreateRequest();
        }
        #region WifiManage.razor.cs  
        string ModalQrCodeTitle = "";
        QRCodeModel? CurrentQRCode;
        string CurrentQrText = "";

        async Task ShowQrCode(QRCodeModel qRCode)
        {
            IsReversed = false;
            if (qRCode.QRType == "WIFI" )
            {
                CurrentQRCode = qRCode;
                ModalQrCodeTitle = "Scan with your Mobile";
                CurrentQrText = $"WIFI:S:{qRCode.SSIDName};T:{qRCode.SecurityType};P:{qRCode.WifiPassHash};;";

                await ModalQrCode.ShowAsync();
                await JSRuntime.InvokeVoidAsync("qrStyling", CurrentQrText, "qrCodeContainer");
            }
            else if (qRCode.QRType == "WIFI-INFO")
            {
                CurrentQRCode = qRCode;
                ModalQrCodeTitle = "Wi-Fi Info as Link";
                CurrentQrText = $"{_configuredBaseUrl}{qRCode.SSIDName}";
                //CurrentQrText = $"{_navigationManager.BaseUri}wifi/{qRCode.SSIDName}";
                await ModalQrCode.ShowAsync();
                await JSRuntime.InvokeVoidAsync("qrStyling", CurrentQrText, "qrCodeContainer");
            }
            else if (qRCode.QRType == "LINK")
            {

                CurrentQRCode = qRCode;
                ModalQrCodeTitle = $"{qRCode.QRName} as Link";
                CurrentQrText = $"{qRCode.QRContent}";
                //CurrentQrText = $"{_navigationManager.BaseUri}wifi/{qRCode.SSIDName}";
                await ModalQrCode.ShowAsync();
                await JSRuntime.InvokeVoidAsync("qrStyling", CurrentQrText, "qrCodeContainer");
            }
        }

        private async Task DownloadCurrentQrCode()
        {
            if (string.IsNullOrWhiteSpace(CurrentQrText) || CurrentQRCode == null)
                return;

            // Xác định loại thiết bị từ DeviceType
            var deviceSuffix = CurrentQRCode.QRType?.ToUpper() switch
            {
                "WIFI" => "WIFI",
                "WIFI-INFO" => "WIFI-INFO",
                "LINK" => "LINK",
                _ => "Unknown"
            };
            // Làm sạch tên WiFi để tránh lỗi tên file
            var safeSsid = (CurrentQRCode.SSIDName ?? "Wifi").Replace(" ", "_").Replace("/", "_");

            var fileName = $"QR_Code_{safeSsid}_{deviceSuffix}";

            await JSRuntime.InvokeVoidAsync("downloadCurrentQrCode", "png", fileName);
        }
        private async Task ReverseQrCode()
        {
            await JSRuntime.InvokeVoidAsync("reverseQrStyling", CurrentQrText, "qrCodeContainer", IsReversed);
            IsReversed = !IsReversed;
        }


        #endregion

    }//


}
