using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    // Endpoint => URL 
    // http://localhost: 5000
    // https://localhost: 5001

    [Route("v1/categories")]
    public class CateroryController: ControllerBase
    {

        //https://localhost:5001/categories
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader= "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        //Define que o método não tem cache.
        //[ResponseCache(Duration =0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        } 

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return category;    
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody] Category model, [FromServices] DataContext context) 
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            
            try {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }catch(Exception){
                return BadRequest(new { message = "não foi possível criar a categoria"});
            }

        }


        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Put(
            int id, [FromBody] Category model, [FromServices] DataContext context) 
        {
            if (id != model.Id)
            {
            return NotFound(new { message = "Categoria não encontrada"});
            }

            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            try{

                //Alterando o state da entidade category para Modified.
                // o EF irá validar todas as propriedade de Caterogy pra ver o que foi alterado para ser alterado para
                // no banco
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch(DbUpdateConcurrencyException){
                return BadRequest(new {message = "Este registro já foi atualizado"});
            }
            catch(Exception){
                return BadRequest(new {message = "não foi possível atualizar a categoria"});
            }
        }



        [HttpDelete()]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Delete(
            int id, [FromServices] DataContext context) 
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(category == null){
                return NotFound(new { message = "Categoria não encontrada."});
            }

            try {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new {message = "Categoria removida com sucesso"});
            }
            catch
            {
                return BadRequest(new {message= "Não foi possível remover a categoria"});
            }

        } 
    }
}