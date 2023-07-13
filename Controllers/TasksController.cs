using LearnSmartCoding.TaskManager.WebApp.Core;
using Microsoft.AspNetCore.Mvc;

namespace LearnSmartCoding.TaskManager.WebApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TasksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var apiBaseUrl = "https://lsc-taskmanagerapi.azurewebsites.net";
            var userId = "LSC";
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{apiBaseUrl}/api/Tasks/user/{userId}/tasks");//7128,32777

            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<IEnumerable<TasksDocument>>();
                return View(tasks);
            }
            else
            {
                // Handle the error response
                return View("Error");
            }
        }
    }

}
