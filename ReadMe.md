# GoogleReCaptcha.Core

A small library in .NET Standard form for using Google reCAPTCHA within an ASP.NET MVC applications. Features standard methods for configuring reCAPTCHA via appsettings.json or custom. Has ability to call Google's verify endpoint after submission of page forms. Supports v2 and v3 versions; v3 is default version assumed to be used.

Follow the steps below to utilize this library to add Google reCAPTCHA support to your projects.

___

## Install the NuGet Package

You can install the package [from NuGet](https://www.nuget.org/packages/GoogleReCaptcha.Core/) to your project from within Visual studio or from command line:

```powershell
Install-Package GoogleReCaptcha.Core
```

> ... or using `dotnet` CLI:

```powershell
dotnet add package GoogleReCaptcha.Core
```

___

## Add Service In Startup

You will need to add a call to `AddGoogleReCaptchaV3` in your Startup class' `ConfigureServices` method. The version below retrieves its settings from `appsettings.json`:

```cs
public IConfiguration Configuration { get; }

public Startup(IConfiguration configuration)
{
    Configuration = configuration;
}

public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddGoogleReCaptchaV3(Configuration); // Add Google ReCaptcha V3 support

    ...
}
```

> Or, you could supply the reCAPTCHA configuration settings directly like this below (not recommended; use appsettings.json so you can attempt to hide items like SiteKey and SecretKey):

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...

    // Add Google ReCaptcha V3 support
    services.AddGoogleReCaptchaV3(() =>
    {
        return new ReCaptchaV3Settings
        {
            SiteKey = "<YOUR-SITE-KEY>",
            SecretKey = "<YOUR-SECRET-KEY>"
        };
    });

    ...
}
```

___

## Startup Settings Configuration

You can apply your Google reCAPTCHA version 3 configuration settings in `appsettings.json` like so:

```json
{
...
    "GoogleReCaptcha": {
        "V3": {
            "Enabled": true,
            "LibUrl": "https://www.google.com/recaptcha/api.js",
            "ApiUrl": "https://www.google.com/recaptcha/api",
            "SiteKey": "<YOUR-SITE-KEY>", // Do not add here; add to environment var ~: GoogleReCaptcha__V3__SitetKey
            "SecretKey": "", // Do not add here; add to environment var ~: GoogleReCaptcha__V3__SecretKey
            "DefaultPassingScore": 0.7
        }
    }
...
}
```

The service collection extension method `AddGoogleReCaptchaV3(IConfiguration)` by default looks for your settings in the configuration section `GoogleReCaptcha:V3`.

___

## Usage On Forms (MVC Only)

When you have the GoogleReCaptcha.Core services configured and its tag helpers imported you can use them inside your views.

You will need these items to utilize Google reCAPTCHA in your forms:

### Import Tag Helpers

This library comes with tag helpers to get your reCAPTCHA configuration settings and features into your view's forms.

You will need to add the tag helpers in your views path, probably in the `~/Views/Shared/_ViewImports.cshtml` file, like so:

```cshtml
@addTagHelper *, GoogleReCaptcha.Core
```

### Add ReCaptcha Script

Add the Google ReCaptcha script using the `</g-recaptcha-script>` tag. This will pull the Google reCAPTCHA javascript library into your view with your settings applied:

```html
<g-recaptcha-script></g-recaptcha-script>
```

> You can use your own `</script>` tag if you would like, and still enforce your own settings:

```html
<script g-recaptcha-from-settings="true" ... ></script>
```

### Add ReCaptcha Submit Button

You will need to add the Google ReCaptcha submit button using `</g-recaptcha-submit-button>`. This tag helper will apply Google's required data attributes into a new `</button>` element in your view using your settings. It applies `submit` value to the `data-action` attribute. The default `data-callback` will be a function you will need to define later named `onGReCaptchaV3Submit` that accepts Google's reCAPTCHA `token` value for the current form's processing.

Apply the submit button like so:

```html
<g-recaptcha-submit-button></g-recaptcha-submit-button>
```

> You can treat this tag like any other `</button>` tag; example using Bootstrap and Fontawesome with button:

```html
<g-recaptcha-submit-button class="btn btn-primary"><span class="fa fa-arrow-circle-up"></span> Submit</g-recaptcha-submit-button>
```

### Add ReCaptcha JS Function

By default the `</g-recaptcha-script>` is looking for a javascript function named `onGReCaptchaV3Submit` which accepts one value, the `token` for the current form's processing. You can define this function and form as below:

Javascript function for Google reCAPTCHA support (either on the view/page, or in another script file as src ref):

```javascript
function onGReCaptchaV3Submit(token) {
    
    ...

    // Programmatically submit the identifiable form
    document.querySelector('#myForm').submit();
}
```

... very simple form view concept:

```html
<!-- Identifiable form element -->
<form id="myForm" method="post" asp-antiforgery="true">
    
    ...

    <!-- Setup Google ReCaptcha supported submit button -->
    <g-recaptcha-submit-button></g-recaptcha-submit-button>

</form>

...
<!-- Load Google ReCaptcha script -->
<g-recaptcha-script></g-recaptcha-script>
```

___

## Usage In Action Methods - Verify ReCaptcha (MVC Only)

Inside your MVC controllers we can call to Google's reCAPTCHA API to verify its evaluation of your POST action methods via the `token` that is placed into the form. In the [example here](https://github.com/kef7/GoogleReCaptcha.Core/blob/ff576c554316747fa8dc6c9535d6fb7341f971f3/examples/GoogleReCaptcha.Examples.Mvc/Controllers/HomeController.cs#L25) we have a HomeController which accepts a POST at action method Index(model). We call the verify service like so:

```cs
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Index(HomeModel model)
{
    ...

    // Call Google reCAPTCHA API to verify if bot
    if (!await ReCaptchaService.VerifyAsync())
    {
        // Reject this model's data because it might be a bot's data
        ...
    }

    ...
}
```

The `VerifyAsync()` method, for V3, will determine if the action is valid based on a successful response, and based on if the response's score is greater-than-or-equal-to your passing score supplied by the `DefaultPassingScore` property in your settings. Or you can provide your own with the overloaded, `VerifyAsync(float)`, method.

The call above requires that your controller accepts constructor DI which assigns to a property both based on `IReCaptchaService`, or `IReCaptchaV3Service`, like this:

```cs
public IReCaptchaService ReCaptchaService { get; }

public HomeController(IReCaptchaService reCaptchaService)
{
    ReCaptchaService = reCaptchaService;
}
```

___

## reCAPTCHA v2 Invisible

To use Google's reCAPTCHA v2 invisible variant in this tool you simply setup your project as you would the v3 version except you will configure v2 with v2 settings:

1. Add a call to `AddGoogleReCaptchaV2` in your Startup class' `ConfigureServices` method
1. Configure v2 settings; similar to v3 but possibly under a `GoogleReCaptcha:V2` entry in your `appsettings.json` file. Note v2 settings are different then v3; it supports `Theme` and `Size`, but not `DefaultPassingScore`.
1. Configure your razor pages the same as the v3 version show above this section

___

## reCAPTCHA v2 Widget

To use Google's reCAPTCHA v2 widget version you will need to do the following:

1. Add a call to `AddGoogleReCaptchaV2` in your Startup class' `ConfigureServices` method
1. Configure v2 settings; similar to v3 but possibly under a `GoogleReCaptcha:V2` entry in your `appsettings.json` file. Note v2 settings are different then v3; it supports `Theme` and `Size`, but not `DefaultPassingScore`.
1. Use the script tag helper, `</g-recaptcha-script>`, as normal
1. Use the widget tag helper, `</g-recaptcha-widget>`, on your razor page where you wish the checkbox widget to be shown
1. Apply a submit script/button as normal to submit your form
1. (Optional) To use the ReCaptcha HTML Helpers you will need to (helpers example in example project @ ~/Views/HomeV2/Explicit.cshtml):
    1. Add a call to `UseGoogleReCaptchaHtmlHelperSupport` in your Startup class' `Configure` method
    2. Add an a using for the `GoogleReCaptcha.Core.Mvc` namespace in a view that is on the path of your resulting razor page; possibly in `_ViewImports.cshtml` like: `@using GoogleReCaptcha.Core.Mvc`

Do ***NOT*** use the button tag helper, `</g-recaptcha-submit-button>`, on your razor pages for v2 widget support.

### Widget Tag Helper

The widget tag helper, `</g-recaptcha-widget>`, supports all the data attributes defined by Google. See [Google's reCAPTCHA g-recaptcha tag attributes](https://developers.google.com/recaptcha/docs/display#render_param) documentation for help on their use.

___

## License

This library/package is license under the [`Don't Be A Dick` public license](https://github.com/kef7/GoogleReCaptcha.Core/blob/main/LICENSE.txt). So, you know, don't be one.

___

## Links

- [Published package on NuGet.org](https://www.nuget.org/packages/GoogleReCaptcha.Core/)
- [Google reCAPTCHA documentation](https://developers.google.com/recaptcha/intro)
- [Google reCAPTCHA Admin](https://www.google.com/recaptcha/admin/)
