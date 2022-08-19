using System.Diagnostics;
using Flourish.Web.ViewModels;
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

    if (showMeTheMoneyEnabled)
    {
      ViewBag.ShowMeTheMoneyEnabled = true;
    }

    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

public enum Flags
{
  ShowMeTheMoney
} 
