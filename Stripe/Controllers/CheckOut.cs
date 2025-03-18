using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Stripe.Checkout;
using Stripe.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stripe.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckOutController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            var courses = new List<Course>
            {
                new Course
                {
                    CourseId = 27,
                    CourseName = "Algorithms and Data Structures",
                    Department = "Computer Science",
                    Price = 300,
                    Synopses = "You will learn an in-depth approach to algorithms and data structuring techniques."
                },
                new Course
                {
                    CourseId = 28,
                    CourseName = "Operating Systems",
                    Department = "Computer Science",
                    Price = 300,
                    Synopses = "You will learn the study of fundamental operating system concepts, multitasking, and memory management."
                },
                new Course
                {
                    CourseId = 29,
                    CourseName = "Network Security",
                    Department = "Information Technology",
                    Price = 3000,
                    Synopses = "You will learn the principles and practices of securing computer networks."
                },
                new Course
                {
                    CourseId = 29,
                    CourseName = "Database Management Systems",
                    Department = "Information Technology",
                    Price = 30003,
                    Synopses = "You will learn about the introduction to database design and the use of database management systems for applications."
                },
            };

            return View(courses);
        }

        public IActionResult CheckOut()
        {
            List<Course> courses = new List<Course>
            {
                new Course
                {
                    CourseId = 27,
                    CourseName = "Algorithms and Data Structures",
                    Department = "Computer Science",
                    Price = 300,
                    Synopses = "You will learn an in-depth approach to algorithms and data structuring techniques."
                },
                new Course
                {
                    CourseId = 28,
                    CourseName = "Operating Systems",
                    Department = "Computer Science",
                    Price = 300,
                    Synopses = "You will learn the study of fundamental operating system concepts, multitasking, and memory management."
                },
                new Course
                {
                    CourseId = 29,
                    CourseName = "Network Security",
                    Department = "Information Technology",
                    Price = 3000,
                    Synopses = "You will learn the principles and practices of securing computer networks."
                },
                new Course
                {
                    CourseId = 29,
                    CourseName = "Database Management Systems",
                    Department = "Information Technology",
                    Price = 30003,
                    Synopses = "You will learn about the introduction to database design and the use of database management systems for applications."
                },
            };
            return View(courses);
        }
        public IActionResult Error()
        {
            // Handle errors here, e.g., display an error view
            return View();
        }

        public async Task<IActionResult> Enroll(string userId, int courseId, string stripeToken)
        {
            var response = await _httpClient.PostAsJsonAsync("https://serverapi-trev.azurewebsites.net/api/Courses/Enroll", new { userId, courseId, stripeToken });

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Success");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public IActionResult Success()
        {
            // Handle successful enrollment here, e.g., display a success view
            return View();
        }
    }
}