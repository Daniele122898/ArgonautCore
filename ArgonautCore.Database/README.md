# Database Fundamental Functions
This mainly includes my implementation of the DB wrapper and transactor allowing you 
to easily write repositories and add normal but also atomic queries.

## How to use the Transactor

```csharp
public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configs)
        {
            services.AddScoped<ITransactor<MyContext>, MyDbTransactor>();

            services.AddScoped<IMyRepository, MyRepository>();
            // [...]
            
            // Use this pool in the transactor as well for improved performance
            services.AddDbContextPool<MyContext>(op =>
            {
                op.UseLazyLoadingProxies(); // For conventient lazy proxies
                op.UseMySql(configs.GetSection("DbSettings").GetValue<string>("DbConnection"));
            });

            // Inject the context factory so we use the ContextPool
            // We will needs this contextFactory in the proper implementation of our
            // own Transactor 'MyDbTransactor'.
            services.AddScoped<Func<MyContext>>(provider => provider.GetRequiredService<MyContext>);
            
            
            return services;
        }
```

In `MyDbTransactor.cs`
```csharp 
public class MyDbTransactor : TransactorBase<MyContext>
    {
        private readonly Func<MyContext> _contextFactory;

        // Inject logging and context factory
        public SoraDbTransactor(ILogger<TransactorBase<MyContext>> logger, Func<MyContext> contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        // Override abstract implementation of the context factory to use our injected func
        protected override MyContext CreateContext()
        {
            return _contextFactory();
        }
    }
```