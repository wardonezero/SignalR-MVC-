using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRMVC.Data;
using SignalRMVC.Hubs;
using SignalRMVC.Models;

namespace SignalRMVC.Controllers;

public class HomeController(IHubContext<DeathlyHallowsHub> deathlyHub, ApplicationDbContext context) : Controller
{
    private readonly IHubContext<DeathlyHallowsHub> _deathlyHub = deathlyHub;
    private readonly ApplicationDbContext _context = context;

    public async Task <IActionResult> VoteDeathlyHallows(string type)
    {
        try
        {
            var deathlyHallowType = Enum.Parse<DeathlyHallowType>(type);
            if (StaticDetails.DealthyHallowRace.TryGetValue(deathlyHallowType, out int value))
            {
                StaticDetails.DealthyHallowRace[deathlyHallowType] = ++value;
            }
            else
            {
                return BadRequest("Hallow type not found in the race dictionary.");
            }
            await _deathlyHub.Clients.All.SendAsync("UpdateDeathlyhallowsCount",
                StaticDetails.DealthyHallowRace[DeathlyHallowType.Cloak],
                StaticDetails.DealthyHallowRace[DeathlyHallowType.Stone],
                StaticDetails.DealthyHallowRace[DeathlyHallowType.Wand]);
        }
        catch (ArgumentException)
        {
            return View("DeathlyHallows");
        }
        return View("DeathlyHallows");

    }

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    public IActionResult DeathlyHallows() => View();

    public IActionResult HarryPotter() => View();

    public IActionResult Notifications() => View();

    public IActionResult BasicChat() => View();

    [ActionName("Order")]
    public IActionResult Order()
    {
        string[] name = ["Charlotte", "Evelyn", "Ava", "Sophia", "Isabella"];
        string[] itemName = ["Pizza", "Hamburger", "Spaghetti", "Pancakes", "Sushi"];

        Random rand = new();
        // Generate a random index less than the size of the array.  
        int index = rand.Next(name.Length);

        Order order = new()
        {
            Name = name[index],
            ItemName = itemName[index],
            Count = index
        };

        return View(order);
    }

    [ActionName("Order")]
    [HttpPost]
    public async Task<IActionResult> OrderPost(Order order)
    {

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Order));
    }

    [ActionName("OrderList")]
    public IActionResult OrderList() => View();

    public IActionResult GetAllOrder()
    {
        var productList = _context.Orders.ToList();
        return Json(new { data = productList });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}