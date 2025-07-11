using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ShippingSystem.API.Helper;

using Microsoft.EntityFrameworkCore;

using ShippingSystem.API.LookUps;
using ShippingSystem.BL.Repositories;
using ShippingSystem.Core.DTO.Order;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork unit;
        private readonly IMapper mapper;

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unit = unitOfWork;
            this.mapper = mapper;
        }
        private async Task<decimal> CalculateTotalPriceAsync(AddOrderDTO orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
                throw new InvalidOperationException("Order must contain at least one item.");

            var calculatedWeight = orderDto.OrderItems.Sum(item => item.Weight * item.Quantity);
            var calculatedItemsPrice = orderDto.OrderItems.Sum(item => item.Price * (decimal)item.Quantity);

            if (orderDto.TotalWeight <= 0 || orderDto.TotalWeight != calculatedWeight)
                throw new InvalidOperationException("Total weight is invalid.");

            if (orderDto.TotalPrice != calculatedItemsPrice)
                throw new InvalidOperationException("Total price does not match the sum of item prices.");

            decimal totalPrice = calculatedItemsPrice;

            var weightSetting = (await unit.WeightSettingRepository.GetAll()).FirstOrDefault();
            if (weightSetting == null)
                throw new InvalidOperationException("Weight settings are not configured.");

            double maxWeight = double.Parse(weightSetting.WeightRange);
            if (orderDto.TotalWeight > maxWeight)
            {
                double extraWeight = orderDto.TotalWeight - maxWeight;
                totalPrice += (decimal)(extraWeight * weightSetting.ExtraPrice);
            }

            var shippingType = await unit.ShippingTypeRepository.GetById(orderDto.ShippingTypeId);
            if (shippingType == null)
                throw new InvalidOperationException("Invalid shipping type.");
            totalPrice += shippingType.ShippingPrice;

            var city = await unit.CityRepository.GetById(orderDto.CityId);
            if (city == null)
                throw new InvalidOperationException("Invalid city.");
            totalPrice += city.Price;

            if (orderDto.IsShippedToVillage)
            {
                //if (string.IsNullOrWhiteSpace(orderDto.Address))
                //    throw new InvalidOperationException("Address is required.");
                totalPrice += 10m;
            }

            return totalPrice;
        }

        //   [HttpGet("GetAllOrders")]
        //   public async Task<IActionResult> GetAllOrders(
        //[FromQuery] int? status,
        //[FromQuery] int pageNumber = 1,
        //[FromQuery] int pageSize = 4)
        //   {
        //       if (pageNumber <= 0 || pageSize <= 0)
        //           return BadRequest("Page number and page size must be greater than zero.");

        //       // Fetch and optionally filter orders by status
        //       var allOrders = await unit.OrderRepository.GetAll(); // You can optimize with a filtered query in your repo
        //       if (status.HasValue)
        //       {
        //           allOrders = allOrders.Where(o => o.StatusId == (int)status.Value).ToList();
        //       }

        //       // Total count for pagination metadata
        //       var totalCount = allOrders.Count;
        //       var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        //       // Apply pagination
        //       var pagedOrders = allOrders
        //           .Skip((pageNumber - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //       if (!pagedOrders.Any())
        //           return NotFound("No orders found for the specified criteria.");

        //       // Map to DTOs
        //       var result = mapper.Map<List<OrderDTO>>(pagedOrders);

        //       // Return result with pagination metadata
        //       return Ok(new
        //       {
        //           data = result,
        //           pageNumber,
        //           pageSize,
        //           totalPages,
        //           totalCount
        //       });
        //   }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders(
    [FromQuery] int? status,
    [FromQuery] string? searchTerm,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 4)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than zero.");

            // 1. Fetch IQueryable to filter on DB level (أفضل أداء)
            var query = await unit.OrderRepository.GetQueryable(); // لازم تجهزي دي في الريبو لو مش موجودة

            // 2. Filter by status if provided
            if (status.HasValue)
            {
                query = query.Where(o => o.StatusId == status.Value);
            }

            // 3. Apply search filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(o =>
                    o.CustomerName.ToLower().Contains(searchTerm) ||
                    o.Vendor.Name.ToLower().Contains(searchTerm) ||
                    o.Status.Name.ToLower().Contains(searchTerm) ||
                    o.City.Name.ToLower().Contains(searchTerm) 
                   
                );
            }

            // 4. Count before pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // 5. Pagination
            var pagedOrders = await query
                .OrderByDescending(o => o.Vendor.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 6. Mapping to DTO
            var result = mapper.Map<List<OrderDTO>>(pagedOrders);

            return Ok(new
            {
                data = result,
                pageNumber,
                pageSize,
                totalPages,
                totalCount
            });
        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] AddOrderDTO orderDto)
        {
            // Basic null and model validation
            if (orderDto == null) return BadRequest("Order data is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
                return BadRequest("At least one order item is required.");
            // Vendor check
            var vendor = await unit.VendorRepository.FindByNameAsync(orderDto.VendorName);

           //to attach order by vendor id not vendor name
            var userId= User.FindFirst("id")?.Value;
            //var vendor = await unit.VendorRepository.FindByUserIdAsync(userId);
            if (vendor == null)
                return BadRequest("Vendor not found. Please ensure the vendor exists before placing an order.");
            orderDto.VendorId = vendor.Id;

            try
            {
                orderDto.TotalPrice = await CalculateTotalPriceAsync(orderDto);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
            // Map and save order
            var order = mapper.Map<Order>(orderDto);
            order.TotalCost = (double)orderDto.TotalPrice;
            order.CreationDate = DateTime.Now;
            order.OrderType = "Normal";
            order.PaymentType = "Cash";
            order.StatusId = (int)orderDto.StatusId;

            await unit.OrderRepository.Add(order);
            await unit.SaveAsync(); // Generate order.Id

            // Map and save products
            var products = orderDto.OrderItems.Select(item =>
            {
                var product = mapper.Map<Product>(item);
                product.OrderId = order.Id;
                return product;
            }).ToList();

            foreach (var product in products)
                await unit.ProductRepository.Add(product);

            await unit.SaveAsync();

            return Ok(new { message = "Order created successfully", orderId = order.Id });
        }

        [HttpGet("GetOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await unit.OrderRepository.GetOrderByID(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
          
            if (order == null)
                return NotFound();

            var dto = new AllOrderDTO
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone1 = order.CustomerPhone1,
                CustomerPhone2 = order.CustomerPhone2,
                Email = order.EmailAddress,
                GovernmentId = order.City?.GovernmentId ?? 0,
                CityId = order.CityId,
                Address = order.Address,
                IsShippedToVillage = order.IsShippedToVillage,
                ShippingTypeId = order.ShippingTypeId,
                VendorName = order.Vendor?.Name??"",
                VendorAddress = order.Vendor?.Address ?? "",
                VendorId = order.VendorId,
                StatusId = order.StatusId,
                TotalPrice = (decimal)order.TotalCost,
                Notes = order.Notes,
                TotalWeight = order.TotalWeight,
                OrderItems = order.Products.Select(p => new loadOrderItemDTO
                {
                    ProductName = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Weight = p.Weight
                }).ToList()
            };


            Console.WriteLine($"Order Email: {order.EmailAddress}");
            Console.WriteLine($"GovernmentId: {order.City}");
            Console.WriteLine($"VillageName: {order}");
            //Console.WriteLine($"TotalPrice: {order.TotalPrice}");

            //return Ok(mapper.Map<AllOrderDTO>(order));

            return Ok(dto);
        }


        [HttpPut("UpdateOrder/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDTO orderDto)
        {
            if (orderDto == null) return BadRequest("Order data is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingOrder = await unit.OrderRepository.GetOrderByID(id);
            if (existingOrder == null)
                return NotFound($"Order with ID {id} not found.");

            // ✅ حدث البيانات
            mapper.Map(orderDto, existingOrder);
            existingOrder.CreationDate = DateTime.Now;
            existingOrder.OrderType = "Normal";
            existingOrder.PaymentType = "Cash";

            await unit.OrderRepository.Update(existingOrder);
            await unit.SaveAsync();

            // ✅ حذف المنتجات القديمة
            var deletedProducts = await unit.ProductRepository.GetProductsByOrderId(existingOrder.Id);
            foreach (var item in deletedProducts)
                await unit.ProductRepository.Delete(item);

            // ✅ إضافة المنتجات الجديدة
            var products = orderDto.OrderItems.Select(item =>
            {
                var product = mapper.Map<Product>(item);
                product.OrderId = existingOrder.Id;
                return product;
            }).ToList();

            foreach (var product in products)
                await unit.ProductRepository.Add(product);

            await unit.SaveAsync();

            // ✅ استخدم DTO في الإرجاع
            var resultDto = mapper.Map<OrderDTO>(existingOrder);
            return Ok(resultDto);
        }


        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Retrieve the order by ID
            var order = await unit.OrderRepository.GetById(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            // Delete related products first (to avoid foreign key constraints)
            var products = await unit.ProductRepository.GetProductsByOrderId(id); // Ensure this method exists
            if (products != null && products.Any())
            {
                foreach (var product in products)
                {
                    await unit.ProductRepository.Delete(product);
                }
            }

            // Delete the order
            await unit.OrderRepository.Delete(order);
            await unit.SaveAsync();

            return Ok(new { message = "Order deleted successfully", orderId = id });
        }
        [HttpPut("UpdateOrderStatus/{orderId}/{statusId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, int statusId)
        {
            // Validate the status ID against the OrderStatus enum
            if (!Enum.IsDefined(typeof(OrderStatus), statusId))
                return BadRequest("Invalid status ID.");

            // Fetch the order from the repository
            var order = await unit.OrderRepository.GetById(orderId);
            if (order == null)
                return NotFound($"Order with ID {orderId} not found.");

            // Update status
            order.StatusId = statusId;
            await unit.OrderRepository.Update(order);
            await unit.SaveAsync();

            return Ok(new { message = "Order status updated successfully", orderId, newStatus = ((OrderStatus)statusId).ToString() });
        }

        [HttpPut("EmployeeAssignOrderToDeliveyMan/{OrderID}/{DeliveryManId}")]
        [Authorize]
        public async Task<IActionResult> AssignOrderToDeliveryMan(int orderId, int deliveryManId)
        {
            int employeeId = GetLoggedInEmployeeId();

            if (employeeId == 0)
                return Unauthorized("Employee not identified from token.");

            var assignment = new EmployeeAssignOrderToDelivery
            {
                OrderID = orderId,
                DeliveryID = deliveryManId,
                EmployeeID = employeeId
            };

            await unit.EmployeeAssignedOrderToDeliveryRepository.Add(assignment);
            var order = await unit.OrderRepository.GetById(orderId);
            if (order == null)
                return NotFound($"Order with ID {orderId} not found.");
            order.StatusId = (int)OrderStatus.Assigned;
            await unit.SaveAsync();

            return Ok("Order assigned successfully.");
        }

        private int GetLoggedInEmployeeId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId");
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        // track orders
        [HttpGet("TrackORders")]
        [Authorize]
        public async Task<IActionResult> TrackOrders([FromQuery] int? statusId)
        {
            //var userRole = User.FindFirst("role")?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst("id")?.Value;
           

            Console.WriteLine("Role from token: " + userRole);
            Console.WriteLine("UserId from token: " + userId);
            //int vendorId = 23;
            IEnumerable<Order> orders = new List<Order> ();
             switch (userRole?.ToLower())
            {
                case "vendor":
                    orders = await unit.OrderRepository.GetOrdersByVendorId(userId);
                    break;
                case "deliveryman":
                    var deliveryMan = await unit.DeliveryManRepository.FindByUserIdAsync(userId);
                    if(deliveryMan!= null)
                    orders = await unit.OrderRepository.GetOrderAssignedToDeliveryMan(deliveryMan.Id);

                    break;
                case "admin":
                case "employee":
                    orders = await unit.OrderRepository.GetAllWithVendorNames();

                    break;
                 
            }

            if(statusId.HasValue)
                orders = orders.Where(o=>o.StatusId == statusId.Value);

            var result =mapper.Map<List<OrderDTO>>(orders);
            return Ok(result);

        }

        [HttpGet("ExportInvoice/{orderId}")]
        public async Task<IActionResult> ExportInvoice(int orderId)
        {
            var order = await unit.OrderRepository.GetByIdWithProducts(orderId);
            if (order == null) return NotFound("Order not found.");

            var weightSetting = (await unit.WeightSettingRepository.GetAll()).FirstOrDefault();
            var pdfBytes = InvoicePdfHelper.Generate(order, weightSetting);

            return File(pdfBytes, "application/pdf", $"Invoice_Order_{orderId}.pdf");
        }




    }


}
