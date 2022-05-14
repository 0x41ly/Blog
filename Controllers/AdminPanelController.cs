
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using Blog.Data.FileManager;
using Blog.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Blog.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers;


[Authorize(Roles = "Admin")]
public class AdminPanelController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SignInManager<BlogUser> _signInManager;
    private readonly UserManager<BlogUser> _userManager;
    private IRepository _repo;
    private IFileManager _fileManager;

    public AdminPanelController(ILogger<HomeController> logger,
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

    public IActionResult Index()
    {

        var vm = "";
        return View(vm);
    }
}