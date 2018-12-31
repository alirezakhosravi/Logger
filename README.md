# Logger

### Configuration Logging
On Startup.cs you must add `` services.AddLogger<Context>(options => {}); ``
```
  services.AddLogger<Context>(options =>
  {
      options.LoggerName = "Logger";
      options.Filter = (category, level) => true;
  });
```

if you want to loged type of action, you must change pattern of Filter attribute in option.
```
  services.AddLogger<Context>(options =>
  {
      options.LoggerName = "Logger";
      options.Filter = (category, level) => level == LogLevel.Information || level == LogLevel.Warning;
  });
```

### Configuration for database
```
  public class Startup
  {
    private static readonly LoggerFactory LoggerFactory
        = new LoggerFactory(new[]
        {
            new DebugLoggerProvider((category, level)
                => category == DbLoggerCategory.Database.Command.Name
                   && level == LogLevel.Information)
        });
    public void ConfigureServices(IServiceCollection services)
    {
      ...
      services.AddDbContext<Context>((provider, builder) =>
                {
                    builder.UseSqlServer("connection string",
                            optionsBuilder =>
                            {
                                optionsBuilder.CommandTimeout(180);
                                optionsBuilder.UseRowNumberForPaging();
                            })
                        .ConfigureWarnings(warnings => warnings.Throw(CoreEventId.IncludeIgnoredWarning))
                        .UseLoggerFactory(LoggerFactory);
                });
      ...
      }
      
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      ...
      loggerFactory.AddEntityFramework<Context>(app.ApplicationServices, LogLevel.Warning);  
      ...
    }
```
