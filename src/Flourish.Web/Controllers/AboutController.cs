using System.Diagnostics;
using Flourish.Web.ViewModels;
using Flourish.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace Flourish.Web.Controllers;

/// <summary>
/// About controller.
/// </summary>

public class AboutController : Controller
{
  private readonly IFeatureManager _featureManager;

  public AboutController(IFeatureManager featureManager)
  {
    _featureManager = featureManager;
  }

  [FeatureGate(Flags.TellMeAboutIt)]
  public IActionResult Index()
  {
    return View();
  }

}
