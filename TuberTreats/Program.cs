using TuberTreats.Models;
using TuberTreats.Models.DTOs;

List<TuberDriver> tuberDrivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "John Doe" },
    new TuberDriver { Id = 2, Name = "Jane Smith" },
    new TuberDriver { Id = 3, Name = "Tom Green" }
};

List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Alice Johnson", Address = "123 Elm St" },
    new Customer { Id = 2, Name = "Bob Lee", Address = "456 Oak St" },
    new Customer { Id = 3, Name = "Charlie Brown", Address = "789 Pine St" },
    new Customer { Id = 4, Name = "Diana Prince", Address = "321 Maple St" },
    new Customer { Id = 5, Name = "Eve Adams", Address = "654 Birch St" }
};

List<Topping> toppings = new List<Topping>
{
    new Topping { Id = 1, Name = "Cheese" },
    new Topping { Id = 2, Name = "Bacon" },
    new Topping { Id = 3, Name = "Sour Cream" },
    new Topping { Id = 4, Name = "Chives" },
    new Topping { Id = 5, Name = "Butter" }
};

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = DateTime.Now.AddDays(1)
    },
    new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 2,
        TuberDriverId = 2,
        DeliveredOnDate = DateTime.Now.AddDays(1)
    },
    new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 3,
        TuberDriverId = 3,
        DeliveredOnDate = DateTime.Now.AddDays(1)
    }
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 }, // Order 1 - Cheese
    new TuberTopping { Id = 2, TuberOrderId = 1, ToppingId = 2 }, // Order 1 - Bacon
    new TuberTopping { Id = 3, TuberOrderId = 2, ToppingId = 3 }, // Order 2 - Sour Cream
    new TuberTopping { Id = 4, TuberOrderId = 2, ToppingId = 4 }, // Order 2 - Chives
    new TuberTopping { Id = 5, TuberOrderId = 3, ToppingId = 1 }, // Order 3 - Cheese
    new TuberTopping { Id = 6, TuberOrderId = 3, ToppingId = 5 }  // Order 3 - Butter
};


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here

app.MapGet("/tuberorders", () =>
{
    return tuberOrders.Select(t => new TuberOrderDTO
    {
        Id = t.Id,
        OrderPlacedOnDate = t.OrderPlacedOnDate,
        CustomerId = t.CustomerId,
        TuberDriverId = t.TuberDriverId,
        DeliveredOnDate = t.DeliveredOnDate,
        Toppings = t.Toppings
    });
});

app.MapGet("/tuberorders/{id}", (int id) =>
{
    TuberOrder matchedOrder = tuberOrders.FirstOrDefault(t => t.Id == id);

    return Results.Ok(matchedOrder);
});

app.MapGet("/toppings", () =>
{
    return toppings.Select(t => new ToppingDTO
    {
        Id = t.Id,
        Name = t.Name
    });
});

app.MapGet("/toppings/{id}", (int id) =>
{
    Topping topping = toppings.FirstOrDefault(t => t.Id == id);

    return Results.Ok(topping);
});

app.MapGet("/tubertoppings", () =>
{
    return tuberToppings.Select(t => new TuberTopping
    {
        Id = t.Id,
        TuberOrderId = t.TuberOrderId,
        ToppingId = t.ToppingId
    });
});

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound($"Customer with ID {id} not found.");
    }

    List<TuberOrder> orders = tuberOrders.Where(to => to.CustomerId == id).ToList();

    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        TuberOrders = orders.Select(o => new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId,
            DeliveredOnDate = o.DeliveredOnDate
        }).ToList()
    });
});

app.MapGet("/tuberdrivers", () =>
{
    return tuberDrivers.Select(t => new TuberDriverDTO
    {
        Id = t.Id,
        Name = t.Name
    });
});

app.MapGet("/tuberdrivers/{id}", (int id) =>
{
    TuberDriver driver = tuberDrivers.FirstOrDefault(td => td.Id == id);

    return Results.Ok(new TuberDriverDTO
    {
        Id = driver.Id,
        Name = driver.Name
    });
});

app.MapPost("/tuberorders/", (TuberOrder newTuberOrder) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == newTuberOrder.CustomerId);
    if (customer == null)
    {
        return Results.BadRequest($"Customer with ID {newTuberOrder.CustomerId} not found.");
    }

    TuberDriver tuberDriver = null;
    if (newTuberOrder.TuberDriverId.HasValue)
    {
        tuberDriver = tuberDrivers.FirstOrDefault(d => d.Id == newTuberOrder.TuberDriverId);
        if (tuberDriver == null)
        {
            return Results.BadRequest($"TuberDriver with ID {newTuberOrder.TuberDriverId} not found.");
        }
    }

    newTuberOrder.Id = tuberOrders.Any() ? tuberOrders.Max(t => t.Id) + 1 : 1;
    tuberOrders.Add(newTuberOrder);

    return Results.Created($"/tuberorders/{newTuberOrder.Id}", new TuberOrderDTO
    {
        Id = newTuberOrder.Id,
        OrderPlacedOnDate = newTuberOrder.OrderPlacedOnDate,
        CustomerId = newTuberOrder.CustomerId,
        TuberDriverId = newTuberOrder.TuberDriverId,
        DeliveredOnDate = newTuberOrder.DeliveredOnDate,
        Toppings = newTuberOrder.Toppings,
        Customer = new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
        },
        TuberDriver = tuberDriver != null
        ? new TuberDriverDTO
        {
            Id = tuberDriver.Id,
            Name = tuberDriver.Name
        } : null
    });

});

app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    // get the order you are going to be working with
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound($"Tuber order with ID {id} not found");
    }

    // update the datetime to now
    order.DeliveredOnDate = DateTime.Today;

    return Results.Ok($"Tuber order with ID {id} marked as complete with delivery date {order.DeliveredOnDate}");
});

app.MapPost("/customers", (Customer newCustomer) =>
{
    newCustomer.Id = customers.Any() ? customers.Max(c => c.Id) + 1 : 1;
    customers.Add(newCustomer);

    List<TuberOrder> orders = tuberOrders.Where(t => t.CustomerId == newCustomer.Id).ToList();

    return Results.Created($"/customers/{newCustomer.Id}", new CustomerDTO
    {
        Id = newCustomer.Id,
        Name = newCustomer.Name,
        Address = newCustomer.Address,
        TuberOrders = orders.Select(o => new TuberOrderDTO
        {
            Id = o.Id,
            OrderPlacedOnDate = o.OrderPlacedOnDate,
            CustomerId = o.CustomerId,
            TuberDriverId = o.TuberDriverId,
            DeliveredOnDate = o.DeliveredOnDate
        }).ToList()
    });

});

app.MapPut("/tuberorders/{id}/assignDriver", (int id, int driverId) =>
{
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound($"Tuber order with ID {id} not found");
    }

    TuberDriver driver = tuberDrivers.FirstOrDefault(d => d.Id == driverId);
    if (driver == null)
    {
        return Results.BadRequest($"TuberDriver with ID {driverId} not found.");
    }

    order.TuberDriverId = driverId;

    TuberOrderDTO updatedOrder = new TuberOrderDTO
    {
        Id = order.Id,
        OrderPlacedOnDate = order.OrderPlacedOnDate,
        CustomerId = order.CustomerId,
        TuberDriverId = order.TuberDriverId,
        DeliveredOnDate = order.DeliveredOnDate,
        Toppings = order.Toppings,
        Customer = new CustomerDTO
        {
            Id = customers.First(c => c.Id == order.CustomerId).Id,
            Name = customers.First(c => c.Id == order.CustomerId).Name,
            Address = customers.First(c => c.Id == order.CustomerId).Address
        },
        TuberDriver = new TuberDriverDTO
        {
            Id = driver.Id,
            Name = driver.Name
        }

    };

    return Results.Ok(updatedOrder);
});


app.MapPut("/tuberorders/{id}/toppings", (int id, int toppingId) =>
{
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound($"Tuber order with ID {id} not found.");
    }

    // Check if the topping exists
    Topping topping = toppings.FirstOrDefault(t => t.Id == toppingId);
    if (topping == null)
    {
        return Results.NotFound($"Topping with ID {toppingId} not found.");
    }

    tuberToppings.Add(new TuberTopping
    {
        Id = tuberToppings.Max(tt => tt.Id) + 1,
        TuberOrderId = id,
        ToppingId = toppingId
    });

    return Results.Ok($"Topping '{topping.Name}' added to order {id}.");


});

app.MapDelete("/tuberorders/{id}/toppings/{toppingId}", (int id, int toppingId) =>
{
    TuberOrder order = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (order == null)
    {
        return Results.NotFound($"Tuber order with ID {id} not found.");
    }

    //locate topping
    TuberTopping topping = tuberToppings.FirstOrDefault(tt => tt.TuberOrderId == id && tt.ToppingId == toppingId);

    if (topping == null)
    {
        return Results.NotFound($"Topping with ID {toppingId} not found for order {id}.");
    }

    tuberToppings.Remove(topping);

    return Results.Ok($"Topping with ID {toppingId} removed from order {id}.");
});

app.MapDelete("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    customers.Remove(customer);

    return Results.Ok($"Deleted customer with name: {customer.Name}");
});

app.Run();
//don't touch or move this!
public partial class Program { }