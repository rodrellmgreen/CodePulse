﻿using CodePulse.API.Models.DTO;
using CodePulse.API.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CodePulse.API.Data;
using CodePulse.API.Repositories.Interface;
using Azure.Core;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;

namespace CodePulse.API.Controllers
{
    // https://localhost:xxxx/api/categories
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        // 
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequestDto request)
        {
            // Map DTO to Domain Model
            var category = new Category
            {
                Name = request.Name,
                UrlHandle= request.UrlHandle,
            };

            await categoryRepository.CreateAsync(category);

    

            //Domain model to DTO

            var response = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,

            };

            return Ok(response);

        }

        //Get: /api/categories
        [HttpGet]
        
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await categoryRepository.GetAllAsync();

            //Map Domain model to Dto
            var response = new List<CategoryDTO>();
            foreach(var category in categories)
            {
                response.Add(new CategoryDTO { Id = category.Id,Name = category.Name,UrlHandle = category.UrlHandle });
            }
            return Ok(response);
        }

        //GET: /api/categories/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute]Guid id)
        {
           var existingCategory = await categoryRepository.GetById(id);
            if(existingCategory == null)
            {
                return NotFound();
            }
            var response = new CategoryDTO { Id = existingCategory.Id, Name = existingCategory.Name, UrlHandle = existingCategory.UrlHandle };
              return Ok(response);
        }

        //PUT: /api/categories/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> EditCategory([FromRoute] Guid id, UpdateCategoryRequestDTO request)
        {
            //Convert DTO to Domain Model
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle,
            };
            category = await categoryRepository.UpdateAsync(category);
            if(category == null)
            {
                return NotFound();
            }
            //Convert Domain Model to DTO
            var response = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };
            return Ok(response);
        }

        //Delete: /api/categories/{id}
        [HttpDelete]
        [Route(template:"{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
           var category = await categoryRepository.DeleteAsync(id);
            if(category is null)
            {
                return NotFound();
            }
            //Convert Domain model to Dto on response
            var response = new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle,
            };
            return Ok(response);
        }
    }

}
