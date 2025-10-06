using Microsoft.AspNetCore.SignalR;

namespace CampusLearn.PrivateMessaging.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetResponse()
    {
        try
        {
            return Ok("Hello, This is the Messages API.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message.ToString()}");
        }
    }
}


public class CounterHub : Hub
{
    private static int _counter = 0;

    public async Task IncrementCounter()
    {
        _counter++;
        await Clients.All.SendAsync("CounterUpdated", _counter);
    }

    public async Task DecrementCounter()
    {
        _counter--;
        await Clients.All.SendAsync("CounterUpdated", _counter);
    }

    public async Task ResetCounter()
    {
        _counter = 0;
        await Clients.All.SendAsync("CounterUpdated", _counter);
    }

    public async Task GetCurrentCount()
    {
        await Clients.Caller.SendAsync("CounterUpdated", _counter);
    }
}