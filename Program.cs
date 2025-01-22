using Microsoft.Extensions.DependencyInjection;
using T3;
using T3.Interfaces;

var services = new ServiceCollection();

services.AddSingleton(args);
services.AddTransient<GameManager>();
services.AddTransient<IRules, GameRules>();
services.AddTransient<ISecureRNG, SecureRNG>();
services.AddTransient<ITrustHMAC, TrustHMAC>();
services.AddSingleton<IDiceProbability, DiceProbability>();
services.AddSingleton<ITableGenerator, TableViewProb>();

var serviceProvider = services.BuildServiceProvider();
var gameManager = serviceProvider.GetService<GameManager>();
gameManager!.Run(args);