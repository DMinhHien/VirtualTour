using Blazored.Toast.Services;
using VirtualTour.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;
namespace VirtualTour.Web.Components.Pages.Admin.Node
{
    partial class NodeEdit
    {
        [Parameter]
        public int Id { get; set; }  // Node ID passed via URL

        NodeModel node = new NodeModel()
        {
            Links = new List<LinkedNodes>(),
        };
        LinkedNodes LinkedNode = new LinkedNodes();
        private IJSRuntime JS { get; set; }
        private IApiClient _apiClient;
        private IToastService _toastService;
        private IWebHostEnvironment Env { get; set; }
        public NavigationManager Navigation { get; set; }
        private string imageUrl { get; set; } = string.Empty;
        private bool IsViewerReady { get; set; } = false;
        public LinkedNodes selectedHotSpot;
        private List<NodeModel> nodes = new List<NodeModel>();
        private int nextMarkerId = 0;
        public List<string> Areas { get; set; } = new List<string>();
        public List<string> Depts { get; set; } = new List<string>();
        public class ViewerPosition
        {
            public double yaw { get; set; }
            public double pitch { get; set; }
        }

        public NodeEdit(NavigationManager navigationManager, IJSRuntime js,
            IWebHostEnvironment env, IApiClient apiClient, IToastService toastService)
        {
            Navigation = navigationManager;
            JS = js;
            Env = env;
            _apiClient = apiClient;
            _toastService = toastService;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            // Load the node data by id passed in URL
            var res = await _apiClient.GetFromJsonAsync<BaseResponseModel>($"/api/node/getElementById/{Id}");
            if (res.Success)
            {
                node = JsonConvert.DeserializeObject<NodeModel>(res.Data.ToString());
                imageUrl = node.Panorama;
                // Set nextMarkerId to the highest existing marker plus one (or 0 if none exist)
                if (node.Links.Any())
                {
                    nextMarkerId = node.Links.Max(x => x.MarkerId) + 1;
                }
                else
                {
                    nextMarkerId = 0;
                }
            }
            else
            {
                _toastService.ShowError("Failed to load node data: " + res.ErrorMessage);
            }
            res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getAllArea");
            if (res.Success && res.Data != null)
            {
                Areas = JsonConvert.DeserializeObject<List<string>>(res.Data.ToString());
            }
            res = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getAllDept");
            if (res.Success && res.Data != null)
            {
                Depts = JsonConvert.DeserializeObject<List<string>>(res.Data.ToString());
            }
            var dotNetRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("initVirtualTourEdit",node, dotNetRef);
            IsViewerReady = true;
            var rep = await _apiClient.GetFromJsonAsync<BaseResponseModel>("/api/node/getListUse");
            if (rep.Success)
            {
                nodes = JsonConvert.DeserializeObject<List<NodeModel>>(rep.Data.ToString());
                var currentNode=nodes.FirstOrDefault(x => x.Id == Id);
                nodes.Remove(currentNode); // Remove current node from the list to avoid linking to itself
            }
        }

        private async Task UploadThumbnailFiles(IBrowserFile file)
        {
            var thumbnails = Path.Combine(Env.WebRootPath, "images", "thumbnails");
            if (!Directory.Exists(thumbnails))
                Directory.CreateDirectory(thumbnails);
            var ext = Path.GetExtension(file.Name);
            var fileName = $"{node.Id}_{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(thumbnails, fileName);
            using var image = await Image.LoadAsync(file.OpenReadStream(15_000_000));
            image.Mutate(x => x.Resize(new ResizeOptions { Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max, Size = new SixLabors.ImageSharp.Size(300, 0) }));
            var encoder = new JpegEncoder { Quality = 50 };
            await using var fs = File.Create(fullPath);
            await image.SaveAsJpegAsync(fs, encoder);
            node.Thumbnail = $"/images/thumbnails/{fileName}";
        }

        private async Task SaveNode()
        {
            // Send update request with PUT to update the node
            var res = await _apiClient.PutAsync<BaseResponseModel, NodeModel>("/api/node/update", node);
            if (res.Success)
            {
                _toastService.ShowSuccess("Node updated successfully!");
                Navigation.NavigateTo("/admin/nodes", true);
            }
            else
            {
                _toastService.ShowError("Failed to update node: " + res.ErrorMessage);
            }
        }

        private async Task SetInitialView()
        {
            if (IsViewerReady)
            {
                var pos = await JS.InvokeAsync<ViewerPosition>("setInitialView");
                node.DefaultYaw = pos.yaw * 180 / Math.PI;
                node.DefaultPitch = pos.pitch * 180 / Math.PI;
                _toastService.ShowSuccess($"Initial view set: Yaw: {Math.Round(node.DefaultYaw, 2)}°, Pitch: {Math.Round(node.DefaultPitch, 2)}°");
            }
            else
            {
                _toastService.ShowError("Viewer is not ready. Please upload a panorama image first.");
            }
        }

        private async Task AddHotSpot()
        {
            int newMarkerId = nextMarkerId;
            nextMarkerId++;
            var newLink = new LinkedNodes
            {
                MarkerId = newMarkerId,
                Yaw = 0,
                rotation = 0,
                Tooltip = "New Hotspot",
                TargetNode = 0
            };
            node.Links.Add(newLink);
            string markerId = $"{newMarkerId}";
            await JS.InvokeVoidAsync("addHotSpot", markerId);
        }

        private async Task DeleteHotSpot()
        {
            await JS.InvokeVoidAsync("removeHotSpot", selectedHotSpot.MarkerId);
            if (selectedHotSpot != null)
            {
                node.Links.Remove(selectedHotSpot);
                _toastService.ShowSuccess($"Hotspot {selectedHotSpot.MarkerId} deleted.");
                selectedHotSpot = null;
                StateHasChanged();
            }
            else
            {
                _toastService.ShowError("No hotspot selected to delete.");
            }
        }

        private async Task RotateHotSpot()
        {
            if (selectedHotSpot.MarkerId >= 0)
            {
                var hotspot = node.Links.FirstOrDefault(x => x.MarkerId == selectedHotSpot.MarkerId);
                hotspot.rotation = hotspot.rotation + 45;
                await JS.InvokeVoidAsync("rotateHotSpot", selectedHotSpot.MarkerId, hotspot.rotation);
            }
            else
            {
                _toastService.ShowError("No hotspot selected or invalid selection.");
            }
        }

        [JSInvokable]
        public Task UpdateHotSpotPosition(int markerId, double yaw, double pitch)
        {
            var hotspot = node.Links.FirstOrDefault(x => x.MarkerId == markerId);
            if (hotspot != null)
            {
                hotspot.Yaw = yaw * 180 / Math.PI;
                hotspot.Pitch = pitch * 180 / Math.PI;
            }
            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task HotSpotSelected(int markerId)
        {
            selectedHotSpot = node.Links.FirstOrDefault(x => x.MarkerId == markerId);
            StateHasChanged();
            _toastService.ShowInfo($"Hotspot {markerId} selected. Yaw: {selectedHotSpot.Yaw:F2}, Pitch: {selectedHotSpot.Pitch:F2}");
            return Task.CompletedTask;
        }

        public Task SaveHotSpot()
        {
            if (selectedHotSpot != null)
            {
                if (selectedHotSpot.TargetNode == 0)
                {
                    _toastService.ShowError("Please select a node to link.");
                    return Task.CompletedTask;
                }
                _toastService.ShowSuccess($"Hotspot {selectedHotSpot.MarkerId} saved. Yaw: {selectedHotSpot.Yaw:F2}, Pitch: {selectedHotSpot.Pitch:F2}");
                selectedHotSpot = null;
                StateHasChanged();
            }
            else
            {
                _toastService.ShowError("No hotspot selected to save.");
            }
            return Task.CompletedTask;
        }
    }
}
