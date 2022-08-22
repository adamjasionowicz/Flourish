using System.Diagnostics;
using Flourish.Web.ViewModels;
using Flourish.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace Flourish.Web.Controllers;

/// <summary>
/// A sample MVC controller that uses views.
/// </summary>
public class HomeController : Controller
{
  private readonly IFeatureManager _featureManager;

  public HomeController(IFeatureManager featureManager)
  {
    _featureManager = featureManager;
  }

  public async Task<IActionResult> Index()
  {
    var showMeTheMoneyEnabled = await _featureManager.IsEnabledAsync(nameof(Flags.ShowMeTheMoney));
    var acceptContactEnabled = await _featureManager.IsEnabledAsync(nameof(Flags.AcceptContact));
    var tellMeAboutItEnabled = await _featureManager.IsEnabledAsync(nameof(Flags.TellMeAboutIt));

    // Main money making feature flag conditions
    if (showMeTheMoneyEnabled)
    {
      ViewBag.ShowMeTheMoneyEnabled = true;
    }

    if (!showMeTheMoneyEnabled)
    {
      ViewBag.ShowMeTheMoneyEnabled = false;
    }

    // Contact Us nav feature flag conditions
    if (acceptContactEnabled)
    {
      ViewBag.AcceptContact = true;
    }

    if (!acceptContactEnabled)
    {
      ViewBag.AcceptContact = false;
    }

    // About nav feature flag conditions
    if (tellMeAboutItEnabled)
    {
      ViewBag.TellMeAboutIt = true;
    }

    if (!tellMeAboutItEnabled)
    {
      ViewBag.TellMeAboutIt = false;
    }

    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
