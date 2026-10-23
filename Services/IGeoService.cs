using Android.Systems;
using Java.Time.Chrono;
using OndeEstouApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static Android.Provider.MediaStore;
using static Android.Telephony.CarrierConfigManager;
namespace OndeEstouApp.Services
{
    public interface IGeoService
    {
        Task<MinhaLocalizacao> GetCurrentLocationAsync();
    }
    public class GeoService : IGeoService
    {
        public async Task<MinhaLocalizacao> GetCurrentLocationAsync()
        {
            try
            {
                // 1. Verifica e solicita permissão do usuário
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
                if (status != PermissionStatus.Granted)
                {
                    // Usuário negou a permissão
                    return null;
                }
                // 2. Pega as coordenadas exatas (com alta precisão)
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);
                if (location != null)
                {
                    // 3. Converte a Lat/Lon em um Endereço Físico (Geocoding)
                    var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        return new MinhaLocalizacao
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            FeatureName = placemark.FeatureName,
                            Thoroughfare = placemark.Thoroughfare,
                            SubLocality = placemark.SubLocality,
                            Locality = placemark.SubAdminArea,
                            AdminArea = placemark.AdminArea,
                            PostalCode = placemark.PostalCode,
                            CountryName = placemark.CountryName
                        };
                    }
                }
                return null;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // GPS não suportado no aparelho
                System.Diagnostics.Debug.WriteLine($"Erro GPS não suportado: {fnsEx.Message}");
                return null;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // GPS está desligado nas configurações do celular
                System.Diagnostics.Debug.WriteLine($"Erro GPS desligado: {fneEx.Message}");
                return null;
            }
            catch (PermissionException pEx)
            {
                // Erro de permissão
                System.Diagnostics.Debug.WriteLine($"Erro de Permissão: {pEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Qualquer outro erro
                System.Diagnostics.Debug.WriteLine($"Erro Geral: {ex.Message}");
                return null;
            }
        }
    }
}


