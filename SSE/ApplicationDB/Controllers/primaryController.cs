using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

using MTFramework.Utilities;
using SSE.ApplicationDB.Models;
using SSE.ApplicationDB.Data;

namespace SSE.ApplicationDB.Controllers
{
    /// <summary>
    /// Application interface itself
    /// </summary>
    [ApiController]
    [Route("v1")] // "primarypath/[controller]")]
    [Produces("application/json")]
    public class primaryController : MTFControllerBase
    {
        private AppDB_Context _appdb { get; init; }
        public primaryController(ILogger<primaryController> logger,
                                 AppDB_Context appdb)
            : base(logger)
        {
            _appdb = appdb;
        }
        /// <summary>
        /// Return a products by code.
        /// </summary>
        /// <param name="id">Product Code</param>
        /// <returns>json object for a product</returns>
        /// <response code="200">Product returned, json object for a product</response>
        /// <response code="404">Product not registered</response>
        /// <response code="500">Operation error - please contact your technical support</response>
        [HttpGet("product/{id}")]
        public async Task<IActionResult> productGetByIdAsync (
                                                        [FromRoute] int? id
                                                                 )
        {
            try
            {
                if (id == null || id<=0) return BadRequest($"{nameof(id)} cannot be empty and should be greater then zero");

                sseProducts_v res = (await _appdb._sseProducts_v.FromSqlRaw("call product_get_byid ({0});",
                                                                                  id)
                                                                .AsNoTracking()
                                                                .ToListAsync())
                                    .FirstOrDefault();

                if (res == null) return NotFound(id);

                return Ok(res);
            }
            catch(Exception ex)
            {
                return exceptionResult(ex, "");
            }
        }
        /// <summary>
        /// Return a list of all products, registered in the system.
        /// </summary>
        /// <returns>json list of objects for a products</returns>
        /// <response code="200">Products returned, returns json list of objects for a products</response>
        /// <response code="500">Operation error - please contact your technical support</response>
        [HttpGet("products")]
        public async Task<IActionResult> productsGetAllAsync()
        {
            try
            {
                List<sseProducts_v> res = (await _appdb._sseProducts_v.FromSqlRaw("call products_get_all ();")
                                                                .AsNoTracking()
                                                                .ToListAsync());

                return Ok(res);
            }
            catch (Exception ex)
            {
                return exceptionResult(ex, "");
            }
        }
        /// <summary>
        /// Update a product using posted form data.
        /// </summary>
        /// <param name="id">Product Code</param>
        /// <param name="name">String with name of a product</param>
        /// <param name="price">String with a text presentation of product price</param>
        /// <returns>json object for a product updated</returns>
        /// <response code="200">Product updated, returns json object for an updated product</response>
        /// <response code="400">Illegal parameters</response>
        /// <response code="500">Operation error - please contact your technical support</response>
        [HttpPut("product/{id}")]
        public async Task<IActionResult> productUpdateByIdAsync(
                                                        [FromRoute] int? id,            
                                                        [FromForm] string name,
                                                        [FromForm] string price
                                                          )
        {
            try
            {
                decimal decPrice = 0m;
                if (id == null || id <= 0) return BadRequest($"{nameof(id)} cannot be empty and should be greater then zero");
                if (String.IsNullOrEmpty(name)) name = String.Empty;
                if (String.IsNullOrEmpty(price))
                {
                    decPrice = -1;
                }
                else
                {
                    if (!(Decimal.TryParse(price, out decPrice))) return BadRequest($"{nameof(price)} shoul be number presentation");
                    if ((decPrice <= 0)) return BadRequest($"{nameof(price)} should be greater then zero");
                }

                var rc = await _appdb.getScalarValue("call product_update_byid", id, name, decPrice);

                if (rc.RetValueInt != 0) return NotFound(id);

                return Ok(new { id = id, name = name, price = decPrice });
            }
            catch (Exception ex)
            {
                return exceptionResult(ex, "");
            }
        }
        /// <summary>
        /// Create a new product using posted form data.
        /// </summary>
        /// <param name="name">String with name of a product</param>
        /// <param name="price">String with a text presentation of product price</param>
        /// <returns>json object for a product created</returns>
        /// <response code="200">Product added, returns json object for an added product</response>
        /// <response code="400">Illegal parameters</response>
        /// <response code="500">Operation error - please contact your technical support</response>
        [HttpPost("product")]
        public async Task<IActionResult> productAddAsync(
                                                        [FromForm] string name,
                                                        [FromForm] string price
                                                          )
        {
            try
            {
                decimal decPrice = 0m;

                if (String.IsNullOrEmpty(name)) return BadRequest($"{nameof(name)} cannot be empty");
                if (String.IsNullOrEmpty(price) || !(Decimal.TryParse(price, out decPrice))) return BadRequest($"{nameof(price)} cannot be empty and shoul be correct number");
                if ((decPrice <= 0)) return BadRequest($"{nameof(price)} should be greater then zero");

                var rc = await _appdb.getScalarValue("call product_add", name, decPrice);

                return Ok(new { id = rc.RetValueInt, name = name, price = decPrice });
            }
            catch (Exception ex)
            {
                return exceptionResult(ex, "");
            }
        }
        /// <summary>
        /// Delete a products by code.
        /// </summary>
        /// <param name="id">Product Code</param>
        /// <returns>json object for a deleted product</returns>
        /// <response code="200">Product deleted, returns json object for a deleted product</response>
        /// <response code="404">Product not registered</response>
        /// <response code="500">Operation error - please contact your technical support</response>
        [HttpDelete("product/{id}")]
        public async Task<IActionResult> productDeleteByIdAsync(
                                                        [FromRoute] int? id
                                                                 )
        {
            try
            {
                if (id == null || id <= 0) return BadRequest($"{nameof(id)} cannot be empty and should be greater then zero");

                sseProducts_v res = (await _appdb._sseProducts_v.FromSqlRaw("call product_delete_byid ({0});",
                                                                                  id)
                                                                .AsNoTracking()
                                                                .ToListAsync())
                                    .FirstOrDefault();

                if (res == null) return NotFound(id);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return exceptionResult(ex, "");
            }
        }
    }
}
