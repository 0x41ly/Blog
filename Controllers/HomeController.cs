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
    public async Task<IActionResult> Article(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        Guid Id = id.Value;
        var user = await _userManager.GetUserAsync(User);
        var UserId = await _userManager.GetUserIdAsync(user);
        _repo.AddView(Id, UserId);
        var vm = _repo.GetArticleViewModel(Id);
        if (vm.NotFound)
        {
            return NotFound();
        }
        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddArticle([Bind("Title"
                                                    , "GenreName"
                                                    , "Categories"
                                                    , "Level"
                                                    , "Description"
                                                    ,"Content")] Article article)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var UserId = await _userManager.GetUserIdAsync(user);
            if (User.IsInRole("BlogOwner") | User.IsInRole("Admin"))
            {
                await PostArticle(article, UserId);
                return RedirectToAction("Article", new { id = article.ArticleId });
            }
            if (user.PlanType == "Premium")
            {
                if (_repo.IsAllowedToPost(UserId))
                {
                    await PostArticle(article, UserId);
                    return RedirectToAction("Article", new { id = article.ArticleId });
                }
                else
                {
                    TempData["Message"] = "warning: You have exceeded your monthly allowed articles";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Message"] = "warning: Only premiums who can post an article";
                return RedirectToAction("Index");
            }
        }
        TempData["Message"] = "danger: Something went wrong";
        return RedirectToAction("Index");
    }

    private async Task PostArticle(Article article, string UserId)
    {
        article.ArticleId = Guid.NewGuid();
        article.Title = System.Net.WebUtility.HtmlEncode(article.Title);
        article.Description = System.Net.WebUtility.HtmlEncode(article.Description);
        article.GenreName = System.Net.WebUtility.HtmlEncode(article.GenreName);
        article.Created = DateTime.Now;
        article.Categories = System.Net.WebUtility.HtmlEncode(article.Categories);
        article.Content = System.Net.WebUtility.HtmlEncode(article.Content);
        article.Level = System.Net.WebUtility.HtmlEncode(article.Level);
        article.AuthorId = UserId;
        _repo.AddArticle(article);
        await _repo.SaveChangesAsync();
        TempData["Message"] = "success: Successfully Created a new article";

    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comment([Bind("ArticleId,ParentId,Message")] Comment comment)
    {
        if (ModelState.IsValid)
        {
            comment.Message = System.Net.WebUtility.HtmlEncode(comment.Message);
            comment.CommentId = Guid.NewGuid();
            comment.Created = DateTime.Now;
            var user = await _userManager.GetUserAsync(User);
            comment.AuthorId = await _userManager.GetUserIdAsync(user);
            if (comment.ParentId == Guid.Empty)
            {
                comment.level = 0;
            }
            else
            {
                comment.level = _repo.GetCommentlevelByID(comment.ParentId) + 1 ;
            }
            _repo.AddComment(comment);
            await _repo.SaveChangesAsync();

            TempData["Message"] = "success: Successfully Added The comment";
            return RedirectToAction("Article", new { id = comment.ArticleId });
        }

        TempData["Message"] = "danger: Something went wrong";
        return RedirectToAction("Article", new { id = comment.ArticleId });
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
