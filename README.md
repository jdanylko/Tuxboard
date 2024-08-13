# Tu><board 

![Tuxboard Example](images/TuxboardExample.png)

<p>
  <img src="https://jdanylko.vsrm.visualstudio.com/_apis/public/Release/badge/624b04d9-c444-4cb9-bd43-23d11c8291b0/1/2" />
  <img alt="GitHub" src="https://img.shields.io/github/license/jdanylko/Tuxboard">
</p>

**UPDATE:** Version [1.7.0](https://github.com/jdanylko/Tuxboard/discussions/31) is now available on <a href="https://www.nuget.org/packages/Tuxboard.Core/" title="Go to Tuxboard.Core on NuGet.org">NuGet</a>.

Tuxboard is a lightweight, open-source dashboard library specifically for the ASP.NET Core platform. It was meant to be
a Lego-style way to build dashboards.

#### Technology Stack

  - ASP.NET Core 8.0 or higher (using C#)
  - Entity Framework Core
  
#### Features

 * Easily attach a dashboard to your project
 * Customize dashboards for general audiences, user-based dashboards, or even role-based dashboards.
 * Extend your dashboard by building dynamic or static widgets.
 * Small, compact code for performance in C# and TypeScript/JavaScript (native JavaScript)
 * While Tuxboard uses Bootstrap, it can easily conform to any CSS Framework.

#### Why build a Dashboard library
In my career, I've built a number of dashboards from scratch. 
As with all projects, each dashboard had pluses and minuses as each project completed.

Even with existing libraries, there really wasn't anything out there for the ASP.NET platform.

So the initiative was set to write one.

The developer could start with a simple structure where
ANY type of dashboard could be generated quickly and 
provide an easy front-end with
their own custom widgets and robust layouts.

## Getting Started

 * Examples are in a separate repository [Tuxboard.Examples](https://github.com/jdanylko/Tuxboard.Examples)
 * I'm writing a collection of [Tuxboard posts](https://www.danylkoweb.com/Tuxboard/) on DanylkoWeb.com


## code sample help snippets on initialization

Alternatively install it using the .NET CLI:

Stp 1
dotnet add package Tuxboard

Step 2: Configure Tuxboard
In your `Startup.cs` file, add the Tuxboard services to the `ConfigureServices` method:

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTuxboard();
    }

Step 3: Create a Dashboard
Create a new class that will serve as your dashboard. This class should inherit from `TuxboardDashboard`:


    using Tuxboard;
    
    public class MyDashboard : TuxboardDashboard
    {
        public MyDashboard()
        {
            // Add widgets to the dashboard here
        }
    }

Step 4: Add Widgets to the Dashboard
Create widget classes that inherit from TuxboardWidget. For example:

    using Tuxboard;
    
    public class MyWidget : TuxboardWidget
    {
        public MyWidget()
        {
            // Configure the widget here
            Title = "My Widget";
            Description = "This is my widget";
        }
    
        public override void Render()
        {
            // Render the widget content here
            Html.RenderPartial("MyWidgetPartial");
        }
    }

Step 5: Add the Dashboard to the Tuxboard Configuration
thaen in your Startup.cs file, add the dashboard to the Tuxboard configuration in the Configure method:

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
    
        app.UseTuxboard(dashboard =>
        {
            dashboard.AddDashboard<MyDashboard>();
        });
    }

Step 6: Create a View for the Dashboard
Finally make ior create a new view that will render the dashboard. For example:

    @using Tuxboard
    
    <div class="dashboard">
        @Html.TuxboardDashboard()
    </div>


//TODO show how to wire up table data from front end
