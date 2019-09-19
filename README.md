[![Build Status](https://dev.azure.com/xabaril/Esquio.Contrib/_apis/build/status/esquio-contrib-master-ci?branchName=master)](https://dev.azure.com/xabaril/Esquio.Contrib/_build/latest?definitionId=6&branchName=master)

# Esquio.Contrib

Esquio.Contrib is a community contribution Toggles and extensions for [Esquio](https://github.com/xabaril/Esquio)

## LocationToggles

### CountryNameLocationToggle

Toggle that is active depending on the request ip country name. To use this toggle you need to install the LocationToggles package using the .NET CLI.

```cmd
dotnet install Esquio.LocationToggles
```

or using PowerShell | Package Manager.

```powershell
install-package Esquio.LocationToggles
```

Now, you can register new toggles on *Esquio* setup.

```csharp
    services.AddEsquio(setup => setup.RegisterTogglesFromAssembly(typeof(CountryNameLocationToggle).Assembly))
        .AddAspNetCoreDefaultServices()
        .AddConfigurationStore(Configuration, "Esquio")
```

and configure your features using this new toggle, as we show on next configuration sample for Esquio. Of course, you can use also our Entity Framework Store.

```json
{
  "Esquio": {
    "Products": [
      {
        "Name": "default",
        "Features": [
          {
            "Name": "HiddenGem",
            "Enabled": true,
            "Toggles": [
              {
                "Type": "LocationToggles.CountryNameLocationToggle, LocationToggles",
                "Parameters": {
                  "Countries": "Spain"
                }
              }
            ]
          }
        ]
      }
    ]
  }
}

```

### CountryCodeLocationToggle

Toggle that is active depending on the request ip country international code. To use this toggle you need to install the LocationToggles package using the .NET CLI.

```cmd
dotnet install Esquio.LocationToggles
```

or using PowerShell | Package Manager.

```powershell
install-package Esquio.LocationToggles
```

Now, you can register new toggles on *Esquio* setup.

```csharp
    services.AddEsquio(setup => setup.RegisterTogglesFromAssembly(typeof(CountryNameLocationToggle).Assembly))
        .AddAspNetCoreDefaultServices()
        .AddConfigurationStore(Configuration, "Esquio")
```

and configure your features using this new toggle, as we show on next configuration sample for Esquio. Of course, you can use also our Entity Framework Store.

```json
{
  "Esquio": {
    "Products": [
      {
        "Name": "default",
        "Features": [
          {
            "Name": "HiddenGem",
            "Enabled": true,
            "Toggles": [
              {
                "Type": "LocationToggles.CountryCodeLocationToggle, LocationToggles",
                "Parameters": {
                  "Countries": "ES;IT"
                }
              }
            ]
          }
        ]
      }
    ]
  }
}

```

### ClientIpAddressToggle

The client IP address toggle activates a feature for ip addresses defined in the IP list. To use this toggle you need to install the LocationToggles package using the .NET CLI.

```cmd
dotnet install Esquio.LocationToggles
```

or using PowerShell | Package Manager.

```powershell
install-package Esquio.LocationToggles
```

Now, you can register new toggles on *Esquio* setup.

```csharp
    services
        .AddEsquio(setup => 
          setup.RegisterTogglesFromAssembly(typeof(ClientIpAddressToggle).Assembly)
        )
        .AddAspNetCoreDefaultServices()
        .AddConfigurationStore(Configuration, "Esquio")
```

and configure your features using this new toggle, as we show on next configuration sample for Esquio. Of course, you can use also our Entity Framework Store.

```json
{
  "Esquio": {
    "Products": [
      {
        "Name": "default",
        "Features": [
          {
            "Name": "HiddenGem",
            "Enabled": true,
            "Toggles": [
              {
                "Type": "LocationToggles.ClientIpAddressToggle, LocationToggles",
                "Parameters": {
                  "IpAddresses": "127.0.0.1;127.0.0.2"
                }
              }
            ]
          }
        ]
      }
    ]
  }
}

```

### ServerIpAddressToggle

The server IP address toggle activates a feature for ip addresses defined in the IP list. To use this toggle you need to install the LocationToggles package using the .NET CLI.

```cmd
dotnet install Esquio.LocationToggles
```

or using PowerShell | Package Manager.

```powershell
install-package Esquio.LocationToggles
```

Now, you can register new toggles on *Esquio* setup.

```csharp
    services
        .AddEsquio(setup => 
          setup.RegisterTogglesFromAssembly(typeof(ServerIpAddressToggle).Assembly)
        )
        .AddAspNetCoreDefaultServices()
        .AddConfigurationStore(Configuration, "Esquio")
```

and configure your features using this new toggle, as we show on next configuration sample for Esquio. Of course, you can use also our Entity Framework Store.

```json
{
  "Esquio": {
    "Products": [
      {
        "Name": "default",
        "Features": [
          {
            "Name": "HiddenGem",
            "Enabled": true,
            "Toggles": [
              {
                "Type": "LocationToggles.ServerIpAddressToggle, LocationToggles",
                "Parameters": {
                  "IpAddresses": "127.0.0.1;127.0.0.2"
                }
              }
            ]
          }
        ]
      }
    ]
  }
}

```

### HostNameToggle

The application hostname toggle activates a feature for client instances with a hostName in the hostNames list.. To use this toggle you need to install the LocationToggles package using the .NET CLI.

```cmd
dotnet install Esquio.LocationToggles
```

or using PowerShell | Package Manager.

```powershell
install-package Esquio.LocationToggles
```

Now, you can register new toggles on *Esquio* setup.

```csharp
    services
        .AddEsquio(setup => 
          setup.RegisterTogglesFromAssembly(typeof(HostNameToggle).Assembly)
        )
        .AddAspNetCoreDefaultServices()
        .AddConfigurationStore(Configuration, "Esquio")
```

and configure your features using this new toggle, as we show on next configuration sample for Esquio. Of course, you can use also our Entity Framework Store.

```json
{
  "Esquio": {
    "Products": [
      {
        "Name": "default",
        "Features": [
          {
            "Name": "HiddenGem",
            "Enabled": true,
            "Toggles": [
              {
                "Type": "LocationToggles.HostNameToggle, LocationToggles",
                "Parameters": {
                  "HostNames": "localhost;domain.com"
                }
              }
            ]
          }
        ]
      }
    ]
  }
}

```

## UserAgentToggles

### UserAgentBrowserToggle

Toggle that is active depending on the request user agent header value. To use this toggle you need to install the LocationToggles package using the .NET CLI.

```cmd
dotnet install Esquio.UserAgentToggles
```

or using PowerShell | Package Manager.

```powershell
install-package Esquio.UserAgentToggles
```

Now, you can register new toggle on *Esquio* setup.

```csharp
    services.AddEsquio(setup => setup.RegisterTogglesFromAssembly(typeof(UserAgentBrowserToggle).Assembly))
        .AddAspNetCoreDefaultServices()
        .AddConfigurationStore(Configuration, "Esquio")
```

and configure your features using this new toggle, as we show on next configuration sample for Esquio. Of course, you can use also our Entity Framework Store.

```json
{
  "Esquio": {
    "Products": [
      {
        "Name": "default",
        "Features": [
          {
            "Name": "HiddenGem",
            "Enabled": true,
            "Toggles": [
              {
                "Type": "UserAgentToggles.UserAgentBrowserToggle, UseAgentToggles",
                "Parameters": {
                  "Browsers": "Chrome"
                }
              }
            ]
          }
        ]
      }
    ]
  }
}
```
