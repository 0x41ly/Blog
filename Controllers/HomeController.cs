using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using Blog.Data.FileManager;
using Blog.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Blog.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SignInManager<BlogUser> _signInManager;
    private readonly UserManager<BlogUser> _userManager;
    private IRepository _repo;
    private IFileManager _fileManager;

    public HomeController(ILogger<HomeController> logger,
        SignInManager<BlogUser> signInManager,
        UserManager<BlogUser> userManager,
        IRepository repo,
        IFileManager fileManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _repo = repo;
        _fileManager = fileManager;
    }

    public IActionResult Index(int pageNumber, string category, string search)
    {

        var vm = _repo.GetIndexViewModel(pageNumber, category, search, _userManager.GetUserId(User));
        return View(vm);
    }
   [Authorize]
    public IActionResult Article(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        Guid Id = id.Value;
        var vm = _repo.GetArticleViewModel(Id);
        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
