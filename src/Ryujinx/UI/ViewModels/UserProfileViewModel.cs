using Ryujinx.Ava.UI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ryujinx.Ava.UI.ViewModels
{
    public class UserProfileViewModel : BaseModel, IDisposable
    {
        public UserProfileViewModel()
        {
            Profiles = new ObservableCollection<BaseModel>();
            LostProfiles = new ObservableCollection<UserProfile>();
        }

        public ObservableCollection<BaseModel> Profiles { get; set; }

        public ObservableCollection<UserProfile> LostProfiles { get; set; }

        public bool IsEmpty => !LostProfiles.Any();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void UpdateLostProfiles(ObservableCollection<UserProfile> newProfiles)
        {
            LostProfiles = newProfiles;
            OnPropertyChanged(nameof(LostProfiles));
            OnPropertyChanged(nameof(IsEmpty));
        }
    }
}
