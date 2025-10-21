using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.BL.Services
{
    public class UserProfileStateService
    {
        private string _avatarUrl = "";
        public string AvatarUrl => _avatarUrl;
        public event Action OnChange;

        public void SetAvatarUrl(string url)
        {
            _avatarUrl = url;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
