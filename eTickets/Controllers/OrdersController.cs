﻿using E_Commerce_Movies.Data.Cart;
using E_Commerce_Movies.Data.Services;
using E_Commerce_Movies.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Commerce_Movies.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {

        private readonly IMoviesRepo _moviesRepo;
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrdersRepo _ordersRepo;

        public OrdersController(IMoviesRepo moviesRepo, ShoppingCart shoppingCart, IOrdersRepo ordersRepo)
        {
            _moviesRepo = moviesRepo;
            _shoppingCart = shoppingCart;
            _ordersRepo = ordersRepo;
        }



        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);               //get the id of the Authenticated User
            string userRole = User.FindFirstValue(ClaimTypes.Role);                        //get the Role of the Authenticated User

            var orders = await _ordersRepo.GetOrdersByUserIdAsync(userId, userRole);
            return View(orders);
        }


        public IActionResult ShoppingCart()
        {
            _shoppingCart.ShoppingCartItems = _shoppingCart.GetShoppingCartItems();            // to get the values from GetShoppingCartItems() method to the list property in ShoppingCart Class to showed

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()

            };


            return View(response);
        }

        public async Task<RedirectToActionResult> AddItemToShoppingCart(int id)
        {
            var item = await _moviesRepo.GetMovieByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.AddItemToCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await _moviesRepo.GetMovieByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> CompleteOrder()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            await _ordersRepo.StoreOrderAsync(items, userId, userEmailAddress);
            await _shoppingCart.ClearShoppingCartAsync();


            return View("OrderCompleted");


        }

    }
}
