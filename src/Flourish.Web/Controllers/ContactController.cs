using System.Diagnostics;
using Flourish.Web.ViewModels;
using Flourish.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace Flourish.Web.Controllers;

/// <summary>
/// Contact controller.
/// </summary>
[FeatureGate(Flags.AcceptContact)]
public class ContactController : Controller
{
  private readonly IFeatureManager _featureManager;

  public ContactController(IFeatureManager featureManager)
  {
    _featureManager = featureManager;
  }

  public async Task<IActionResult> IndexAsync()
  {
    var showMeTheMoneyEnabled = await _featureManager.IsEnabledAsync(nameof(Flags.ShowMeTheMoney));

    // Main money making feature flag conditions
    if (showMeTheMoneyEnabled)
    {
      ViewBag.ShowMeTheMoneyEnabled = true;
    }

    if (!showMeTheMoneyEnabled)
    {
      ViewBag.ShowMeTheMoneyEnabled = false;
    }

    return View();
  }

}
