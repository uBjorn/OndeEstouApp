using Android.Renderscripts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliJ.Lang.Annotations;
using OndeEstouApp.Models;
using OndeEstouApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
namespace OndeEstouApp.ViewModels

{
    public partial class LocalViewModel : ObservableObject
    {
        private readonly IGeoService _geoService;
        public LocalViewModel(IGeoService geoService)
        {
            _geoService = geoService;
        }
        [ObservableProperty]
        private MinhaLocalizacao currentLocation;
        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private bool isNotBusy = true;
        [ObservableProperty]
        private bool hasLocation;
        [RelayCommand]
        private async Task GetLocationAsync()
        {
            IsBusy = true;
            IsNotBusy = false;
            HasLocation = false;
            CurrentLocation = null;
            var location = await _geoService.GetCurrentLocationAsync();
            if (location != null)
            {
                CurrentLocation = location;
                HasLocation = true;
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Ops!", "Não foi possível obter sua localização. Verifique se o GPS está ligado e se as permissões foram concedidas.", "OK");
            }
            IsBusy = false;
            IsNotBusy = true;
        }
    }
}